using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Strategy for restocking bait with configurable acquisition methods
	/// </summary>
	public class BaitRestockStrategy
	{
		private readonly bool _loggingEnabled;

		public BaitRestockStrategy(bool enableLogging = true)
		{
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Bait acquisition configuration
		/// </summary>
		private class BaitConfig
		{
			public uint BaitId { get; set; }
			public int Threshold { get; set; }
			public Func<int, uint> AmountCalculator { get; set; }
			public string AcquisitionMethod { get; set; }
			public string UseHQ { get; set; }
			public Func<bool> CanAcquire { get; set; }
		}

		/// <summary>
		/// Restock all required baits based on inventory thresholds
		/// </summary>
		public async Task RestockBait(int defaultThreshold, uint defaultAmount)
		{
			// Build configuration for each bait type
			var baitConfigs = new List<BaitConfig>
			{
				// Purchasable baits
				new BaitConfig { BaitId = FishBait.Ragworm, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.Krill, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.PlumpWorm, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.RatTail, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.GlowWorm, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.ShrimpCageFeeder, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.PillBug, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.StoneflyNymph, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Purchase", UseHQ = "true", CanAcquire = () => true },

				// Craftable bait (Heavy Steel Jig - requires GSM 36+)
				new BaitConfig { BaitId = FishBait.HeavySteelJig, Threshold = 5, AmountCalculator = (t) => 10, AcquisitionMethod = "Goldsmith", UseHQ = "true", CanAcquire = () => Core.Me.Levels[ClassJobType.Goldsmith] >= 36 },

				// Exchangeable baits
				new BaitConfig { BaitId = FishBait.SquidStrip, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Exchange", UseHQ = "true", CanAcquire = () => true },
				new BaitConfig { BaitId = FishBait.MackerelStrip, Threshold = defaultThreshold, AmountCalculator = (t) => defaultAmount, AcquisitionMethod = "Exchange", UseHQ = "true", CanAcquire = () => true }
			};

			// Determine which baits need restocking
			var baitsToRestock = baitConfigs
				.Where(config => PassTheTime.inventoryCount((int)config.BaitId) < config.Threshold && config.CanAcquire())
				.ToList();

			if (!baitsToRestock.Any())
				return;

			Log("Restocking bait with Lisbeth...");

			// Restock each bait
			foreach (var config in baitsToRestock)
			{
				uint amount = config.AmountCalculator(config.Threshold);
				await PassTheTime.IdleLisbeth(
					(int)config.BaitId,
					(int)amount,
					config.AcquisitionMethod,
					config.UseHQ,
					0,
					true
				);
			}

			Log("Restocking bait complete");
		}

		/// <summary>
		/// Internal logging method
		/// </summary>
		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text);
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
