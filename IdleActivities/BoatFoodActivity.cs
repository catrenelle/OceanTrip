using System.Threading.Tasks;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm Nasi Goreng, the food eaten when boarding the boat if the "Ocean Food" trip option is enabled
	/// </summary>
	public class BoatFoodActivity : IIdleActivity
	{
		public int Priority => 35;
		public string Name => "Boat Food";

		private const int FOOD_THRESHOLD = 5;
		private const int FOOD_AMOUNT = 20;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!OceanTripNewSettings.Instance.restockOceanFood)
				return;

			int foodId = (int)OceanFood.NasiGoreng;
			int foodCount = context.GetInventoryCountCallback(foodId);

			if (context.IsFreeToCraft() && foodCount < FOOD_THRESHOLD)
			{
				if (context.LoggingMode)
					context.LogCallback($"Farming {FOOD_AMOUNT} of {ItemDataCache.GetItemName((uint)foodId)}.");

				await context.ExecuteLisbethCallback(foodId, FOOD_AMOUNT, "Culinarian", "false", context.LisbethFoodId, false);
			}
		}
	}
}
