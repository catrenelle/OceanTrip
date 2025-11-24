using System.Threading.Tasks;
using ff14bot.Managers;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm crafting materials
	/// </summary>
	public class MaterialFarmingActivity : IIdleActivity
	{
		public int Priority => 80;
		public string Name => "Material Farming";

		private const int MATERIAL_THRESHOLD = 300;
		private const int MATERIAL_BATCH_SIZE = 50;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var itemList = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledMaterialIds();

			foreach (var item in itemList)
			{
				if (!context.IsFreeToCraft())
					break;

				int currentCount = context.GetInventoryCountCallback(item);

				if (context.LoggingMode && currentCount <= MATERIAL_THRESHOLD)
					context.LogCallback($"Farming {(MATERIAL_THRESHOLD - currentCount)} of {ItemDataCache.GetItemName((uint)item)} in increments of {MATERIAL_BATCH_SIZE}.");

				while (context.IsFreeToCraft() && currentCount <= MATERIAL_THRESHOLD)
				{
					await context.ExecuteLisbethCallback(item, MATERIAL_BATCH_SIZE, "Exchange", "false", context.LisbethFoodId, false);
					currentCount = context.GetInventoryCountCallback(item);
				}
			}
		}
	}
}
