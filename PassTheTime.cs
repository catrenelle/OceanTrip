using System.Threading;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using Clio.Utilities;
using Buddy.Coroutines;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Helpers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner
{
    class PassTheTime
    {
		public static bool freeToCraft;
		private static SpellData action;

		public static async Task Craft()
		{
			if (freeToCraft)
			{
				if ((freeToCraft && DataManager.GetItem(27870).ItemCount() < 10) && OceanTripSettings.Instance.OceanFood == true)
				{
					await IdleLisbeth(27870, 40, "Culinarian", "false"); //Peppered Popotos
				}

				//Potions
				while (freeToCraft && (DataManager.GetItem(29979).ItemCount() >= 40) && OceanTripSettings.Instance.CraftPotions)
				{
					await IdleLisbeth(29492, 60, "Alchemist", "false"); //Grade 3 Tincture of Strength
				}

				//Food
				if (freeToCraft && OceanTripSettings.Instance.CraftFood)
				{
					while (freeToCraft && DataManager.GetItem(30482).ItemCount() < 300)
					{
						await IdleLisbeth(30482, 52, "Culinarian", "false"); //Chili Crab
					}
					while (freeToCraft && DataManager.GetItem(29501).ItemCount() < 300)
					{
						await IdleLisbeth(29501, 52, "Culinarian", "false"); //Sausage and Sauerkraut
					}
					while (freeToCraft && DataManager.GetItem(29502).ItemCount() < 300)
					{
						await IdleLisbeth(29502, 52, "Culinarian", "false"); //Stuffed Highland Cabbage
					}
					while (freeToCraft && DataManager.GetItem(29504).ItemCount() < 300)
					{
						await IdleLisbeth(29504, 52, "Culinarian", "false"); //Herring Pie
					}
					while (freeToCraft && DataManager.GetItem(29497).ItemCount() < 120)
					{
						await IdleLisbeth(29497, 52, "Culinarian", "false"); //Ovim Meatballs
					}
				}

				//Gear
				if (freeToCraft && OceanTripSettings.Instance.CraftGear)
				{				
					if (freeToCraft && (DataManager.GetItem(30462).ItemCount() == 0))
					{
						await IdleLisbeth(30462, 1, "Weaver", "false", 1030482); //Crafter Body
					}
					if (freeToCraft && (DataManager.GetItem(30465).ItemCount() == 0))
					{
						await IdleLisbeth(30465, 1, "Leatherworker", "false", 1030482); //Crafter Legs
					}
					if (freeToCraft && (DataManager.GetItem(30464).ItemCount() == 0))
					{
						await IdleLisbeth(30464, 1, "Weaver", "false", 1030482); //Crafter Pants
					}
					if (freeToCraft && (DataManager.GetItem(30463).ItemCount() == 0))
					{
						await IdleLisbeth(30463, 2, "Leatherworker", "false", 1030482); //Crafter Gloves
					}
				}

				//Materia
				if (freeToCraft && OceanTripSettings.Instance.GetMateria)
				{				
					while (freeToCraft && DataManager.GetItem(25194).ItemCount() < 10)
					{
						await IdleLisbeth(25194, 10, "Exchange", "false"); //crafter competence vii
					}
					while (freeToCraft && DataManager.GetItem(25195).ItemCount() < 500)
					{
						await IdleLisbeth(25195, 10, "Exchange", "false"); //crafter cunning vii
					}
					while (freeToCraft && DataManager.GetItem(25196).ItemCount() < 400)
					{
						await IdleLisbeth(25196, 10, "Exchange", "false"); //crafter command vii
					}

					while (freeToCraft && DataManager.GetItem(5703).ItemCount() < 100)
					{
						await IdleLisbeth(5703, 10, "Exchange", "false"); //crafter competence v
					}
					while (freeToCraft && DataManager.GetItem(5708).ItemCount() < 100)
					{
						await IdleLisbeth(5708, 10, "Exchange", "false"); //crafter cunning v
					}
					while (freeToCraft && DataManager.GetItem(5713).ItemCount() < 100)
					{
						await IdleLisbeth(5713, 10, "Exchange", "false"); //crafter command v
					}

					while (freeToCraft && DataManager.GetItem(26735).ItemCount() < 30)
					{
						await IdleLisbeth(26735, 4, "Exchange", "false"); //crafter materia
					}
					while (freeToCraft && DataManager.GetItem(26736).ItemCount() < 30)
					{
						await IdleLisbeth(26736, 4, "Exchange", "false"); //crafter materia
					}
					while (freeToCraft && DataManager.GetItem(26737).ItemCount() < 30)
					{
						await IdleLisbeth(26737, 4, "Exchange", "false"); //crafter materia
					}
				}

				//Scrip
				if (freeToCraft && OceanTripSettings.Instance.RefillScrips)
				{
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)25199) <= 1500)
					{
						await IdleLisbeth(25199, 500, "CraftMasterpiece", "false"); //White Scrip
					}
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)17833) <= 1500)
					{
						await IdleLisbeth(17833, 500, "CraftMasterpiece", "false"); //Yellow Scrip
					}
				}

				//Multifaceted
				//while (TimeCheck(59) && DataManager.GetItem(27744).ItemCount() >= 20 && DataManager.GetItem(27737).ItemCount() >= 40)
				//{
				//	await IdleLisbeth(27743, 10, "Leatherworker", "false"); //chalicotherium leather
				//}			
				//while (TimeCheck(59) && DataManager.GetItem(27695).ItemCount() >= 20)
				//{
				//	await IdleLisbeth(27694, 10, "Carpenter", "false"); //sandalwood lumber
				//}
				//while (TimeCheck(59) && DataManager.GetItem(27762).ItemCount() >= 50)
				//{
				//	await IdleLisbeth(27760, 10, "Weaver", "false"); //ethereal silk
				//}
				//while (TimeCheck(59) && DataManager.GetItem(27718).ItemCount() >= 20)
				//{
				//	await IdleLisbeth(27716, 10, "Blacksmith", "false"); //tungsten steel ingot
				//}
				//while (TimeCheck(59) && DataManager.GetItem(27719).ItemCount() >= 20)
				//{
				//	await IdleLisbeth(27717, 10, "Goldsmith", "false"); //prismatic ingot
				//}

				//Mats
				if (freeToCraft && OceanTripSettings.Instance.CraftMats)
				{
					while (DataManager.GetItem(27714).ItemCount() < 400)
					{
						await IdleLisbeth(27714, 30, "Blacksmith", "false"); //Dwarven Mythril Ingot
					}
					while (DataManager.GetItem(27715).ItemCount() < 400)
					{
						await IdleLisbeth(27715, 30, "Goldsmith", "false"); //Dwarven Mythril Nugget
					}
					while (DataManager.GetItem(27693).ItemCount() < 400)
					{
						await IdleLisbeth(27693, 30, "Carpenter", "false"); //Lignum Vitae Lumber
					}
					while (DataManager.GetItem(27852).ItemCount() < 300)
					{
						await IdleLisbeth(27852, 40, "Grind", "false"); //Ovim Meat
					}
					while (DataManager.GetItem(27742).ItemCount() < 990)
					{
						await IdleLisbeth(27742, 10, "Leatherworker", "false"); //Sea Swallow Leather
					}
				}
			}

			//Shards
			if (freeToCraft && OceanTripSettings.Instance.GatherShards)
			{
				while (freeToCraft && (ConditionParser.ItemCount(4) < 8700))
				{
					await IdleLisbeth(4, 500, "Gather", "false"); //Wind Shard Gets Stuck
				}
				while (freeToCraft && (ConditionParser.ItemCount(10) < 8700))
				{
					await IdleLisbeth(10, 500, "Gather", "false"); //Wind Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(2) < 8700))
				{
					await IdleLisbeth(2, 500, "Gather", "false"); //Fire Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(8) < 8700))
				{
					await IdleLisbeth(8, 500, "Gather", "false"); //Fire Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(3) < 8700))
				{
					await IdleLisbeth(3, 500, "Gather", "false"); //Ice Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(9) < 8700))
				{
					await IdleLisbeth(9, 500, "Gather", "false"); //Ice Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(5) < 8700))
				{
					await IdleLisbeth(5, 500, "Gather", "false"); //Earth Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(11) < 8700))
				{
					await IdleLisbeth(11, 500, "Gather", "false"); //Earth Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(6) < 8700))
				{
					await IdleLisbeth(6, 500, "Gather", "false"); //Lightning Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(12) < 8700))
				{
					await IdleLisbeth(12, 500, "Gather", "false"); //Lightning Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(7) < 8700))
				{
					await IdleLisbeth(7, 500, "Gather", "false"); //Water Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(13) < 8700))
				{
					await IdleLisbeth(13, 500, "Gather", "false"); //Water Crystal
				}
			}

			//Fieldcraft III
			if (freeToCraft && OceanTripSettings.Instance.FieldcraftIII)
			{
				await IdleLisbeth(5118, 30, "Gather", "false"); //Gold Ore

				while (freeToCraft && (DataManager.GetItem(5118).ItemCount() >= 204))
				{
					await IdleLisbeth(7615, 34, "Goldsmith", "false"); //Rose Gold Cog
					await ExchangeCogs();
				}
				while (freeToCraft && (DataManager.GetItem(5068).ItemCount() >= 4))
				{
					await IdleLisbeth(7615, (int)DataManager.GetItem(5068).ItemCount() / 2, "Goldsmith", "false"); //Rose Gold Cog
					await ExchangeCogs();
				}
			}
		}

		public static async Task IdleLisbeth(int itemId, int amount, string type, string quicksynth, int food = 4717)
		{
			Log($"{type}ing {amount} {DataManager.GetItem((uint)itemId).CurrentLocaleName}");

			if (BotManager.Bots.FirstOrDefault(c => c.Name == "Lisbeth") != null)
			{
				await Lisbeth.ExecuteOrders("[{'Item':" + itemId + ",'Amount':" + amount + ",'Type':'" + type + "','QuickSynth':" + quicksynth + ",'Food':" + food + ",'Enabled': true}]");

				await Coroutine.Sleep(2000);
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
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickLineEquals("Nothing");
				}
				if (CraftingManager.IsCrafting == true)
				{
					Log("Lisbeth borked. Trying to finish craft");
					while (CraftingManager.Progress < CraftingManager.ProgressRequired && CraftingManager.Progress != -1)
					{
						await CraftAction("Basic Synthesis");
					}
				}
				await Coroutine.Sleep(2000);
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

		private static async Task ExchangeCogs()
		{
			await Navigation.GetTo(156, new Vector3(26.29833f, 28.99997f, -730.8021f));
			var Talan = GameObjectManager.GetObjectByNPCId(1006972);

			while (DataManager.GetItem(7615).ItemCount() > 0)
			{
				if (Talan != null)
				{
					Talan.Interact();
					if (await Coroutine.Wait(20000, () => SelectIconString.IsOpen))
					{
						ff14bot.RemoteWindows.SelectIconString.ClickSlot(2);
						if (await Coroutine.Wait(20000, () => Talk.DialogOpen))
						{
							Talk.Next();
						}
					}

					AtkAddonControl CogExchangeWindow = RaptureAtkUnitManager.GetWindowByName("ShopExchangeItem");
					CogExchangeWindow = null;
					while (DataManager.GetItem(7615).ItemCount() > 0 && InventoryManager.FreeSlots > 2)
					{
						while (CogExchangeWindow == null)
						{
							await Coroutine.Sleep(200);
							CogExchangeWindow = RaptureAtkUnitManager.GetWindowByName("ShopExchangeItem");
						}
						await Coroutine.Sleep(200);
						CogExchangeWindow.SendAction(2, 0, 0, 1, 3);

						CogExchangeWindow = null;
						while (CogExchangeWindow == null)
						{
							await Coroutine.Sleep(200);
							CogExchangeWindow = RaptureAtkUnitManager.GetWindowByName("ShopExchangeItemDialog");
						}
						await Coroutine.Sleep(200);
						CogExchangeWindow.SendAction(1, 0, 0);

						if (await Coroutine.Wait(20000, () => Request.IsOpen))
						{
							foreach (BagSlot slot in InventoryManager.FilledSlots)
							{
								if (slot.RawItemId == 7615)
								{
									slot.Handover();
								}
							}
						}
						await Coroutine.Sleep(200);
						if (await Coroutine.Wait(2000, () => Request.HandOverButtonClickable))
						{
							Request.HandOver();
							await Coroutine.Wait(20000, () => !Request.IsOpen);
							await Coroutine.Sleep(500);
						}
						if (DataManager.GetItem(7615).ItemCount() == 1)
						{
							await Coroutine.Sleep(500);
						}
						CogExchangeWindow = null;
					}

					while (CogExchangeWindow == null)
					{
						await Coroutine.Sleep(1000);
						CogExchangeWindow = RaptureAtkUnitManager.GetWindowByName("ShopExchangeItem");
					}
					CogExchangeWindow.SendAction(1, 3, uint.MaxValue);
				}
				await DesynthOcean(new int[] { 7521 });
				await Coroutine.Sleep(1000);
				await DesynthOcean(new int[] { 7521 });
			}
		}

		public static async Task DesynthOcean(int[] itemId)
		{
			var itemsToDesynth = InventoryManager.FilledSlots.Where(bs => bs.IsDesynthesizable && itemId.Contains((int)bs.RawItemId));

			if (itemsToDesynth.Count() != 0)
			{
				Log("Desynthing...");
				foreach (var item in itemsToDesynth)
				{
					var name = item.EnglishName;
					var currentStackSize = item.Item.StackSize;

					while (item.Count > 0)
					{
						await CommonTasks.Desynthesize(item, 20000);

						await Coroutine.Wait(20000, () => (!item.IsFilled || !item.EnglishName.Equals(name) || item.Count != currentStackSize));
					}
					await Coroutine.Sleep(200);
				}
				Log("Desynth complete");
			}
		}

		private static void Log(string text)
		{
			var msg = "[Ocean Trip] " + text;
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
