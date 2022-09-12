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
using OceanTripPlanner.Definitions;
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
				int lisFood = (int)OceanTripSettings.Instance.LisbethFood;

				//Ocean Food
				if (OceanTripSettings.Instance.OceanFood != OceanFood.None)
				{
					if ((freeToCraft && DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood).ItemCount() < 10))
					{
						await IdleLisbeth((int)OceanTripSettings.Instance.OceanFood, 40, "Culinarian", "false", lisFood);
					}
				}


                //Resume last order
                if (freeToCraft && File.Exists($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json") && OceanTripSettings.Instance.ResumeOrder)
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
				if (freeToCraft && OceanTripSettings.Instance.CraftPotions)
				{ 
					while (freeToCraft && (DataManager.GetItem(36109).ItemCount() <= 200))
					{
						await IdleLisbeth(36109, 150, "Alchemist", "false", lisFood); //Grade 6 Tincture of Strength
					}
					while (freeToCraft && (DataManager.GetItem(36110).ItemCount() <= 200))
					{
						await IdleLisbeth(36110, 150, "Alchemist", "false", lisFood); //Grade 6 Tincture of Dexterity
					}
					while (freeToCraft && (DataManager.GetItem(36112).ItemCount() <= 200))
					{
						await IdleLisbeth(36112, 150, "Alchemist", "false", lisFood); //Grade 6 Tincture of Intelligence
					}
				}

				//Food
				if (freeToCraft && OceanTripSettings.Instance.CraftFood)
				{
					while (freeToCraft && DataManager.GetItem(36060).ItemCount() < 150)
					{
						await IdleLisbeth(36060, 400, "Culinarian", "false", lisFood); //Tsai tou Vounou
					}
					while (freeToCraft && DataManager.GetItem(36069).ItemCount() < 150)
					{
						await IdleLisbeth(36069, 400, "Culinarian", "false", lisFood); //Pumpkin Ratatouille
					}
					while (freeToCraft && DataManager.GetItem(36067).ItemCount() < 150)
					{
						await IdleLisbeth(36067, 400, "Culinarian", "false", lisFood); //Archon Burger
					}
					while (freeToCraft && DataManager.GetItem(36074).ItemCount() < 150)
					{
						await IdleLisbeth(36074, 400, "Culinarian", "false", lisFood); //Thavnairian Chai
					}
					while (freeToCraft && DataManager.GetItem(36070).ItemCount() < 150)
					{
						await IdleLisbeth(36070, 400, "Culinarian", "false", lisFood); //Pumpkin Potage
					}
				}


				/* MATS and MATERIA are removed because Lisbeth Exchange is broken */
				/*
				//Mats
				if (freeToCraft && OceanTripSettings.Instance.CraftMats)
				{
					while (freeToCraft && (DataManager.GetItem(37284).ItemCount() <= 300))
					{
						await IdleLisbeth(37284, 50, "Exchange", "false", lisFood); //Immutable Solution
					}
				}

				//Materia
				if (freeToCraft && OceanTripSettings.Instance.GetMateria)
				{				
					while (freeToCraft && DataManager.GetItem(33925).ItemCount() < 200)
					{
						await IdleLisbeth(33925, 20, "Exchange", "false", 0); //crafter competence IX
					}
					while (freeToCraft && DataManager.GetItem(33926).ItemCount() < 200)
					{
						await IdleLisbeth(33926, 20, "Exchange", "false", 0); //crafter cunning IX
					}
					while (freeToCraft && DataManager.GetItem(33927).ItemCount() < 200)
					{
						await IdleLisbeth(33927, 20, "Exchange", "false", 0); //crafter command IX
					}

					while (freeToCraft && DataManager.GetItem(33938).ItemCount() < 200)
					{
						await IdleLisbeth(33938, 30, "Exchange", "false", 0); //crafter competence X
					}
					while (freeToCraft && DataManager.GetItem(33939).ItemCount() < 200)
					{
						await IdleLisbeth(33939, 30, "Exchange", "false", 0); //crafter cunning X
					}
					while (freeToCraft && DataManager.GetItem(33940).ItemCount() < 200)
					{
						await IdleLisbeth(33940, 30, "Exchange", "false", 0); //crafter command X
					}
				}*/

				//Scrip
				if (freeToCraft && OceanTripSettings.Instance.RefillScrips)
				{
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)Currency.WhiteCraftersScrips) <= 1500)
					{
						await IdleLisbeth((int)Currency.WhiteCraftersScrips, 500, "CraftMasterpiece", "false", 0); 
					}
					while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)Currency.PurpleCraftersScrips) <= 1500)
					{
						await IdleLisbeth((int)Currency.PurpleCraftersScrips, 500, "CraftMasterpiece", "false", 0); 
					}
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
				while (freeToCraft && (ConditionParser.ItemCount(14) < 9000))
				{
					await IdleLisbeth(14, 500, "Gather", "false", 0); //Fire Cluster
				}
				while (freeToCraft && (ConditionParser.ItemCount(15) < 9000))
				{
					await IdleLisbeth(15, 500, "Gather", "false", 0); //Ice Cluster
				}
				while (freeToCraft && (ConditionParser.ItemCount(16) < 9000))
				{
					await IdleLisbeth(16, 500, "Gather", "false", 0); //Wind Cluster
				}
				while (freeToCraft && (ConditionParser.ItemCount(17) < 9000))
				{
					await IdleLisbeth(17, 500, "Gather", "false", 0); //Earth Cluster
				}
				while (freeToCraft && (ConditionParser.ItemCount(18) < 9000))
				{
					await IdleLisbeth(18, 500, "Gather", "false", 0); //Lightning Cluster
				}
				while (freeToCraft && (ConditionParser.ItemCount(19) < 9000))
				{
					await IdleLisbeth(19, 500, "Gather", "false", 0); //Water Cluster
				}
			}
		}


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
