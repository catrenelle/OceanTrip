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
			var availableFish = achievementFish.Where(f => f.SpectralFish == isSpectral).ToList();

			// Not in spectral but achievement fish here are mostly spectral? Pop spectral first.
			if (!isSpectral && !availableFish.Any())
			{
				int spectralCount = achievementFish.Count(f => f.SpectralFish);
				if (spectralCount > 0)
				{
					var allFish = FishDataCache.GetFish();
					var spectralTrigger = allFish.FirstOrDefault(f => f.RouteShortName == context.Location && f.CausesSpectral);
					if (spectralTrigger != null)
					{
						await _baitChanger.ChangeBait(spectralTrigger.FavoriteBait,
							$"Achievement ({targetAchievement}) — popping spectral, {spectralCount} achievement fish need it");
						if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience)
							await _patienceManager.UsePatience();
						// Note: not spectral here (we're trying to pop it), so SpectralOnly doesn't apply
						return;
					}
				}

				Log($"No {targetAchievement} fish available for current conditions. Using default bait.", OceanLogLevel.Debug);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			if (!availableFish.Any())
			{
				Log($"No {targetAchievement} fish available at {context.Location}. Using default bait.", OceanLogLevel.Debug);
				await _baitChanger.ChangeBait(context.DefaultBaitId);
				return;
			}

			// Select the preferred bait for achievement fish
			var baitCounts = availableFish
				.GroupBy(f => f.FavoriteBait)
				.Select(g => new { Bait = g.Key, Count = g.Count() })
				.OrderByDescending(x => x.Count)
				.ToList();

			if (baitCounts.Any())
			{
				var selectedBait = baitCounts.First().Bait;
				var fishNames = string.Join(", ", availableFish.Where(f => f.FavoriteBait == selectedBait).Select(f => f.FishName));

				await _baitChanger.ChangeBait(selectedBait,
					$"Achievement ({targetAchievement}) — targeting: {fishNames}");
			}
			else
			{
				await _baitChanger.ChangeBait(context.DefaultBaitId,
					$"Achievement ({targetAchievement}) — no matching fish, using default bait");
			}

			if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience
				|| (isSpectral && OceanTripNewSettings.Instance.Patience == ShouldUsePatience.SpectralOnly))
				await _patienceManager.UsePatience();
		}

		private AchievementType GetSelectedAchievement()
		{
			return AchievementFishDataCache.GetCurrentAchievementFocus();
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
