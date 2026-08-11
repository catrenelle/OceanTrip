# Ocean Trip — Post-Review Fix Plan

Source: code review of commit `a69945e` ("Replace WinForms UI with WPF, add
self-updating offsets and RB_TC 7.2 support") on branch `develop`, run
2026-08-10. 8 independent finder passes + 1-vote verification against the
actual diff/files. 10 findings confirmed real; 2 candidates (a possible
`BaitRestockAmount` uint underflow, an unbounded `LureMaxStacks` stepper) were
checked and refuted — both are already guarded/clamped elsewhere, no action
needed there.

Ordered by severity. Each item has enough context to fix cold in a fresh
session — no need to re-read the whole diff first.

---

## 1. `ChangeBait` failure is silently swallowed by every caller
**File:** `Strategies/BaitChanger.cs:36` (and call sites in `OceanTrip.cs:1199`,
`NormalBaitSelector.cs:172`, `SpectralBaitSelector.cs`, `AchievementBaitSelector.cs`)

`ChangeBait` now returns `Task<bool>` (success/failure after
`BAIT_CHANGE_MAX_ATTEMPTS` retries), but every call site is a bare
`await ChangeBait(...)` — the bool is never checked. When a bait swap fails,
the old bait stays equipped, but `context.ChainCastTargetFishId` /
`ChainMoochTargetFishId` (set right before the call) is never cleared.
`HookingStrategy.ShouldAttemptHook` then declines every real bite for the
rest of the stop because the wrong fish never matches the stale chain target.

**Fix:** check the return value at each call site. On `false`, clear the
chain-target fields on `context` (or otherwise bail out of the chain
attempt) instead of proceeding as if the swap succeeded.

---

## 2. Mooch/intuition prereq lookups use the unfiltered fish list
**Files:** `Strategies/NormalBaitSelector.cs:132`, `Strategies/SpectralBaitSelector.cs:110`

Both resolve the mooch chain-source fish via unfiltered
`FishDataCache.GetFish()` instead of the time/weather-filtered list
(`availableNormalFish` in `NormalBaitSelector.cs:99` two lines above shows
the correct pattern). If the source fish is currently excluded by time of
day or weather, it still resolves non-null, `ChainCastTargetFishId`/
`ShouldMooch` get set anyway, and — same failure mode as #1 —
`HookingStrategy` declines every bite for the rest of the stop since that
fish can never actually bite right now.

**Fix:** filter the candidate list the same way `availableNormalFish` does
before resolving the mooch/intuition source fish, in both files. The
`SpectralBaitSelector.cs` comment claims it already "walks the prereq list
the same way NormalBaitSelector does" — it doesn't; fix the comment too once
the logic actually matches.

---

## 3. `TargetFishId` narrow-target gate ignores Leveling mode
**File:** `Strategies/HookingStrategy.cs:269` (`ShouldAttemptHook`)

Reads `OceanTripNewSettings.Instance.TargetFishId` directly with no
`EffectiveFishPriority == Leveling` check — every other Leveling-aware
branch touched in this diff has one (`LureStrategy.cs:118-121`,
`BaitRestockStrategy`'s allowedBaits gating, `OceanTrip.cs:1077/1194`,
`HookingStrategy.cs:134-138` itself for the DH/TH branch). A leftover
`TargetFishId` from a prior high-level session + Auto priority resolving to
Leveling (below level 90) causes every non-matching bite to be declined via
Rest, defeating Leveling's "catch whatever bites" design.

**Fix:** add the same `EffectiveFishPriority != FishPriority.Leveling` guard
around the `TargetFishId` check that the sibling branches use.

---

## 4. Mission-required tug type/tags go stale on mooch-continuation casts
**Files:** `Strategies/HookingStrategy.cs:157`, `Strategies/FishingSessionManager.cs:94-111`,
`OceanTrip.cs:501-509` (`SelectAndApplyBaitCallback`)

`context.MissionRequiredTugType`/`MissionRequiredAchievementTags` only
refresh inside `SelectAndApplyBaitCallback`, called from
`OceanTrip.SelectAndApplyBait`. `FishingSessionManager`'s mooch branch
(lines 94-111) skips that callback entirely. If a mission completes (or
newly activates) exactly on a cast that's also a mooch continuation, the
DH/TH mission override at hook time sees the pre-mooch snapshot — wasting GP
on a completed mission, or missing DH/TH for a newly-relevant one.

**Fix:** either call a lighter mission-state refresh on the mooch path too,
or have `HookingStrategy` re-derive current mission state directly from
`Endeavor` at hook-decision time instead of trusting the cached context
fields.

---

## 5. Offset self-sync omits the 5 voyage-results-screen offsets
**File:** `Helpers/OceanFishingOffsetSync.cs:34` (`TrackedFields`)

`Endeavor.cs`'s `!RB_TC` `Offsets` class has 15 mutable fields; only 10 are
in `TrackedFields` (Status/Zone/Duration/TimeOffset/Mission1-3 Type+Progress).
The 5 omitted ones — `allResultSizeOffset`, `localIndexInAllResultOffset`,
`individualResultOffset`, `localPlayerAllResultOffset`, `allResultsOffset` —
are exactly what `Endeavor.ReadVoyageResult()` reads to decode the results
screen (points, XP, scrips, caught-fish count, bonuses, leaderboard). The
file's own doc comment claims untracked fields are "ignored since Endeavor
doesn't read them" and even lists "fish-catch results" as an example — that
comment is wrong for these 5. If FFXIVClientStructs' struct layout shifts
there after a future patch, these 5 silently go stale with no error, and
`ReadVoyageResult()` reads/logs garbage every voyage.

**Fix:** add the 5 VoyageResult fields to `TrackedFields` with matching
upstream field names (`AllResultSize`, `LocalIndexInAllResult`,
`IndividualResult`, `LocalPlayerAllResult`, `AllResults` — confirm exact
names against the FFXIVClientStructs source `OceanFishingOffsetSync.cs`
already parses). Fix the stale doc comment either way.

---

## 6. RB_TC has no offset self-sync path at all
**File:** `Helpers/Endeavor.cs:50` (RB_TC branch), `Helpers/OceanFishingOffsetSync.cs`

This is the answer to the original "do we have all the RB_TC Patch 7.2
offsets" question: **the values themselves check out** — every field's
byte-offset delta from `statusOffset` matches the global client's struct
layout exactly (e.g. `zoneOffset` is `+0x4` from `statusOffset` in both
builds, `mission1TypeOffset` is `+0x60C` in both), which is strong evidence
the Patch 7.20 values were derived correctly, not guessed.

What's missing: `OceanFishingOffsetSync.RunAsync()` is an explicit no-op
under `#if RB_TC` — only the global client self-corrects from
FFXIVClientStructs at runtime. RB_TC's 13 offsets are pinned consts with no
automated cross-check, and only `statusOffset`/`zoneOffset` kept the
"commented-out alternate patch value" convention (7.25/7.10/7.00) for manual
swapping — the other 11 fields added in this diff have no fallback history
at all.

**This is a design gap, not a live bug** — TC has no upstream project
tracking its patches the way FFXIVClientStructs tracks global, so a parallel
auto-sync isn't straightforwardly available. Low priority unless/until TC
advances past 7.20.

**Fix (when TC patches forward):** at minimum, extend the "commented-out
prior-patch values" convention to all 13 fields (not just Status/Zone) going
forward, so the next patch swap has anchors to work from. If it's ever worth
the effort, a small version-keyed offset lookup table would generalize this
better than inline comments.

---

## 7. Verbose logging toggle is orphaned in the new UI
**File:** `UI/Wpf/Xaml/Shell.xaml` (and all of `UI/Wpf/Xaml/Pages/`)

The old WinForms `FormSettings.cs` had a `verboseLoggingToggle` checkbox
bound to `LoggingMode`. No equivalent exists anywhere in the new WPF UI.
`LoggingMode` still gates `OceanLogLevel.Debug` output across ~14 files
(every `Strategies/*.cs`, `IdleActivities/*.cs`, `OceanTrip.cs`,
`PassTheTime.cs`) — there's currently no way for a user to turn it on.

**Fix:** add a checkbox/toggle for `LoggingMode` to `OceanSettingsPage.xaml`,
following the same binding pattern used for the other bool settings on that
page (e.g. `LateBoatQueue`, `OceanFood`).

---

## 8. DH/TH "worth it" formula duplicated between bot logic and UI badge
**File:** `UI/Wpf/CurrentRoutePageBehavior.cs:588`, `Strategies/HookingStrategy.cs:332` (`IsPointsWorthDoubleHook`)

The UI's `pointsWorthTriple`/`pointsWorthDouble` hand-roll the exact same
GP-cost-vs-points math as `HookingStrategy.IsPointsWorthDoubleHook`, because
that method is `private`. A comment in `CurrentRoutePageBehavior.cs`
acknowledges the duplication, but nothing enforces the two stay in sync — a
future tuning change to one and not the other makes the UI badge lie about
what the bot will actually do.

**Fix:** make `IsPointsWorthDoubleHook` (or a refactored version of it)
internal/public and call it directly from the UI instead of reimplementing
the formula.

---

## 9. Bait tooltips lost client-locale-aware naming
**File:** `UI/Wpf/OceanSettingsPageBehavior.cs:110` (`BuildBaitTile`)

Old WinForms UI built tooltips from `DataManager.ItemCache[bait].CurrentLocaleName`
(tracks the FFXIV client's language). New WPF `BuildBaitTile` uses a
hardcoded English label from the static `BaitItems` array. Non-English
clients (DE/FR/JP) now see English bait names in tooltips — a regression.

**Fix:** look up `DataManager.ItemCache[...].CurrentLocaleName` in
`BuildBaitTile` the same way the old form did, instead of the static English
label (keep the static array for layout/icon-index purposes, just don't use
its label for the tooltip text).

---

## 10. BotBases resource-path-probe loop duplicated 9x
**Files:** `Definitions/BonusData.cs:46`, `Definitions/MissionData.cs:128`,
`UI/Wpf/XamlLoader.cs:95` (`ResolvePath`), `UI/Wpf/IconAtlas.cs:53`
(`ResolvePath`), `UI/Wpf/CurrentRoutePageBehavior.cs:139`
(`LoadBannerImage`), `UI/Wpf/ShellWindow.cs:131` (`TryLoadLogoImage`),
`UI/Wpf/SchedulePageBehavior.cs:167` (`LoadImage`), plus pre-existing copies
in `Definitions/Fish.cs` and `Definitions/Routes.cs`.

Same "try OceanTrip / Ocean Trip / Ocean-Trip under BotBases" loop
copy-pasted 9 times. `IconAtlas.ResolvePath` and `XamlLoader.ResolvePath` are
both `private` and only self-used — none of the other 7 copies call them.

**Fix:** promote one of the `ResolvePath` implementations to a small shared
public helper (e.g. a static `BotBasesResourceLocator` class) and have all 9
call sites delegate to it. Lowest priority — cleanup only, no behavior bug.

---

## Suggested fix order
1-4 (correctness bugs affecting live fishing behavior) → 5-6 (offset
reliability, directly requested) → 7-9 (UI regressions from the WPF port) →
10 (cleanup, do whenever convenient, e.g. alongside #7-9 while already in
those files).
