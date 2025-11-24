using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteAgents;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTrip
{
	public static class FishingLog
	{
		private static string name = "IKDFishingLog";
		private static int elementCount => LlamaElements.ElementCount(name);
		private static TwoInt[] Elements => LlamaElements.___Elements(name);


		private static string fileName = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");
		private static HashSet<uint> _cachedMissingFishSet;
		public static HashSet<uint> MissingFish()
		{
			return _cachedMissingFishSet;
		}


		//public static string AreaName 
		//{ 
		//	get 
		//	{
		//		if (elementCount > 0)
		//			return Core.Memory.ReadString((IntPtr)Elements[2].Data, Encoding.UTF8);

		//		return ""; 
		//	} 
		//}

		//public static int Points
		//{
		//	get 
		//	{
		//		if (elementCount > 0)
		//			return Elements[7].TrimmedData;

		//		return 0;
		//	}
		//}

		public static uint LastFishCaught
	{
		get
		{
			// Prefer the new Catch API if available (more reliable and efficient)
			// Catch.FishName can throw ArgumentNullException if window isn't fully initialized
			try
			{
				if (!string.IsNullOrEmpty(Catch.FishName) && Catch.CaughtFish != null && Catch.CaughtFish.Id > 0)
					return Catch.CaughtFish.Id;
			}
			catch (System.ArgumentNullException)
			{
				// Catch window not ready, fall through to legacy method
			}

			// Fallback to legacy IKDFishingLog window reading
			if (elementCount > 0)
				return (uint)Elements[8].TrimmedData;

			return 0;
		}
	}

		/// <summary>
		/// Get the name of the last caught fish using the new Catch API
		/// Returns empty string if no fish or Catch window is not available
		/// </summary>
		public static string LastFishName
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(Catch.FishName))
						return Catch.FishName;
				}
				catch (System.ArgumentNullException)
				{
					// Catch window not ready
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Get additional catch details from the new Catch API
		/// </summary>
		public static (bool IsLarge, float Size, int Stars, long Quantity) GetCatchDetails()
		{
			try
			{
				return (
					Catch.Large,
					Catch.FishSize,
					Catch.QualityStars,
					Catch.Quantity
				);
			}
			catch (System.ArgumentNullException)
			{
				// Catch window not ready
				return (false, 0, 0, 0);
			}
		}

		public static void InvalidateCache()
		{
			_cachedMissingFishSet = null;
		}

		public static void AddFish(uint fishId)
		{
			_cachedMissingFishSet?.Add(fishId);
		}

		public static void RemoveFish(uint fishId)
		{
			_cachedMissingFishSet?.Remove(fishId);
		}


		public static async Task InitializeFishLog()
		{

			if (_cachedMissingFishSet != null)
				_cachedMissingFishSet = null;

			if (!File.Exists(fileName))
			{

				var fishList = await AgentFishGuide2.Instance.GetFishList();
				var recordedFish = fishList.Where(x => x.HasCaught).Select(x => (int)x.FishItem).ToList();
				var oceanFish = FishDataCache.GetFish().Select(x => x.FishID).ToList();

				// Convert to HashSet for O(1) lookups
				var newOceanFishSet = new HashSet<uint>(oceanFish.Except(recordedFish).Select(x => (uint)x));

				Logging.Write($"  Ocean Fish: {oceanFish.Count()}");
				Logging.Write($"Missing Fish: {newOceanFishSet.Count}");

				fishList = null;
				recordedFish = null;
				oceanFish = null;

				_cachedMissingFishSet = newOceanFishSet;
				SaveMissingFishLog();
			}
			else
				LoadMissingFishLog();
		}

		public static void SaveMissingFishLog()
		{
			if (File.Exists(fileName))
				File.Delete(fileName);

			File.WriteAllLines(fileName, _cachedMissingFishSet.Select(x => x.ToString()));
			return;
		}

		public static void LoadMissingFishLog()
		{
			if (_cachedMissingFishSet != null)
				_cachedMissingFishSet = null;

			if (File.Exists(fileName))
				_cachedMissingFishSet = new HashSet<uint>(File.ReadAllLines(fileName).Select(x => (uint)Convert.ToInt32(x)));
		}
	}
}
