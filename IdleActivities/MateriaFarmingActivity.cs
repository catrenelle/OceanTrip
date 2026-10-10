using System.Threading.Tasks;
using ff14bot.Managers;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm materia via scrip exchange
	/// </summary>
	public class MateriaFarmingActivity : IIdleActivity
	{
		public int Priority => 100; // Execute last
		public string Name => "Materia Farming";

		private const int MATERIA_THRESHOLD = 200;
		private const int MATERIA_BATCH_SIZE = 20;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!context.IsFreeToCraft())
				return;

			var materiaList = OceanTripPlanner.Settings.OceanTripSettings.Instance.GetEnabledMateriaIds();

			foreach (var materia in materiaList)
			{
				if (!context.IsFreeToCraft())
					break;

				int currentCount = context.GetInventoryCountCallback(materia);

				if (context.LoggingMode && currentCount <= MATERIA_THRESHOLD)
					context.LogCallback($"Farming {(MATERIA_THRESHOLD - currentCount)} of {ItemDataCache.GetItemName((uint)materia)} in increments of {MATERIA_BATCH_SIZE}.");

				while (context.IsFreeToCraft() && currentCount <= MATERIA_THRESHOLD)
				{
					await context.ExecuteLisbethCallback(materia, MATERIA_BATCH_SIZE, "Exchange", "false", 0, false);
					currentCount = context.GetInventoryCountCallback(materia);
				}
			}
		}
	}
}
