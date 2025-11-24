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
	}
}
