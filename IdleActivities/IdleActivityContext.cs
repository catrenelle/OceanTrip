using System;
using System.Threading.Tasks;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Context object passed to all idle activities containing shared resources and state
	/// </summary>
	public class IdleActivityContext
	{
		/// <summary>
		/// Whether the bot is still free to craft (not interrupted for boat)
		/// </summary>
		public Func<bool> IsFreeToCraft { get; set; }

		/// <summary>
		/// Callback to execute Lisbeth orders
		/// </summary>
		public Func<int, int, string, string, int, bool, Task> ExecuteLisbethCallback { get; set; }

		/// <summary>
		/// Callback to get inventory count (normal + HQ)
		/// </summary>
		public Func<int, int> GetInventoryCountCallback { get; set; }

		/// <summary>
		/// Callback for logging
		/// </summary>
		public Action<string> LogCallback { get; set; }

		/// <summary>
		/// Whether logging mode is enabled
		/// </summary>
		public bool LoggingMode { get; set; }

		/// <summary>
		/// Food ID for Lisbeth (0 if none, includes HQ offset if HQ)
		/// </summary>
		public int LisbethFoodId { get; set; }
	}
}
