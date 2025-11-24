using ff14bot.Enums;
using ff14bot.Helpers;
using Newtonsoft.Json;
using Ocean_Trip.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean_Trip.Definitions
{
	public class Fish
	{
		public uint RouteID { get; set; }
		public string RouteShortName { get; set; }
		public int FishID { get; set; }
		public string FishName { get; set; }
		public int IconX { get; set; }
		public int IconY { get; set; }
		public TugType BiteType { get; set; }
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
				var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };
				string filePath = null;

				foreach (var dir in possibleDirectories)
				{
					var potentialPath = Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "fishList.json");
					if (File.Exists(potentialPath))
					{
						filePath = potentialPath;
						break;
					}
				}

				if (filePath == null || !File.Exists(filePath))
				{
					throw new FileNotFoundException("The fish list file was not found.", filePath);
				}

				var json = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<List<Fish>>(json);
			}
			catch (Exception ex)
			{
				// Log the exception or handle it as needed
				Logging.Write($"[Ocean Trip] Error loading fish list: {ex.Message}");
				return new List<Fish>(); // Return an empty list in case of error
			}
		}

		private static List<Fish> FishAvailable()
		{
			return null;
		}
	}
}
