using System.Threading.Tasks;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm crystals, clusters, and shards
	/// </summary>
	public class CrystalFarmingActivity : IIdleActivity
	{
		public int Priority => 40;
		public string Name => "Crystal Farming";

		private const int CRYSTAL_THRESHOLD = 9000;
		private const int CRYSTAL_BATCH_SIZE = 500;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var enabledCrystals = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledCrystalIds();

			foreach (var (cluster, crystal, shard) in enabledCrystals)
			{
				// Process cluster
				await FarmCrystalType(context, cluster, "cluster");

				// Process crystal
				await FarmCrystalType(context, crystal, "crystal");

				// Process shard
				await FarmCrystalType(context, shard, "shard");
			}
		}

		private async Task FarmCrystalType(IdleActivityContext context, int itemId, string typeName)
		{
			if (!context.IsFreeToCraft())
				return;

			int currentCount = ConditionParser.ItemCount((uint)itemId);

			if (context.LoggingMode && currentCount < CRYSTAL_THRESHOLD)
				context.LogCallback($"Farming {(CRYSTAL_THRESHOLD - currentCount)} of {ItemDataCache.GetItemName((uint)itemId)} in bundles of {CRYSTAL_BATCH_SIZE}.");

			while (context.IsFreeToCraft() && currentCount < CRYSTAL_THRESHOLD)
			{
				await context.ExecuteLisbethCallback(itemId, CRYSTAL_BATCH_SIZE, "Gather", "false", 0, false);
				currentCount = ConditionParser.ItemCount((uint)itemId);
			}
		}
	}
}
