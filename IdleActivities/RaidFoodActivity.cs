using System.Threading.Tasks;
using ff14bot.Managers;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm raid food
	/// </summary>
	public class RaidFoodActivity : IIdleActivity
	{
		public int Priority => 60;
		public string Name => "Raid Food";

		private const int FOOD_THRESHOLD = 150;
		private const int FOOD_BATCH_SIZE = 50;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var foodList = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledFoodIds();

			foreach (var food in foodList)
			{
				if (!context.IsFreeToCraft())
					break;

				int currentCount = context.GetInventoryCountCallback(food);

				if (context.LoggingMode && currentCount < FOOD_THRESHOLD)
					context.LogCallback($"Farming {(FOOD_THRESHOLD - currentCount)} of {ItemDataCache.GetItemName((uint)food)} in increments of {FOOD_BATCH_SIZE}.");

				while (context.IsFreeToCraft() && currentCount < FOOD_THRESHOLD)
				{
					await context.ExecuteLisbethCallback(food, FOOD_BATCH_SIZE, "Culinarian", "false", context.LisbethFoodId, false);
					currentCount = context.GetInventoryCountCallback(food);
				}
			}
		}
	}
}
