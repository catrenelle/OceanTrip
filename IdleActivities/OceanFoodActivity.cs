using System.Threading.Tasks;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm ocean food (R'roneek Steak) for crafting
	/// </summary>
	public class OceanFoodActivity : IIdleActivity
	{
		public int Priority => 30;
		public string Name => "Ocean Food";

		private const int FOOD_THRESHOLD = 10;
		private const int FOOD_AMOUNT = 100;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			// Only execute if food ID was set (lisFood > 0 in original code)
			if (context.LisbethFoodId == 0)
				return;

			int hqOffset = 1000000;
			bool isHQ = context.LisbethFoodId >= hqOffset;
			int baseItemId = isHQ ? context.LisbethFoodId - hqOffset : context.LisbethFoodId;

			int foodCount = (int)DataManager.GetItem((uint)baseItemId, isHQ).ItemCount();

			if (context.IsFreeToCraft() && foodCount < FOOD_THRESHOLD)
			{
				if (context.LoggingMode)
					context.LogCallback($"Farming {FOOD_AMOUNT} of {ItemDataCache.GetItemName((uint)baseItemId, isHQ)}.");

				await context.ExecuteLisbethCallback(
					baseItemId,
					FOOD_AMOUNT,
					"Culinarian",
					"false",
					foodCount > 0 ? context.LisbethFoodId : 0,
					false
				);
			}
		}
	}
}
