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
		private string _lastLoggedReason;

		public BaitChanger(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Change to the specified bait. Returns true if the change is confirmed to have taken
		/// effect (or the requested bait was already equipped), false otherwise.
		/// </summary>
		public async Task<bool> ChangeBait(ulong baitId, string logMessage = null)
		{
			if ((baitId != FishingManager.SelectedBaitItemId)
				&& PassTheTime.inventoryCount((int)baitId) > 0
				&& (ItemDataCache.GetItemRequiredLevel((uint)baitId) <= Core.Me.ClassLevel))
			{
				if (!string.IsNullOrEmpty(logMessage))
					Log($"Changing bait to {_gameCache.GetItemName((uint)baitId)}: {logMessage}");
				else
					Log($"Changing bait to {_gameCache.GetItemName((uint)baitId)}");

				_lastLoggedReason = logMessage;

				// FishingManager.ChangeBait opens the tackle box, sends the bait-select action ONCE, then
				// waits ~5s for it to register — and that only works while the pole is idle. During
				// Spectral Current the client re-deploys the line within a poll tick, so the single select
				// fires into a churning state and doesn't stick (which is why spam-clicking the bait by
				// hand works: many selects, one lands in a live moment). We can't spam RB's single-select
				// internals, but we CAN sync each attempt to a genuine PoleReady window and retry between
				// every attempt instead of burning them all mid-churn.
				for (int attempt = 1; attempt <= FishingConstants.BAIT_CHANGE_MAX_ATTEMPTS; attempt++)
				{
					if (FishingManager.State != FishingState.PoleReady)
						await Coroutine.Wait(FishingConstants.BAIT_CHANGE_STATE_WAIT_MS,
							() => FishingManager.State == FishingState.PoleReady);

					bool changed = await FishingManager.ChangeBait((uint)baitId);
					if (changed && FishingManager.SelectedBaitItemId == baitId)
						return true;

					Log($"Bait change attempt {attempt}/{FishingConstants.BAIT_CHANGE_MAX_ATTEMPTS} did not " +
						$"stick (state {FishingManager.State}, still equipped " +
						$"{_gameCache.GetItemName(FishingManager.SelectedBaitItemId)}).", OceanLogLevel.Debug);

					if (attempt < FishingConstants.BAIT_CHANGE_MAX_ATTEMPTS)
						await Coroutine.Sleep(FishingConstants.BAIT_CHANGE_RETRY_DELAY_MS);
				}

				Logging.Write(System.Windows.Media.Colors.Red,
					$"[Ocean Trip] Failed to change bait to {_gameCache.GetItemName((uint)baitId)} after " +
					$"{FishingConstants.BAIT_CHANGE_MAX_ATTEMPTS} attempts — still equipped: " +
					$"{_gameCache.GetItemName(FishingManager.SelectedBaitItemId)}.");
				return false;
			}
			else if (PassTheTime.inventoryCount((int)baitId) == 0)
			{
				Log($"Out of {_gameCache.GetItemName((uint)baitId)}! Cannot change bait.");
				return false;
			}
			else if (!string.IsNullOrEmpty(logMessage) && logMessage != _lastLoggedReason)
			{
				Log($"Keeping {_gameCache.GetItemName((uint)baitId)}: {logMessage}");
				_lastLoggedReason = logMessage;
			}

			return true;
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
