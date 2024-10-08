﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Timers;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using GreyMagic;
using OceanTripPlanner.Helpers;
using OceanTripPlanner.Definitions;
using TreeSharp;
using OceanTrip;
using LlamaLibrary;
using LlamaLibrary.RemoteAgents;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Input;
using Ocean_Trip.Helpers;
using LlamaLibrary.Structs;
using System.Runtime.CompilerServices;
using Ocean_Trip.Definitions;
using Roslyn.Utilities;

namespace OceanTripPlanner
{
	public class OceanTrip : BotBase
	{
		//public override Composite Root => _root;
		private Composite _root;

		public OceanTrip()
		{
		}

		private static readonly Vector3[] fishSpots =
		{
            new Vector3(-7.541584f, 6.74677f, -7.7191f),
            new Vector3(-7.419403f, 6.73973f, -2.7815f),
            new Vector3(7.538965f, 6.745806f, -10.44607f),
            new Vector3(7.178741f, 6.749996f, -4.165483f),
            new Vector3(7.313677f, 6.711103f, -8.10146f),
            new Vector3(7.53893f, 6.745699f, 1.881091f)
		};
		private static readonly float[] headings = new[] 
		{
			4.622331f, 
			4.684318f, 
			1.569952f, 
			1.509215f, 
			1.553197f, 
			1.576235f
        };


		private List<uint> caughtFish;
		private uint lastCaughtFish = 0;
		private bool caughtFishLogged = false;

		private bool ignoreBoat { get { if (OceanTripNewSettings.Instance.FishPriority == FishPriority.IgnoreBoat) { return true; } else { return false; } } }

		private static readonly Tuple<uint, Vector3>[] SummoningBells = new Tuple<uint, Vector3>[]
		{
			new Tuple<uint, Vector3>(Zones.LimsaLominsaLowerDecks, new Vector3(-123.888062f, 17.990356f, 21.469421f)),	// Limsa
			new Tuple<uint, Vector3>(Zones.Uldah,           new Vector3(148.91272f, 3.982544f, -44.205383f)),		// Ul'dah
			new Tuple<uint, Vector3>(Zones.OldGridania,     new Vector3(160.234863f, 15.671021f, -55.649719f)),		// Old Gridania (Gridania) 
			new Tuple<uint, Vector3>(Zones.MorDhona,        new Vector3(11.001709f, 28.976807f, -734.554077f)),		// Mor Dhona (Mor Dhona) 
			new Tuple<uint, Vector3>(Zones.Ishgard,         new Vector3(-151.171204f, -12.64978f, -11.764771f)),	// The Pillars (Ishgard) 
			new Tuple<uint, Vector3>(Zones.Idyllshire,      new Vector3(34.775269f, 208.148193f, -50.858398f)),		// Idyllshire (Dravania)  
			new Tuple<uint, Vector3>(Zones.Kugane,          new Vector3(19.394226f, 4.043579f, 53.025024f)),		// Kugane 
			new Tuple<uint, Vector3>(Zones.RhalgrsReach,    new Vector3(-57.633362f, -0.01532f, 49.30188f)),		// Rhalgr's Reach (Gyr Abania) 
			new Tuple<uint, Vector3>(Zones.Crystarium,      new Vector3(-69.840576f, -7.705872f, 123.491211f)),		// The Crystarium
			new Tuple<uint, Vector3>(Zones.Eulmore,         new Vector3(7.186951f, 83.17688f, 31.448853f))			// Eulmore 
		};

		private static Random rnd = new Random();

		private DateTime startedCast;
        private double bite;


        private bool InitializationComplete = false;

		private Tuple<string, string>[] schedule;
		private bool lastCastMooch = false;
		System.Timers.Timer execute = new System.Timers.Timer();

        public override string Name => "Ocean Trip";

		public override PulseFlags PulseFlags => PulseFlags.All;

		public override bool IsAutonomous => true;
		public override bool RequiresProfile => false;

		public override Composite Root => _root;

		public override bool WantButton { get; } = true;

		private static Ocean_Trip.FormSettings settings;

		public Ocean_Trip.Endeavor Endeavor;


        public override async void OnButtonPress()
		{
			if (settings == null || settings.IsDisposed)
				settings = new Ocean_Trip.FormSettings();

			try
			{
				// Temporary. Future item to come. Currently not working and is a proof of concept. :)
				//settings.tempHideRouteInformationTab();


				settings.Show();
				settings.Activate();
			}
			catch
			{
			}

            FFXIV_Databinds.Instance.RefreshBait();
            FFXIV_Databinds.Instance.RefreshAchievements();


            // Let's pre-cache the fish data
            FishDataCache.GetFish();
			RouteDataCache.GetRoutesWithFish();

            FFXIV_Databinds.Instance.RefreshCurrentRoute();

            if (Endeavor == null)
				Endeavor = new Ocean_Trip.Endeavor();
        }

		public override void Start()
		{
			TreeHooks.Instance.ClearAll();

            Log("Initializing OceanTrip Settings.");
			if (settings == null || settings.IsDisposed)
				settings = new Ocean_Trip.FormSettings();
			Log("OceanTrip Settings Loaded.");


            FFXIV_Databinds.Instance.RefreshBait();
            FFXIV_Databinds.Instance.RefreshAchievements();


			// Let's pre-cache the fish data
            FishDataCache.GetFish();
			RouteDataCache.GetRoutesWithFish();

            if (Endeavor == null)
                Endeavor = new Ocean_Trip.Endeavor();

            caughtFish = new List<uint>();
			lastCaughtFish = 0;
			caughtFishLogged = false;

			TimeSpan timeLeftUntilFirstRun = TimeUntilNextBoat();
			if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
				execute.Interval = 100;
			else
				execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

			execute.Elapsed += new ElapsedEventHandler(KillLisbeth);
			execute.Start();

			Log("BotBase is initialized, beginning execution.");

			_root = new ActionRunCoroutine(r => Run());
		}

		public static TimeSpan TimeUntilNextBoat()
		{
			TimeSpan stop = new TimeSpan();
			int min = (OceanTripNewSettings.Instance.LateBoatQueue ? 13 : 0);


			if ((DateTime.UtcNow.Hour % 2 == 0)
					&& ((DateTime.UtcNow.Minute > 12 && !OceanTripNewSettings.Instance.LateBoatQueue) || (DateTime.UtcNow.Minute > 14 && OceanTripNewSettings.Instance.LateBoatQueue)))
			{
				stop = new TimeSpan(DateTime.UtcNow.Hour + 2, min, 0);
			}
			else
			{
				stop = new TimeSpan(DateTime.UtcNow.Hour + DateTime.UtcNow.Hour % 2, min, 0);
			}

			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;
			return timeLeftUntilFirstRun;
		}

		private async void KillLisbeth(object sender, ElapsedEventArgs e)
		{
			TimeSpan stop = new TimeSpan((DateTime.UtcNow.Hour % 2 == 0 ? DateTime.UtcNow.Hour + 2 : DateTime.UtcNow.Hour), (OceanTripNewSettings.Instance.LateBoatQueue ? 13 : 0), 0); //TimeUntilNextBoat();


			schedule = Routes.GetSchedule();

			if (!ignoreBoat)
			{
				if ((OceanTripNewSettings.Instance.FishPriority != FishPriority.FishLog)
						|| (FocusFishLog && FishingLog.MissingFish().Count() > 0))
				{
					//Log("Stopping Lisbeth!");
					Lisbeth.StopGently();
					PassTheTime.freeToCraft = false;
				}
				else
				{
					Log("Not getting on the boat, no fish needed");
				}
			}

			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;

			if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
				execute.Interval = 100;
			else
				execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

			execute.Start();
		}

		public override void Stop()
		{
			execute.Elapsed -= new ElapsedEventHandler(KillLisbeth);
			_root = null;

			InitializationComplete = false;

			RouteDataCache.InvalidateCache();
			FishDataCache.InvalidateCache();
			FishingLog.InvalidateCache();

			Navigator.NavigationProvider = new NullProvider();
			Navigator.Clear();
		}

		private async Task<bool> Run()
		{
			Navigator.PlayerMover = new SlideMover();
			Navigator.NavigationProvider = new ServiceNavigationProvider();
			caughtFish.Clear();


			//FishingLog.MissingFish();
			await FishingLog.InitializeFishLog();
            FFXIV_Databinds.Instance.RefreshBait();
            FFXIV_Databinds.Instance.RefreshCurrentRoute();

            InitializationComplete = true;
            await OceanFishing();

			return true;
		}

		private async Task OceanFishing()
		{
			await Coroutine.Sleep(1000);

			//GetSchedule();
			if (!OnBoat)
			{
				//missingFish = await GetFishLog();
				if (Core.Me.CurrentJob == ClassJobType.Fisher)
				{
					if (OceanTripNewSettings.Instance.ExchangeFish == ExchangeFish.Sell)
					{
                        await Coroutine.Sleep(3000);
                        await LandSell(FishDataCache.GetFish().Where(x => x.Rarity != "Rare").Select(x => x.FishID).ToList());
					}
					else if (OceanTripNewSettings.Instance.ExchangeFish == ExchangeFish.Desynth)
					{
                        await Coroutine.Sleep(3000);
                        await PassTheTime.DesynthOcean(FishDataCache.GetFish().Where(x => x.Rarity != "Rare").Select(x => x.FishID).ToList());
					}

					//await Lisbeth.SelfRepairWithMenderFallback();
					await LandRepair(50);
				}

                FFXIV_Databinds.Instance.RefreshBait();
                FFXIV_Databinds.Instance.RefreshAchievements();
				FFXIV_Databinds.Instance.RefreshCurrentRoute();

                if (OceanTripNewSettings.Instance.BaitRestockThreshold > 10 && OceanTripNewSettings.Instance.BaitRestockAmount > 30)
					await RestockBait(OceanTripNewSettings.Instance.BaitRestockThreshold, (uint)OceanTripNewSettings.Instance.BaitRestockAmount);
				else
					Log("Bait Restock Threshold or Restock Amount is set too low. Skipping bait restock. If you are missing the required baits for ocean fishing, the bot may not operate properly.");


				if (OceanTripNewSettings.Instance.purchaseHiCordials)
				{
					await EmptyScrips((int)Cordials.HiCordial, 1500);
				}

				if (!ignoreBoat)
				{
					TimeSpan timeLeftUntilNextSpawn = TimeUntilNextBoat();
					if (timeLeftUntilNextSpawn.TotalMinutes < 1)
					{
						Log($"The boat is ready to be boarded!");
						PassTheTime.freeToCraft = false;
					}
					else
					{
						Log($"Next boat is in {Math.Ceiling(timeLeftUntilNextSpawn.TotalMinutes)} minutes. Passing the time until then.");
						PassTheTime.freeToCraft = true;
					}
				}
				else
					PassTheTime.freeToCraft = true;

				await PassTheTime.Craft();

				if (!ignoreBoat)
				{
					if (Core.Me.CurrentJob != ClassJobType.Fisher)
					{
						await SwitchToJob(ClassJobType.Fisher);
						Log("Switching to FSH class...");
					}
				}

				//await Lisbeth.SelfRepairWithMenderFallback();

				// LongBoatQueue = true = 13-15 Minutes
				// LongBoatQueue = false = 0-13 minutes
				while (!((DateTime.UtcNow.Hour % 2 == 0) &&
						((DateTime.UtcNow.Minute < 13 && !OceanTripNewSettings.Instance.LateBoatQueue)
						|| (DateTime.UtcNow.Minute >= 13 && DateTime.UtcNow.Minute < 15 && OceanTripNewSettings.Instance.LateBoatQueue)))
						|| ignoreBoat)
				{
					await Coroutine.Sleep(1000);

					if (OceanTripNewSettings.Instance.OpenWorldFishing && FishingManager.State != FishingState.None && Core.Me.CurrentJob == ClassJobType.Fisher)
					{
						await GoOpenWorldFishing();
					}
				}

				if (FishingManager.State != FishingState.None)
					ActionManager.DoAction(Actions.Quit, Core.Me);


                if (Core.Me.CurrentJob != ClassJobType.Fisher)
				{
					await SwitchToJob(ClassJobType.Fisher);
					Log("Switching to FSH class...");
				}

				Log("Time to queue up for the boat!");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-410.1068f, 3.999944f, 74.89863f));

				await GetOnBoat();
			}



			int spot = rnd.Next(6);
			schedule = Routes.GetSchedule();
			string location = "";
			string TimeOfDay = "";

			// Cache the director if needed
			if (OnBoat)
				Endeavor.CheckDirector();

			while (OnBoat && Endeavor.waitingOnBoat)
			{
				// Reset for this round
				caughtFish.Clear();
				lastCaughtFish = 0;
				caughtFishLogged = false;

                FFXIV_Databinds.Instance.RefreshBait();
                FFXIV_Databinds.Instance.RefreshCurrentRoute();

                ulong baitId = FishBait.Krill;
				ulong spectralbaitId = FishBait.Krill;
				
				location = schedule[Endeavor.CurrentZone].Item1;
				TimeOfDay = schedule[Endeavor.CurrentZone].Item2;

				if (String.IsNullOrEmpty(TimeOfDay))
					TimeOfDay = "Day";

				// Get the baits required
				var currentRoute = RouteDataCache.GetRoutesWithFish().FirstOrDefault(x => x.Route.RouteShortName == location);
				if (currentRoute == null)
				{
                    baitId = FishBait.Ragworm;
                    spectralbaitId = FishBait.Ragworm;
                    Log($"Cannot determine location. Zone: {Endeavor.CurrentZone}, Status: {Endeavor.Status}, On Boat: {OnBoat}");
                }
                else
				{
					baitId = currentRoute.Route.NormalBait;
					spectralbaitId = currentRoute.Route.SpectralBait;
				}

                await GoFish(baitId, spectralbaitId, location, TimeOfDay, spot, currentRoute);
				await Coroutine.Sleep(2000);
			}

			AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
			if (windowByName != null)
			{
				if (OceanTripNewSettings.Instance.LoggingMode)
					Log($"Found results window. Waiting for calculation to end.");

				// This is super sloppy as we have to rely on a bunch of sleeps right now.
				await Coroutine.Sleep(12000);

				// What if the player already clicked the button and we're now loading or something else? This will potentially CRASH the client. Look into refining this later.
				windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
				if (windowByName != null)
					windowByName.SendAction(1, 3, 0);
                
				if (OceanTripNewSettings.Instance.LoggingMode)
                    Log($"Sent confirmation to close results window. Now waiting for loading screen.");


                if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}

                if (OceanTripNewSettings.Instance.LoggingMode)
                    Log($"Done loading! This voyage is OVER! Time to wait for the next boat.");

                FFXIV_Databinds.Instance.RefreshCurrentRoute();
				PassTheTime.freeToCraft = true;
			}

			await Coroutine.Sleep(2000);
		}

		private async Task ChangeBait(ulong baitId)
		{
			if ((baitId != FishingManager.SelectedBaitItemId) && PassTheTime.inventoryCount((int)baitId) > 0 && (DataManager.GetItem((uint)baitId).RequiredLevel <= Core.Me.ClassLevel))
			{
				int sleepTimer = 300;

				// Increase sleep timer for if running low fps
				if (TreeRoot.TicksPerSecond < 17)
					sleepTimer = 1200;

                AtkAddonControl baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				if (baitWindow == null)
				{
					if (OceanTripNewSettings.Instance.LoggingMode)
						Log($"Opening Bait Window.");

                    ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
					await Coroutine.Sleep(sleepTimer);
					baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				}

				if (baitWindow != null)
				{
					baitWindow.SendAction(1, 1, baitId);
					Log($"Applied {DataManager.GetItem((uint)baitId).CurrentLocaleName}");
					await Coroutine.Sleep(sleepTimer-300);
					ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
                    
					if (OceanTripNewSettings.Instance.LoggingMode)
                        Log($"Closed Bait Window.");
                }
            }
			else if (PassTheTime.inventoryCount((int)baitId) == 0)
            {
                Log($"Out of {DataManager.GetItem((uint)baitId).CurrentLocaleName}! Cannot change bait.");
            }
		}

		private async Task GoOpenWorldFishing()
		{
			if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
			{
                FFXIV_Databinds.Instance.RefreshBait();

                if ((Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
				{
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
				}

                if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both)
                {
                    FishingManager.Mooch();
					await WaitForCastLog();
                    startedCast = DateTime.Now;
				}
				else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
				{
					FishingManager.MoochTwo();
                    await WaitForCastLog();
                    startedCast = DateTime.Now;
				}
				else
				{
					FishingManager.Cast();
                    await WaitForCastLog();
                    startedCast = DateTime.Now;
				}
            }

            if ((Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
			{
				ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
			}

			while (FishingManager.State != FishingState.PoleReady)
			{
				if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
				{
					// My testing has shown that there is always a 2.8f variance here, so remove that variance
					// May need future adjustment. Might just be an issue in latency.
					bite = (DateTime.Now - startedCast).TotalSeconds + 2.8f;

                    Log($"Bite Time: {bite:F1}s");
					FishingManager.Hook();
				}

                FFXIV_Databinds.Instance.RefreshBait();
                await Coroutine.Sleep(100);
			}
		}

		private async Task GoFish(ulong baitId, ulong spectralbaitId, string location, string timeOfDay, int spot, RouteWithFish currentRoute = null)
		{
			bool spectraled = false;


            uint edibleFood = 0;
            bool edibleFoodHQ = false;

            if (OceanTripNewSettings.Instance.OceanFood && !Core.Player.HasAura(CharacterAuras.WellFed))
            {
				uint food = (uint)OceanFood.NasiGoreng;

                if (DataManager.GetItem(food, true).ItemCount() >= 1)
                {
                    edibleFood = food;
                    edibleFoodHQ = true;
                }
                else if (DataManager.GetItem(food, false).ItemCount() >= 1)
                {
                    edibleFood = food;
                    edibleFoodHQ = false;
                }
                else
                {
                    edibleFood = 0;
                    edibleFoodHQ = false;
                }

                if (edibleFood > 0)
                {
                    do
                    {
                        Log($"Eating {DataManager.GetItem(edibleFood, edibleFoodHQ).CurrentLocaleName}...");

                        foreach (BagSlot slot in InventoryManager.FilledSlots)
                        {
                            if (slot.RawItemId == (uint)edibleFood)
                            {
                                slot.UseItem();
                            }
                        }
                        await Coroutine.Sleep(3000);

                    } while (!Core.Player.Auras.Any(x => x.Id == CharacterAuras.WellFed));
                    await Coroutine.Sleep(1000);
                }
                else
                {
                    Log($"Out of {DataManager.GetItem(food, false).CurrentLocaleName} to eat!");
                }
            }


			while (OnBoat && Endeavor.shouldFish)
			{
				if (FishingManager.State == FishingState.None)
				{
                    FFXIV_Databinds.Instance.RefreshAchievements();
                    FFXIV_Databinds.Instance.RefreshCurrentRoute();
                }

                // Just in case you're already standing in a fishing spot. IE: Restarting botbase/rebornbuddy			
                if (!ActionManager.CanCast(Actions.Cast, Core.Me) && FishingManager.State == FishingState.None)
                {
                    //Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
                    while (fishSpots[spot].Distance2DSqr(Core.Me.Location) > 2)
                    {
                        Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
                        await Coroutine.Sleep(100);
                    }
                    Navigator.PlayerMover.MoveStop();
                    await Coroutine.Sleep(300);
                    Core.Me.SetFacing(headings[spot]);

                    await Coroutine.Sleep(100);
                }

                FFXIV_Databinds.Instance.RefreshBait();

                if (WorldManager.CurrentWeatherId != Weather.Spectral)
				{
					if (spectraled == true)
					{
						Log("Spectral over.");
						spectraled = false;
					}
				}
				else
				{
					if (spectraled == false)
					{
						Log("Spectral popped!");
						spectraled = true;
					}
				}

				// Should we Cordial?
				if (OceanTripNewSettings.Instance.LoggingMode)
					Log("Checking for Hi-Cordial Use.");
                if ((Core.Me.MaxGP - Core.Me.CurrentGP) >= 400 && spectraled)
					await UseCordial();
				else if ((Core.Me.MaxGP - Core.Me.CurrentGP) >= 400 && Core.Me.CurrentGPPercent < 25.00f)
					await UseCordial();
                if (OceanTripNewSettings.Instance.LoggingMode)
                    Log("Done with Cordials.");

                // Should we use Thaliak's Favor?
                if (OceanTripNewSettings.Instance.LoggingMode)
                    Log("Checking if we need to use Thaliak's Favor");

                if (spectraled && (Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
				{
					Log("Using Thaliak's Favor!");
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
					await Coroutine.Sleep(400);
				}
				else if (!spectraled && (Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me) && Core.Player.Auras.Any(x => x.Id == CharacterAuras.AnglersArt && x.Value >= 7))
				{
					Log("Currently at >7 Angler's Art Stacks - Using Thaliak's Favor!");
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
					await Coroutine.Sleep(400);
				}
                if (OceanTripNewSettings.Instance.LoggingMode)
                    Log("Done with Thaliak's Favor.");

                if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
				{
					//await Coroutine.Sleep(300);

                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Log("Checking for a recently caught fish.");

                    // Did we catch a fish? Let's log it.
                    if (lastCaughtFish != FishingLog.LastFishCaught && !caughtFishLogged)
					{
						lastCaughtFish = FishingLog.LastFishCaught;
						caughtFish.Add(FishingLog.LastFishCaught);
						caughtFishLogged = true;

						if (OceanTripNewSettings.Instance.LoggingMode)
							Log($"Caught {DataManager.GetItem(FishingLog.LastFishCaught).CurrentLocaleName}.");

						if (FishingLog.MissingFish().Contains(FishingLog.LastFishCaught))
							FishingLog.RemoveFish(FishingLog.LastFishCaught);
					}
                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Log("Done checking for a recently caught fish.");


                    //Identical Cast for Blue fish
                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Log("Checking if we need to use Identical Cast.");

                    if (((lastCaughtFish == OceanFish.Gugrusaurus && caughtFish.Where(x => x == OceanFish.Gugrusaurus).Count() < 3)
							|| (lastCaughtFish == OceanFish.Heavenskey && caughtFish.Where(x => x == OceanFish.Heavenskey).Count() < 2)
							|| (lastCaughtFish == OceanFish.GreatGrandmarlin && caughtFish.Where(x => x == OceanFish.GreatGrandmarlin).Count() < 2)
							|| (lastCaughtFish == OceanFish.CrimsonMonkfish && caughtFish.Where(x => x == OceanFish.CrimsonMonkfish).Count() < 2)
							|| (lastCaughtFish == OceanFish.JetborneManta && caughtFish.Where(x => x == OceanFish.JetborneManta).Count() < 2)
							|| (lastCaughtFish == OceanFish.BeatificVision && caughtFish.Where(x => x == OceanFish.BeatificVision).Count() < 3)
							|| (lastCaughtFish == OceanFish.YanxianGoby && caughtFish.Where(x => x == OceanFish.YanxianGoby).Count() < 2)
							|| (lastCaughtFish == OceanFish.CatchingCarp && caughtFish.Where(x => x == OceanFish.CatchingCarp).Count() < 3)
							|| (lastCaughtFish == OceanFish.FleetingSquid && caughtFish.Where(x => x == OceanFish.FleetingSquid).Count() < 2)
							|| (lastCaughtFish == OceanFish.CrimsonKelp && caughtFish.Where(x => x == OceanFish.CrimsonKelp).Count() < 3)
							|| (lastCaughtFish == OceanFish.Shoshitsuki && caughtFish.Where(x => x == OceanFish.Shoshitsuki).Count() < 2)
							|| (lastCaughtFish == OceanFish.SilentShark && caughtFish.Where(x => x == OceanFish.SilentShark).Count() < 2)
							|| (lastCaughtFish == OceanFish.SunkenCoelacanth && caughtFish.Where(x => x == OceanFish.SunkenCoelacanth).Count() < 3)
							|| (lastCaughtFish == OceanFish.PoetsPipe && caughtFish.Where(x => x == OceanFish.PoetsPipe).Count() < 2)
							|| (lastCaughtFish == OceanFish.FunnelShark && (OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto || OceanTripNewSettings.Instance.FishPriority == FishPriority.Points) && Core.Me.CurrentGP >= 700)
						)
						&& (ActionManager.CanCast(Actions.IdenticalCast, Core.Me)) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
					{
						if (ActionManager.CanCast(Actions.IdenticalCast, Core.Me))
						{
							Log("Identical Cast!");
							lastCastMooch = false;
							ActionManager.DoAction(Actions.IdenticalCast, Core.Me);
                            await WaitForCastLog();
                            startedCast = DateTime.Now;
						}
					}
                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Log("Done checking for Identical Cast.");


                    // Check for Mooch before using Mooch II
                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Log("Checking for Mooch before moving into bait checks.");
                    if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both)
					{
						Log("Using Mooch!");
						FishingManager.Mooch();
						lastCastMooch = true;
                        await WaitForCastLog();
                        startedCast = DateTime.Now;
					}
					else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
					{
						Log("Using Mooch II!");
						FishingManager.MoochTwo();
						lastCastMooch = true;
                        await WaitForCastLog();
                        startedCast = DateTime.Now;
					}
					else
					{
						if (spectraled)
						{
							// Build Spectral Fish List
							List<Fish> spectralFishToCatch = (currentRoute == null ? new List<Fish>() : currentRoute.SpectralFish.Where(x => FishingLog.MissingFish().Contains((uint)x.FishID) && x.TimeOfDayExclusion1 != timeOfDay && x.TimeOfDayExclusion2 != timeOfDay)).ToList();

                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Spectral: Checking if we need to use Patience.");

                            if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience || OceanTripNewSettings.Instance.Patience == ShouldUsePatience.SpectralOnly)
								await UsePatience();
                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Done checking for Patience.");


							//Bait for Blue fish
							if (
									Core.Player.HasAura(CharacterAuras.FishersIntuition) &&
									(
										((location == "galadion") && (timeOfDay == "Night"))
										|| ((location == "south") && (timeOfDay == "Night"))
										|| ((location == "north") && (timeOfDay == "Day"))
										|| ((location == "rhotano") && (timeOfDay == "Sunset"))
										|| ((location == "ciel") && (timeOfDay == "Night"))
										|| ((location == "blood") && (timeOfDay == "Day"))
										|| ((location == "sound") && (timeOfDay == "Sunset"))
										|| ((location == "sirensong") && (timeOfDay == "Day"))
										|| ((location == "kugane") && (timeOfDay == "Night"))
										|| ((location == "rubysea") && (timeOfDay == "Sunset"))
										|| ((location == "oneriver") && (timeOfDay == "Day"))
									)
							)
							{
								caughtFish.Clear();

                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Switching bait to {DataManager.GetItem((uint)spectralbaitId).CurrentLocaleName} to catch a blue fish now that we have intuition.");
								await ChangeBait(spectralbaitId);
							}
							else if ((location == "galadion") && (timeOfDay == "Night") && FishingLog.MissingFish().Contains((uint)OceanFish.Sothis) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Heavenskey).Count() < 2) // Needs 2 Heavenskey. Use Ragworm to catch.
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Ragworm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.Heavenskey).CurrentLocaleName}");
									await ChangeBait(FishBait.Ragworm);
								}
								else if (!caughtFish.Contains(OceanFish.NavigatorsPrint)) // Requires 1 Navigators Print.
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Krill).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.NavigatorsPrint).CurrentLocaleName}");
									await ChangeBait(FishBait.Krill);
								}
							}
							else if ((location == "south") && (timeOfDay == "Night") && FishingLog.MissingFish().Contains((uint)OceanFish.CoralManta) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.GreatGrandmarlin).Count() < 2) // Needs 2 Great Grandmarlin. Mooch from Hi-Aetherlouse.
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.GreatGrandmarlin).CurrentLocaleName} via mooching {DataManager.GetItem((uint)OceanFish.HiAetherlouse).CurrentLocaleName}.");
									await ChangeBait(FishBait.PlumpWorm);
								}
							}
							else if ((location == "north") && (timeOfDay == "Day") && FishingLog.MissingFish().Contains((uint)OceanFish.Elasmosaurus) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Gugrusaurus).Count() < 3) // Needs 3 Gugrusaurus
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 3x {DataManager.GetItem((uint)OceanFish.Gugrusaurus).CurrentLocaleName}");
									await ChangeBait(FishBait.PlumpWorm);
								}
							}
							else if ((location == "rhotano") && (timeOfDay == "Sunset") && FishingLog.MissingFish().Contains((uint)OceanFish.Stonescale) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.CrimsonMonkfish).Count() < 2) // Needs 2 Crimson Monkfish
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.CrimsonMonkfish).CurrentLocaleName}");
									await ChangeBait(FishBait.PlumpWorm);
								}
							}
							else if ((location == "ciel") && (timeOfDay == "Night") && FishingLog.MissingFish().Contains((uint)OceanFish.Hafgufa) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.JetborneManta).Count() < 2) // Needs 2 Jetborne Manta
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.JetborneManta).CurrentLocaleName}");
									await ChangeBait(FishBait.PlumpWorm);
								}
								else if (!caughtFish.Contains(OceanFish.MistbeardsCup)) // Needs 1 Mistbeard's Cup
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Krill).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.MistbeardsCup).CurrentLocaleName}");
                                    await ChangeBait(FishBait.Krill);
								}
							}
							else if ((location == "blood") && (timeOfDay == "Day") && FishingLog.MissingFish().Contains((uint)OceanFish.SeafaringToad) && FocusFishLog)
							{
								// This will help increase the chances of catching Seafaring Toad.
								await UsePatience();

                                // Catch 3 Beatific Vision to trigger intuition
                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Krill).CurrentLocaleName} in order to catch 3x {DataManager.GetItem((uint)OceanFish.BeatificVision).CurrentLocaleName}");
                                await ChangeBait(FishBait.Krill);
							}
							else if ((location == "sound") && (timeOfDay == "Sunset") && FishingLog.MissingFish().Contains((uint)OceanFish.Placodus) && FocusFishLog)
							{
								await UsePatience();

                                // Use Ragworm to catch Rothlyt Mussel, then Mooch to Trollfish to trigger intuition.
                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Ragworm).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.RothlytMussel).CurrentLocaleName} to mooch into {DataManager.GetItem((uint)OceanFish.Trollfish).CurrentLocaleName}");
                                await ChangeBait(FishBait.Ragworm);
							}
							else if ((location == "sirensong") && (timeOfDay == "Day") && FishingLog.MissingFish().Contains((uint)OceanFish.Taniwha) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.SunkenCoelacanth).Count() < 3) // Needs 3 Sunken Coelacanth
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 3x {DataManager.GetItem((uint)OceanFish.SunkenCoelacanth).CurrentLocaleName}");
                                    await ChangeBait(FishBait.PlumpWorm);
								}
							}
							else if ((location == "kugane") && (timeOfDay == "Night") && FishingLog.MissingFish().Contains((uint)OceanFish.GlassDragon) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Shoshitsuki).Count() < 2) // Needs Shoshitsuki
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.Shoshitsuki).CurrentLocaleName}");
                                    await ChangeBait(FishBait.PlumpWorm);
								}
								else
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Krill).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.SnappingKoban).CurrentLocaleName} to mooch into {DataManager.GetItem((uint)OceanFish.GlassDragon).CurrentLocaleName}");
                                    await ChangeBait(FishBait.Krill); // Try to get Snapping Koban to mooch Glass Dragon
								}
							}
							else if ((location == "rubysea") && (timeOfDay == "Sunset") && FishingLog.MissingFish().Contains((uint)OceanFish.HellsClaw) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.FlyingSquid).Count() < 1) // Needs 1x Flying Squid
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.FlyingSquid).CurrentLocaleName}");
									await ChangeBait(FishBait.PlumpWorm);
								}
								else if (caughtFish.Where(x => x == OceanFish.FleetingSquid).Count() < 2) // Needs 2x Fleeting Squid
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.PlumpWorm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.FleetingSquid).CurrentLocaleName}");
                                    await ChangeBait(FishBait.PlumpWorm);
								}
							}
							else if ((location == "oneriver") && (timeOfDay == "Day") && FishingLog.MissingFish().Contains((uint)OceanFish.JewelofPlumSpring) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.YanxianGoby).Count() < 2) // Needs 2x Yanxian Goby
								{
                                    if (OceanTripNewSettings.Instance.LoggingMode)
                                        Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Ragworm).CurrentLocaleName} in order to catch 2x {DataManager.GetItem((uint)OceanFish.YanxianGoby).CurrentLocaleName}");
									await ChangeBait(FishBait.Ragworm);
								}
								else if (caughtFish.Where(x => x == OceanFish.GensuiShrimp).Count() < 1) // Needs 1x Gensui Shrimp
								{
									if (OceanTripNewSettings.Instance.LoggingMode)
										Log($"Switching bait to {DataManager.GetItem((uint)FishBait.Ragworm).CurrentLocaleName} in order to catch 1x {DataManager.GetItem((uint)OceanFish.GensuiShrimp).CurrentLocaleName}");
                                    await ChangeBait(FishBait.Ragworm);
								}
							}
							else if (FocusFishLog && (spectralFishToCatch.Where(x => x.FavoriteBait == FishBait.PlumpWorm).Count() > 0) && PassTheTime.inventoryCount((int)FishBait.PlumpWorm) > 0)
							{
								await ChangeBait(FishBait.PlumpWorm);
							}
                            else if (FocusFishLog && (spectralFishToCatch.Where(x => x.FavoriteBait == FishBait.Krill).Count() > 0) && PassTheTime.inventoryCount((int)FishBait.Krill) > 0)
							{
								await ChangeBait(FishBait.Krill);
							}
                            else if (FocusFishLog && (spectralFishToCatch.Where(x => x.FavoriteBait == FishBait.Ragworm).Count() > 0) && PassTheTime.inventoryCount((int)FishBait.Ragworm) > 0)
                            {
                                await ChangeBait(FishBait.Ragworm);
							}
							else if (
										((location == "galadion") && (timeOfDay == "Sunset"))
										|| ((location == "rhotano") && (timeOfDay == "Day"))
										|| ((location == "north") && (timeOfDay == "Day"))
										|| ((location == "south") && (timeOfDay == "Night"))
										|| ((location == "ciel") && (timeOfDay == "Sunset"))
										|| ((location == "blood") && (timeOfDay == "Sunset"))
										|| ((location == "rubysea"))
							)
							{
								await ChangeBait(FishBait.PlumpWorm);
							}
							else if (
										((location == "south") && (timeOfDay == "Day"))
										|| ((location == "rhotano") && (timeOfDay == "Night")
											&& (!FocusFishLog || !FishingLog.MissingFish().Contains((uint)OceanFish.Slipsnail)))
										|| ((location == "north") && (timeOfDay == "Sunset")
											&& FishingLog.MissingFish().Contains((uint)OceanFish.TheFallenOne)
											&& FocusFishLog)
										|| ((location == "north") && (timeOfDay == "Night")
											&& (!FocusFishLog
											|| !FishingLog.MissingFish().Contains((uint)OceanFish.BartholomewTheChopper)))
										|| ((location == "galadion") && (timeOfDay == "Night"))
										|| ((location == "ciel") && (timeOfDay == "Day" || timeOfDay == "Night"))
										|| ((location == "blood") && (timeOfDay == "Night"))
										|| ((location == "sound"))
										|| ((location == "kugane"))
										|| ((location == "sirensong"))
										|| ((location == "oneriver"))
							)
							{
								await ChangeBait(FishBait.Krill);
							}
							else if (
										((location == "galadion") && (timeOfDay == "Day"))
										|| ((location == "south") && (timeOfDay == "Sunset"))
										|| ((location == "north") && (timeOfDay == "Sunset"))
										|| ((location == "north") && (timeOfDay == "Sunset"))
										|| ((location == "rhotano") && (timeOfDay == "Sunset"))
										|| ((location == "blood") && (timeOfDay == "Day"))
							)
							{
								await ChangeBait(FishBait.Ragworm);
							}
							else
							{
								await ChangeBait(spectralbaitId);
							}
						}
						else
						{
                            // Build Normal Fish List
                            List<Fish> normalFishToCatch = (currentRoute == null ? new List<Fish>() : currentRoute.NormalFish.Where(x => FishingLog.MissingFish().Contains((uint)x.FishID) 
																																&& x.TimeOfDayExclusion1 != timeOfDay 
																																&& x.TimeOfDayExclusion2 != timeOfDay
																																&& x.WeatherExclusion1 != WorldManager.CurrentWeather
																																&& x.WeatherExclusion2 != WorldManager.CurrentWeather)
																															).ToList();
                            
							if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience)
                                await UsePatience();

							// Deal with Intuition fish first... if we have the intution buff
							if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "galadion" || location == "rhotano" || location == "ciel" || location == "blood" || location == "rubysea"))
								await ChangeBait(FishBait.Krill);
							else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && ((location == "south" && ((WorldManager.CurrentWeather != "Wind" && WorldManager.CurrentWeather != "Gales"))) || location == "sirensong" || location == "oneriver"))
								await ChangeBait(FishBait.PlumpWorm);
							else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "sound" || location == "north" || location == "kugane"))
								await ChangeBait(FishBait.Ragworm);

							// Deal with all the rest
							else if (FocusFishLog && normalFishToCatch.Where(x => x.FavoriteBait == FishBait.Krill).Count() > 0)
								await ChangeBait(FishBait.Krill);
                            else if (FocusFishLog && normalFishToCatch.Where(x => x.FavoriteBait == FishBait.PlumpWorm).Count() > 0) 
								await ChangeBait(FishBait.PlumpWorm);
                            else if (FocusFishLog && normalFishToCatch.Where(x => x.FavoriteBait == FishBait.Ragworm).Count() > 0)
                                await ChangeBait(FishBait.Ragworm);
							else if (location == "galadion" || location == "rhotano" || location == "sound" || location == "oneriver" || location == "sirensong")
                                await ChangeBait(FishBait.PlumpWorm);
							else if (location == "south" || location == "blood")
								await ChangeBait(FishBait.Krill);
							else if (location == "north" || location == "ciel" || location == "kugane" || location == "rubysea")
								await ChangeBait(FishBait.Ragworm);	
							else
								await ChangeBait(baitId);


                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Done Bait checks.");


                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Checking if we should use Chum.");

                            // Should we use Chum?
                            if (Core.Me.MaxGP >= 100 && ((Core.Me.MaxGP - Core.Me.CurrentGP) <= 100) && OceanTripNewSettings.Instance.FullGPAction == FullGPAction.Chum)
							{
								if (ActionManager.CanCast(Actions.Chum, Core.Me))
								{
									Log("Triggering Full GP Action to keep regen going - Chum!");
									ActionManager.DoAction(Actions.Chum, Core.Me);
									//await Coroutine.Sleep(800);
								}
							}

                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Done checking for Chum.");

                        }

                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Log("Casting and then waiting for chat message about cast!");

                        FishingManager.Cast();
                        await WaitForCastLog();
                        startedCast = DateTime.Now;
						lastCastMooch = false;
					}

					await Coroutine.Sleep(100);
				}

				while ((FishingManager.State != FishingState.PoleReady) && Endeavor.shouldFish)
				{
                    //Spectral popped, don't wait for normal fish
                    if (WorldManager.CurrentWeatherId == Weather.Spectral && !spectraled)
					{
						Log("Spectral popped!");
						spectraled = true;

						if (FishingManager.CanHook)
							FishingManager.Hook();
					}

					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
					{
                        double biteElapsed = Math.Round((DateTime.Now - startedCast).TotalSeconds + 2.8f, 1); // There is always a 2.8f variance for the actual bite timer vs GatherBuddy
                        const double Tolerance = 1e-6; // Floating Point Precision
                        bool doubleHook = false;

                        // Build Spectral Fish List
                        List<Fish> spectralFishToCatch = (currentRoute == null ? new List<Fish>() : currentRoute.SpectralFish.Where(x => x.TimeOfDayExclusion1 != timeOfDay && x.TimeOfDayExclusion2 != timeOfDay && x.BiteType == FishingManager.TugType && (x.BiteStart - Tolerance) <= biteElapsed && (x.BiteEnd + Tolerance) >= biteElapsed)).ToList();

                        List<Fish> normalFishToCatch = (currentRoute == null ? new List<Fish>() : currentRoute.NormalFish.Where(x => x.TimeOfDayExclusion1 != timeOfDay
                                                                                                                            && x.TimeOfDayExclusion2 != timeOfDay
                                                                                                                            && x.WeatherExclusion1 != WorldManager.CurrentWeather
                                                                                                                            && x.WeatherExclusion2 != WorldManager.CurrentWeather
                                                                                                                            && x.BiteType == FishingManager.TugType
                                                                                                                            && (x.BiteStart - Tolerance) <= biteElapsed && (x.BiteEnd + Tolerance) >= biteElapsed)
                                                                                                                        ).ToList();
						
						var potentialFish = String.Join(", ", (spectraled ? spectralFishToCatch : normalFishToCatch).Select(x => DataManager.GetItem((uint)x.FishID).CurrentLocaleName).ToList());

						if (Core.Player.HasAura(CharacterAuras.Chum))
							potentialFish = "Cannot predict due to Chum.";

						Log($"Bite Time: {biteElapsed:F1}s, Potential Fish: {(String.IsNullOrWhiteSpace(potentialFish) ? "Unable to determine" : potentialFish)}");

                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Log("Checking if we should double hook based on bite timer and current fishing conditions!");


						if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Points || OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto)
						{
							// Check if we should DH or TH - Special handling for South's lastMooch rule - Always DH/TH after a Mooch in South if spectral.
							if (FishDataCache.GetFish().Any(x => x.RouteShortName == location
													&& (biteElapsed >= (x.BiteStart - Tolerance) && biteElapsed <= (x.BiteEnd + Tolerance)
													&& x.WeatherExclusion1 != WorldManager.CurrentWeather
													&& x.WeatherExclusion2 != WorldManager.CurrentWeather
													&& x.TimeOfDayExclusion1 != timeOfDay
													&& x.TimeOfDayExclusion2 != timeOfDay
													&& x.BiteType == FishingManager.TugType
													&& (((x.Points * x.THBonus > 600 && x.THBonus > 1) || (x.Points * x.DHBonus > 400 && x.DHBonus > 1)) || (x.THBonus > 5 || x.DHBonus > 3))
													) || (location == "south" && lastCastMooch && (timeOfDay == "Sunset" || timeOfDay == "Night") && spectraled)))
								doubleHook = true;
						}

                        if (OceanTripNewSettings.Instance.LoggingMode)
	                        Log("Done checking for double hook conditions.");

                        if (doubleHook && ActionManager.CanCast(Actions.DoubleHook, Core.Me))
						{
							if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
							{
                                Log("Using Triple Hook!");
                                ActionManager.DoAction(Actions.TripleHook, Core.Me);
                            }
                            else
							{
								Log("Using Double Hook!");
								ActionManager.DoAction(Actions.DoubleHook, Core.Me);
							}

							lastCastMooch = false;
						}
						else if (FishingManager.HasPatience)
						{
                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Player has patience on them. Need to use special hooking.");

                            if (FishingManager.TugType == TugType.Light)
							{
                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Using Precision Hookset!");

                                ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
								lastCastMooch = false;
							}
							else
							{
                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Using Powerful Hookset!");

                                ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
								lastCastMooch = false;
							}
						}
						else
						{
                            if (OceanTripNewSettings.Instance.LoggingMode)
                                Log("Checking if Full GP action is Double Hook.");

                            if (!spectraled && Core.Me.MaxGP >= 500 && ((Core.Me.MaxGP - Core.Me.CurrentGP) <= 100) && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && OceanTripNewSettings.Instance.FullGPAction == FullGPAction.DoubleHook)
							{
								if (ActionManager.CanCast(Actions.TripleHook, Core.Me))
								{
                                    Log("Triggering Full GP Action to keep regen going - Triple Hook!");
                                    ActionManager.DoAction(Actions.TripleHook, Core.Me);
                                }
                                else
								{
									Log("Triggering Full GP Action to keep regen going - Double Hook!");
									ActionManager.DoAction(Actions.DoubleHook, Core.Me);
								}
							}
							else
							{
                                if (OceanTripNewSettings.Instance.LoggingMode)
                                    Log($"Hooking Fish!");

                                FishingManager.Hook();
							}

							lastCastMooch = false;
						}

                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Log("Refreshing UI for Bait and Achievements in case something changed.");

                        caughtFishLogged = false;
                        FFXIV_Databinds.Instance.RefreshBait();
                        FFXIV_Databinds.Instance.RefreshAchievements();
                    }

                    await Coroutine.Sleep(100); 
                }
            }


            spectraled = false;
			await Coroutine.Sleep(100); 
            //await Coroutine.Yield();

            //Log("Waiting for next stop...");
            if (FishingManager.State != FishingState.None)
			{
				ActionManager.DoAction(Actions.Quit, Core.Me);
			}
		}

		private static async Task GetOnBoat()
		{
			var Dryskthota = GameObjectManager.GetObjectByNPCId(NPC.Dryskthota);

            if (Core.Me.CurrentJob != ClassJobType.Fisher)
            {
                await SwitchToJob(ClassJobType.Fisher);
                Logging.Write(Colors.Aqua, "[Ocean Trip] Switching to FSH class...");
            }

            if (!PartyManager.IsInParty || (PartyManager.IsInParty && PartyManager.IsPartyLeader && !PartyManager.CrossRealm))
			{
				// Wait for party members to be nearby - Thanks zzi and nt153133!
				await Coroutine.Wait(TimeSpan.FromMinutes(30), PartyLeaderWaitConditions);

				if (Dryskthota != null && Dryskthota.IsWithinInteractRange)
				{
                    if (OceanTripNewSettings.Instance.LoggingMode)
                        Logging.Write(Colors.Aqua, $"[Ocean Trip] Interacting with Dryskthota.");

                    Dryskthota.Interact();
					if (await Coroutine.Wait(5000, () => Talk.DialogOpen))
					{
						Talk.Next();
					}

					await Coroutine.Wait(5000, () => SelectString.IsOpen);

					// Click ready to board
					if (SelectString.IsOpen)
					{
						SelectString.ClickSlot(0);

						await Coroutine.Sleep(1000); // Sleep for a second


                        if (OceanTripNewSettings.Instance.LoggingMode && OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Indigo)
                            Logging.Write(Colors.Aqua, $"[Ocean Trip] Selecting Indigo Route.");

                        if (OceanTripNewSettings.Instance.LoggingMode && OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Ruby)
                            Logging.Write(Colors.Aqua, $"[Ocean Trip] Selecting Ruby Route.");

                        // Select route (0 = Indigo, 1 = Ruby)
                        SelectString.ClickSlot((uint)OceanTripNewSettings.Instance.FishingRoute);

                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Logging.Write(Colors.Aqua, $"[Ocean Trip] Waiting for Yes/No dialog to appear for boarding confirmation.");

                        await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
						SelectYesno.Yes();

                        if (OceanTripNewSettings.Instance.LoggingMode)
                            Logging.Write(Colors.Aqua, $"[Ocean Trip] Boat confirmed. We're now in the duty finder.");

                    }
                }
			}


            if (OceanTripNewSettings.Instance.LoggingMode)
                Logging.Write(Colors.Aqua, $"[Ocean Trip] Waiting for Duty Finder.");

            await Coroutine.Wait(1000000, () => ContentsFinderConfirm.IsOpen);

			await Coroutine.Yield();
			while (ContentsFinderConfirm.IsOpen)
			{
                if (OceanTripNewSettings.Instance.LoggingMode)
                    Logging.Write(Colors.Aqua, $"[Ocean Trip] Commencing Duty.");

                DutyManager.Commence();
				await Coroutine.Yield();

                if (OceanTripNewSettings.Instance.LoggingMode)
                    Logging.Write(Colors.Aqua, $"[Ocean Trip] Waiting for loading screen.");

                if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}

                if (OceanTripNewSettings.Instance.LoggingMode)
                    Logging.Write(Colors.Aqua, $"[Ocean Trip] Loading screen found.");
            }
            while (!OnBoat)
			{
				await Coroutine.Sleep(1000);
			}
			await Coroutine.Sleep(2500);
			Logging.Write(Colors.Aqua, "[Ocean Trip] We're on the boat!");
		}

		private static async Task SwitchToJob(ClassJobType job)
		{
			if (Core.Me.CurrentJob == job) return;

			var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

			if (gearSets.Any(gs => gs.Class == job))
			{
				Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet");
				gearSets.First(gs => gs.Class == job).Activate();
				await Coroutine.Sleep(1000);
			}

			if (Core.Me.CurrentJob != job)
				Logging.Write(Colors.Red, $"[Ocean Trip] Could not change to FSH.");
		}

		private async Task UseCordial()
		{
			uint cordial = 0;

			if (DataManager.GetItem(Cordials.HiCordial).ItemCount() > 0)
				cordial = Cordials.HiCordial;
			else if (DataManager.GetItem(Cordials.Cordial).ItemCount() > 0)
				cordial = Cordials.Cordial;
			else if (DataManager.GetItem(Cordials.WateredCordial).ItemCount() > 0)
				cordial = Cordials.WateredCordial;

			// Yay, we have a cordial!
			if (cordial > 0 && InventoryManager.FilledSlots.Any(x => x.RawItemId == cordial))
			{
				var slot = InventoryManager.FilledSlots.First(x => x.RawItemId == cordial);
				await Coroutine.Sleep(600);

				if (slot.UseItem())
					Logging.Write(Colors.Aqua, $"[Ocean Trip] Used a {DataManager.GetItem(cordial).CurrentLocaleName}!");

				await Coroutine.Sleep(600);
			}
		}

		private static async Task LandRepair(int repairThreshold)
		{
			if (InventoryManager.EquippedItems.Where(r => r.IsFilled && r.Condition < repairThreshold).Count() > 0)
			{
				Logging.Write(Colors.Aqua, "Starting repair...");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(1);
					await Coroutine.Wait(3000, () => Repair.IsOpen);
					if (Repair.IsOpen)
					{
						Repair.RepairAll();
						await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
						if (SelectYesno.IsOpen)
						{
							SelectYesno.ClickYes();
						}
						Repair.Close();
						await Coroutine.Wait(5000, () => !Repair.IsOpen);
					}
				}
				Logging.Write(Colors.Aqua, "Repair complete!");
			}
		}

		private async Task RestockBait(int baitThreshold, uint baitCap)
		{
			List<uint> itemsToBuy = new List<uint>();
			List<uint> baitList = new List<uint>();

			baitList.Add(FishBait.Ragworm);
			baitList.Add(FishBait.Krill);
			baitList.Add(FishBait.PlumpWorm);
			baitList.Add(FishBait.RatTail);
			baitList.Add(FishBait.GlowWorm);
			baitList.Add(FishBait.HeavySteelJig);
			baitList.Add(FishBait.ShrimpCageFeeder);
			baitList.Add(FishBait.PillBug);
			baitList.Add(FishBait.SquidStrip);
			baitList.Add(FishBait.MackerelStrip);
			baitList.Add(FishBait.StoneflyNymph);

			foreach (var bait in baitList)
			{
				if (bait == FishBait.HeavySteelJig && PassTheTime.inventoryCount((int)bait) < 5)
				{
					itemsToBuy.Add(bait);
				}
				else if (bait != FishBait.HeavySteelJig && PassTheTime.inventoryCount((int)bait) < baitThreshold)
				{
                    itemsToBuy.Add(bait);
                }
            }


			if (itemsToBuy.Any())
			{
				Log("Restocking bait with Lisbeth...");

				foreach (var bait in itemsToBuy)
				{
					if (bait == FishBait.StoneflyNymph
						|| bait == FishBait.RatTail 
						|| bait == FishBait.GlowWorm
						|| bait == FishBait.ShrimpCageFeeder
						|| bait == FishBait.PillBug
                        || bait == FishBait.Ragworm 
						|| bait == FishBait.Krill
						|| bait == FishBait.PlumpWorm)
					{
						await PassTheTime.IdleLisbeth((int)bait, OceanTripNewSettings.Instance.BaitRestockAmount, "Purchase", "true", 0, true);
					}
				}

				if (itemsToBuy.Contains(FishBait.HeavySteelJig) && Core.Me.Levels[ClassJobType.Goldsmith] >= 36)
				{
					await PassTheTime.IdleLisbeth((int)FishBait.HeavySteelJig, 10, "Goldsmith", "true", 0, true);
				}

				if (itemsToBuy.Contains(FishBait.SquidStrip))
				{
					await PassTheTime.IdleLisbeth((int)FishBait.SquidStrip, OceanTripNewSettings.Instance.BaitRestockAmount, "Exchange", "true", 0, true);
				}

				if (itemsToBuy.Contains(FishBait.MackerelStrip))
				{
					await PassTheTime.IdleLisbeth((int)FishBait.MackerelStrip, OceanTripNewSettings.Instance.BaitRestockAmount, "Exchange", "true", 0, true);
				}

				Log("Restocking bait complete");
			}
		}

		private async Task LandSell(List<int> itemIds)
		{
			var itemsToSell = InventoryManager.FilledSlots.Where(bs => bs.IsSellable && itemIds.Contains((int)bs.RawItemId));
			if (itemsToSell.Count() != 0)
			{
				Log("Selling fish...");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(0);
					await Coroutine.Wait(5000, () => Shop.Open);
					foreach (var item in itemsToSell)
					{
						if (item.Value <= 18)
						{
							var name = item.Name;
							await CommonTasks.SellItem(item);
							await Coroutine.Wait(10000, () => !item.IsFilled || !item.Name.Equals(name));
							await Coroutine.Sleep(1000);
						}
					}
					Shop.Close();
					await Coroutine.Wait(2000, () => !Shop.Open);
				}
				Log("Fish selling complete!");
			}
		}

		private async Task EmptyScrips(int itemId, int scripThreshold)
		{
#if RB_DT
			SpecialCurrency currency = SpecialCurrency.PurpleGatherersScrips;
#else
            SpecialCurrency currency = SpecialCurrency.WhiteGatherersScrips;
#endif

            //TODO: Buy other stuff with scrip
            if (SpecialCurrencyManager.GetCurrencyCount(currency) > scripThreshold)
			{
				Logging.Write(Colors.Aqua, $"[Ocean Trip] Purchasing {(int)SpecialCurrencyManager.GetCurrencyCount(currency) / 20} Hi-Cordials!");

                await PassTheTime.IdleLisbeth(itemId, (int)SpecialCurrencyManager.GetCurrencyCount(currency) / 20, "Exchange", "false", 0);
			}

        }

		private async Task WaitForCastLog()
		{
			//await Coroutine.Wait(3000, () => (ChatCheck("cast your line", "cast your line") || ChatCheck("recast your line", "recast your line")));
			await Coroutine.Wait(3000, () => (FishingManager.State == FishingState.Reelin));
		}

		private bool FocusFishLog
		{
			get
			{
				if (OceanTripNewSettings.Instance.FishPriority == FishPriority.FishLog || OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto)
					return true;

				return false;
			}
		}
		
		public static bool OnBoat
		{
			get 
			{
				if (WorldManager.RawZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby)
					return true;

				return false;
			}
		}

		private async Task UsePatience()
		{
			if (ActionManager.CanCast(Actions.PatienceII, Core.Me) && !FishingManager.HasPatience)
			{
				if (OceanTripNewSettings.Instance.LoggingMode)
					Log($"Applying Patience II!");

				ActionManager.DoAction(Actions.PatienceII, Core.Me);
			}
			else if (ActionManager.CanCast(Actions.Patience, Core.Me) && !FishingManager.HasPatience)
			{
                if (OceanTripNewSettings.Instance.LoggingMode)
                    Log($"Applying Patience!");

                ActionManager.DoAction(Actions.Patience, Core.Me);
			}

			await Coroutine.Sleep(200);
        }

        private static bool PartyLeaderWaitConditions()
        {
            return PartyManager.VisibleMembers.Count() == PartyManager.AllMembers.Count();
        }

        private void Log(string text, params object[] args)
		{
			var msg = string.Format("[Ocean Trip] " + text, args);
			Logging.Write(Colors.Aqua, msg);
		}
	}
}