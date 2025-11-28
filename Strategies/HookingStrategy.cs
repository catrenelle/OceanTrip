using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Enums;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;
using ff14bot.Helpers;
using System.Windows.Media;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Strategy for determining and executing the appropriate hook action based on fishing conditions
	/// </summary>
	public class HookingStrategy
	{
		private readonly GameStateCache _gameCache;
		private readonly bool _loggingEnabled;

		public HookingStrategy(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Handle fish bite - determine hook type and execute the appropriate action
		/// </summary>
		/// <param name="context">Context containing fishing state and bite information</param>
		public Task ExecuteHook(HookContext context)
		{
			double biteElapsed = Math.Round(context.BiteElapsedSeconds, 1);
			bool doubleHook = false;

			// Cache current weather to avoid repeated API calls in LINQ queries
			string currentWeather = _gameCache.CurrentWeather;

			// Build fish lists for bite prediction - first try exact match, then fallback to nearest
			List<Fish> spectralFishToCatch = FindMatchingFish(
				context.CurrentRoute?.SpectralFish,
				biteElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: false);

			List<Fish> normalFishToCatch = FindMatchingFish(
				context.CurrentRoute?.NormalFish,
				biteElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: true);

			var potentialFish = String.Join(", ", (context.Spectraled ? spectralFishToCatch : normalFishToCatch).Select(x => _gameCache.GetItemName((uint)x.FishID)).ToList());

			if (Core.Player.HasAura(CharacterAuras.Chum))
				potentialFish = "Cannot predict due to Chum.";

			Log($"Bite Time: {biteElapsed:F1}s, Potential Fish: {(String.IsNullOrWhiteSpace(potentialFish) ? "Unable to determine" : potentialFish)}");

			Log("Checking if we should double hook based on bite timer and current fishing conditions!", OceanLogLevel.Debug);

			// Determine if Double/Triple Hook should be used for points
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Points || OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto)
			{
				// Special handling for South's lastMooch rule - Always DH/TH after a Mooch in South if spectral.
				if (context.Location == "south" && context.LastCastMooch && (context.TimeOfDay == "Sunset" || context.TimeOfDay == "Night") && context.Spectraled)
				{
					doubleHook = true;
				}
				else
				{
					// Find matching fish for DH/TH decision with fallback to nearest
					var matchingFish = FindMatchingFishForHook(context.Location, biteElapsed, context.TimeOfDay, currentWeather);
					doubleHook = matchingFish.Any(x =>
						((x.Points * x.THBonus > 600 && x.THBonus > 1) || (x.Points * x.DHBonus > 400 && x.DHBonus > 1)) || (x.THBonus > 5 || x.DHBonus > 3));
				}
			}

			Log("Done checking for double hook conditions.", OceanLogLevel.Debug);

			// Execute the appropriate hook action
			if (doubleHook && ActionManager.CanCast(Actions.DoubleHook, Core.Me))
			{
				if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
				{
					Log("Using Triple Hook!");
					ActionManager.DoAction(Actions.TripleHook, Core.Me);
				}
				else
				{
					Log("Using Double Hook!");
					ActionManager.DoAction(Actions.DoubleHook, Core.Me);
				}

				context.OnHookExecuted(false);
			}
			else if (FishingManager.HasPatience)
			{
				Log("Player has patience on them. Need to use special hooking.", OceanLogLevel.Debug);

				if (FishingManager.TugType == TugType.Light)
				{
					Log($"Using Precision Hookset!", OceanLogLevel.Debug);

					ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
					context.OnHookExecuted(false);
				}
				else
				{
					Log($"Using Powerful Hookset!", OceanLogLevel.Debug);

					ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
					context.OnHookExecuted(false);
				}
			}
			else
			{
				Log("Checking if Full GP action is Double Hook.", OceanLogLevel.Debug);

				if (!context.Spectraled && _gameCache.MaxGP >= 500 && (_gameCache.GPDeficit <= FishingConstants.FULL_GP_BUFFER) && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && OceanTripNewSettings.Instance.FullGPAction == FullGPAction.DoubleHook)
				{
					if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
					{
						Log("Triggering Full GP Action to keep regen going - Triple Hook!");
						ActionManager.DoAction(Actions.TripleHook, Core.Me);
					}
					else
					{
						Log("Triggering Full GP Action to keep regen going - Double Hook!");
						ActionManager.DoAction(Actions.DoubleHook, Core.Me);
					}
				}
				else
				{
					Log($"Hooking Fish!", OceanLogLevel.Debug);

					FishingManager.Hook();
				}

				context.OnHookExecuted(false);
			}

			Log("Refreshing UI for Bait and Achievements in case something changed.", OceanLogLevel.Debug);

			context.OnHookExecuted(true); // Mark fish as caught and ready to log
			FFXIV_Databinds.Instance.RefreshBait();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Find matching fish from a list, with fallback to nearest fish if no exact match
		/// </summary>
		private List<Fish> FindMatchingFish(IEnumerable<Fish> fishList, double biteElapsed, string timeOfDay, string currentWeather, bool excludeWeather)
		{
			if (fishList == null)
				return new List<Fish>();

			// Filter by time of day, weather (if applicable), and tug type
			var eligibleFish = fishList.Where(x =>
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.BiteType == FishingManager.TugType &&
				(!excludeWeather || (x.WeatherExclusion1 != currentWeather && x.WeatherExclusion2 != currentWeather))).ToList();

			// First try exact match (within bite range)
			var exactMatches = eligibleFish.Where(x =>
				x.BiteStart <= biteElapsed && x.BiteEnd >= biteElapsed).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed);
		}

		/// <summary>
		/// Find matching fish for hook decision from all fish data, with fallback to nearest
		/// </summary>
		private List<Fish> FindMatchingFishForHook(string location, double biteElapsed, string timeOfDay, string currentWeather)
		{
			var allFish = FishDataCache.GetFish();

			// Filter by location, time of day, weather, and tug type
			var eligibleFish = allFish.Where(x =>
				x.RouteShortName == location &&
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.WeatherExclusion1 != currentWeather &&
				x.WeatherExclusion2 != currentWeather &&
				x.BiteType == FishingManager.TugType).ToList();

			// First try exact match (within bite range)
			var exactMatches = eligibleFish.Where(x =>
				x.BiteStart <= biteElapsed && x.BiteEnd >= biteElapsed).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed);
		}

		/// <summary>
		/// Find the nearest fish by bite time within the configured tolerance
		/// </summary>
		private List<Fish> FindNearestFish(List<Fish> eligibleFish, double biteElapsed)
		{
			if (!eligibleFish.Any())
				return new List<Fish>();

			// Calculate distance to each fish's bite range
			var fishWithDistance = eligibleFish.Select(x =>
			{
				double distance;
				if (biteElapsed < x.BiteStart)
					distance = x.BiteStart - biteElapsed;
				else if (biteElapsed > x.BiteEnd)
					distance = biteElapsed - x.BiteEnd;
				else
					distance = 0; // Within range (shouldn't happen as we check exact matches first)

				return new { Fish = x, Distance = distance };
			}).ToList();

			// Find minimum distance
			double minDistance = fishWithDistance.Min(x => x.Distance);

			// Only return fish within tolerance
			if (minDistance > FishingConstants.BITE_TIME_TOLERANCE)
				return new List<Fish>();

			// Return all fish at the minimum distance (could be multiple with same bite time)
			return fishWithDistance.Where(x => Math.Abs(x.Distance - minDistance) < 0.001).Select(x => x.Fish).ToList();
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

	/// <summary>
	/// Context for hook execution containing all necessary state
	/// </summary>
	public class HookContext
	{
		public double BiteElapsedSeconds { get; set; }
		public bool Spectraled { get; set; }
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public RouteWithFish CurrentRoute { get; set; }
		public bool LastCastMooch { get; set; }

		private Action<bool> _onHookExecutedCallback;

		/// <summary>
		/// Set callback to be invoked after hook is executed
		/// </summary>
		public void SetHookExecutedCallback(Action<bool> callback)
		{
			_onHookExecutedCallback = callback;
		}

		/// <summary>
		/// Invoke the hook executed callback
		/// </summary>
		public void OnHookExecuted(bool caughtFishLogged)
		{
			_onHookExecutedCallback?.Invoke(caughtFishLogged);
		}
	}
}
