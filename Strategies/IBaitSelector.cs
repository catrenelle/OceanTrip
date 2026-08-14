using System.Collections.Generic;
using System.Threading.Tasks;
using Ocean_Trip.Definitions;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Interface for bait selection strategies (normal vs spectral fishing)
	/// </summary>
	public interface IBaitSelector
	{
		/// <summary>
		/// Select the appropriate bait based on current fishing conditions
		/// </summary>
		/// <param name="context">Current bait selection context including location, time, route, etc.</param>
		/// <returns>Task representing the async bait selection operation</returns>
		Task SelectBait(BaitSelectionContext context);
	}

	/// <summary>
	/// Context class containing all information needed for bait selection
	/// </summary>
	public class BaitSelectionContext
	{
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public ulong DefaultBaitId { get; set; }
		public RouteWithFish CurrentRoute { get; set; }
		public HashSet<uint> MissingFish { get; set; }
		public List<uint> CaughtFish { get; set; }
		public bool FocusFishLog { get; set; }
		public string CurrentWeather { get; set; }

		/// <summary>
		/// True when the PREVIOUS stop never saw a spectral current — the community-confirmed
		/// Ocean Fishing pity rule then makes THIS stop's spectral current run longer (3 min
		/// instead of 2) with rising trigger odds on every spectral-fish catch, and the bonus
		/// doesn't stack (missing a pity-boosted current again just repeats the same boost next
		/// stop, not a bigger one) — so this is the best this trigger opportunity will ever be.
		/// Set once per stop transition in OceanTrip's main loop from _spectralPityActive.
		/// </summary>
		public bool SpectralPityActive { get; set; }

		/// <summary>
		/// True while on the 3rd/final stop of the voyage (Endeavor.CurrentZone == 2) — there's no
		/// next stop left to carry an unclaimed pity bonus into, so a spectral current missed here
		/// is gone for the rest of the voyage, not just delayed. See NormalBaitSelector.NeedsSpectral.
		/// </summary>
		public bool IsLastStop { get; set; }

		/// <summary>
		/// True once a spectral current has already triggered at THIS stop — Ocean Fishing allows
		/// at most one trigger per stop, so there's nothing left to chase trigger-bait for here
		/// regardless of pity/last-stop/points considerations. Mirrors OceanTrip's
		/// _hadSpectralThisStop (reset on stop transition, set once spectral is observed active).
		/// </summary>
		public bool SpectralAlreadyTriggeredThisStop { get; set; }

		/// <summary>
		/// When non-zero, overrides the goal fish for bait selection in the matching zone.
		/// Set from the Target Fish setting when the bot is in the target fish's zone.
		/// </summary>
		public uint TargetFishId { get; set; }

		/// <summary>
		/// Set by bait selectors when they are targeting a mooch chain (e.g., catch source fish → mooch → blue fish).
		/// FishingSessionManager checks this before mooching — prevents blind mooch loops.
		/// </summary>
		public bool ShouldMooch { get; set; }

		/// <summary>
		/// When non-zero, the specific fish this cast is chasing as a prerequisite (e.g. the source
		/// fish for a mooch chain, or an Intuition prereq). Lets HookingStrategy decline bites that
		/// aren't this fish instead of catching whatever bites and derailing the chain.
		/// </summary>
		public uint ChainCastTargetFishId { get; set; }

		/// <summary>
		/// When non-zero, the specific fish the *mooch* off this cast's catch is chasing (only
		/// relevant once ShouldMooch fires and the following cast is a mooch, not a plain cast).
		/// </summary>
		public uint ChainMoochTargetFishId { get; set; }

		/// <summary>
		/// Call when the bait swap for a chain attempt fails (ChangeBait returned false) — the old
		/// bait is still equipped, so the chain target set for the intended bait no longer applies.
		/// Leaving it set makes HookingStrategy decline every bite for the rest of the stop since
		/// the wrong fish never matches the stale target.
		/// </summary>
		public void ClearChainTargets()
		{
			ShouldMooch = false;
			ChainCastTargetFishId = 0;
			ChainMoochTargetFishId = 0;
		}
	}
}
