using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Bait selector optimized for catching specific fish types to complete achievements
	/// (e.g., "What did mantas do to you?", "What did seadragons do to you?", etc.)
	/// </summary>
	public class AchievementBaitSelector : IBaitSelector
	{
		private readonly BaitChanger _baitChanger;
		private readonly PatienceManager _patienceManager;
		private readonly GameStateCache _gameCache;

		public AchievementBaitSelector(BaitChanger baitChanger, PatienceManager patienceManager, GameStateCache gameCache)
		{
			_baitChanger = baitChanger;
			_patienceManager = patienceManager;
			_gameCache = gameCache;
		}

		/// <summary>
		/// Select bait based on which achievement the user is targeting
		/// </summary>
		public async Task SelectBait(BaitSelectionContext context)
		{
			// Refresh game state
			_gameCache.RefreshIfNeeded();

			// Determine which achievement is selected
			var targetAchievement = GetSelectedAchievement();

			if (targetAchievement == AchievementType.None)
			{
				Log("No achievement selected. Falling back to default bait.", OceanLogLevel.Info);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			// Get current route
			var currentRoute = OceanTripNewSettings.Instance.FishingRoute;

			// Validate that the selected achievement is valid for the current route
			var validAchievements = AchievementFishDataCache.GetValidAchievementsForRoute(currentRoute);
			if (!validAchievements.Contains(targetAchievement))
			{
				Log($"Achievement {targetAchievement} is not valid for {currentRoute} route. Please select a valid achievement.", OceanLogLevel.Always);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			// Get achievement fish available in current location
			var achievementFish = AchievementFishDataCache.GetFishForLocation(context.Location, targetAchievement);

			if (achievementFish == null || !achievementFish.Any())
			{
				Log($"No {targetAchievement} fish available at {context.Location}. Using default bait.", OceanLogLevel.Debug);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			// Determine spectral status
			bool isSpectral = (_gameCache.CurrentWeatherId == Weather.Spectral);

			// Filter fish by spectral status
			var availableFish = achievementFish.Where(f => f.IsSpectral == isSpectral).ToList();

			if (!availableFish.Any())
			{
				Log($"No {targetAchievement} fish available for current conditions (Spectral: {isSpectral}). Using default bait.", OceanLogLevel.Debug);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			// Select the preferred bait for achievement fish
			// Priority: Choose bait that catches the most achievement fish in this location
			var baitCounts = availableFish
				.GroupBy(f => f.PreferredBait)
				.Select(g => new { Bait = g.Key, Count = g.Count() })
				.OrderByDescending(x => x.Count)
				.ToList();

			if (baitCounts.Any())
			{
				var selectedBait = baitCounts.First().Bait;
				var fishNames = string.Join(", ", availableFish.Where(f => f.PreferredBait == selectedBait).Select(f => f.FishName));

				await _baitChanger.ChangeBait(selectedBait,
					$"Targeting {targetAchievement} achievement: {fishNames}");
			}
			else
			{
				// Fallback to default bait
				await _baitChanger.ChangeBait(context.DefaultBaitId);
			}

			// Use Patience if appropriate
			await _patienceManager.UsePatience();
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
		/// Helper method for logging
		/// </summary>
		private void Log(string message, OceanLogLevel level = OceanLogLevel.Info)
		{
			_baitChanger.Log(message, level);
		}
	}
}
