using System.Collections.Generic;
using System.Linq;
using Ocean_Trip.Definitions;

namespace OceanTripPlanner.Strategies
{
	static class BaitRanker
	{
		public static uint SelectBaitForMissingFish(List<Fish> availableFish, HashSet<uint> missingFish)
		{
			var missing = availableFish
				.Where(f => missingFish.Contains((uint)f.FishID) && !f.RequiresIntuition)
				.ToList();

			if (!missing.Any())
				return 0;

			var sorted = missing
				.OrderBy(f => GetRarityOrder(f.Rarity))
				.ToList();

			var topRarity = sorted.First().Rarity;
			var topRarityFish = sorted.Where(f => f.Rarity == topRarity).ToList();

			return topRarityFish
				.GroupBy(f => f.FavoriteBait)
				.OrderByDescending(g => g.Count())
				.ThenByDescending(g => g.Sum(f => f.Points))
				.First()
				.Key;
		}

		public static uint SelectBaitForPoints(List<Fish> availableFish)
		{
			var catchable = availableFish.Where(f => !f.RequiresIntuition).ToList();

			if (!catchable.Any())
				return 0;

			return catchable
				.GroupBy(f => f.FavoriteBait)
				.OrderByDescending(g => g.Sum(f => f.Points))
				.First()
				.Key;
		}

		private static int GetRarityOrder(string rarity)
		{
			switch (rarity)
			{
				case "Rare": return 0;
				case "Uncommon": return 1;
				case "Common": return 2;
				default: return 3;
			}
		}
	}
}
