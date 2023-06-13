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
using System.Windows.Documents;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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
							await Lisbeth.ExecuteOrders(File.ReadAllText("BoatOrder.json"));
                }
                catch { Log("Encountered error reading BoatOrder.json, ignoring the file."); }

                //Ocean Food
                if (OceanTripSettings.Instance.OceanFood != OceanFood.None)
                {
                    if (freeToCraft && DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood).ItemCount() < 10)
                    {
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose)
                            Log($"Farming 100 of {DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood).CurrentLocaleName}.");


                        await IdleLisbeth((int)OceanTripSettings.Instance.OceanFood, 100, "Culinarian", "false", lisFood);
                    }
                }

                //Shards
                if (freeToCraft && OceanTripSettings.Instance.GatherShards)
                {
                    var crystalList = new List<int>();
                    crystalList.Add(Crystals.WindShard);
                    crystalList.Add(Crystals.WindCluster);
                    crystalList.Add(Crystals.WindCrystal);
                    crystalList.Add(Crystals.FireShard);
                    crystalList.Add(Crystals.FireCluster);
                    crystalList.Add(Crystals.FireCrystal);
                    crystalList.Add(Crystals.IceShard);
                    crystalList.Add(Crystals.IceCluster);
                    crystalList.Add(Crystals.IceCrystal);
                    crystalList.Add(Crystals.EarthShard);
                    crystalList.Add(Crystals.EarthCluster);
                    crystalList.Add(Crystals.EarthCrystal);
                    crystalList.Add(Crystals.LightningShard);
                    crystalList.Add(Crystals.LightningCluster);
                    crystalList.Add(Crystals.LightningCrystal);
                    crystalList.Add(Crystals.WaterShard);
                    crystalList.Add(Crystals.WaterCluster);
                    crystalList.Add(Crystals.WaterCrystal);


                    foreach (var crystal in crystalList)
                    {
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && ConditionParser.ItemCount((uint)crystal) < 9000)
                            Log($"Farming {(9000 - ConditionParser.ItemCount((uint)crystal))} of {DataManager.GetItem((uint)crystal).CurrentLocaleName} in bundles of 500.");

                        while (freeToCraft && ConditionParser.ItemCount((uint)crystal) < 9000)
                            await IdleLisbeth(crystal, 500, "Gather", "false", 0);
                    }

                    crystalList.Clear();
                }

                //Scrip
                if (freeToCraft && OceanTripSettings.Instance.RefillScrips)
                {
					var currencyList = new List<int>();
                    currencyList.Add((int)Currency.WhiteCraftersScrips);
                    currencyList.Add((int)Currency.PurpleCraftersScrips);

					foreach (var currency in currencyList)
					{
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency) <= 3000)
                            Log($"Farming {(3000 - (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency))} of {SpecialCurrencyManager.SpecialCurrencies.Where(x => x.Item.Id == (uint)currency).FirstOrDefault().Item.CurrentLocaleName}.");

                        while (freeToCraft && (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency) <= 3000)
							await IdleLisbeth(currency, 500, "CraftMasterpiece", "false", 0);
					}
                }


				//Food
				if (freeToCraft && OceanTripSettings.Instance.CraftFood)
				{
					var foodList = new List<int>();
					foodList.Add(FoodList.HoneyedDragonfruit);
					foodList.Add(FoodList.BabaGhanoush);
					foodList.Add(FoodList.BakedEggplant);
                    foodList.Add(FoodList.CaviarCanapes);

                    foreach (var food in foodList)
					{
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && DataManager.GetItem((uint)food).ItemCount() < 150)
                            Log($"Farming {(150 - DataManager.GetItem((uint)food).ItemCount())} of {DataManager.GetItem((uint)food).CurrentLocaleName}.");

                        while (freeToCraft && DataManager.GetItem((uint)food).ItemCount() < 150)
							await IdleLisbeth(food, 400, "Culinarian", "false", lisFood);
					}

					foodList.Clear();
                }

                //Potions
                if (freeToCraft && OceanTripSettings.Instance.CraftPotions != 0)
                {
					var potionList = new List<int>();

					switch(OceanTripSettings.Instance.CraftPotions)
					{
						case LisbethPotionCrafting.Grade8:
                            potionList.Add(Potions.Grade8TinctureStrength);
                            potionList.Add(Potions.Grade8TinctureDexterity);
                            potionList.Add(Potions.Grade8TinctureIntelligence);
                            potionList.Add(Potions.Grade8TinctureMind);
                            break;

                        case LisbethPotionCrafting.Grade7:
							potionList.Add(Potions.Grade7TinctureStrength);
							potionList.Add(Potions.Grade7TinctureDexterity);
							potionList.Add(Potions.Grade7TinctureIntelligence);
                            potionList.Add(Potions.Grade7TinctureMind);
                            break;

						default:
							potionList.Add(Potions.Grade6TinctureStrength);
							potionList.Add(Potions.Grade6TinctureDexterity);
							potionList.Add(Potions.Grade6TinctureIntelligence);
                            potionList.Add(Potions.Grade6TinctureMind);
                            break;
					}

					foreach (var potion in potionList)
					{
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && DataManager.GetItem((uint)potion).ItemCount() <= 200)
                            Log($"Farming {(200 - DataManager.GetItem((uint)potion).ItemCount())} of {DataManager.GetItem((uint)potion).CurrentLocaleName}.");

                        while (freeToCraft && DataManager.GetItem((uint)potion).ItemCount() <= 200)
							await IdleLisbeth(potion, 200, "Alchemist", "false", lisFood);
					}

					potionList.Clear();
                }

                //Mats
                if (freeToCraft && OceanTripSettings.Instance.CraftMats)
				{
					var itemList = new List<int>();
					itemList.Add(Material.ImmutableSolution);
                    itemList.Add(Material.DinosaurLeather);
                    itemList.Add(Material.Sphalerite);
                    itemList.Add(Material.RoyalMistletoe);
                    itemList.Add(Material.CloudCottonBoll);
                    itemList.Add(Material.CloudMythrilOre);
                    itemList.Add(Material.StormcloudCottonBoll);


                    foreach (var item in itemList)
					{
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && DataManager.GetItem((uint)item).ItemCount() <= 300)
                            Log($"Farming {(300 - DataManager.GetItem((uint)item).ItemCount())} of {DataManager.GetItem((uint)item).CurrentLocaleName}.");

                        while (freeToCraft && DataManager.GetItem((uint)item).ItemCount() <= 300)
							await IdleLisbeth(item, 50, "Exchange", "false", lisFood);
					}
				}

				//Aethersand
				if (freeToCraft && OceanTripSettings.Instance.Aethersand)
				{
                    var itemList = new List<int>();
                    itemList.Add(Material.DusklightAethersand);
                    itemList.Add(Material.DawnlightAethersand);
                    itemList.Add(Material.EverbrightAethersand);
                    itemList.Add(Material.EverbornAethersand);
                    itemList.Add(Material.EverdeepAethersand);
                    itemList.Add(Material.EndstoneAethersand);
                    itemList.Add(Material.EndwoodAethersand);
                    itemList.Add(Material.EndtideAethersand);
                    itemList.Add(Material.EarthbreakAethersand);

                    foreach (var item in itemList)
                    {
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && DataManager.GetItem((uint)item).ItemCount() <= 300)
                            Log($"Farming {(300 - DataManager.GetItem((uint)item).ItemCount())} of {DataManager.GetItem((uint)item).CurrentLocaleName}.");

                        while (freeToCraft && DataManager.GetItem((uint)item).ItemCount() <= 300)
                            await IdleLisbeth(item, 50, "Exchange", "false", lisFood);
                    }
                }

                //Materia
                if (freeToCraft && OceanTripSettings.Instance.GetMateria != LisbethMateriaGathering.None)
				{
					var materiaList = new List<int>();
					switch(OceanTripSettings.Instance.GetMateria)
					{
                        case LisbethMateriaGathering.Grade_X:
                            materiaList.Add(Materia.CrafterCompetenceX);
                            materiaList.Add(Materia.CrafterCunningX);
                            materiaList.Add(Materia.CrafterCommandX);
                            materiaList.Add(Materia.GatherGuerdonX);
                            materiaList.Add(Materia.GatherGuileX);
                            materiaList.Add(Materia.GatherGraspX);
                            break;
                        case LisbethMateriaGathering.Grade_IX:
                            materiaList.Add(Materia.CrafterCompetenceIX);
                            materiaList.Add(Materia.CrafterCunningIX);
                            materiaList.Add(Materia.CrafterCommandIX);
                            materiaList.Add(Materia.GatherGuerdonIX);
                            materiaList.Add(Materia.GatherGuileIX);
                            materiaList.Add(Materia.GatherGraspIX);
                            break;
                        case LisbethMateriaGathering.Grade_VIII:
                            materiaList.Add(Materia.CrafterCompetenceVIII);
                            materiaList.Add(Materia.CrafterCunningVIII);
                            materiaList.Add(Materia.CrafterCommandVIII);
                            materiaList.Add(Materia.GatherGuerdonVIII);
                            materiaList.Add(Materia.GatherGuileVIII);
                            materiaList.Add(Materia.GatherGraspVIII);
                            break;
                        case LisbethMateriaGathering.Grade_VII:
                            materiaList.Add(Materia.CrafterCompetenceVII);
                            materiaList.Add(Materia.CrafterCunningVII);
                            materiaList.Add(Materia.CrafterCommandVII);
                            materiaList.Add(Materia.GatherGuerdonVII);
                            materiaList.Add(Materia.GatherGuileVII);
                            materiaList.Add(Materia.GatherGraspVII);
                            break;
                        case LisbethMateriaGathering.Grade_VI:
                            materiaList.Add(Materia.CrafterCompetenceVI);
                            materiaList.Add(Materia.CrafterCunningVI);
                            materiaList.Add(Materia.CrafterCommandVI);
                            materiaList.Add(Materia.GatherGuerdonVI);
                            materiaList.Add(Materia.GatherGuileVI);
                            materiaList.Add(Materia.GatherGraspVI);
                            break;
                        case LisbethMateriaGathering.Grade_V:
                            materiaList.Add(Materia.CrafterCompetenceV);
                            materiaList.Add(Materia.CrafterCunningV);
                            materiaList.Add(Materia.CrafterCommandV);
                            materiaList.Add(Materia.GatherGuerdonV);
                            materiaList.Add(Materia.GatherGuileV);
                            materiaList.Add(Materia.GatherGraspV);
                            break;
                        case LisbethMateriaGathering.Grade_IV:
                            materiaList.Add(Materia.CrafterCompetenceIV);
                            materiaList.Add(Materia.CrafterCunningIV);
                            materiaList.Add(Materia.CrafterCommandIV);
                            materiaList.Add(Materia.GatherGuerdonIV);
                            materiaList.Add(Materia.GatherGuileIV);
                            materiaList.Add(Materia.GatherGraspIV);
                            break;
						default:
                            break;
                    }

					foreach(var materia in materiaList)
					{
                        if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose && DataManager.GetItem((uint)materia).ItemCount() <= 200)
                            Log($"Farming {(200-DataManager.GetItem((uint)materia).ItemCount())} of {DataManager.GetItem((uint)materia).CurrentLocaleName}.");

						while(freeToCraft && DataManager.GetItem((uint)materia).ItemCount() <= 200)
                            await IdleLisbeth(materia, 20, "Exchange", "false", 0);
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
				Log($"Desynthing {itemsToDesynth.Count()} valid inventory slots...");
				foreach (var item in itemsToDesynth)
				{
					await Coroutine.Sleep(500);

					var name = item.EnglishName;
					var currentStackSize = item.Item.StackSize;

                    if (OceanTripSettings.Instance.LoggingMode == LoggingMode.Verbose)
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

		private static void Log(string text)
		{
			var msg = "[Ocean Trip] " + text;
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
