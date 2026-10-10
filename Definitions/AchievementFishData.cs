using OceanTripPlanner;
using OceanTripPlanner.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ocean_Trip.Definitions
{
	/// <summary>
	/// Types of ocean fishing achievements based on fish categories
	/// </summary>
	public enum AchievementType
	{
		None = 0,

		// Indigo Route Achievements
		Mantas = 1,
		Octopods = 2,
		Sharks = 3,
		Jellyfish = 4,
		Seadragons = 5,
		Balloons = 6,
		Crabs = 7,

		// Ruby Route Achievements
		Shrimp = 8,
		Shellfish = 9,
		Squid = 10,
		MantisShrimp = 11,
		Prehistoric = 12
	}

	/// <summary>
	/// Static helpers for achievement fish data, operating on Fish objects from FishDataCache
	/// </summary>
	public static class AchievementFishDataCache
	{
		private static readonly Dictionary<string, AchievementType> AchievementStringMap = new Dictionary<string, AchievementType>
		{
			{ "Manta", AchievementType.Mantas },
			{ "Octopus", AchievementType.Octopods },
			{ "Shark", AchievementType.Sharks },
			{ "Jellyfish", AchievementType.Jellyfish },
			{ "Seadragon", AchievementType.Seadragons },
			{ "Boxfish", AchievementType.Balloons },
			{ "Crab", AchievementType.Crabs },
			{ "Shrimp", AchievementType.Shrimp },
			{ "Mussel", AchievementType.Shellfish },
			{ "Squid", AchievementType.Squid },
			{ "Mantis", AchievementType.MantisShrimp },
			{ "Prehistoric", AchievementType.Prehistoric }
		};

		/// <summary>
		/// Gets achievement fish for a specific location and achievement type
		/// </summary>
		public static List<Fish> GetFishForLocation(string location, AchievementType achievementType)
		{
			return FishDataCache.GetFish()
				.Where(f => f.RouteShortName == location &&
					!string.IsNullOrEmpty(f.Achievement) &&
					MapAchievementString(f.Achievement) == achievementType)
				.ToList();
		}

		/// <summary>
		/// Determines which achievement types are valid for the given route
		/// </summary>
		public static List<AchievementType> GetValidAchievementsForRoute(FishingRoute route)
		{
			if (route == FishingRoute.Indigo)
			{
				return new List<AchievementType>
				{
					AchievementType.Mantas,
					AchievementType.Octopods,
					AchievementType.Sharks,
					AchievementType.Jellyfish,
					AchievementType.Seadragons,
					AchievementType.Balloons,
					AchievementType.Crabs
				};
			}
			else
			{
				return new List<AchievementType>
				{
					AchievementType.Shrimp,
					AchievementType.Shellfish,
					AchievementType.Squid,
#if !RB_TC
					AchievementType.MantisShrimp,
					AchievementType.Prehistoric,
#endif
				};
			}
		}

		/// <summary>
		/// Returns the user's currently selected achievement focus based on the active route
		/// </summary>
		public static AchievementType GetCurrentAchievementFocus()
		{
			int focus = OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Indigo
				? OceanTripNewSettings.Instance.IndigoAchievementFocus
				: OceanTripNewSettings.Instance.RubyAchievementFocus;
			if (Enum.IsDefined(typeof(AchievementType), focus))
				return (AchievementType)focus;
			return AchievementType.None;
		}

		/// <summary>
		/// Maps an Achievement string from fishList.json to the AchievementType enum
		/// </summary>
		public static AchievementType MapAchievementString(string achievement)
		{
			if (achievement != null && AchievementStringMap.TryGetValue(achievement, out var type))
				return type;
			return AchievementType.None;
		}
	}
}
