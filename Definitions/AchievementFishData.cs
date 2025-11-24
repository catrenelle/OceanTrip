using ff14bot.Enums;
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
		Squid = 10
	}

	/// <summary>
	/// Hook type preference for catching specific fish
	/// </summary>
	public enum HookType
	{
		Normal = 0,
		Double = 1,
		Triple = 2
	}

	/// <summary>
	/// Information about a fish that counts toward an achievement
	/// </summary>
	public class AchievementFishInfo
	{
		/// <summary>Fish ID from OceanFish definitions</summary>
		public uint FishId { get; set; }

		/// <summary>Display name of the fish</summary>
		public string FishName { get; set; }

		/// <summary>Which achievement category this fish belongs to</summary>
		public AchievementType Achievement { get; set; }

		/// <summary>Location/zone short name (e.g., "galadion", "rhotano", "south")</summary>
		public string Location { get; set; }

		/// <summary>Which route this fish appears on</summary>
		public FishingRoute Route { get; set; }

		/// <summary>Preferred hook type for catching this fish (Normal, Double, or Triple)</summary>
		public HookType PreferredHookType { get; set; }

		/// <summary>Preferred bait ID for catching this fish</summary>
		public uint PreferredBait { get; set; }

		/// <summary>Whether this is a spectral current fish</summary>
		public bool IsSpectral { get; set; }

		/// <summary>Expected bite type (Light, Medium, Heavy)</summary>
		public TugType BiteType { get; set; }

		/// <summary>Minimum bite time in seconds</summary>
		public float BiteStart { get; set; }

		/// <summary>Maximum bite time in seconds</summary>
		public float BiteEnd { get; set; }
	}

	/// <summary>
	/// Static cache for achievement fish data
	/// TODO: Populate this with data from "Ocean Fishing Data.xlsx"
	/// </summary>
	public static class AchievementFishDataCache
	{
		private static List<AchievementFishInfo> _achievementFishList;

		/// <summary>
		/// Gets all achievement fish data
		/// </summary>
		public static List<AchievementFishInfo> GetAchievementFish()
		{
			if (_achievementFishList == null)
			{
				_achievementFishList = InitializeAchievementFishData();
			}
			return _achievementFishList;
		}

		/// <summary>
		/// Gets achievement fish for a specific achievement type
		/// </summary>
		public static List<AchievementFishInfo> GetFishForAchievement(AchievementType achievementType)
		{
			return GetAchievementFish()
				.Where(f => f.Achievement == achievementType)
				.ToList();
		}

		/// <summary>
		/// Gets achievement fish for a specific location and achievement
		/// </summary>
		public static List<AchievementFishInfo> GetFishForLocation(string location, AchievementType achievementType)
		{
			return GetAchievementFish()
				.Where(f => f.Achievement == achievementType && f.Location == location)
				.ToList();
		}

		/// <summary>
		/// Gets achievement fish for a specific route
		/// </summary>
		public static List<AchievementFishInfo> GetFishForRoute(FishingRoute route)
		{
			return GetAchievementFish()
				.Where(f => f.Route == route)
				.ToList();
		}

		/// <summary>
		/// Determines which achievement type is valid for the given route
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
			else // Ruby
			{
				return new List<AchievementType>
				{
					AchievementType.Shrimp,
					AchievementType.Shellfish,
					AchievementType.Squid
				};
			}
		}

		/// <summary>
		/// Initialize achievement fish data
		/// TODO: Populate with actual data from "Ocean Fishing Data.xlsx"
		/// This is a placeholder structure that should be filled with real data
		/// </summary>
		private static List<AchievementFishInfo> InitializeAchievementFishData()
		{
			var fishList = new List<AchievementFishInfo>();

			// TODO: Add fish data from the XLSX spreadsheet
			// Example structure (replace with actual data):
			/*
			fishList.Add(new AchievementFishInfo
			{
				FishId = (uint)OceanFish.CoralManta,
				FishName = "Coral Manta",
				Achievement = AchievementType.Mantas,
				Location = "galadion", // or "rhotano", "sound", etc.
				Route = FishingRoute.Indigo,
				PreferredHookType = HookType.Triple,
				PreferredBait = FishBait.Krill,
				IsSpectral = true,
				BiteType = TugType.Heavy,
				BiteStart = 10.0f,
				BiteEnd = 15.0f
			});
			*/

			// Placeholder: Return empty list until data is populated
			return fishList;
		}

		/// <summary>
		/// Clears the cache to force reload
		/// </summary>
		public static void InvalidateCache()
		{
			_achievementFishList = null;
		}
	}
}
