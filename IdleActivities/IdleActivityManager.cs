using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Manages and coordinates execution of all idle activities
	/// </summary>
	public class IdleActivityManager
	{
		private readonly List<IIdleActivity> _activities;

		public IdleActivityManager()
		{
			// Register all activities
			_activities = new List<IIdleActivity>
			{
				new ResumeLisbethActivity(),
				new CustomBoatOrderActivity(),
				new OceanFoodActivity(),
				new CrystalFarmingActivity(),
				new ScripFarmingActivity(),
				new RaidFoodActivity(),
				new PotionFarmingActivity(),
				new MaterialFarmingActivity(),
				new AethersandFarmingActivity(),
				new MateriaFarmingActivity()
			};

			// Sort by priority
			_activities = _activities.OrderBy(a => a.Priority).ToList();
		}

		/// <summary>
		/// Execute all registered idle activities in priority order
		/// </summary>
		/// <param name="context">Shared context for activities</param>
		public async Task ExecuteActivitiesAsync(IdleActivityContext context)
		{
			foreach (var activity in _activities)
			{
				// Check if we should stop (boat time)
				if (!context.IsFreeToCraft())
				{
					Log($"Stopping idle activities - boat time approaching.");
					break;
				}

				try
				{
					if (context.LoggingMode)
						Log($"Starting activity: {activity.Name}");

					await activity.ExecuteAsync(context);

					if (context.LoggingMode)
						Log($"Completed activity: {activity.Name}");
				}
				catch (Exception ex)
				{
					Log($"Error in activity '{activity.Name}': {ex.Message}");
					// Continue with next activity instead of failing completely
				}
			}
		}

		/// <summary>
		/// Add a custom activity at runtime
		/// </summary>
		public void RegisterActivity(IIdleActivity activity)
		{
			_activities.Add(activity);
			_activities.Sort((a, b) => a.Priority.CompareTo(b.Priority));
		}

		/// <summary>
		/// Remove an activity by type
		/// </summary>
		public void UnregisterActivity<T>() where T : IIdleActivity
		{
			_activities.RemoveAll(a => a is T);
		}

		private void Log(string text)
		{
			var msg = "[Ocean Trip] " + text;
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
