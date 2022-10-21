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
            //Shards
            if (freeToCraft && OceanTripSettings.Instance.GatherShards)
            {
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WindShard) < 9000))
                {
                    await IdleLisbeth(Crystals.WindShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WindCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.WindCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WindCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.WindCrystal, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.FireShard) < 9000))
                {
                    await IdleLisbeth(Crystals.FireShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.FireCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.FireCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.FireCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.FireCrystal, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.IceShard) < 9000))
                {
                    await IdleLisbeth(Crystals.IceShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.IceCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.IceCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.IceCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.IceCrystal, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.EarthShard) < 9000))
                {
                    await IdleLisbeth(Crystals.EarthShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.EarthCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.EarthCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.EarthCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.EarthCrystal, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.LightningShard) < 9000))
                {
                    await IdleLisbeth(Crystals.LightningShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.LightningCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.LightningCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.LightningCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.LightningCrystal, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WaterShard) < 9000))
                {
                    await IdleLisbeth(Crystals.WaterShard, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WaterCluster) < 9000))
                {
                    await IdleLisbeth(Crystals.WaterCluster, 500, "Gather", "false", 0);
                }
                while (freeToCraft && (ConditionParser.ItemCount((uint)Crystals.WaterCrystal) < 9000))
                {
                    await IdleLisbeth(Crystals.WaterCrystal, 500, "Gather", "false", 0);
                }
            }

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
                try
                {
                    if (freeToCraft && File.Exists($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json") && OceanTripSettings.Instance.ResumeOrder)
					{
							if (File.ReadAllText($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json") != "[]")
							{
								Log("Resuming last Lisbeth order.");
								await Lisbeth.ExecuteOrders(File.ReadAllText($"Settings\\{Core.Me.Name}_World{OceanTrip.HomeWorld}\\lisbeth-resume.json"));
							}
					}
                }
                catch { Log("Encountered an error with lisbeth-resume.json, ignoring the file/setting."); }

                //Custom Order
                try
                {
                    if (freeToCraft && File.Exists("BoatOrder.json") && OceanTripSettings.Instance.CustomOrder)
					{
							await Lisbeth.ExecuteOrders(File.ReadAllText("BoatOrder.json"));
					}
                }
                catch { Log("Encountered error reading BoatOrder.json, ignoring the file."); }


                //Scrip
                if (freeToCraft && OceanTripSettings.Instance.RefillScrips)
                {
                    while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)Currency.WhiteCraftersScrips) <= 3000)
                    {
                        await IdleLisbeth((int)Currency.WhiteCraftersScrips, 500, "CraftMasterpiece", "false", 0);
                    }
                    while (freeToCraft && SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)Currency.PurpleCraftersScrips) <= 3000)
                    {
                        await IdleLisbeth((int)Currency.PurpleCraftersScrips, 500, "CraftMasterpiece", "false", 0);
                    }
                }


				//Food
				if (freeToCraft && OceanTripSettings.Instance.CraftFood)
				{
					while (freeToCraft && DataManager.GetItem((uint)FoodList.CalamariRipieni).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.CalamariRipieni, 400, "Culinarian", "false", lisFood);
                    }
					while (freeToCraft && DataManager.GetItem((uint)FoodList.TsaiTouVounou).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.TsaiTouVounou, 400, "Culinarian", "false", lisFood);
					}
					while (freeToCraft && DataManager.GetItem((uint)FoodList.PumpkinRatatouille).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.PumpkinRatatouille, 400, "Culinarian", "false", lisFood);
					}
					while (freeToCraft && DataManager.GetItem((uint)FoodList.ArchonBurger).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.ArchonBurger, 400, "Culinarian", "false", lisFood);
					}
					while (freeToCraft && DataManager.GetItem((uint)FoodList.PumpkinPotage).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.PumpkinPotage, 400, "Culinarian", "false", lisFood); 
					}
					while (freeToCraft && DataManager.GetItem((uint)FoodList.ThavnairianChai).ItemCount() < 150)
					{
						await IdleLisbeth(FoodList.ThavnairianChai, 400, "Culinarian", "false", lisFood); 
					}
                }

                //Potions
                if (freeToCraft && OceanTripSettings.Instance.CraftPotions != 0)
                {
                    if (OceanTripSettings.Instance.CraftPotions == LisbethPotionCrafting.Grade7)
                    {
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade7TinctureStrength).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade7TinctureStrength, 150, "Alchemist", "false", lisFood);
                        }
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade7TinctureDexterity).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade7TinctureDexterity, 150, "Alchemist", "false", lisFood);
                        }
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade7TinctureIntelligence).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade7TinctureIntelligence, 150, "Alchemist", "false", lisFood);
                        }
                    }
                    else if (OceanTripSettings.Instance.CraftPotions == LisbethPotionCrafting.Grade6)
                    {
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade6TinctureStrength).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade6TinctureStrength, 150, "Alchemist", "false", lisFood);
                        }
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade6TinctureDexterity).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade6TinctureDexterity, 150, "Alchemist", "false", lisFood);
                        }
                        while (freeToCraft && (DataManager.GetItem((uint)Potions.Grade6TinctureIntelligence).ItemCount() <= 200))
                        {
                            await IdleLisbeth(Potions.Grade6TinctureIntelligence, 150, "Alchemist", "false", lisFood);
                        }
                    }
                }

                //Mats
                if (freeToCraft && OceanTripSettings.Instance.CraftMats)
				{
					while (freeToCraft && (DataManager.GetItem((uint)Material.ImmutableSolution).ItemCount() <= 300))
					{
						await IdleLisbeth(Material.ImmutableSolution, 50, "Exchange", "false", lisFood); 
					}
				}

				//Materia
				if (freeToCraft && OceanTripSettings.Instance.GetMateria)
				{				
					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCompetenceIX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCompetenceIX, 20, "Exchange", "false", 0);
					}
					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCunningIX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCunningIX, 20, "Exchange", "false", 0);
					}
					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCommandIX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCommandIX, 20, "Exchange", "false", 0);
					}

					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCompetenceX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCompetenceX, 30, "Exchange", "false", 0); 
					}
					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCunningX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCunningX, 30, "Exchange", "false", 0); 
					}
					while (freeToCraft && DataManager.GetItem((uint)Materia.CrafterCommandX).ItemCount() < 200)
					{
						await IdleLisbeth(Materia.CrafterCommandX, 30, "Exchange", "false", 0); 
					}
				}

			}


		}


		public static async Task IdleLisbeth(int itemId, int amount, string type, string quicksynth, int food)
		{
			Log($"{type}ing {amount} {DataManager.GetItem((uint)itemId).CurrentLocaleName}");

			if (BotManager.Bots.FirstOrDefault(c => c.Name == "Lisbeth") != null)
			{
				await Lisbeth.ExecuteOrders("[{'Item':" + itemId + ",'Amount':" + amount + ",'Type':'" + type + "','QuickSynth':" + quicksynth + ",'Food':" + food + ",'Enabled': true, 'IsPrimary': true, 'AmountMode':'Absolute'}]");
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
						await CommonTasks.Desynthesize(item);
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
