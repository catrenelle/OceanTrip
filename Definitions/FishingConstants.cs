using System.Collections.Generic;
using Clio.Utilities;

namespace OceanTripPlanner.Definitions
{
	/// <summary>
	/// Central location for all Ocean Trip fishing constants
	/// </summary>
	public static class FishingConstants
	{
		// ========================================
		// LEVELING MODE CONSTANTS
		// ========================================

		/// <summary>
		/// Fisher level below which Auto priority resolves to Leveling mode instead — see
		/// OceanTripNewSettings.EffectiveFishPriority.
		/// </summary>
		public const int LEVELING_MODE_LEVEL_CAP = 90;

		/// <summary>
		/// The only baits Leveling mode ever equips or restocks — cheap, purchasable, and enough to
		/// catch something on every route without needing any of the specialty/prereq-chain baits the
		/// other priorities use.
		/// </summary>
		public static readonly HashSet<uint> LEVELING_ALLOWED_BAITS = new HashSet<uint>
		{
			FishBait.Krill,
			FishBait.Ragworm,
			FishBait.PlumpWorm,
		};

		// ========================================
		// QUEST GATING CONSTANTS
		// ========================================

		/// <summary>
		/// "All the Fish in the Sea" — the Fisher quest that unlocks Ocean Fishing voyages.
		/// Verified via XIVAPI Quest 69379. Passed directly to QuestLogManager.IsQuestCompleted, which
		/// takes the raw Quest-sheet row ID (matches FFXIVClientStructs' own IsQuestComplete(uint)
		/// convenience overload — no manual 0x10000 offset needed).
		/// </summary>
		public const uint OCEAN_FISHING_UNLOCK_QUEST_ID = 69379;

		// ========================================
		// GP MANAGEMENT CONSTANTS
		// ========================================

		/// <summary>
		/// GP threshold below which to use Cordials (400 GP)
		/// </summary>
		public const int CORDIAL_GP_THRESHOLD = 400;

		/// <summary>
		/// GP percentage threshold, while still banking for spectral, below which to use a Hi-Cordial
		/// anyway instead of just passively regenerating (50%) — natural GP regen is slow enough that
		/// sitting in the 25-50% band for the rest of a stop wastes more time than the Cordial saves
		/// for the spectral burst.
		/// </summary>
		public const float LOW_GP_PERCENT = 50.0f;

		/// <summary>
		/// GP threshold below which to use Thaliak's Favor (200 GP)
		/// </summary>
		public const int THALIAK_GP_THRESHOLD = 200;

		/// <summary>
		/// GP buffer to prevent overfilling GP (100 GP)
		/// </summary>
		public const int FULL_GP_BUFFER = 100;

		/// <summary>
		/// Real GP cost of Double Hook, verified via game data (XIVAPI Action 269)
		/// </summary>
		public const int DOUBLE_HOOK_GP_COST = 400;

		/// <summary>
		/// Real GP cost of Triple Hook, verified via game data (XIVAPI Action 27523)
		/// </summary>
		public const int TRIPLE_HOOK_GP_COST = 700;

		/// <summary>
		/// Real GP cost of Prize Catch, verified via game data (XIVAPI Action 26806)
		/// </summary>
		public const int PRIZE_CATCH_GP_COST = 200;

		/// <summary>
		/// Real GP cost of Identical Cast, verified via game data (XIVAPI Action 4596)
		/// </summary>
		public const int IDENTICAL_CAST_GP_COST = 350;

		/// <summary>
		/// Safety cap on consecutive Thaliak's Favor casts per check (stacks self-limit at ~3 uses
		/// per full Angler's Art bank of 10, this just guards against an unexpected infinite loop)
		/// </summary>
		public const int THALIAK_MAX_CHAIN_USES = 4;

		/// <summary>
		/// A spectral current can no longer start once a stop's remaining time drops below this
		/// (game rule, not a tunable). Past this point there's nothing left to bank GP for at this
		/// stop, so GP-banking should release back to normal top-up behavior.
		/// </summary>
		public const int SPECTRAL_CUTOFF_SECONDS = 90;

		/// <summary>
		/// In Points/Auto priority, spectral fish here must average at least this multiple of
		/// normal fish's average points to be worth actively popping spectral for (rather than
		/// just fishing points-optimal normal bait) — a bare "spectral averages more" isn't
		/// enough to justify the detour on its own. Ignored while spectral pity is active (see
		/// OceanTrip._spectralPityActive) — a pity-boosted current runs longer with rising trigger
		/// odds, so it's worth chasing as long as there's any spectral fish worth catching here at
		/// all, without needing to clear this bar.
		/// </summary>
		public const double SPECTRAL_POINTS_MARGIN = 1.15;

		// ========================================
		// LURE CONSTANTS
		// ========================================

		/// <summary>
		/// Delay after cast lands before applying lure (ms). Line must be in water first.
		/// </summary>
		public const int LURE_POST_CAST_DELAY_MS = 1200;

		/// <summary>
		/// GP costs for each lure stack: 10 + 20 + 30 = 60 total
		/// </summary>
		public static readonly int[] LURE_GP_COSTS = { 10, 20, 30 };

		/// <summary>
		/// Grace period per lure stack where no bite can land (~5s each)
		/// </summary>
		public const double LURE_GRACE_PERIOD_SECONDS = 5.0;

		/// <summary>
		/// Threshold: skip lures if target hookset fish are this % or more of the pool
		/// </summary>
		public const float LURE_SKIP_DOMINANT_THRESHOLD = 0.6f;

		/// <summary>
		/// Maximum lure stacks per cast
		/// </summary>
		public const int LURE_MAX_STACKS = 3;

		// ========================================
		// INVENTORY MANAGEMENT CONSTANTS
		// ========================================

		/// <summary>
		/// Default equipment repair threshold (50%)
		/// </summary>
		public const int REPAIR_THRESHOLD_DEFAULT = 50;

		/// <summary>
		/// Minimum bait count threshold before restocking (10)
		/// </summary>
		public const int MIN_BAIT_THRESHOLD = 10;

		/// <summary>
		/// Minimum bait amount to purchase when restocking (30)
		/// </summary>
		public const int MIN_BAIT_AMOUNT = 30;

		/// <summary>
		/// Default scrip threshold for purchasing cordials (1500)
		/// </summary>
		public const int DEFAULT_SCRIP_THRESHOLD = 1500;

		// ========================================
		// TIMING CONSTANTS
		// ========================================

		/// <summary>
		/// Offset to adjust FishingManager.TimeSinceCast to match expected bite times (-0.3s).
		/// Used by the open-world fishing path, which still times from the cast command.
		/// </summary>
		public const double BITE_TIMER_OFFSET = -0.3;

		/// <summary>
		/// Offset for the ocean-voyage bite timer to match GatherBuddy. Our splashdown latch anchors at
		/// the first "settled waiting" fishing state, which lands ~1.3s AFTER GatherBuddy's own anchor,
		/// so RB measures a shorter interval. Measured live: RB read a consistent 1.3s LOWER than
		/// GatherBuddy, so add it back. TUNING KNOB: if a future run shows a new consistent bias vs
		/// GatherBuddy, adjust this value (not BITE_TIMER_OFFSET, which the open-world path uses).
		/// </summary>
		public const double LANDED_BITE_OFFSET = 1.3;


		/// <summary>
		/// Buff check interval in milliseconds (5000ms = 5 seconds)
		/// </summary>
		public const int BUFF_CHECK_INTERVAL_MS = 5000;

		// ========================================
		// COROUTINE TIMING CONSTANTS
		// ========================================

		/// <summary>
		/// Standard delay after navigation/interaction (1000ms = 1 second)
		/// </summary>
		public const int STANDARD_DELAY_MS = 1000;

		/// <summary>
		/// Delay after fish exchange actions like sell/desynth (3000ms = 3 seconds)
		/// </summary>
		public const int FISH_EXCHANGE_DELAY_MS = 3000;

		/// <summary>
		/// Delay after voyage completion before checking results (2000ms = 2 seconds)
		/// </summary>
		public const int VOYAGE_COMPLETION_DELAY_MS = 2000;

		/// <summary>
		/// Delay for results window calculation (12000ms = 12 seconds)
		/// </summary>
		public const int RESULTS_CALCULATION_DELAY_MS = 12000;

		/// <summary>
		/// Timeout for loading screen detection (30000ms = 30 seconds)
		/// </summary>
		public const int LOADING_TIMEOUT_MS = 30000;

		/// <summary>
		/// Delay before/after cordial use. Applied twice per use (before the UseItem and after, to let
		/// the GP change register), so this is doubled in the cast-cycle cost. Trimmed from 600ms to
		/// 250ms — during Spectral Current cordials fire nearly every cast, so 2x600ms was a big chunk
		/// of the between-cast time; the GP change registers well within 250ms.
		/// </summary>
		public const int CORDIAL_USE_DELAY_MS = 250;

		/// <summary>
		/// Timeout for waiting for dialog windows to open (3000ms = 3 seconds)
		/// </summary>
		public const int DIALOG_WINDOW_TIMEOUT_MS = 3000;

		/// <summary>
		/// Timeout for waiting for repair window to close (5000ms = 5 seconds)
		/// </summary>
		public const int REPAIR_WINDOW_TIMEOUT_MS = 5000;

		/// <summary>
		/// Timeout for waiting for shop window to open (5000ms = 5 seconds)
		/// </summary>
		public const int SHOP_WINDOW_TIMEOUT_MS = 5000;

		/// <summary>
		/// Timeout for waiting for item sell completion (10000ms = 10 seconds)
		/// </summary>
		public const int ITEM_SELL_TIMEOUT_MS = 10000;

		/// <summary>
		/// Timeout for waiting for shop window to close (2000ms = 2 seconds)
		/// </summary>
		public const int SHOP_CLOSE_TIMEOUT_MS = 2000;

		/// <summary>
		/// FishingManager.ChangeBait can legitimately return false (no exception) if its internal
		/// bait-selection window isn't open/ready yet — a brief retry clears that race. Delay
		/// between attempts (300ms).
		/// </summary>
		public const int BAIT_CHANGE_RETRY_DELAY_MS = 300;

		/// <summary>
		/// Bounded wait for the pole to be idle (PoleReady) before spending a bait-change attempt.
		/// ChangeBait's own ~5s internal waits just burn out if the line is in the water, and during
		/// Spectral Current the client re-deploys the line within a poll tick — so we sync each attempt
		/// to a real PoleReady moment. Returns immediately if already PoleReady; caps the wait otherwise.
		/// </summary>
		public const int BAIT_CHANGE_STATE_WAIT_MS = 1000;

		/// <summary>
		/// Max attempts for a single BaitChanger.ChangeBait call before giving up and logging a
		/// failure instead of silently casting with the wrong bait still equipped.
		/// </summary>
		public const int BAIT_CHANGE_MAX_ATTEMPTS = 3;

		/// <summary>
		/// FishingManager.State flips to PoleReady as soon as a catch resolves in memory, but the
		/// client's reel-in animation and tackle-box UI can still be settling for a moment after
		/// that. Observed live: calling ChangeBait immediately (zero delay) failed all
		/// BAIT_CHANGE_MAX_ATTEMPTS retries 100% of the time right after a catch — each attempt
		/// burning ~5s inside FishingManager.ChangeBait itself, ~16s wasted total, casting with
		/// the wrong bait every time. A short settle delay before the first attempt (not the retry
		/// loop, which already has its own delay) gives the client time to catch up. Only applies
		/// post-catch (State == PoleReady) — the very first cast of a stop (State == None) has
		/// nothing to settle from and doesn't need it.
		/// </summary>
		public const int POST_CATCH_BAIT_SETTLE_MS = 1500;

		/// <summary>
		/// After a hook, cap on how long to wait for the fishing state machine to actually advance past
		/// the catch (reach PoleReady, or show the line already back out) before looping to re-cast.
		/// Forces the states to update instead of blurring past a single poll tick — needed so the
		/// splashdown latch and ChangeBait's tackle-box window aren't raced by a fast Spectral re-deploy.
		/// Returns as soon as the state advances, so in practice it only spans the natural reel-in.
		/// </summary>
		public const int POST_HOOK_SETTLE_TIMEOUT_MS = 2000;

		// ========================================
		// BOAT QUEUE TIMING CONSTANTS
		// ========================================

		/// <summary>
		/// Early boat queue start time in minutes (0)
		/// </summary>
		public const int EARLY_QUEUE_MINUTE = 0;

		/// <summary>
		/// Late boat queue start time in minutes (13)
		/// </summary>
		public const int LATE_QUEUE_MINUTE = 13;

		/// <summary>
		/// Late boat queue end time in minutes (15)
		/// </summary>
		public const int LATE_QUEUE_END_MINUTE = 15;

		/// <summary>
		/// Standard queue start time in minutes (13)
		/// </summary>
		public const int QUEUE_START_MINUTE = 13;

		// ========================================
		// BOAT FISHING SPOT POSITIONS
		// ========================================

		/// <summary>
		/// Fishing spot positions on the boat
		/// </summary>
		public static readonly Vector3[] FishSpots = new Vector3[]
		{
			new Vector3(-7.541584f, 6.74677f, -7.7191f),
			new Vector3(-7.419403f, 6.73973f, -2.7815f),
			new Vector3(7.538965f, 6.745806f, -10.44607f),
			new Vector3(7.178741f, 6.749996f, -4.165483f),
			new Vector3(7.313677f, 6.711103f, -8.10146f),
			new Vector3(7.53893f, 6.745699f, 1.881091f)
		};

		/// <summary>
		/// Facing headings for each fishing spot
		/// </summary>
		public static readonly float[] Headings = new float[]
		{
			4.622331f,
			4.684318f,
			1.569952f,
			1.509215f,
			1.553197f,
			1.576235f
		};

		// ========================================
		// SUMMONING BELL LOCATIONS
		// ========================================

		/// <summary>
		/// Summoning bell locations across Eorzea for Lisbeth integration
		/// </summary>
		public static readonly (uint ZoneId, Vector3 Position, string Name)[] SummoningBells = new[]
		{
			(Zones.LimsaLominsaLowerDecks, new Vector3(-123.888062f, 17.990356f, 21.469421f), "Limsa Lominsa"),
			(Zones.Uldah, new Vector3(148.91272f, 3.982544f, -44.205383f), "Ul'dah"),
			(Zones.OldGridania, new Vector3(160.234863f, 15.671021f, -55.649719f), "Gridania"),
			(Zones.MorDhona, new Vector3(11.001709f, 28.976807f, -734.554077f), "Mor Dhona"),
			(Zones.Ishgard, new Vector3(-151.171204f, -12.64978f, -11.764771f), "Ishgard"),
			(Zones.Idyllshire, new Vector3(34.775269f, 208.148193f, -50.858398f), "Idyllshire"),
			(Zones.Kugane, new Vector3(19.394226f, 4.043579f, 53.025024f), "Kugane"),
			(Zones.RhalgrsReach, new Vector3(-57.633362f, -0.01532f, 49.30188f), "Rhalgr's Reach"),
			(Zones.Crystarium, new Vector3(-69.840576f, -7.705872f, 123.491211f), "Crystarium"),
			(Zones.Eulmore, new Vector3(7.186951f, 83.17688f, 31.448853f), "Eulmore")
		};

		// ========================================
		// IDENTICAL CAST TARGET FISH
		// ========================================

		/// <summary>
		/// Maps fish IDs to the required catch count for Identical Cast usage.
		/// Key: Fish ID, Value: Number of times to catch before moving on
		/// </summary>
		public static readonly Dictionary<uint, int> IdenticalCastTargets = new Dictionary<uint, int>
		{
			{ OceanFish.Gugrusaurus, 3 },
			{ OceanFish.Heavenskey, 2 },
			{ OceanFish.GreatGrandmarlin, 2 },
			{ OceanFish.CrimsonMonkfish, 2 },
			{ OceanFish.JetborneManta, 2 },
			{ OceanFish.BeatificVision, 3 },
			{ OceanFish.YanxianGoby, 2 },
			{ OceanFish.CatchingCarp, 3 },
			{ OceanFish.FleetingSquid, 2 },
			{ OceanFish.CrimsonKelp, 3 },
			{ OceanFish.Shoshitsuki, 2 },
			{ OceanFish.SilentShark, 2 },
			{ OceanFish.SunkenCoelacanth, 3 },
			{ OceanFish.PoetsPipe, 2 }
		};
	}
}
