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
				int lisFood = 0;
                int hqOffset = 1000000;
                bool hqFood = false;

                if (OceanTripNewSettings.Instance.useCraftingFood)
                {
#if RB_DT
                    int food = FoodList.RroneekSteak;
#else
                    int food = FoodList.CalamariRipieni;
#endif

                    if (DataManager.GetItem((uint)food, true).ItemCount() > 0)
                    {
                        lisFood = food + hqOffset; // HQ
                        hqFood = true;
                    }
                    else
                        lisFood = food; // Reg
                }

                //Resume last order
                try
                {
                    if (freeToCraft && File.Exists($"Settings\\{Core.Me.Name}_World{WorldHelper.HomeWorldId}\\lisbeth-resume.json") && OceanTripNewSettings.Instance.resumeLisbeth)
					{
							if (File.ReadAllText($"Settings\\{Core.Me.Name}_World{WorldHelper.HomeWorldId}\\lisbeth-resume.json") != "[]")
							{
								Log("Resuming last Lisbeth order.");
								await Lisbeth.ExecuteOrders(File.ReadAllText($"Settings\\{Core.Me.Name}_World{WorldHelper.HomeWorldId}\\lisbeth-resume.json"));
							}
					}
                }
                catch { Log("Encountered an error with lisbeth-resume.json, ignoring the file/setting."); }

                //Custom Order
                try
                {
                    if (freeToCraft && File.Exists("BoatOrder.json") && OceanTripNewSettings.Instance.customBoatOrders)
							await Lisbeth.ExecuteOrders(File.ReadAllText("BoatOrder.json"));
                }
                catch { Log("Encountered error reading BoatOrder.json, ignoring the file."); }

                //Ocean Food
                if (lisFood > 0)
                {
                    int foodCount = foodCount = (int)DataManager.GetItem((uint)(hqFood ? lisFood-hqOffset : lisFood), hqFood).ItemCount();
                    if (freeToCraft && foodCount < 10)
                    {
                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Log($"Farming 100 of {DataManager.GetItem((uint)(hqFood ? lisFood-hqOffset : lisFood), hqFood).CurrentLocaleName}.");


                        await IdleLisbeth((hqFood ? lisFood-hqOffset : lisFood), 100, "Culinarian", "false", (foodCount > 0 ? lisFood : 0));
                    }
                }

                //Shards
                if (freeToCraft)
                {
                    var crystalList = new List<int>();
                    
                    if (OceanTripNewSettings.Instance.firecrystal)
                    {
                        crystalList.Add(Crystals.FireCluster);
                        crystalList.Add(Crystals.FireCrystal);
                        crystalList.Add(Crystals.FireShard);
                    }

                    if (OceanTripNewSettings.Instance.icecrystal)
                    {
                        crystalList.Add(Crystals.IceCluster);
                        crystalList.Add(Crystals.IceCrystal);
                        crystalList.Add(Crystals.IceShard);
                    }

                    if (OceanTripNewSettings.Instance.windcrystal)
                    {
                        crystalList.Add(Crystals.WindCluster);
                        crystalList.Add(Crystals.WindCrystal);
                        crystalList.Add(Crystals.WindShard);
                    }

                    if (OceanTripNewSettings.Instance.earthcrystal)
                    {
                        crystalList.Add(Crystals.EarthCluster);
                        crystalList.Add(Crystals.EarthCrystal);
                        crystalList.Add(Crystals.EarthShard);
                    }

                    if (OceanTripNewSettings.Instance.lightningcrystal)
                    {
                        crystalList.Add(Crystals.LightningCluster);
                        crystalList.Add(Crystals.LightningCrystal);
                        crystalList.Add(Crystals.LightningShard);
                    }

                    if (OceanTripNewSettings.Instance.watercrystal)
                    {
                        crystalList.Add(Crystals.WaterCluster);
                        crystalList.Add(Crystals.WaterCrystal);
                        crystalList.Add(Crystals.WaterShard);
                    }


                    foreach (var crystal in crystalList)
                    {
                        if (OceanTripNewSettings.Instance.LoggingMode && ConditionParser.ItemCount((uint)crystal) < 9000)
                            Log($"Farming {(9000 - ConditionParser.ItemCount((uint)crystal))} of {DataManager.GetItem((uint)crystal).CurrentLocaleName} in bundles of 500.");

                        while (freeToCraft && ConditionParser.ItemCount((uint)crystal) < 9000)
                            await IdleLisbeth(crystal, 500, "Gather", "false", 0);
                    }

                    crystalList.Clear();
                }

                //Scrip
                if (freeToCraft)
                {
					var currencyList = new List<int>();
                    if (OceanTripNewSettings.Instance.refillScrips)
                    {
#if !RB_DT
                        currencyList.Add((int)Currency.WhiteCraftersScrips);
#endif

                        currencyList.Add((int)Currency.PurpleCraftersScrips);

#if RB_DT
                        currencyList.Add((int)Currency.OrangeCraftersScrips);
#endif
                    }

					foreach (var currency in currencyList)
					{
                        if (OceanTripNewSettings.Instance.LoggingMode && (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency) <= 3000)
                            Log($"Farming {(3000 - (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency))} of {SpecialCurrencyManager.SpecialCurrencies.Where(x => x.Item.Id == (uint)currency).FirstOrDefault().Item.CurrentLocaleName}.");

                        while (freeToCraft && (int)SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency)currency) <= 3000)
							await IdleLisbeth(currency, 500, "CraftMasterpiece", "false", 0);
					}
                }


				//Food
				if (freeToCraft)
				{
					var foodList = new List<int>();
                    if (OceanTripNewSettings.Instance.food1)
                        foodList.Add(OceanTripPlanner.Definitions.Defaults.raidfood[0]);
                    if (OceanTripNewSettings.Instance.food2)
                        foodList.Add(OceanTripPlanner.Definitions.Defaults.raidfood[1]);
                    if (OceanTripNewSettings.Instance.food3)
                        foodList.Add(OceanTripPlanner.Definitions.Defaults.raidfood[2]);
                    if (OceanTripNewSettings.Instance.food4)
                        foodList.Add(OceanTripPlanner.Definitions.Defaults.raidfood[3]);

                    foreach (var food in foodList)
					{
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(food) < 150)
                            Log($"Farming {(150 - inventoryCount(food))} of {DataManager.GetItem((uint)food).CurrentLocaleName} in increments of 50.");

                        while (freeToCraft && inventoryCount(food) < 150)
							await IdleLisbeth(food, 50, "Culinarian", "false", lisFood);
					}

					foodList.Clear();
                }

                //Potions
                if (freeToCraft)
                {
					var potionList = new List<int>();

                    if (OceanTripNewSettings.Instance.potion1)
                        potionList.Add(OceanTripPlanner.Definitions.Defaults.raidpotions[0]);
                    if (OceanTripNewSettings.Instance.potion2)
                        potionList.Add(OceanTripPlanner.Definitions.Defaults.raidpotions[1]);
                    if (OceanTripNewSettings.Instance.potion3)
                        potionList.Add(OceanTripPlanner.Definitions.Defaults.raidpotions[2]);
                    if (OceanTripNewSettings.Instance.potion4)
                        potionList.Add(OceanTripPlanner.Definitions.Defaults.raidpotions[3]);

                    foreach (var potion in potionList)
					{
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(potion) <= 200)
                            Log($"Farming 200 of {DataManager.GetItem((uint)potion).CurrentLocaleName}.");

                        while (freeToCraft && inventoryCount(potion) <= 200)
							await IdleLisbeth(potion, 200, "Alchemist", "false", lisFood);
					}

					potionList.Clear();
                }

                //Mats
                if (freeToCraft)
				{
					var itemList = new List<int>();
                    if (OceanTripNewSettings.Instance.material1)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[0]);
                    if (OceanTripNewSettings.Instance.material2)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[1]);
                    if (OceanTripNewSettings.Instance.material3)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[2]);
                    if (OceanTripNewSettings.Instance.material4)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[3]);
                    if (OceanTripNewSettings.Instance.material5)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[4]);
                    if (OceanTripNewSettings.Instance.material6)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[5]);
                    if (OceanTripNewSettings.Instance.material7)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.materials[6]);

                    foreach (var item in itemList)
					{
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(item) <= 300)
                            Log($"Farming {(300 - inventoryCount(item))} of {DataManager.GetItem((uint)item).CurrentLocaleName} in increments of 50.");

                        while (freeToCraft && inventoryCount(item) <= 300)
							await IdleLisbeth(item, 50, "Exchange", "false", lisFood);
					}
				}

				//Gatherable Aethersand
				if (freeToCraft)
				{
                    var itemList = new List<int>();
                    if (OceanTripNewSettings.Instance.aethersand1)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[0]);
                    if (OceanTripNewSettings.Instance.aethersand2)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[1]);
                    if (OceanTripNewSettings.Instance.aethersand3)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[2]);
                    if (OceanTripNewSettings.Instance.aethersand4)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[3]);
                    if (OceanTripNewSettings.Instance.aethersand6)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[5]);
                    if (OceanTripNewSettings.Instance.aethersand7)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[6]);
                    if (OceanTripNewSettings.Instance.aethersand9)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[8]);

                    foreach (var item in itemList)
                    {
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(item) <= 300)
                            Log($"Farming {(300 - inventoryCount(item))} of {DataManager.GetItem((uint)item).CurrentLocaleName} in increments of 50.");

                        while (freeToCraft && inventoryCount(item) <= 300)
                            await IdleLisbeth(item, 50, "Gather", "false", lisFood);
                    }
                }

                // Exchangable Aethersand
                if (freeToCraft)
                {
                    var itemList = new List<int>();
                    if (OceanTripNewSettings.Instance.aethersand5)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[4]);
                    if (OceanTripNewSettings.Instance.aethersand8)
                        itemList.Add(OceanTripPlanner.Definitions.Defaults.aethersands[7]);

                    foreach (var item in itemList)
                    {
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(item) <= 300)
                            Log($"Farming {(300 - inventoryCount(item))} of {DataManager.GetItem((uint)item).CurrentLocaleName} in increments of 50.");

                        while (freeToCraft && inventoryCount(item) <= 300)
                            await IdleLisbeth(item, 50, "Exchange", "false", lisFood);
                    }
                }


                //Materia
                if (freeToCraft)
				{
					var materiaList = new List<int>();

#if RB_DT
                    // Grade XII
                    if (OceanTripNewSettings.Instance.materiaxii1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[0]);
                    if (OceanTripNewSettings.Instance.materiaxii2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[1]);
                    if (OceanTripNewSettings.Instance.materiaxii3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[2]);
                    if (OceanTripNewSettings.Instance.materiaxii4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[3]);
                    if (OceanTripNewSettings.Instance.materiaxii5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[4]);
                    if (OceanTripNewSettings.Instance.materiaxii6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxii[5]);
                    // Grade XI
                    if (OceanTripNewSettings.Instance.materiaxi1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[0]);
                    if (OceanTripNewSettings.Instance.materiaxi2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[1]);
                    if (OceanTripNewSettings.Instance.materiaxi3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[2]);
                    if (OceanTripNewSettings.Instance.materiaxi4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[3]);
                    if (OceanTripNewSettings.Instance.materiaxi5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[4]);
                    if (OceanTripNewSettings.Instance.materiaxi6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaxi[5]);
#endif

                    // Grade X
                    if (OceanTripNewSettings.Instance.materiax1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[0]);
                    if (OceanTripNewSettings.Instance.materiax2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[1]);
                    if (OceanTripNewSettings.Instance.materiax3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[2]);
                    if (OceanTripNewSettings.Instance.materiax4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[3]);
                    if (OceanTripNewSettings.Instance.materiax5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[4]);
                    if (OceanTripNewSettings.Instance.materiax6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiax[5]);
                    // Grade IX
                    if (OceanTripNewSettings.Instance.materiaix1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[0]);
                    if (OceanTripNewSettings.Instance.materiaix2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[1]);
                    if (OceanTripNewSettings.Instance.materiaix3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[2]);
                    if (OceanTripNewSettings.Instance.materiaix4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[3]);
                    if (OceanTripNewSettings.Instance.materiaix5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[4]);
                    if (OceanTripNewSettings.Instance.materiaix6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaix[5]);
                    // Grade VIII
                    if (OceanTripNewSettings.Instance.materiaviii1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[0]);
                    if (OceanTripNewSettings.Instance.materiaviii2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[1]);
                    if (OceanTripNewSettings.Instance.materiaviii3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[2]);
                    if (OceanTripNewSettings.Instance.materiaviii4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[3]);
                    if (OceanTripNewSettings.Instance.materiaviii5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[4]);
                    if (OceanTripNewSettings.Instance.materiaviii6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaviii[5]);
                    // Grade VII
                    if (OceanTripNewSettings.Instance.materiavii1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[0]);
                    if (OceanTripNewSettings.Instance.materiavii2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[1]);
                    if (OceanTripNewSettings.Instance.materiavii3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[2]);
                    if (OceanTripNewSettings.Instance.materiavii4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[3]);
                    if (OceanTripNewSettings.Instance.materiavii5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[4]);
                    if (OceanTripNewSettings.Instance.materiavii6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavii[5]);
                    // Grade VI
                    if (OceanTripNewSettings.Instance.materiavi1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[0]);
                    if (OceanTripNewSettings.Instance.materiavi2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[1]);
                    if (OceanTripNewSettings.Instance.materiavi3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[2]);
                    if (OceanTripNewSettings.Instance.materiavi4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[3]);
                    if (OceanTripNewSettings.Instance.materiavi5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[4]);
                    if (OceanTripNewSettings.Instance.materiavi6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiavi[5]);
                    // Grade V
                    if (OceanTripNewSettings.Instance.materiav1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[0]);
                    if (OceanTripNewSettings.Instance.materiav2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[1]);
                    if (OceanTripNewSettings.Instance.materiav3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[2]);
                    if (OceanTripNewSettings.Instance.materiav4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[3]);
                    if (OceanTripNewSettings.Instance.materiav5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[4]);
                    if (OceanTripNewSettings.Instance.materiav6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiav[5]);
                    // Grade IV
                    if (OceanTripNewSettings.Instance.materiaiv1)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[0]);
                    if (OceanTripNewSettings.Instance.materiaiv2)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[1]);
                    if (OceanTripNewSettings.Instance.materiaiv3)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[2]);
                    if (OceanTripNewSettings.Instance.materiaiv4)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[3]);
                    if (OceanTripNewSettings.Instance.materiaiv5)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[4]);
                    if (OceanTripNewSettings.Instance.materiaiv6)
                        materiaList.Add(OceanTripPlanner.Definitions.Defaults.materiaiv[5]);

                    foreach (var materia in materiaList)
					{
                        if (OceanTripNewSettings.Instance.LoggingMode && inventoryCount(materia) <= 200)
                            Log($"Farming {(200-inventoryCount(materia))} of {DataManager.GetItem((uint)materia).CurrentLocaleName} in increments of 20.");

						while(freeToCraft && inventoryCount(materia) <= 200)
                            await IdleLisbeth(materia, 20, "Exchange", "false", 0);
                    }
				}
			}
		}


		public static async Task IdleLisbeth(int itemId, int amount, string type, string quicksynth, int food, bool defaultmode = false)
		{
			Log($"{type}ing {amount} {DataManager.GetItem((uint)itemId).CurrentLocaleName}");

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
