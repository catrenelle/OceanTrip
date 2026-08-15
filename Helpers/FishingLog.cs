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
				return (uint)Elements[8].Int;

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

			bool needsReinit = !File.Exists(fileName);

			if (!needsReinit)
			{
				var firstLine = File.ReadLines(fileName).FirstOrDefault();
				var currentFishCount = FishDataCache.GetFish().Count;

				if (firstLine != null && firstLine.StartsWith("#fishDataCount="))
				{
					var cachedCount = int.Parse(firstLine.Substring("#fishDataCount=".Length));
					if (cachedCount != currentFishCount)
					{
						Logging.Write($"[Ocean Trip] Fish data updated ({cachedCount} → {currentFishCount} fish) — reinitializing fishing log...");
						needsReinit = true;
					}
				}
				else
				{
					Logging.Write("[Ocean Trip] Fishing log cache missing version header — reinitializing...");
					needsReinit = true;
				}
			}

			if (needsReinit)
			{
				if (File.Exists(fileName))
					File.Delete(fileName);

				var fishList = await AgentFishGuide2.Instance.GetFishList();
				var recordedFish = fishList.Where(x => x.HasCaught).Select(x => (int)x.FishItem).ToList();
				var oceanFish = FishDataCache.GetFish().Select(x => x.FishID).ToList();

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
			{
				LoadMissingFishLog();
				await ReconcileWithFishGuide();
			}
		}

		/// <summary>
		/// Cross-check the disk-cached missing set against AgentFishGuide2's live catch data and drop
		/// anything that's actually already been caught. The cache is otherwise only invalidated by a
		/// fish DATA-count change (a fishList.json update) — a fish caught by any means that didn't
		/// route through RemoveFish (e.g. the game's own catch tracking syncing late for a
		/// newly-added fish right when the cache was first built, or a missed catch-detection edge
		/// case) would otherwise stay marked "missing" indefinitely, since nothing else ever
		/// re-validates it against reality.
		/// </summary>
		private static async Task ReconcileWithFishGuide()
		{
			if (_cachedMissingFishSet == null || _cachedMissingFishSet.Count == 0)
				return;

			var fishList = await AgentFishGuide2.Instance.GetFishList();
			var caughtIds = new HashSet<uint>(fishList.Where(x => x.HasCaught).Select(x => (uint)x.FishItem));

			int removed = _cachedMissingFishSet.RemoveWhere(caughtIds.Contains);
			if (removed > 0)
			{
				Logging.Write($"[Ocean Trip] Fishing log cache was stale — {removed} already-caught fish were still marked missing, reconciled.");
				SaveMissingFishLog();
			}
		}

		public static void SaveMissingFishLog()
		{
			if (File.Exists(fileName))
				File.Delete(fileName);

			var lines = new List<string> { $"#fishDataCount={FishDataCache.GetFish().Count}" };
			lines.AddRange(_cachedMissingFishSet.Select(x => x.ToString()));
			File.WriteAllLines(fileName, lines);
		}

		public static void LoadMissingFishLog()
		{
			if (_cachedMissingFishSet != null)
				_cachedMissingFishSet = null;

			if (File.Exists(fileName))
				_cachedMissingFishSet = new HashSet<uint>(
					File.ReadAllLines(fileName)
						.Where(x => !x.StartsWith("#"))
						.Select(x => (uint)Convert.ToInt32(x)));
		}
	}
}
