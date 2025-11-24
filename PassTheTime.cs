using System;
using System.Threading;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using Clio.Utilities;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Helpers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using OceanTripPlanner.Definitions;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LlamaLibrary.Helpers;
using LlamaLibrary.Helpers.WorldTravel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Lisbeth = OceanTripPlanner.Helpers.Lisbeth;
using Ocean_Trip.Definitions;
using OceanTripPlanner.IdleActivities;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner
{
	/// <summary>
	/// Manages idle activities between ocean fishing trips using strategy pattern
	/// </summary>
	class PassTheTime
	{
		public static bool freeToCraft;
		private static SpellData action;
		private static readonly IdleActivityManager _activityManager = new IdleActivityManager();

		/// <summary>
		/// Execute all configured idle activities using the strategy pattern
		/// </summary>
		public static async Task Craft()
		{
			if (!freeToCraft)
				return;

			// Prepare Lisbeth food ID if crafting food is enabled
			int lisFood = 0;
			int hqOffset = 1000000;

			if (OceanTripNewSettings.Instance.useCraftingFood)
			{
				int food = FoodList.RroneekSteak;

				if (DataManager.GetItem((uint)food, true).ItemCount() > 0)
					lisFood = food + hqOffset; // HQ
				else
					lisFood = food; // Regular
			}

			// Build context for all activities
			var context = new IdleActivityContext
			{
				IsFreeToCraft = () => freeToCraft,
				ExecuteLisbethCallback = IdleLisbeth,
				GetInventoryCountCallback = inventoryCount,
				LogCallback = Log,
				LoggingMode = OceanTripNewSettings.Instance.LoggingMode,
				LisbethFoodId = lisFood
			};

			// Execute all activities through the manager
			await _activityManager.ExecuteActivitiesAsync(context);
		}


		/// <summary>
		/// Execute a Lisbeth order and handle cleanup of any leftover windows
		/// </summary>
		/// <param name="itemId">Item to craft/gather/exchange</param>
		/// <param name="amount">Quantity to obtain</param>
		/// <param name="type">Type of action (Gather, Culinarian, Alchemist, Exchange, CraftMasterpiece)</param>
		/// <param name="quicksynth">Whether to use quick synth (string "true" or "false")</param>
		/// <param name="food">Food buff to use (0 for none)</param>
		/// <param name="defaultmode">Use default amount mode instead of absolute</param>
		public static async Task IdleLisbeth(int itemId, int amount, string type, string quicksynth, int food, bool defaultmode = false)
		{
			Log($"{type}ing {amount} {ItemDataCache.GetItemName((uint)itemId)}");

			if (BotManager.Bots.FirstOrDefault(c => c.Name == "Lisbeth") != null)
			{
				await Lisbeth.ExecuteOrders("[{'Item':" + itemId + ",'Amount':" + amount + ",'Type':'" + type + "','QuickSynth':" + quicksynth + ",'Food':" + food + ",'Enabled': true, 'IsPrimary': true, 'AmountMode':'" + (defaultmode ? "Default" : "Absolute") + "'}]");

				AtkAddonControl masterWindow = RaptureAtkUnitManager.GetWindowByName("MasterPieceSupply");
				if (masterWindow != null)
				{
					masterWindow.SendAction(1, 3uL, 4294967295uL);
				}
				if (ShopExchangeCurrency.Open)
				{
					ShopExchangeCurrency.Close();
					await Coroutine.Sleep(2000);
				}
				if (SelectString.IsOpen)
				{
					SelectString.ClickLineEquals("Cancel");
				}
				NPCInteractionHelper.CancelSelectIconString();
				if (CraftingManager.IsCrafting == true)
				{
					Log("Lisbeth borked. Trying to finish craft");
					while (CraftingManager.Progress < CraftingManager.ProgressRequired && CraftingManager.Progress != -1)
					{
						await CraftAction("Basic Synthesis");
					}
					await Coroutine.Sleep(2000);
				}       
				if (CraftingLog.IsOpen == true)
				{
					CraftingLog.Close();
					await Coroutine.Wait(10000, () => !CraftingLog.IsOpen);
					await Coroutine.Wait(Timeout.InfiniteTimeSpan, () => !CraftingManager.AnimationLocked);
				}
				Log("Crafting complete");
			}
			else
			{
				Log("Failed to craft.");
			}
		}
		/// <summary>
		/// Execute a single crafting action (used for recovery when Lisbeth fails)
		/// </summary>
		private static async Task CraftAction(string actionName)
		{
			ActionManager.CurrentActions.TryGetValue(actionName, out action);
			await Coroutine.Wait(Timeout.InfiniteTimeSpan, () => !CraftingManager.AnimationLocked);

			if (ActionManager.CanCast(action, null))
			{
				ActionManager.DoAction(action, null);
			}

			await Coroutine.Wait(10000, () => CraftingManager.AnimationLocked);
			await Coroutine.Wait(Timeout.InfiniteTimeSpan, () => !CraftingManager.AnimationLocked);

			await Coroutine.Sleep(500);
		}

		/// <summary>
		/// Desynthesize ocean fishing catches
		/// </summary>
		/// <param name="itemId">List of fish item IDs to desynth</param>
		public static async Task DesynthOcean(List<int> itemId)
		{
			var itemsToDesynth = InventoryManager.FilledSlots.Where(bs => bs.IsDesynthesizable && itemId.Contains((int)bs.RawItemId));

			if (itemsToDesynth.Count() != 0)
			{
				Log($"Desynthing {itemsToDesynth.Count()} valid inventory slots...");
				foreach (var item in itemsToDesynth)
				{
					await Coroutine.Sleep(500);

					var name = item.EnglishName;
					var currentStackSize = item.Item.StackSize;

					if (OceanTripNewSettings.Instance.LoggingMode)
						Log($"Desynthing {name}, stack size of {item.Count}.");

					while (item.Count > 0)
					{
						await LlamaLibrary.Utilities.Inventory.Desynth(item);
						await Coroutine.Wait(20000, () => (!item.IsFilled || !item.EnglishName.Equals(name) || item.Count != currentStackSize));
					}

					await Coroutine.Sleep(500);
				}
				Log("Desynth complete");
			}
		}

		/// <summary>
		/// Get total inventory count (normal + HQ) for an item
		/// </summary>
		public static int inventoryCount(int id)
		{
			int normal = (int)DataManager.GetItem((uint)id, false).ItemCount();
			int hq = (int)DataManager.GetItem((uint)id, true).ItemCount();

			return (normal + hq);
		}

		private static void Log(string text)
		{
			var msg = "[Ocean Trip] " + text;
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
