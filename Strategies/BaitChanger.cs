using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using Ocean_Trip;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;
using TreeSharp;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Handles bait changing operations for fishing
	/// </summary>
	public class BaitChanger
	{
		private readonly GameStateCache _gameCache;
		private readonly bool _loggingEnabled;

		public BaitChanger(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Change to the specified bait
		/// </summary>
		public async Task ChangeBait(ulong baitId, string logMessage = null)
		{
			if ((baitId != FishingManager.SelectedBaitItemId)
				&& PassTheTime.inventoryCount((int)baitId) > 0
				&& (ItemDataCache.GetItemRequiredLevel((uint)baitId) <= Core.Me.ClassLevel))
			{
				if (!string.IsNullOrEmpty(logMessage))
					Log(logMessage, OceanLogLevel.Debug);
				else
					Log($"Changing Bait!", OceanLogLevel.Debug);

				await FishingManager.ChangeBait((uint)baitId);

				Log($"Finished Changing Bait!", OceanLogLevel.Debug);
			}
			else if (PassTheTime.inventoryCount((int)baitId) == 0)
			{
				Log($"Out of {_gameCache.GetItemName((uint)baitId)}! Cannot change bait.");
			}
		}

		/// <summary>
		/// Internal logging method
		/// </summary>
		public void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text);
			Logging.Write(System.Windows.Media.Colors.Aqua, msg);
		}
	}
}
