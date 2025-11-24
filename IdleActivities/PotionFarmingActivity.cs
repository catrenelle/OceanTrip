using System.Threading.Tasks;
using ff14bot.Managers;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm raid potions
	/// </summary>
	public class PotionFarmingActivity : IIdleActivity
	{
		public int Priority => 70;
		public string Name => "Potion Farming";

		private const int POTION_THRESHOLD = 200;
		private const int POTION_BATCH_SIZE = 200;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var potionList = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledPotionIds();

			foreach (var potion in potionList)
			{
				if (!context.IsFreeToCraft())
					break;

				int currentCount = context.GetInventoryCountCallback(potion);

				if (context.LoggingMode && currentCount <= POTION_THRESHOLD)
					context.LogCallback($"Farming {POTION_BATCH_SIZE} of {ItemDataCache.GetItemName((uint)potion)}.");

				while (context.IsFreeToCraft() && currentCount <= POTION_THRESHOLD)
				{
					await context.ExecuteLisbethCallback(potion, POTION_BATCH_SIZE, "Alchemist", "false", context.LisbethFoodId, false);
					currentCount = context.GetInventoryCountCallback(potion);
				}
			}
		}
	}
}
