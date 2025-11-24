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

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Hooking strategy optimized for catching achievement fish
	/// Uses Double/Triple Hook based on achievement fish data from XLSX spreadsheet
	/// </summary>
	public class AchievementHookingStrategy
	{
		private readonly GameStateCache _gameCache;
		private readonly bool _loggingEnabled;

		// Track fish caught this voyage for each achievement type
		private readonly Dictionary<AchievementType, int> _fishCaughtThisVoyage = new Dictionary<AchievementType, int>();

		public AchievementHookingStrategy(GameStateCache gameCache, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Handle fish bite for achievement fishing - use appropriate hook type
		/// </summary>
		/// <param name="context">Context containing fishing state and bite information</param>
		public Task ExecuteHook(HookContext context)
		{
			double biteElapsed = Math.Round(context.BiteElapsedSeconds, 1);
			const double Tolerance = 1e-6; // Floating Point Precision
			bool useDoubleHook = false;
			bool useTripleHook = false;

			// Determine which achievement is selected
			var targetAchievement = GetSelectedAchievement();

			if (targetAchievement == AchievementType.None)
			{
				Log("No achievement selected. Using normal hook.", OceanLogLevel.Debug);
				ExecuteNormalHook();
				context.OnHookExecuted(false);
				return Task.CompletedTask;
			}

			// Get achievement fish for current location
			bool isSpectral = context.Spectraled;
			var achievementFish = AchievementFishDataCache.GetFishForLocation(context.Location, targetAchievement)
				.Where(f => f.IsSpectral == isSpectral)
				.ToList();

			if (!achievementFish.Any())
			{
				Log($"No {targetAchievement} fish at {context.Location} (Spectral: {isSpectral}). Using normal hook.", OceanLogLevel.Debug);
				ExecuteNormalHook();
				context.OnHookExecuted(false);
				return Task.CompletedTask;
			}

			// Find matching fish based on bite characteristics
			var currentTug = FishingManager.TugType;
			var matchingFish = achievementFish
				.Where(f => f.BiteType == currentTug
					&& (f.BiteStart - Tolerance) <= biteElapsed
					&& (f.BiteEnd + Tolerance) >= biteElapsed)
				.ToList();

			if (!matchingFish.Any())
			{
				Log($"No {targetAchievement} fish match current bite (Tug: {currentTug}, Time: {biteElapsed}s). Using normal hook.", OceanLogLevel.Debug);
				ExecuteNormalHook();
				context.OnHookExecuted(false);
				return Task.CompletedTask;
			}

			// Determine hook type based on fish data
			// If multiple fish match, prioritize Triple > Double > Normal
			var preferredHookType = matchingFish
				.Select(f => f.PreferredHookType)
				.OrderByDescending(h => h)
				.FirstOrDefault();

			bool hookLogged = false;

			switch (preferredHookType)
			{
				case HookType.Triple:
					useTripleHook = true;
					break;
				case HookType.Double:
					useDoubleHook = true;
					break;
				default:
					// Normal hook
					break;
			}

			// Execute appropriate hook action
			if (useTripleHook && ActionManager.CanCast(Actions.TripleHook, Core.Me))
			{
				var fishNames = string.Join(", ", matchingFish.Select(f => f.FishName).Distinct());
				Log($"Using Triple Hook for {targetAchievement}: {fishNames}", OceanLogLevel.Info);
				ActionManager.DoAction(Actions.TripleHook, Core.Me);
				hookLogged = true;
			}
			else if (useDoubleHook && ActionManager.CanCast(Actions.DoubleHook, Core.Me))
			{
				var fishNames = string.Join(", ", matchingFish.Select(f => f.FishName).Distinct());
				Log($"Using Double Hook for {targetAchievement}: {fishNames}", OceanLogLevel.Info);
				ActionManager.DoAction(Actions.DoubleHook, Core.Me);
				hookLogged = true;
			}
			else if (FishingManager.HasPatience)
			{
				// Use Patience hooksets when Patience is active
				if (FishingManager.TugType == TugType.Light)
				{
					Log($"Using Precision Hookset (Patience active, Light tug)", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
				}
				else
				{
					Log($"Using Powerful Hookset (Patience active, {FishingManager.TugType} tug)", OceanLogLevel.Debug);
					ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
				}
				hookLogged = true;
			}
			else
			{
				// Normal hook
				Log($"Using normal hook for {targetAchievement} fish", OceanLogLevel.Debug);
				FishingManager.Hook();
				hookLogged = true;
			}

			// Invoke callback
			context.OnHookExecuted(hookLogged);

			// Track fish caught (this will be incremented after successful catch)
			IncrementFishCount(targetAchievement);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Execute normal hook (fallback when no achievement fish detected)
		/// </summary>
		private void ExecuteNormalHook()
		{
			if (FishingManager.HasPatience)
			{
				if (FishingManager.TugType == TugType.Light)
					ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
				else
					ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
			}
			else
			{
				FishingManager.Hook();
			}
		}

		/// <summary>
		/// Determines which achievement the user has selected based on the UI checkboxes
		/// </summary>
		private AchievementType GetSelectedAchievement()
		{
			var databinds = FFXIV_Databinds.Instance;

			// Check Indigo route achievements
			if (databinds.achievementMantas) return AchievementType.Mantas;
			if (databinds.achievementOctopods) return AchievementType.Octopods;
			if (databinds.achievementSharks) return AchievementType.Sharks;
			if (databinds.achievementJellyfish) return AchievementType.Jellyfish;
			if (databinds.achievementSeadragons) return AchievementType.Seadragons;
			if (databinds.achievementBalloons) return AchievementType.Balloons;
			if (databinds.achievementCrabs) return AchievementType.Crabs;

			// Check Ruby route achievements
			if (databinds.achievementShrimp) return AchievementType.Shrimp;
			if (databinds.achievementShellfish) return AchievementType.Shellfish;
			if (databinds.achievementSquid) return AchievementType.Squid;

			return AchievementType.None;
		}

		/// <summary>
		/// Increment count of fish caught for an achievement
		/// </summary>
		private void IncrementFishCount(AchievementType achievement)
		{
			if (!_fishCaughtThisVoyage.ContainsKey(achievement))
				_fishCaughtThisVoyage[achievement] = 0;

			_fishCaughtThisVoyage[achievement]++;

			// Some achievements require 25 fish (e.g., "What did mantas do to you?")
			// Log progress toward achievement
			int count = _fishCaughtThisVoyage[achievement];
			if (count % 5 == 0 || count >= 20) // Log every 5 fish or when close to goal
			{
				Log($"Progress: {count} {achievement} caught this voyage", OceanLogLevel.Info);
			}
		}

		/// <summary>
		/// Reset fish count (call this at start of new voyage)
		/// </summary>
		public void ResetVoyageTracking()
		{
			_fishCaughtThisVoyage.Clear();
			Log("Reset achievement fish tracking for new voyage", OceanLogLevel.Debug);
		}

		/// <summary>
		/// Get current count of fish caught for an achievement
		/// </summary>
		public int GetFishCount(AchievementType achievement)
		{
			return _fishCaughtThisVoyage.ContainsKey(achievement) ? _fishCaughtThisVoyage[achievement] : 0;
		}

		/// <summary>
		/// Helper method for logging
		/// </summary>
		private void Log(string message, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip - Achievement] " + message);
			Logging.Write(System.Windows.Media.Colors.Aqua, msg);
		}
	}
}
