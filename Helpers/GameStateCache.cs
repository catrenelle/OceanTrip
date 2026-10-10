using System;
using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Caches frequently-accessed game state to reduce API calls and improve performance
	/// </summary>
	public class GameStateCache
	{
		private static GameStateCache _instance;
		public static GameStateCache Instance => _instance ?? (_instance = new GameStateCache());

		// Cache update frequency
		private DateTime _lastUpdate = DateTime.MinValue;
		private const int CACHE_REFRESH_MS = 100; // Refresh every 100ms max

		// Cached game state
		public uint CurrentWeatherId { get; private set; }
		public string CurrentWeather { get; private set; }
		public int CurrentGP { get; private set; }
		public int MaxGP { get; private set; }
		public float CurrentGPPercent { get; private set; }
		public int GPDeficit { get; private set; }
		public uint CurrentZoneId { get; private set; }

		// Item name cache (persists across refreshes)
		private readonly Dictionary<uint, string> _itemNameCache = new Dictionary<uint, string>();
		private readonly Dictionary<Tuple<uint, bool>, string> _itemNameHQCache = new Dictionary<Tuple<uint, bool>, string>();

		private GameStateCache()
		{
			Refresh();
		}

		/// <summary>
		/// Refresh cached game state if cache is stale
		/// </summary>
		public void RefreshIfNeeded()
		{
			var now = DateTime.Now;
			if ((now - _lastUpdate).TotalMilliseconds >= CACHE_REFRESH_MS)
			{
				Refresh();
			}
		}

		/// <summary>
		/// Force refresh of all cached game state
		/// </summary>
		public void Refresh()
		{
			_lastUpdate = DateTime.Now;

			CurrentWeatherId = WorldManager.CurrentWeatherId;
			CurrentWeather = WorldManager.CurrentWeather;
			CurrentZoneId = WorldManager.RawZoneId;

			if (Core.Me != null)
			{
				CurrentGP = Core.Me.CurrentGP;
				MaxGP = Core.Me.MaxGP;
				CurrentGPPercent = Core.Me.CurrentGPPercent;
				GPDeficit = MaxGP - CurrentGP;
			}
		}

		/// <summary>
		/// Get cached item name, or fetch and cache if not present
		/// </summary>
		public string GetItemName(uint itemId)
		{
			if (!_itemNameCache.TryGetValue(itemId, out string name))
			{
				name = DataManager.GetItem(itemId).CurrentLocaleName;
				_itemNameCache[itemId] = name;
			}
			return name;
		}

		/// <summary>
		/// Get cached item name with HQ support
		/// </summary>
		public string GetItemName(uint itemId, bool isHQ)
		{
			var key = Tuple.Create(itemId, isHQ);
			if (!_itemNameHQCache.TryGetValue(key, out string name))
			{
				name = DataManager.GetItem(itemId, isHQ).CurrentLocaleName;
				_itemNameHQCache[key] = name;
			}
			return name;
		}

		/// <summary>
		/// Clear all caches (useful when stopping bot)
		/// </summary>
		public void ClearAll()
		{
			_itemNameCache.Clear();
			_itemNameHQCache.Clear();
			_lastUpdate = DateTime.MinValue;
		}

		/// <summary>
		/// Check if player has enough GP for an action
		/// </summary>
		public bool HasGP(int required)
		{
			return CurrentGP >= required;
		}

		/// <summary>
		/// Check if GP deficit exceeds threshold
		/// </summary>
		public bool NeedsGPRecovery(int threshold)
		{
			return GPDeficit >= threshold;
		}
	}
}
