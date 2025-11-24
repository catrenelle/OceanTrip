using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ff14bot.Managers;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm aethersands (both gatherable and exchangeable types)
	/// </summary>
	public class AethersandFarmingActivity : IIdleActivity
	{
		public int Priority => 90;
		public string Name => "Aethersand Farming";

		private const int AETHERSAND_THRESHOLD = 300;
		private const int AETHERSAND_BATCH_SIZE = 50;

		// Indices 5 and 8 are exchangeable in the aethersands array
		private static readonly List<int> ExchangeableIndices = new List<int> { 5, 8 };

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var aethersandList = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledAethersandIds();

			// Categorize aethersands
			var gatherableAethersands = new List<int>();
			var exchangeableAethersands = new List<int>();

			foreach (var aethersandId in aethersandList)
			{
				// Determine if this aethersand is exchangeable by checking its position in the Defaults array
				int indexInDefaults = Array.IndexOf(Defaults.aethersands, aethersandId);
				if (indexInDefaults >= 0 && ExchangeableIndices.Contains(indexInDefaults + 1))
					exchangeableAethersands.Add(aethersandId);
				else
					gatherableAethersands.Add(aethersandId);
			}

			// Process gatherable aethersands
			foreach (var item in gatherableAethersands)
			{
				if (!context.IsFreeToCraft())
					break;

				await FarmAethersand(context, item, "Gather");
			}

			// Process exchangeable aethersands
			foreach (var item in exchangeableAethersands)
			{
				if (!context.IsFreeToCraft())
					break;

				await FarmAethersand(context, item, "Exchange");
			}
		}

		private async Task FarmAethersand(IdleActivityContext context, int itemId, string type)
		{
			int currentCount = context.GetInventoryCountCallback(itemId);

			if (context.LoggingMode && currentCount <= AETHERSAND_THRESHOLD)
				context.LogCallback($"Farming {(AETHERSAND_THRESHOLD - currentCount)} of {ItemDataCache.GetItemName((uint)itemId)} in increments of {AETHERSAND_BATCH_SIZE}.");

			while (context.IsFreeToCraft() && currentCount <= AETHERSAND_THRESHOLD)
			{
				await context.ExecuteLisbethCallback(itemId, AETHERSAND_BATCH_SIZE, type, "false", context.LisbethFoodId, false);
				currentCount = context.GetInventoryCountCallback(itemId);
			}
		}
	}
}
