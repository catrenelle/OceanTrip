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
			bool hasChum = Core.Player.HasAura(CharacterAuras.Chum);

			// Chum reduces bite time by ~50% — double observed time to match our database windows
			double matchElapsed = hasChum ? biteElapsed * 2.0 : biteElapsed;

			// Cache current weather to avoid repeated API calls in LINQ queries
			string currentWeather = _gameCache.CurrentWeather;

			// Build fish lists for bite prediction - first try exact match, then fallback to nearest
			List<Fish> spectralFishToCatch = FindMatchingFish(
				context.CurrentRoute?.SpectralFish,
				matchElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: false);

			List<Fish> normalFishToCatch = FindMatchingFish(
				context.CurrentRoute?.NormalFish,
				matchElapsed,
				context.TimeOfDay,
				currentWeather,
				excludeWeather: true);

			var matchedFish = context.Spectraled ? spectralFishToCatch : normalFishToCatch;
			var potentialFish = String.Join(", ", matchedFish.Select(x => _gameCache.GetItemName((uint)x.FishID)).ToList());

			// Check if this is a fuzzy match (nearest fish outside its expected window)
			bool isFuzzy = false;
			if (matchedFish.Any())
			{
				var firstFish = matchedFish.First();
				var (start, end) = firstFish.GetBiteRange(FishingManager.SelectedBaitItemId);
				isFuzzy = matchElapsed < start || matchElapsed > end;
			}

			string tugName = FishingManager.TugType == TugType.Light ? "!" : FishingManager.TugType == TugType.Medium ? "!!" : "!!!";
			string fuzzyTag = isFuzzy ? " (fuzzy)" : "";
			string chumTag = hasChum ? $" (Chum: raw {biteElapsed:F1}s)" : "";
			Log($"Bite Time: {matchElapsed:F1}s, Tug: {tugName}, Potential Fish: {(String.IsNullOrWhiteSpace(potentialFish) ? "Unable to determine" : potentialFish)}{fuzzyTag}{chumTag}");

			if (isFuzzy)
				LogExcludedFish(context, currentWeather);

			Log("Checking if we should double hook based on bite timer and current fishing conditions!", OceanLogLevel.Debug);

			// DH/TH cannot be used during Patience — fish always escapes without Precision/Powerful Hookset
			if (!FishingManager.HasPatience)
			{
				if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
				{
					// Achievement mode: DH/TH when predicted fish matches the target achievement category
					var achievementFocus = AchievementFishDataCache.GetCurrentAchievementFocus();
					if (achievementFocus != AchievementType.None)
					{
						var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
						doubleHook = matchingFish.Any(f =>
							!string.IsNullOrEmpty(f.Achievement) &&
							AchievementFishDataCache.MapAchievementString(f.Achievement) == achievementFocus);
					}

					// Only fall back to points-based DH/TH if no achievement fish can spawn here at all
					if (!doubleHook)
					{
						bool achievementFishAvailable = AchievementFishDataCache.GetFishForLocation(context.Location, achievementFocus)
							.Any(f => f.TimeOfDayExclusion1 != context.TimeOfDay &&
								f.TimeOfDayExclusion2 != context.TimeOfDay &&
								f.WeatherExclusion1 != currentWeather &&
								f.WeatherExclusion2 != currentWeather);

						if (!achievementFishAvailable)
						{
							var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
							doubleHook = matchingFish.Any(x =>
								((x.Points * x.THBonus > 600 && x.THBonus > 1) || (x.Points * x.DHBonus > 400 && x.DHBonus > 1)) || (x.THBonus > 5 || x.DHBonus > 3));
						}
					}
				}
				else if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Points || OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto)
				{
					// Special handling for South's lastMooch rule - Always DH/TH after a Mooch in South if spectral.
					if (context.Location == "south" && context.LastCastMooch && (context.TimeOfDay == "Sunset" || context.TimeOfDay == "Night") && context.Spectraled)
					{
						doubleHook = true;
					}
					else
					{
						// Find matching fish for DH/TH decision with fallback to nearest
						var matchingFish = FindMatchingFishForHook(context.Location, matchElapsed, context.TimeOfDay, currentWeather);
						doubleHook = matchingFish.Any(x =>
							((x.Points * x.THBonus > 600 && x.THBonus > 1) || (x.Points * x.DHBonus > 400 && x.DHBonus > 1)) || (x.THBonus > 5 || x.DHBonus > 3));
					}
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

				var predictedFish = context.Spectraled ? spectralFishToCatch : normalFishToCatch;
				var hooksetFish = predictedFish.FirstOrDefault(f => (int)f.HooksetType != 0);

				bool usePrecision;
				if (hooksetFish != null)
				{
					usePrecision = hooksetFish.HooksetType == TugType.Light;
					Log($"Hookset override from {_gameCache.GetItemName((uint)hooksetFish.FishID)}: {(usePrecision ? "Precision" : "Powerful")}");
				}
				else
				{
					usePrecision = FishingManager.TugType == TugType.Light;
				}

				if (usePrecision)
				{
					Log($"Using Precision Hookset!", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
				}
				else
				{
					Log($"Using Powerful Hookset!", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
				}
				context.OnHookExecuted(false);
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

			uint currentBait = FishingManager.SelectedBaitItemId;
			bool hasIntuition = Core.Player.HasAura(CharacterAuras.FishersIntuition);

			// Filter by time of day, weather (if applicable), tug type, and intuition requirement
			var eligibleFish = fishList.Where(x =>
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.BiteType == FishingManager.TugType &&
				(!excludeWeather || (x.WeatherExclusion1 != currentWeather && x.WeatherExclusion2 != currentWeather)) &&
				(!x.RequiresIntuition || hasIntuition)).ToList();

			// First try exact match using bite range for current bait
			var exactMatches = eligibleFish.Where(x =>
			{
				var (start, end) = x.GetBiteRange(currentBait);
				return start <= biteElapsed && end >= biteElapsed;
			}).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed, currentBait);
		}

		/// <summary>
		/// Find matching fish for hook decision from all fish data, with fallback to nearest
		/// </summary>
		private List<Fish> FindMatchingFishForHook(string location, double biteElapsed, string timeOfDay, string currentWeather)
		{
			var allFish = FishDataCache.GetFish();
			uint currentBait = FishingManager.SelectedBaitItemId;
			bool hasIntuition = Core.Player.HasAura(CharacterAuras.FishersIntuition);

			// Filter by location, time of day, weather, tug type, and intuition requirement
			var eligibleFish = allFish.Where(x =>
				x.RouteShortName == location &&
				x.TimeOfDayExclusion1 != timeOfDay &&
				x.TimeOfDayExclusion2 != timeOfDay &&
				x.WeatherExclusion1 != currentWeather &&
				x.WeatherExclusion2 != currentWeather &&
				x.BiteType == FishingManager.TugType &&
				(!x.RequiresIntuition || hasIntuition)).ToList();

			// First try exact match using bite range for current bait
			var exactMatches = eligibleFish.Where(x =>
			{
				var (start, end) = x.GetBiteRange(currentBait);
				return start <= biteElapsed && end >= biteElapsed;
			}).ToList();

			if (exactMatches.Any())
				return exactMatches;

			// No exact match - find nearest fish within tolerance
			return FindNearestFish(eligibleFish, biteElapsed, currentBait);
		}

		/// <summary>
		/// Return ALL eligible fish sorted by distance to the observed bite time.
		/// The caught fish must be one of the known fish at this location — bite time
		/// narrows the prediction but any eligible fish with the right tug could have bitten.
		/// </summary>
		private List<Fish> FindNearestFish(List<Fish> eligibleFish, double biteElapsed, uint currentBait = 0)
		{
			if (!eligibleFish.Any())
				return new List<Fish>();

			return eligibleFish.Select(x =>
			{
				var (start, end) = currentBait > 0 ? x.GetBiteRange(currentBait) : (x.BiteStart, x.BiteEnd);
				double distance;
				if (biteElapsed < start)
					distance = start - biteElapsed;
				else if (biteElapsed > end)
					distance = biteElapsed - end;
				else
					distance = 0;

				return new { Fish = x, Distance = distance };
			})
			.OrderBy(x => x.Distance)
			.Select(x => x.Fish)
			.ToList();
		}

		/// <summary>
		/// Log fish excluded by time/weather filters when a fuzzy match occurs.
		/// Helps identify if the actual caught fish was filtered out of the eligible pool.
		/// </summary>
		private void LogExcludedFish(HookContext context, string currentWeather)
		{
			var fishList = context.Spectraled ? context.CurrentRoute?.SpectralFish : context.CurrentRoute?.NormalFish;
			if (fishList == null)
				return;

			bool excludeWeather = !context.Spectraled;

			var excluded = fishList.Where(x =>
				x.BiteType == FishingManager.TugType &&
				(x.TimeOfDayExclusion1 == context.TimeOfDay ||
				x.TimeOfDayExclusion2 == context.TimeOfDay ||
				(excludeWeather && (x.WeatherExclusion1 == currentWeather || x.WeatherExclusion2 == currentWeather))))
				.ToList();

			if (excluded.Any())
			{
				uint currentBait = FishingManager.SelectedBaitItemId;
				var excludedStr = String.Join(", ", excluded.Select(x =>
				{
					var reasons = new List<string>();
					if (x.TimeOfDayExclusion1 == context.TimeOfDay || x.TimeOfDayExclusion2 == context.TimeOfDay)
						reasons.Add($"time={context.TimeOfDay}");
					if (excludeWeather && (x.WeatherExclusion1 == currentWeather || x.WeatherExclusion2 == currentWeather))
						reasons.Add($"weather={currentWeather}");
					var (start, end) = x.GetBiteRange(currentBait);
					return $"{_gameCache.GetItemName((uint)x.FishID)} [{start:F0}-{end:F0}s, excluded: {String.Join("+", reasons)}]";
				}));
				Log($"  Excluded: {excludedStr}");
			}
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
