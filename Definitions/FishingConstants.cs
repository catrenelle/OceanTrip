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
		// GP MANAGEMENT CONSTANTS
		// ========================================

		/// <summary>
		/// GP threshold below which to use Cordials (400 GP)
		/// </summary>
		public const int CORDIAL_GP_THRESHOLD = 400;

		/// <summary>
		/// GP percentage threshold for low GP situations (25%)
		/// </summary>
		public const float LOW_GP_PERCENT = 25.0f;

		/// <summary>
		/// GP threshold below which to use Thaliak's Favor (200 GP)
		/// </summary>
		public const int THALIAK_GP_THRESHOLD = 200;

		/// <summary>
		/// GP buffer to prevent overfilling GP (100 GP)
		/// </summary>
		public const int FULL_GP_BUFFER = 100;

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
		/// Offset to adjust FishingManager.TimeSinceCast to match expected bite times (-0.3s)
		/// </summary>
		public const double BITE_TIMER_OFFSET = -0.3;

		/// <summary>
		/// Tolerance for matching fish by bite time (0.1s)
		/// Used as fallback when no exact match is found
		/// </summary>
		public const double BITE_TIME_TOLERANCE = 0.1;

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
		/// Delay before/after cordial use (600ms)
		/// </summary>
		public const int CORDIAL_USE_DELAY_MS = 600;

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
