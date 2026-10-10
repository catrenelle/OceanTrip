using System.Collections.Generic;
using ff14bot.Managers;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Caches static item data (names, levels) to reduce DataManager lookups
	/// Note: Does NOT cache dynamic data like ItemCount() which changes frequently
	/// </summary>
	public static class ItemDataCache
	{
		private static readonly Dictionary<uint, string> _itemNames = new Dictionary<uint, string>();
		private static readonly Dictionary<uint, int> _itemLevels = new Dictionary<uint, int>();

		/// <summary>
		/// Get the localized name for an item (cached)
		/// </summary>
		/// <param name="itemId">Item ID to lookup</param>
		/// <param name="isHQ">Whether to get HQ version name (default: false)</param>
		/// <returns>Localized item name</returns>
		public static string GetItemName(uint itemId, bool isHQ = false)
		{
			// Create unique key for HQ vs NQ items
			uint cacheKey = isHQ ? itemId + 1000000 : itemId;

			if (!_itemNames.ContainsKey(cacheKey))
			{
				var item = DataManager.GetItem(itemId, isHQ);
				_itemNames[cacheKey] = item?.CurrentLocaleName ?? $"Unknown Item {itemId}";
			}

			return _itemNames[cacheKey];
		}

		/// <summary>
		/// Get the required level for an item (cached)
		/// </summary>
		/// <param name="itemId">Item ID to lookup</param>
		/// <returns>Required level for the item</returns>
		public static int GetItemRequiredLevel(uint itemId)
		{
			if (!_itemLevels.ContainsKey(itemId))
			{
				var item = DataManager.GetItem(itemId);
				_itemLevels[itemId] = item?.RequiredLevel ?? 0;
			}

			return _itemLevels[itemId];
		}

		/// <summary>
		/// Get current item count (NOT cached - always fresh from game data)
		/// This is a convenience method that doesn't cache since inventory changes frequently
		/// </summary>
		/// <param name="itemId">Item ID to check</param>
		/// <param name="isHQ">Whether to check HQ version (default: false)</param>
		/// <returns>Current count in inventory</returns>
		public static uint GetItemCount(uint itemId, bool isHQ = false)
		{
			return DataManager.GetItem(itemId, isHQ)?.ItemCount() ?? 0;
		}

		/// <summary>
		/// Clear all cached data (use when changing characters or after major game updates)
		/// </summary>
		public static void ClearCache()
		{
			_itemNames.Clear();
			_itemLevels.Clear();
		}
	}
}
