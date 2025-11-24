using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Farm crafter scrips
	/// </summary>
	public class ScripFarmingActivity : IIdleActivity
	{
		public int Priority => 50;
		public string Name => "Scrip Farming";

		private const int SCRIP_THRESHOLD = 3000;
		private const int SCRIP_BATCH_SIZE = 500;

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!OceanTripNewSettings.Instance.refillScrips || !context.IsFreeToCraft())
				return;

			var currencyList = new List<int>
			{
				(int)Currency.PurpleCraftersScrips,
				(int)Currency.OrangeCraftersScrips
			};

			foreach (var currency in currencyList)
			{
				if (!context.IsFreeToCraft())
					break;

				int currentAmount = (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency);

				if (context.LoggingMode && currentAmount <= SCRIP_THRESHOLD)
				{
					var currencyStorage = SpecialCurrencyManager.SpecialCurrencies
						.FirstOrDefault(x => x.Item != null && x.Item.Id == (uint)currency);
					var currencyName = currencyStorage.Item != null ? currencyStorage.Item.CurrentLocaleName : $"Currency {currency}";
					context.LogCallback($"Farming {(SCRIP_THRESHOLD - currentAmount)} of {currencyName}.");
				}

				while (context.IsFreeToCraft() && currentAmount <= SCRIP_THRESHOLD)
				{
					await context.ExecuteLisbethCallback(currency, SCRIP_BATCH_SIZE, "CraftMasterpiece", "false", 0, false);
					currentAmount = (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency);
				}
			}
		}
	}
}
