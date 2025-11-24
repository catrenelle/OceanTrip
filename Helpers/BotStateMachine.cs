using System;
using Ocean_Trip.Definitions;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Bot state enum representing all possible states in the Ocean Trip lifecycle
	/// </summary>
	public enum BotState
	{
		// === INITIALIZATION ===
		Initializing,

		// === LAND PREPARATION STATES ===
		ProcessingFishInventory,    // Sell or desynth caught fish
		Repairing,
		RestockingBait,
		BuyingCordials,
		RefreshingData,             // Refresh bait, achievements, route data

		// === IDLE/WAITING STATES ===
		CheckingBoatSchedule,
		PassingTime,                // Lisbeth crafting while waiting
		WaitingForBoatTime,         // Waiting in open world for boat time
		OpenWorldFishing,           // Optional fishing while waiting

		// === BOARDING SEQUENCE ===
		SwitchingToFisher,
		NavigatingToLimsa,
		InteractingWithNPC,
		SelectingRoute,
		ConfirmingBoarding,
		WaitingInQueue,
		CommencingDuty,
		LoadingToBoat,

		// === ON-BOAT PRE-FISHING ===
		InitializingBoatFishing,    // Set fishing spot, initialize zone
		EatingFood,
		MovingToFishingSpot,

		// === FISHING CYCLE STATES ===
		CheckingBuffs,              // Cordials, Thaliak's Favor
		CheckingSpectral,           // Monitor for spectral weather changes
		ProcessingCaughtFish,       // Log fish, check for Identical Cast
		DeterminingCastType,        // Decide: Normal cast, Mooch, Mooch II, or Identical Cast
		SelectingBait,
		Casting,
		WaitingForBite,
		HookingFish,
		WaitingForReelComplete,     // Waiting for fishing animation (15 seconds)

		// === ZONE TRANSITION (ON BOAT) ===
		WaitingForNextZone,         // Between fishing zones on boat

		// === END OF VOYAGE ===
		VoyageEnding,               // Boat trip complete
		HandlingResultsScreen,      // Close results UI
		LoadingToLand,

		// === TERMINAL STATES ===
		Error,
		Stopped
	}

	/// <summary>
	/// Context class holding all state needed for state machine transitions
	/// </summary>
	public class BotStateContext
	{
		// Fishing spot information
		public int FishingSpot { get; set; }
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public RouteWithFish CurrentRoute { get; set; }

		// Bait information
		public ulong NormalBaitId { get; set; }
		public ulong SpectralBaitId { get; set; }

		// Spectral tracking
		public bool IsSpectral { get; set; }
		public bool SpectralJustChanged { get; set; }

		// Buff management timing
		public DateTime BuffLastChecked { get; set; }

		// Movement tracking
		public bool IsAtFishingSpot { get; set; }
		public bool HasEatenFood { get; set; }

		// Cast tracking
		public DateTime StartedCast { get; set; }
		public bool LastCastWasMooch { get; set; }

		// State transition flags
		public bool SkipBuffCheck { get; set; }
		public bool SkipBaitCheck { get; set; }
		public bool ForceImmediateCast { get; set; }

		public BotStateContext()
		{
			BuffLastChecked = DateTime.MinValue;
			IsSpectral = false;
			SpectralJustChanged = false;
			IsAtFishingSpot = false;
			HasEatenFood = false;
			LastCastWasMooch = false;
			SkipBuffCheck = false;
			SkipBaitCheck = false;
			ForceImmediateCast = false;
		}

		/// <summary>
		/// Reset context for a new fishing zone
		/// </summary>
		public void ResetForNewZone()
		{
			IsSpectral = false;
			SpectralJustChanged = false;
			IsAtFishingSpot = false;
			HasEatenFood = false;
			BuffLastChecked = DateTime.MinValue;
		}

		/// <summary>
		/// Reset flags after cast completes
		/// </summary>
		public void ResetCastFlags()
		{
			SkipBuffCheck = false;
			SkipBaitCheck = false;
			ForceImmediateCast = false;
			LastCastWasMooch = false;
		}
	}
}
