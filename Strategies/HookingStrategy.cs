using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Enums;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;
using ff14bot.Helpers;
using System.Windows.Media;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Strategy for determining and executing the appropriate hook action based on fishing conditions
	/// </summary>
	public class HookingStrategy
	{
		private readonly GameStateCache _gameCache;
		private readonly bool _loggingEnabled;

		public HookingStrategy(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Handle fish bite - determine hook type and execute the appropriate action
		/// </summary>
		/// <param name="context">Context containing fishing state and bite information</param>
		public Task ExecuteHook(HookContext context)
		{
			double biteElapsed = Math.Round(context.BiteElapsedSeconds, 1);
			bool doubleHook = false;
			bool hasChum = Core.Player.HasAura(CharacterAuras.Chum);

			// Chum reduces bite time by ~50% — double observed time to match our database windows
			double matchElapsed = hasChum ? biteElapsed * 2.0 : biteElapsed;

			// Force a fresh read rather than relying on GameStateCache's ambient 100ms-throttled
			// RefreshIfNeeded() polling from the outer bite-wait loop — weather can change (e.g. the
			// moment Spectral Current ends) in that gap, and this decision (hook accept/decline,
			// DH/TH, exclusion logging) only runs once per bite, so a full refresh here is cheap.
			_gameCache.Refresh();
			string currentWeather = _gameCache.CurrentWeather;

			// Build fish lists for bite prediction - first try exact match, then fallback to nearest
			List<Fish> spectralFishToCatch = FindMatchingFish(
				context.CurrentRoute?.SpectralFish,
				matchElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: false);

			List<Fish> normalFishToCatch = FindMatchingFish(
				context.CurrentRoute?.NormalFish,
				matchElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: true);

			var matchedFish = context.Spectraled ? spectralFishToCatch : normalFishToCatch;
			var potentialFish = String.Join(", ", matchedFish.Select(x => _gameCache.GetItemName((uint)x.FishID)).ToList());

			// Check if this is a fuzzy match (nearest fish outside its expected window)
			bool isFuzzy = false;
			if (matchedFish.Any())
			{
				var firstFish = matchedFish.First();
				var (start, end) = firstFish.GetBiteRange(FishingManager.SelectedBaitItemId);
				isFuzzy = matchElapsed < start || matchElapsed > end;
			}

			string tugName = FishingManager.TugType == TugType.Light ? "!" : FishingManager.TugType == TugType.Medium ? "!!" : "!!!";
			string fuzzyTag = isFuzzy ? " (fuzzy)" : "";
			string chumTag = hasChum ? $" (Chum: raw {biteElapsed:F1}s)" : "";
			Log($"Bite Time: {matchElapsed:F1}s, Tug: {tugName}, Potential Fish: {(String.IsNullOrWhiteSpace(potentialFish) ? "Unable to determine" : potentialFish)}{fuzzyTag}{chumTag}");

			if (isFuzzy)
				LogExcludedFish(context, currentWeather);

			// Narrow-target modes (Achievement focus / TargetFishId): decline bites that can't be
			// what we're after. Hooking and declining both end this attempt either way, but only
			// hooking incurs an unpredictable-length catch resolution (size-roll/log/achievement
			// popups) — skip it when bite-time+tug prediction already rules out our target.
			if (!ShouldAttemptHook(context, matchedFish))
			{
				var bestGuessName = matchedFish.Any() ? _gameCache.GetItemName((uint)matchedFish.First().FishID) : "unknown fish";

				// Rest (0 GP) safely abandons the bite immediately instead of waiting for it to
				// time out on its own — no risk of accidentally hooking the unwanted fish, and it
				// doesn't strip buffs (Intuition, Chum, etc.) the way some other bail-out paths would.
				if (ActionManager.CanCast(Actions.Rest, Core.Me))
				{
					Log($"Declining bite — predicted {bestGuessName} isn't the current target. Using Rest to bail out.", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.Rest, Core.Me);
				}
				else
				{
					Log($"Declining bite — predicted {bestGuessName} isn't the current target, skipping to avoid a wasted catch.", OceanLogLevel.Debug);
				}

				context.OnHookExecuted(false);
				return Task.CompletedTask;
			}

			Log("Checking if we should double hook based on bite timer and current fishing conditions!", OceanLogLevel.Debug);

			// DH/TH cannot be used during Patience — fish always escapes without Precision/Powerful Hookset
			if (!FishingManager.HasPatience)
			{
				if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
				{
					// Achievement mode: DH/TH when predicted fish matches the target achievement category
					var achievementFocus = AchievementFishDataCache.GetCurrentAchievementFocus();
					if (achievementFocus != AchievementType.None)
					{
						var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
						doubleHook = matchingFish.Any(f =>
							!string.IsNullOrEmpty(f.Achievement) &&
							AchievementFishDataCache.MapAchievementString(f.Achievement) == achievementFocus);
					}

					// No achievement fish matched — fall through to points-based DH/TH
					if (!doubleHook)
					{
						var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
						doubleHook = IsPointsWorthDoubleHook(matchingFish);
					}
				}
				else if (OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Points || OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Auto)
				{
					// Leveling mode (a raw Auto reading resolves here when EffectiveFishPriority
					// isn't Points/Auto) never reaches this branch — no DH/TH points-chasing while
					// leveling, just a plain Hook on whatever bites.

					// Special handling for South's lastMooch rule - Always DH/TH after a Mooch in South if spectral.
					if (context.Location == "south" && context.LastCastMooch && (context.TimeOfDay == "Sunset" || context.TimeOfDay == "Night") && context.Spectraled)
					{
						doubleHook = true;
					}
					else
					{
						// Find matching fish for DH/TH decision with fallback to nearest
						var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
						doubleHook = IsPointsWorthDoubleHook(matchingFish);
					}
				}

				// Bite-strength missions ("Catch fish with a weak/strong/ferocious bite"): a single
				// hookset catches several fish sharing this tug, multiplying mission progress. Worth
				// it regardless of the points/GP-cost math above and regardless of FishPriority — a
				// mission's value is a 5-20% multiplier on the ENTIRE voyage score, not just this catch.
				if (!doubleHook && context.MissionRequiredTugType.HasValue && context.MissionRequiredTugType.Value == FishingManager.TugType)
				{
					Log("Bite matches an active bite-strength mission — Double/Triple Hooking to accelerate it.", OceanLogLevel.Debug);
					doubleHook = true;
				}

				// Category missions ("Catch sharks", "Catch fugu"): same idea, but matched by the
				// predicted fish's Achievement tag instead of tug type — a multi-catch hookset nets
				// several of the matching category at once.
				if (!doubleHook && context.MissionRequiredAchievementTags != null)
				{
					var bestGuess = matchedFish.FirstOrDefault();
					if (bestGuess != null && !string.IsNullOrEmpty(bestGuess.Achievement)
						&& context.MissionRequiredAchievementTags.Contains(bestGuess.Achievement))
					{
						Log("Bite matches an active category mission — Double/Triple Hooking to accelerate it.", OceanLogLevel.Debug);
						doubleHook = true;
					}
				}
			}

			Log("Done checking for double hook conditions.", OceanLogLevel.Debug);

			// Execute the appropriate hook action
			if (doubleHook && ActionManager.CanCast(Actions.DoubleHook, Core.Me))
			{
				if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
				{
					Log("Using Triple Hook!");
					ActionManager.DoAction(Actions.TripleHook, Core.Me);
				}
				else
				{
					Log("Using Double Hook!");
					ActionManager.DoAction(Actions.DoubleHook, Core.Me);
				}

				context.OnHookExecuted(false);
			}
			else if (FishingManager.HasPatience)
			{
				Log("Player has patience on them. Need to use special hooking.", OceanLogLevel.Debug);

				var predictedFish = context.Spectraled ? spectralFishToCatch : normalFishToCatch;
				var hooksetFish = predictedFish.FirstOrDefault(f => (int)f.HooksetType != 0);

				bool usePrecision;
				if (hooksetFish != null)
				{
					usePrecision = hooksetFish.HooksetType == TugType.Light;
					Log($"Hookset override from {_gameCache.GetItemName((uint)hooksetFish.FishID)}: {(usePrecision ? "Precision" : "Powerful")}");
				}
				else
				{
					usePrecision = FishingManager.TugType == TugType.Light;
				}

				if (usePrecision)
				{
					Log($"Using Precision Hookset!", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
				}
				else
				{
					Log($"Using Powerful Hookset!", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
				}
				context.OnHookExecuted(false);
			}
			else
			{
				Log("Checking if Full GP action is Double Hook.", OceanLogLevel.Debug);

				if (!context.Spectraled && _gameCache.MaxGP >= 500 && (_gameCache.GPDeficit <= FishingConstants.FULL_GP_BUFFER) && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && OceanTripNewSettings.Instance.FullGPAction == FullGPAction.DoubleHook)
				{
					if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
					{
						Log("Triggering Full GP Action to keep regen going - Triple Hook!");
						ActionManager.DoAction(Actions.TripleHook, Core.Me);
					}
					else
					{
						Log("Triggering Full GP Action to keep regen going - Double Hook!");
						ActionManager.DoAction(Actions.DoubleHook, Core.Me);
					}
				}
				else
				{
					Log($"Hooking Fish!", OceanLogLevel.Debug);

					FishingManager.Hook();
				}

				context.OnHookExecuted(false);
			}

			Log("Refreshing UI for Bait and Achievements in case something changed.", OceanLogLevel.Debug);

			FFXIV_Databinds.Instance.RefreshBait();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Narrow-target gate for Achievement mode / TargetFishId: only hook bites that plausibly
		/// are the target. Only applies when that target is actually reachable at this location
		/// and spectral state right now — otherwise every bite here would be wrongly declined for
		/// the whole zone visit (e.g. achievement focus has no fish here, or TargetFishId is for a
		/// different zone), which falls back to hooking normally instead.
		/// </summary>
		private bool ShouldAttemptHook(HookContext context, List<Fish> matchedFish)
		{
			uint targetFishId = OceanTripNewSettings.Instance.TargetFishId;
			bool targetFishHere = targetFishId != 0
				&& OceanTripNewSettings.Instance.EffectiveFishPriority != FishPriority.Leveling
				&& FishDataCache.GetFish().Any(f => f.FishID == (int)targetFishId && f.RouteShortName == context.Location);

			AchievementType achievementFocus = AchievementType.None;
			bool achievementFishHere = false;
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
			{
				achievementFocus = AchievementFishDataCache.GetCurrentAchievementFocus();
				if (achievementFocus != AchievementType.None)
				{
					achievementFishHere = AchievementFishDataCache.GetFishForLocation(context.Location, achievementFocus)
						.Any(f => f.SpectralFish == context.Spectraled);
				}
			}

			// A bait selector explicitly flagged this cast as chasing one specific prerequisite fish
			// (mooch-chain source, or an Intuition prereq) — honor that regardless of FishPriority;
			// the bait itself was chosen for that fish, so anything else biting is a distraction.
			uint chainTarget = context.LastCastMooch ? context.ChainMoochTargetFishId : context.ChainCastTargetFishId;
			bool chainTargetActive = chainTarget != 0;

			// No narrow-target condition is actually active right now — hook everything as normal.
			if (!targetFishHere && !achievementFishHere && !chainTargetActive)
				return true;

			var bestGuess = matchedFish.FirstOrDefault();
			if (bestGuess == null)
				return true; // no prediction available — don't block hooking on uncertainty

			if (chainTargetActive)
			{
				if (bestGuess.FishID == (int)chainTarget)
					return true;

				// A mooch can sometimes catch the same source fish again instead of the intended
				// target (e.g. Snapping Koban can mooch into itself) — accept a re-catch of the
				// cast-source fish too, so the chain continues with another mooch attempt instead
				// of discarding a valid catch and restarting from a fresh cast.
				if (context.LastCastMooch && context.ChainCastTargetFishId != 0 && bestGuess.FishID == (int)context.ChainCastTargetFishId)
					return true;

				return false;
			}

			if (targetFishHere && bestGuess.FishID == (int)targetFishId)
				return true;

			if (achievementFishHere &&
				!string.IsNullOrEmpty(bestGuess.Achievement) &&
				AchievementFishDataCache.MapAchievementString(bestGuess.Achievement) == achievementFocus)
				return true;

			return false;
		}

		/// <summary>
		/// Points-based DH/TH decision: only worth it if the top-candidate fish's total expected
		/// payoff (Points x DH/TH catch-count bonus) exceeds the real GP cost of the action
		/// (400 for Double Hook, 700 for Triple Hook — verified against game data). Uses only the
		/// closest bite-time match, not any fuzzy candidate in the pool — a low-confidence guess
		/// shouldn't justify a 400-700 GP spend.
		/// </summary>
		private bool IsPointsWorthDoubleHook(List<Fish> matchingFish)
		{
			var bestGuess = matchingFish.FirstOrDefault();
			if (bestGuess == null)
				return false;

			return IsPointsWorthTripleHook(bestGuess) || IsPointsWorthDoubleHook(bestGuess);
		}

		/// <summary>
		/// Per-fish half of the points-based DH/TH formula above — exposed so the UI's DH/TH badge
		/// (CurrentRoutePageBehavior.BuildFishIcon) can call the exact same math the bot hooks with,
		/// instead of hand-rolling a copy that can drift out of sync with a future tuning change.
		/// </summary>
		public static bool IsPointsWorthTripleHook(Fish fish) =>
			fish.THBonus > 1 && fish.Points * fish.THBonus > FishingConstants.TRIPLE_HOOK_GP_COST;

		/// <summary>
		/// Per-fish half of the points-based DH/TH formula above — see IsPointsWorthTripleHook.
		/// </summary>
		public static bool IsPointsWorthDoubleHook(Fish fish) =>
			fish.DHBonus > 1 && fish.Points * fish.DHBonus > FishingConstants.DOUBLE_HOOK_GP_COST;

		/// <summary>
		/// Find matching fish from a list, with fallback to nearest fish if no exact match
		/// </summary>
		private List<Fish> FindMatchingFish(IEnumerable<Fish> fishList, double biteElapsed, string timeOfDay, string currentWeather, bool excludeWeather)
		{
			if (fishList == null)
				return new List<Fish>();

			uint currentBait = FishingManager.SelectedBaitItemId;
			bool hasIntuition = Core.Player.HasAura(CharacterAuras.FishersIntuition);

			// Filter by time of day, weather (if applicable), tug type, and intuition requirement
			var eligibleFish = fishList.Where(x =>
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.BiteType == FishingManager.TugType &&
				(!excludeWeather || (x.WeatherExclusion1 != currentWeather && x.WeatherExclusion2 != currentWeather)) &&
				(!x.RequiresIntuition || hasIntuition)).ToList();

			// First try exact match using bite range for current bait
			var exactMatches = eligibleFish.Where(x =>
			{
				var (start, end) = x.GetBiteRange(currentBait);
				return start <= biteElapsed && end >= biteElapsed;
			}).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed, currentBait);
		}

		/// <summary>
		/// Find matching fish for hook decision from all fish data, with fallback to nearest
		/// </summary>
		private List<Fish> FindMatchingFishForHook(string location, double biteElapsed, string timeOfDay, string currentWeather)
		{
			var allFish = FishDataCache.GetFish();
			uint currentBait = FishingManager.SelectedBaitItemId;
			bool hasIntuition = Core.Player.HasAura(CharacterAuras.FishersIntuition);

			// Filter by location, time of day, weather, tug type, and intuition requirement
			var eligibleFish = allFish.Where(x =>
				x.RouteShortName == location &&
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.WeatherExclusion1 != currentWeather &&
				x.WeatherExclusion2 != currentWeather &&
				x.BiteType == FishingManager.TugType &&
				(!x.RequiresIntuition || hasIntuition)).ToList();

			// First try exact match using bite range for current bait
			var exactMatches = eligibleFish.Where(x =>
			{
				var (start, end) = x.GetBiteRange(currentBait);
				return start <= biteElapsed && end >= biteElapsed;
			}).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed, currentBait);
		}

		/// <summary>
		/// Return ALL eligible fish sorted by distance to the observed bite time.
		/// The caught fish must be one of the known fish at this location — bite time
		/// narrows the prediction but any eligible fish with the right tug could have bitten.
		/// </summary>
		private List<Fish> FindNearestFish(List<Fish> eligibleFish, double biteElapsed, uint currentBait = 0)
		{
			if (!eligibleFish.Any())
				return new List<Fish>();

			return eligibleFish.Select(x =>
			{
				var (start, end) = currentBait > 0 ? x.GetBiteRange(currentBait) : (x.BiteStart, x.BiteEnd);
				double distance;
				if (biteElapsed < start)
					distance = start - biteElapsed;
				else if (biteElapsed > end)
					distance = biteElapsed - end;
				else
					distance = 0;

				return new { Fish = x, Distance = distance };
			})
			.OrderBy(x => x.Distance)
			.Select(x => x.Fish)
			.ToList();
		}

		/// <summary>
		/// Log fish excluded by time/weather filters when a fuzzy match occurs.
		/// Helps identify if the actual caught fish was filtered out of the eligible pool.
		/// </summary>
		private void LogExcludedFish(HookContext context, string currentWeather)
		{
			var fishList = context.Spectraled ? context.CurrentRoute?.SpectralFish : context.CurrentRoute?.NormalFish;
			if (fishList == null)
				return;

			bool excludeWeather = !context.Spectraled;

			var excluded = fishList.Where(x =>
				x.BiteType == FishingManager.TugType &&
				(x.TimeOfDayExclusion1 == context.TimeOfDay ||
				x.TimeOfDayExclusion2 == context.TimeOfDay ||
				(excludeWeather && (x.WeatherExclusion1 == currentWeather || x.WeatherExclusion2 == currentWeather))))
				.ToList();

			if (excluded.Any())
			{
				uint currentBait = FishingManager.SelectedBaitItemId;
				var excludedStr = String.Join(", ", excluded.Select(x =>
				{
					var reasons = new List<string>();
					if (x.TimeOfDayExclusion1 == context.TimeOfDay || x.TimeOfDayExclusion2 == context.TimeOfDay)
						reasons.Add($"time={context.TimeOfDay}");
					if (excludeWeather && (x.WeatherExclusion1 == currentWeather || x.WeatherExclusion2 == currentWeather))
						reasons.Add($"weather={currentWeather}");
					var (start, end) = x.GetBiteRange(currentBait);
					return $"{_gameCache.GetItemName((uint)x.FishID)} [{start:F0}-{end:F0}s, excluded: {String.Join("+", reasons)}]";
				}));
				Log($"  Excluded: {excludedStr}");
			}
		}

		/// <summary>
		/// Internal logging method
		/// </summary>
		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text);
			Logging.Write(Colors.Aqua, msg);
		}
	}

	/// <summary>
	/// Context for hook execution containing all necessary state
	/// </summary>
	public class HookContext
	{
		public double BiteElapsedSeconds { get; set; }
		public bool Spectraled { get; set; }
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public RouteWithFish CurrentRoute { get; set; }
		public bool LastCastMooch { get; set; }

		/// <summary>
		/// Non-zero when this cast is chasing a specific prerequisite fish (mooch-chain source,
		/// or an Intuition prereq) — set by the bait selector that ran before this cast.
		/// </summary>
		public uint ChainCastTargetFishId { get; set; }

		/// <summary>
		/// Non-zero when a mooch off this cast's catch is chasing a specific fish. Only relevant
		/// when LastCastMooch is true (i.e. this bite came from a mooch, not a plain cast).
		/// </summary>
		public uint ChainMoochTargetFishId { get; set; }

		/// <summary>
		/// Set when an active bite-strength mission ("Catch fish with a weak/strong/ferocious
		/// bite") still needs progress — a bite matching this tug is worth Double/Triple Hooking
		/// regardless of the usual points/GP-cost math, since it multiplies mission progress.
		/// </summary>
		public TugType? MissionRequiredTugType { get; set; }

		/// <summary>
		/// Set when an active category mission ("Catch sharks", "Catch fugu") still needs progress —
		/// a bite whose predicted fish's Achievement tag is in this list is worth Double/Triple
		/// Hooking for the same reason as MissionRequiredTugType.
		/// </summary>
		public string[] MissionRequiredAchievementTags { get; set; }

		private Action<bool> _onHookExecutedCallback;

		/// <summary>
		/// Set callback to be invoked after hook is executed
		/// </summary>
		public void SetHookExecutedCallback(Action<bool> callback)
		{
			_onHookExecutedCallback = callback;
		}

		/// <summary>
		/// Invoke the hook executed callback
		/// </summary>
		public void OnHookExecuted(bool caughtFishLogged)
		{
			_onHookExecutedCallback?.Invoke(caughtFishLogged);
		}
	}
}
