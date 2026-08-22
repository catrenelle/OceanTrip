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
			// Session cache — populated by InitializeFishLog when the botbase starts. When it hasn't run
			// yet (the Schedule / Current Route UI is open before Start), fall back to the persisted file
			// so those pages can still show missing-fish highlights instead of nothing until Start.
			//
			// The fallback deliberately does NOT assign _cachedMissingFishSet: InitializeFishLog guards on
			// it being null to decide whether to reinit (fish-data-count change) and reconcile against the
			// live Fish Guide, so pre-filling it here would silently skip that on the session's first Start.
			if (_cachedMissingFishSet != null)
				return _cachedMissingFishSet;

			return ReadMissingFishFile();
		}

		/// <summary>
		/// Read the persisted missing-fish set straight off disk without touching the game or the session
		/// cache. Returns null when the file doesn't exist yet (log never built for this character).
		/// </summary>
		private static HashSet<uint> ReadMissingFishFile()
		{
			if (!File.Exists(fileName))
				return null;

			return new HashSet<uint>(
				File.ReadAllLines(fileName)
					.Where(x => !x.StartsWith("#"))
					.Select(x => (uint)Convert.ToInt32(x)));
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
			// Already built this session — reuse it and do NOT touch the game. Run() is the botbase root
			// (ActionRunCoroutine), so RB re-ticks it every time it completes and calls InitializeFishLog
			// each tick; without this guard we'd rebuild and re-open the in-game Fish Guide (via
			// ReconcileWithFishGuide → GetFishList) on every tick — exactly what this cache exists to
			// avoid. The in-memory set stays current through RemoveFish on each catch, and Stop() calls
			// InvalidateCache() to force a fresh load next session.
			if (_cachedMissingFishSet != null)
				return;

			bool needsReinit = !File.Exists(fileName);

			if (!needsReinit)
			{
				var firstLine = File.ReadLines(fileName).FirstOrDefault();
				var currentSig = FishDataSignature();

				if (firstLine != null && firstLine.StartsWith(SignatureHeaderPrefix))
				{
					var cachedSig = firstLine.Substring(SignatureHeaderPrefix.Length);
					if (cachedSig != currentSig)
					{
						Logging.Write($"[Ocean Trip] Fish data changed ({cachedSig} → {currentSig}) — reinitializing fishing log...");
						needsReinit = true;
					}
				}
				else
				{
					// No signature header — either a pre-signature cache (old #fishDataCount= format) or a
					// corrupt/missing one. Rebuild so any stale IDs (e.g. the Junior Jinbei 51236→51687 fix)
					// are purged rather than lingering as orphaned "missing" entries forever.
					Logging.Write("[Ocean Trip] Fishing log cache missing/old version header — reinitializing...");
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
				// Just load the cached set from disk — do NOT auto-reconcile against the in-game Fish
				// Guide here. AgentFishGuide2.GetFishList() visibly opens and closes the Fish Guide window,
				// and doing that on every Start is jarring when the cache is already current: the botbase
				// removes each fish from the set the instant it's caught (RemoveFish + SaveMissingFishLog
				// in the catch flow), so a valid cache stays accurate on its own. A full rebuild
				// (needsReinit above) still reads the Guide, but only when the dataset actually changed.
				// Out-of-band catches (manual fishing between sessions) can be reconciled on demand via
				// ResyncWithFishGuide().
				LoadMissingFishLog();
			}
		}

		/// <summary>
		/// On-demand reconcile of the disk-cached missing set against AgentFishGuide2's live catch data,
		/// dropping anything that's actually already been caught. Deliberately NOT called at Start:
		/// GetFishList() visibly opens and closes the in-game Fish Guide, which is poor UX every session
		/// when the cache is already kept current by per-catch RemoveFish + SaveMissingFishLog. Exposed
		/// for a manual "resync" to recover fish caught out-of-band — e.g. manual fishing between
		/// sessions, or a missed catch-detection edge case — that never routed through RemoveFish and
		/// would otherwise stay marked "missing" until the next full rebuild.
		/// </summary>
		public static async Task ResyncWithFishGuide()
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

		// Manual Fish Guide reconcile, triggered by the status-bar refresh icon (ShellWindow). The
		// WPF click can't run ResyncWithFishGuide directly — GetFishList opens the in-game Fish Guide,
		// a framethread game action — so the click just raises _resyncRequested and the botbase
		// coroutine drains it via ProcessPendingResync at a point where the line isn't out.
		private static volatile bool _resyncRequested;
		private static volatile bool _resyncRunning;

		/// <summary>Raise a request to reconcile the missing-fish cache against the in-game Fish Guide.
		/// Safe to call from the WPF thread; actual work happens on the botbase coroutine.</summary>
		public static void RequestResync() => _resyncRequested = true;

		/// <summary>True from the instant a resync is requested until it finishes — drives the status-bar
		/// spinner. _resyncRunning is set before _resyncRequested is cleared in ProcessPendingResync, so a
		/// UI poll never catches a false "done" gap between the two.</summary>
		public static bool ResyncPending => _resyncRequested || _resyncRunning;

		/// <summary>Botbase-coroutine pickup (framethread): run a pending manual reconcile if one was
		/// requested, else a cheap no-op. Must only be awaited where the line isn't out, since it can open
		/// the in-game Fish Guide. Swallows its own errors so a reconcile failure never aborts the voyage.</summary>
		public static async Task ProcessPendingResync()
		{
			if (!_resyncRequested)
				return;

			_resyncRunning = true;
			_resyncRequested = false;
			try
			{
				Logging.Write("[Ocean Trip] Manual Fish Guide reconcile requested — reading the in-game Fish Guide...");
				await ResyncWithFishGuide();
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Manual Fish Guide reconcile failed: {ex.Message}");
			}
			finally
			{
				_resyncRunning = false;
			}
		}

		public static void SaveMissingFishLog()
		{
			if (File.Exists(fileName))
				File.Delete(fileName);

			var lines = new List<string> { SignatureHeaderPrefix + FishDataSignature() };
			lines.AddRange(_cachedMissingFishSet.Select(x => x.ToString()));
			File.WriteAllLines(fileName, lines);
		}

		private const string SignatureHeaderPrefix = "#fishDataSig=";

		/// <summary>
		/// Stable signature of the current ocean-fish dataset — the fish count plus an order-independent
		/// FNV-1a hash of the fish IDs. The disk cache is keyed on this so it rebuilds not only when fish
		/// are ADDED/REMOVED (count change), but also when an existing fish's ID is CORRECTED with the
		/// count unchanged (e.g. the Junior Jinbei 51236 → 51687 fix). The old count-only header missed
		/// that class of change, leaving corrected fish stuck flagged "missing" until some later update
		/// happened to alter the count.
		/// </summary>
		private static string FishDataSignature()
		{
			var ids = FishDataCache.GetFish().Select(f => f.FishID).OrderBy(x => x).ToList();
			unchecked
			{
				uint hash = 2166136261;
				foreach (var id in ids)
				{
					hash ^= (uint)id;
					hash *= 16777619;
				}
				return $"{ids.Count}:{hash:X8}";
			}
		}

		public static void LoadMissingFishLog()
		{
			_cachedMissingFishSet = ReadMissingFishFile();
		}
	}
}
