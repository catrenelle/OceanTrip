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
using OceanTripPlanner.Helpers;
using System.IO;

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
				//Ocean Food
				if ((freeToCraft && DataManager.GetItem(27870).ItemCount() < 10) && OceanTripSettings.Instance.OceanFood)
				{
					await IdleLisbeth(27870, 40, "Culinarian", "false", (int)OceanTripSettings.Instance.LisbethFood); //Peppered Popotos
				}

				//Resume last order
				if (freeToCraft && File.Exists($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json"))
				{
					if (File.ReadAllText($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json") != "[]")
					{
						Log("Resuming last Lisbeth order.");
						await Lisbeth.ExecuteOrders(File.ReadAllText($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json"));
					}
				}

				//Custom Order
				if (freeToCraft && File.Exists("BoatOrder.json") && OceanTripSettings.Instance.CustomOrder)
				{
					await Lisbeth.ExecuteOrders(File.ReadAllText("BoatOrder.json"));
				}

				//Potions
				while (freeToCraft && (DataManager.GetItem(32949).ItemCount() >= 40) && OceanTripSettings.Instance.CraftPotions)
				{
					await IdleLisbeth(31893, 60, "Alchemist", "false", 1030482); //Grade 4 Tincture of Strength
				}

				//Food
				if (freeToCraft && OceanTripSettings.Instance.CraftFood)
				{
					while (freeToCraft && DataManager.GetItem(30482).ItemCount() < 200)
					{
						await IdleLisbeth(30482, 200, "Culinarian", "false", (int)OceanTripSettings.Instance.LisbethFood); //Chili Crab
					}
					while (freeToCraft && DataManager.GetItem(31898).ItemCount() < 200)
					{
						await IdleLisbeth(31898, 200, "Culinarian", "false", 1030482); //Pizza
					}
					while (freeToCraft && DataManager.GetItem(31900).ItemCount() < 200)
					{
						await IdleLisbeth(31900, 200, "Culinarian", "false", 1030482); //Chicken Fettuccine
					}
					while (freeToCraft && DataManager.GetItem(31901).ItemCount() < 200)
					{
						await IdleLisbeth(31901, 200, "Culinarian", "false", 1030482); //Smoked Chicken
					}
					while (freeToCraft && DataManager.GetItem(31905).ItemCount() < 200)
					{
						await IdleLisbeth(31905, 200, "Culinarian", "false", 1030482); //Twilight Popoto Salad
					}
					while (freeToCraft && DataManager.GetItem(29497).ItemCount() < 39)
					{
						await IdleLisbeth(29497, 50, "Culinarian", "false", (int)OceanTripSettings.Instance.LisbethFood); //Ovim Meatballs
					}
					while (freeToCraft && DataManager.GetItem(31320).ItemCount() < 50)
					{
						await IdleLisbeth(31320, 10, "Exchange", "false", 0); //Slithersand
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
						await IdleLisbeth(30463, 1, "Leatherworker", "false", 1030482); //Crafter Gloves
					}
				}

				//Materia
				if (freeToCraft && OceanTripSettings.Instance.GetMateria)
				{				
					while (freeToCraft && DataManager.GetItem(25194).ItemCount() < 200)
					{
						await IdleLisbeth(25194, 20, "Exchange", "false", 0); //crafter competence vii
					}
					while (freeToCraft && DataManager.GetItem(25195).ItemCount() < 200)
					{
						await IdleLisbeth(25195, 20, "Exchange", "false", 0); //crafter cunning vii
					}
					while (freeToCraft && DataManager.GetItem(25196).ItemCount() < 200)
					{
						await IdleLisbeth(25196, 20, "Exchange", "false", 0); //crafter command vii
					}

					while (freeToCraft && DataManager.GetItem(5703).ItemCount() < 200)
					{
						await IdleLisbeth(5703, 30, "Exchange", "false", 0); //crafter competence v
					}
					while (freeToCraft && DataManager.GetItem(5708).ItemCount() < 200)
					{
						await IdleLisbeth(5708, 30, "Exchange", "false", 0); //crafter cunning v
					}
					while (freeToCraft && DataManager.GetItem(5713).ItemCount() < 200)
					{
						await IdleLisbeth(5713, 30, "Exchange", "false", 0); //crafter command v
					}

					while (freeToCraft && DataManager.GetItem(26735).ItemCount() < 50)
					{
						await IdleLisbeth(26735, 10, "Exchange", "false", 0); //crafter competence viii
					}
					while (freeToCraft && DataManager.GetItem(26736).ItemCount() < 50)
					{
						await IdleLisbeth(26736, 10, "Exchange", "false", 0); //crafter cunning viii
					}
					while (freeToCraft && DataManager.GetItem(26737).ItemCount() < 200)
					{
						await IdleLisbeth(26737, 20, "Exchange", "false", 0); //crafter command viii
					}
				}

				//Scrip
				if (freeToCraft && OceanTripSettings.Instance.RefillScrips)
				{
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)25199) <= 1500)
					{
						await IdleLisbeth(25199, 500, "CraftMasterpiece", "false", (int)OceanTripSettings.Instance.LisbethFood); //White Scrip
					}
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)17833) <= 1500)
					{
						await IdleLisbeth(17833, 500, "CraftMasterpiece", "false", (int)OceanTripSettings.Instance.LisbethFood); //Yellow Scrip
					}
				}

				//Multifaceted
				while (freeToCraft && DataManager.GetItem(29977).ItemCount() >= 20)
				{
					await IdleLisbeth(29962, 10, "Leatherworker", "false", (int)OceanTripSettings.Instance.LisbethFood); //megalania leather
				}
				while (freeToCraft && DataManager.GetItem(29973).ItemCount() >= 20)
				{
					await IdleLisbeth(29960, 10, "Carpenter", "false", (int)OceanTripSettings.Instance.LisbethFood); //merbau lumber
				}
				while (freeToCraft && DataManager.GetItem(29975).ItemCount() >= 50)
				{
					await IdleLisbeth(29961, 10, "Weaver", "false", (int)OceanTripSettings.Instance.LisbethFood); //duskcourt cloth
				}
				while (freeToCraft && DataManager.GetItem(29969).ItemCount() >= 20)
				{
					await IdleLisbeth(29958, 10, "Blacksmith", "false", (int)OceanTripSettings.Instance.LisbethFood); //cobalt alloy ingot
				}
				while (freeToCraft && DataManager.GetItem(29971).ItemCount() >= 20)
				{
					await IdleLisbeth(29959, 10, "Goldsmith", "false", (int)OceanTripSettings.Instance.LisbethFood); //workbench resin
				}

				//Mats
				if (freeToCraft && OceanTripSettings.Instance.CraftMats)
				{
					while (DataManager.GetItem(27714).ItemCount() < 400)
					{
						await IdleLisbeth(27714, 30, "Blacksmith", "false", (int)OceanTripSettings.Instance.LisbethFood); //Dwarven Mythril Ingot
					}
					while (DataManager.GetItem(27715).ItemCount() < 400)
					{
						await IdleLisbeth(27715, 30, "Goldsmith", "false", (int)OceanTripSettings.Instance.LisbethFood); //Dwarven Mythril Nugget
					}
					while (DataManager.GetItem(27693).ItemCount() < 400)
					{
						await IdleLisbeth(27693, 30, "Carpenter", "false", (int)OceanTripSettings.Instance.LisbethFood); //Lignum Vitae Lumber
					}
					while (DataManager.GetItem(27852).ItemCount() < 300)
					{
						await IdleLisbeth(27852, 40, "Grind", "false", (int)OceanTripSettings.Instance.LisbethFood); //Ovim Meat
					}
					//while (DataManager.GetItem(27742).ItemCount() < 990)
					//{
					//	await IdleLisbeth(27742, 10, "Leatherworker", "false", (int)OceanTripSettings.Instance.LisbethFood); //Sea Swallow Leather
					//}
				}
			}

			//Shards
			if (freeToCraft && OceanTripSettings.Instance.GatherShards)
			{
				while (freeToCraft && (ConditionParser.ItemCount(4) < 9000))
				{
					await IdleLisbeth(4, 500, "Gather", "false", 0); //Wind Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(10) < 9000))
				{
					await IdleLisbeth(10, 500, "Gather", "false", 0); //Wind Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(2) < 9000))
				{
					await IdleLisbeth(2, 500, "Gather", "false", 0); //Fire Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(8) < 9000))
				{
					await IdleLisbeth(8, 500, "Gather", "false", 0); //Fire Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(3) < 9000))
				{
					await IdleLisbeth(3, 500, "Gather", "false", 0); //Ice Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(9) < 9000))
				{
					await IdleLisbeth(9, 500, "Gather", "false", 0); //Ice Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(5) < 9000))
				{
					await IdleLisbeth(5, 500, "Gather", "false", 0); //Earth Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(11) < 9000))
				{
					await IdleLisbeth(11, 500, "Gather", "false", 0); //Earth Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(6) < 9000))
				{
					await IdleLisbeth(6, 500, "Gather", "false", 0); //Lightning Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(12) < 9000))
				{
					await IdleLisbeth(12, 500, "Gather", "false", 0); //Lightning Crystal
				}
				while (freeToCraft && (ConditionParser.ItemCount(7) < 9000))
				{
					await IdleLisbeth(7, 500, "Gather", "false", 0); //Water Shard
				}
				while (freeToCraft && (ConditionParser.ItemCount(13) < 9000))
				{
					await IdleLisbeth(13, 500, "Gather", "false", 0); //Water Crystal
				}
			}

			//Fieldcraft III
			if (freeToCraft && OceanTripSettings.Instance.FieldcraftIII)
			{
				while (freeToCraft && (DataManager.GetItem(5118).ItemCount() < 204))
				{
					await IdleLisbeth(5118, 30, "Gather", "false", 0); //Gold Ore
				}
				while (freeToCraft && (DataManager.GetItem(5118).ItemCount() >= 204))
				{
					await IdleLisbeth(7615, 34, "Goldsmith", "false", 0); //Rose Gold Cog
					await ExchangeCogs();
				}
				while (freeToCraft && (DataManager.GetItem(5068).ItemCount() >= 4))
				{
					await IdleLisbeth(7615, (int)DataManager.GetItem(5068).ItemCount() / 2, "Goldsmith", "false", 0); //Rose Gold Cog
					await ExchangeCogs();
				}
			}
		}

		//stone soup: 4717
		//seafood stew: 12865
		public static async Task IdleLisbeth(int itemId, int amount, string type, string quicksynth, int food)
		{
			Log($"{type}ing {amount} {DataManager.GetItem((uint)itemId).CurrentLocaleName}");

			if (BotManager.Bots.FirstOrDefault(c => c.Name == "Lisbeth") != null)
			{
				await Lisbeth.ExecuteOrders("[{'Item':" + itemId + ",'Amount':" + amount + ",'Type':'" + type + "','QuickSynth':" + quicksynth + ",'Food':" + food + ",'Enabled': true}]");

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
					await Coroutine.Sleep(500);
					var name = item.EnglishName;
					var currentStackSize = item.Item.StackSize;

					while (item.Count > 0)
					{
						await CommonTasks.Desynthesize(item, 20000);
						await Coroutine.Wait(5000, () => SalvageDialog.IsOpen);
						if (SalvageDialog.IsOpen)
						{
							RaptureAtkUnitManager.GetWindowByName("SalvageDialog").SendAction(1, 3, 0);
							await Coroutine.Wait(10000, () => SalvageResult.IsOpen);

							if (SalvageResult.IsOpen)
							{
								SalvageResult.Close();
								await Coroutine.Wait(5000, () => !SalvageResult.IsOpen);
								await Coroutine.Sleep(2000);
							}
							else
							{
								Log("Result didn't open");
								break;
							}
						}
						else
						{
							Log("SalvageDialog didn't open");
							break;
						}
						await Coroutine.Wait(20000, () => (!item.IsFilled || !item.EnglishName.Equals(name) || item.Count != currentStackSize));
					}
					await Coroutine.Sleep(500);
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
