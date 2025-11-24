using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Strategy for cordial selection and usage with configurable priority
	/// </summary>
	public class CordialStrategy
	{
		private readonly GameStateCache _gameCache;
		private readonly bool _loggingEnabled;

		/// <summary>
		/// Cordial types in priority order (highest to lowest quality)
		/// </summary>
		private static readonly uint[] CordialPriority = new uint[]
		{
			Cordials.HiCordial,        // Best: 400 GP recovery
			Cordials.Cordial,          // Mid: 300 GP recovery
			Cordials.WateredCordial    // Lowest: 200 GP recovery
		};

		public CordialStrategy(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache ?? throw new ArgumentNullException(nameof(gameCache));
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Find and use the best available cordial based on priority
		/// </summary>
		/// <returns>True if a cordial was used, false otherwise</returns>
		public async Task<bool> UseBestAvailableCordial()
		{
			uint cordialToUse = SelectBestCordial();

			if (cordialToUse == 0)
			{
				Log("No cordials available in inventory", OceanLogLevel.Debug);
				return false;
			}

			return await UseCordial(cordialToUse);
		}

		/// <summary>
		/// Select the best available cordial from inventory based on priority
		/// </summary>
		/// <returns>Cordial ID to use, or 0 if none available</returns>
		private uint SelectBestCordial()
		{
			// Check cordials in priority order and return the first one found
			foreach (var cordialId in CordialPriority)
			{
				if (HasCordialInInventory(cordialId))
					return cordialId;
			}

			return 0; // No cordials available
		}

		/// <summary>
		/// Check if a specific cordial exists in inventory
		/// </summary>
		private bool HasCordialInInventory(uint cordialId)
		{
			return DataManager.GetItem(cordialId).ItemCount() > 0;
		}

		/// <summary>
		/// Use a specific cordial from inventory
		/// </summary>
		/// <param name="cordialId">The cordial ID to use</param>
		/// <returns>True if successfully used, false otherwise</returns>
		private async Task<bool> UseCordial(uint cordialId)
		{
			var slot = InventoryManager.FilledSlots.FirstOrDefault(x => x.RawItemId == cordialId);
			if (slot == null)
			{
				Log($"Cordial {cordialId} not found in filled slots", OceanLogLevel.Debug);
				return false;
			}

			await Coroutine.Sleep(FishingConstants.CORDIAL_USE_DELAY_MS);

			if (slot.UseItem())
			{
				string cordialName = _gameCache.GetItemName(cordialId);
				Log($"Used a {cordialName}!");
				await Coroutine.Sleep(FishingConstants.CORDIAL_USE_DELAY_MS);
				return true;
			}

			Log($"Failed to use cordial {cordialId}", OceanLogLevel.Debug);
			return false;
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
