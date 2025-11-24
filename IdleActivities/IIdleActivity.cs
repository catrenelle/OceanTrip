using System.Threading.Tasks;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Base interface for all idle activities that can be performed between boat trips
	/// </summary>
	public interface IIdleActivity
	{
		/// <summary>
		/// Execute the idle activity
		/// </summary>
		/// <param name="context">Context containing shared resources and callbacks</param>
		/// <returns>Task representing the async operation</returns>
		Task ExecuteAsync(IdleActivityContext context);

		/// <summary>
		/// Priority order for execution (lower = earlier)
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// Human-readable name for logging
		/// </summary>
		string Name { get; }
	}
}
