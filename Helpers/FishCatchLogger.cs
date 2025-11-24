using System;
using OceanTripPlanner.Definitions;
using Ocean_Trip.Helpers;
using OceanTrip;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Handles logging of caught fish and updating fish lists
	/// Separates fish tracking concerns from identical cast logic
	/// </summary>
	public class FishCatchLogger
	{
		private readonly GameStateCache _gameCache;
		private readonly Action<string, OceanLogLevel> _logAction;

		public FishCatchLogger(GameStateCache gameCache, Action<string, OceanLogLevel> logAction)
		{
			_gameCache = gameCache ?? throw new ArgumentNullException(nameof(gameCache));
			_logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
		}

		/// <summary>
		/// Check if a new fish was caught and log it
		/// </summary>
		/// <param name="lastCaughtFish">Reference to the last caught fish ID (will be updated)</param>
		/// <param name="caughtFishLogged">Reference to the caught fish logged flag (will be updated)</param>
		/// <param name="logDetails">Whether to log additional fish details (size, stars, etc.)</param>
		/// <returns>True if a new fish was logged, false otherwise</returns>
		//public bool LogCaughtFish(ref uint lastCaughtFish, ref bool caughtFishLogged, bool logDetails = false)
		//{
		//	_logAction("Checking for a recently caught fish.", OceanLogLevel.Debug);

		//	// Cache LastFishCaught to avoid reading game memory multiple times (expensive operation)
		//	var currentFish = FishingLog.LastFishCaught;

		//	// Did we catch a fish? Let's log it.
		//	if (lastCaughtFish != currentFish && !caughtFishLogged)
		//	{
		//		lastCaughtFish = currentFish;
		//		caughtFishLogged = true;

		//		// Use Catch.FishName if available, otherwise fall back to gameCache lookup
		//		string fishName = FishingLog.LastFishName;
		//		if (string.IsNullOrEmpty(fishName))
		//			fishName = _gameCache.GetItemName(currentFish);

		//		// Build log message
		//		string logMessage = $"Caught {fishName}";

		//		// Optionally add catch details from new API
		//		if (logDetails)
		//		{
		//			var details = FishingLog.GetCatchDetails();
		//			if (details.Size > 0 || details.Stars > 0)
		//			{
		//				logMessage += $" (Size: {details.Size:F1}cm";
		//				if (details.IsLarge)
		//					logMessage += " - Large";
		//				if (details.Stars > 0)
		//					logMessage += $", {details.Stars} star{(details.Stars > 1 ? "s" : "")}";
		//				logMessage += ")";
		//			}
		//		}

		//		_logAction($"{logMessage}.", OceanLogLevel.Info);

		//		// Remove from missing fish list if needed
		//		if (FishingLog.MissingFish().Contains(currentFish))
		//			FishingLog.RemoveFish(currentFish);

		//		_logAction("Done checking for a recently caught fish.", OceanLogLevel.Debug);
		//		return true;
		//	}

		//	_logAction("Done checking for a recently caught fish.", OceanLogLevel.Debug);
		//	return false;
		//}
	}
}
