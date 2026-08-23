using ff14bot.Enums;
using ff14bot.Helpers;
using Newtonsoft.Json;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean_Trip.Definitions
{
	public class IntuitionPrereq
	{
		public int FishID { get; set; }
		public int Count { get; set; }
		public bool IsMooch { get; set; }
	}

	public class Fish
	{
		public uint RouteID { get; set; }
		public string RouteShortName { get; set; }
		public int FishID { get; set; }
		public string FishName { get; set; }
		public int IconX { get; set; }
		public int IconY { get; set; }
		public TugType BiteType { get; set; }
		public TugType HooksetType { get; set; }
		public string Rarity { get; set; }
		public uint FavoriteBait { get; set; }
		public bool CausesSpectral { get; set; }
		public bool SpectralFish { get; set; }
		public float BiteStart { get; set; }
		public float BiteEnd { get; set; }
		public int Points { get; set; }
		public int DHBonus { get; set; }
		public int THBonus { get; set; }
		public string Achievement { get; set; }
		public string WeatherExclusion1 { get; set; }
		public string WeatherExclusion2 { get; set; }
		public string TimeOfDayExclusion1 { get; set; }
		public string TimeOfDayExclusion2 { get; set; }
		public bool RequiresIntuition { get; set; }
		public List<IntuitionPrereq> IntuitionPrereqs { get; set; }
		public Dictionary<string, float[]> BiteTimers { get; set; }

		/// <summary>
		/// Get bite range for a specific bait. Falls back to BiteStart/BiteEnd if no BiteTimers entry.
		/// </summary>
		public (float start, float end) GetBiteRange(uint baitId)
		{
			if (BiteTimers != null && BiteTimers.TryGetValue(baitId.ToString(), out var range) && range.Length >= 2)
				return (range[0], range[1]);
			return (BiteStart, BiteEnd);
		}
	}

	public static class FishDataCache
	{
		private static List<Fish> _cachedFishList;

		public static List<Fish> GetFish()
		{
			if (_cachedFishList == null)
			{
				_cachedFishList = LoadFishData();
			}
			return _cachedFishList;
		}

		public static void InvalidateCache()
		{
			_cachedFishList = null;
		}

		private static List<Fish> LoadFishData()
		{
			try
			{
				var filePath = BotBasesResourceLocator.Resolve("Resources", "fishList.json");
				if (filePath == null)
				{
					throw new FileNotFoundException("The fish list file was not found.");
				}

				var json = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<List<Fish>>(json);
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Error loading fish list: {ex.Message}");
				return new List<Fish>();
			}
		}

		private static List<Fish> FishAvailable()
		{
			return null;
		}
	}
}
