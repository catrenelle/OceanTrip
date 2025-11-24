using System;
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
using OceanTripPlanner.Strategies;

namespace OceanTripPlanner
{
	public class OceanTrip : BotBase
	{
		//public override Composite Root => _root;
		private Composite _root;


		private List<uint> caughtFish;
		private uint lastCaughtFish = 0;
		private bool caughtFishLogged = false;

		private bool ignoreBoat { get { if (OceanTripNewSettings.Instance.FishPriority == FishPriority.IgnoreBoat) { return true; } else { return false; } } }

		private static Random rnd = new Random();

		private DateTime startedCast;
		private double bite;

		private Tuple<string, string>[] schedule;
		private System.Timers.Timer execute;
		private readonly object timerLock = new object();

		// Fishing state tracking for optimized flow
		private DateTime buffLastChecked = DateTime.MinValue;

		// Strategy pattern dependencies
		private BaitChanger baitChanger;
		private PatienceManager patienceManager;
		private SpectralBaitSelector spectralBaitSelector;
		private NormalBaitSelector normalBaitSelector;
		private AchievementBaitSelector achievementBaitSelector;
		private HookingStrategy hookingStrategy;
		private AchievementHookingStrategy achievementHookingStrategy;
		private BoatBoardingHandler boatBoardingHandler;
		private FishingSessionManager fishingSessionManager;
		private FishingSessionManager achievementFishingSessionManager;
		private BaitRestockStrategy baitRestockStrategy;
		private CordialStrategy cordialStrategy;
	private FishCatchLogger fishCatchLogger;
	private SpectralDetector spectralDetector;

		public override string Name => "Ocean Trip";

		public override PulseFlags PulseFlags => PulseFlags.All;

		public override bool IsAutonomous => true;
		public override bool RequiresProfile => false;

		public override Composite Root => _root;

		public override bool WantButton { get; } = true;

		private static Ocean_Trip.FormSettings settings;

		public Ocean_Trip.Endeavor Endeavor;

		private GameStateCache gameCache => GameStateCache.Instance;

		/// <summary>
		/// Initialize or reinitialize settings form
		/// </summary>
		private void InitializeSettings()
		{
			if (settings == null || settings.IsDisposed)
				settings = new Ocean_Trip.FormSettings();
		}

		/// <summary>
		/// Initialize shared resources used by both OnButtonPress and Start
		/// </summary>
		private void InitializeSharedResources()
		{
			FFXIV_Databinds.Instance.RefreshBait();
			FFXIV_Databinds.Instance.RefreshAchievements();

			// Pre-cache the fish data
			FishDataCache.GetFish();
			RouteDataCache.GetRoutesWithFish();

			FFXIV_Databinds.Instance.RefreshCurrentRoute();

			if (Endeavor == null)
				Endeavor = new Ocean_Trip.Endeavor();
		}

		public override void OnButtonPress()
		{
			InitializeSettings();

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

			InitializeSharedResources();
		}

		public override void Start()
		{
			TreeHooks.Instance.ClearAll();

			Log("Initializing OceanTrip Settings.");
			InitializeSettings();
			Log("OceanTrip Settings Loaded.");

			InitializeSharedResources();

			caughtFish = new List<uint>();
			lastCaughtFish = 0;
			caughtFishLogged = false;

			// Initialize strategy pattern dependencies
			baitChanger = new BaitChanger(gameCache);
			patienceManager = new PatienceManager();
			spectralBaitSelector = new SpectralBaitSelector(baitChanger, patienceManager, gameCache);
			normalBaitSelector = new NormalBaitSelector(baitChanger, patienceManager, gameCache);
			achievementBaitSelector = new AchievementBaitSelector(baitChanger, patienceManager, gameCache);
			hookingStrategy = new HookingStrategy(gameCache);
			achievementHookingStrategy = new AchievementHookingStrategy(gameCache);
			boatBoardingHandler = new BoatBoardingHandler();
			fishingSessionManager = new FishingSessionManager(gameCache, hookingStrategy);
			achievementFishingSessionManager = new FishingSessionManager(gameCache, hookingStrategy); // TODO: Update to use achievementHookingStrategy once interface is created
			baitRestockStrategy = new BaitRestockStrategy();
			cordialStrategy = new CordialStrategy(gameCache);
			fishCatchLogger = new FishCatchLogger(gameCache, (text, level) => Log(text, level));
			spectralDetector = new SpectralDetector(gameCache, (text, level) => Log(text, level));

			// Initialize timer with thread-safe lock
			lock (timerLock)
			{
				// Create new timer if null or disposed
				if (execute == null)
					execute = new System.Timers.Timer();

				TimeSpan timeLeftUntilFirstRun = TimeUntilNextBoat();
				if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
					execute.Interval = 100;
				else
					execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

				execute.Elapsed += new ElapsedEventHandler(KillLisbeth);
				execute.Start();
			}

			Log("BotBase is initialized, beginning execution.");

			_root = new ActionRunCoroutine(r => Run());
		}

	/// <summary>
	/// Calculate time remaining until next boat departure
	/// </summary>
	public static TimeSpan TimeUntilNextBoat()
	{
		return BoatScheduleCalculator.TimeUntilNextBoat(OceanTripNewSettings.Instance.LateBoatQueue);
	}

		private void KillLisbeth(object sender, ElapsedEventArgs e)
		{
			schedule = Routes.GetSchedule();

			if (!ignoreBoat)
			{
				if ((OceanTripNewSettings.Instance.FishPriority != FishPriority.FishLog)
						|| (FocusFishLog && FishingLog.MissingFish().Count > 0))
				{
					//Log("Stopping Lisbeth!");
					_ = Lisbeth.StopGently(); // Fire and forget - can't await in timer event handler
					PassTheTime.freeToCraft = false;
				}
				else
				{
					Log("Not getting on the boat, no fish needed");
				}
			}

		TimeSpan timeLeftUntilFirstRun = BoatScheduleCalculator.TimeUntilNextBoat(OceanTripNewSettings.Instance.LateBoatQueue);

			// Thread-safe timer access
			lock (timerLock)
			{
				if (execute != null && !execute.Enabled)
				{
					if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
						execute.Interval = 100;
					else
						execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

					execute.Start();
				}
			}
		}

		public override void Stop()
		{
			// Thread-safe cleanup
			lock (timerLock)
			{
				if (execute != null)
				{
					execute.Stop();
					execute.Elapsed -= new ElapsedEventHandler(KillLisbeth);
					execute.Dispose();
					execute = null; // Set to null after disposal
				}
			}

			_root = null;

			RouteDataCache.InvalidateCache();
			FishDataCache.InvalidateCache();
			FishingLog.InvalidateCache();
			GameStateCache.Instance.ClearAll();

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

			await OceanFishing();

			return true;
		}

		/// <summary>
		/// Prepare for ocean fishing voyage (sell/desynth fish, repair, restock bait, idle activities, board boat)
		/// </summary>
		private async Task PrepareForVoyage()
		{
			//missingFish = await GetFishLog();
			if (Core.Me.CurrentJob == ClassJobType.Fisher)
			{
				if (OceanTripNewSettings.Instance.ExchangeFish == ExchangeFish.Sell)
				{
					await Coroutine.Sleep(FishingConstants.FISH_EXCHANGE_DELAY_MS);
					await LandSell(FishDataCache.GetFish().Where(x => x.Rarity != "Rare").Select(x => x.FishID).ToList());
				}
				else if (OceanTripNewSettings.Instance.ExchangeFish == ExchangeFish.Desynth)
				{
					await Coroutine.Sleep(FishingConstants.FISH_EXCHANGE_DELAY_MS);
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
				await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);

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

			var boardingContext = new BoardingContext
			{
				SwitchToJobCallback = SwitchToJob,
				IsOnBoatCallback = () => OnBoat
			};
			await boatBoardingHandler.BoardBoat(boardingContext);
		}

		/// <summary>
		/// Execute the fishing voyage on the boat
		/// </summary>
		private async Task ExecuteVoyage()
		{
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

				var fishingContext = new FishingSessionContext
			{
				Location = location,
				TimeOfDay = TimeOfDay,
				Spot = spot,
				CurrentRoute = currentRoute,
				BaitId = baitId,
				SpectralBaitId = spectralbaitId,
				ShouldContinueFishingCallback = () => OnBoat && Endeavor.shouldFish,
				RefreshUICallback = () =>
				{
					FFXIV_Databinds.Instance.RefreshAchievements();
					FFXIV_Databinds.Instance.RefreshCurrentRoute();
				},
				RefreshBaitCallback = () => FFXIV_Databinds.Instance.RefreshBait(),
				ManageBuffsCallback = ManageBuffsAndConsumables,
				ProcessCaughtFishCallback = ProcessCaughtFish,
				WaitForCastLogCallback = WaitForCastLog,
				SelectAndApplyBaitCallback = async (spectraled) =>
				{
					await SelectAndApplyBait(spectraled, location, TimeOfDay, baitId, spectralbaitId, currentRoute);
				},
				OnHookExecutedCallback = (logged) => caughtFishLogged = logged
			};
			await fishingSessionManager.ExecuteFishingSession(fishingContext);
			}
		}

		/// <summary>
		/// Handle voyage completion and results screen
		/// </summary>
		private async Task HandleVoyageCompletion()
		{
			// Handle results screen when voyage ends (waitingOnBoat becomes false when status is Finished)
			if (!Endeavor.waitingOnBoat)
			{
				await Coroutine.Sleep(FishingConstants.VOYAGE_COMPLETION_DELAY_MS);

				AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
				if (windowByName != null)
				{
					Log($"Found results window. Waiting for calculation to end.", OceanLogLevel.Debug);

					// This is super sloppy as we have to rely on a bunch of sleeps right now.
					await Coroutine.Sleep(FishingConstants.RESULTS_CALCULATION_DELAY_MS);

					// What if the player already clicked the button and we're now loading or something else? This will potentially CRASH the client. Look into refining this later.
					windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
					if (windowByName != null)
						windowByName.SendAction(1, 3, 0);

					Log($"Sent confirmation to close results window. Now waiting for loading screen.", OceanLogLevel.Debug);


					if (await Coroutine.Wait(FishingConstants.LOADING_TIMEOUT_MS, () => CommonBehaviors.IsLoading))
					{
						await Coroutine.Yield();
						await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
					}

					Log($"Done loading! This voyage is OVER! Time to wait for the next boat.", OceanLogLevel.Debug);

					FFXIV_Databinds.Instance.RefreshCurrentRoute();
					PassTheTime.freeToCraft = true;
				}

				await Coroutine.Sleep(FishingConstants.VOYAGE_COMPLETION_DELAY_MS);
			}
		}

		private async Task OceanFishing()
	{
		await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);

			//GetSchedule();
			if (!OnBoat)
			{
				await PrepareForVoyage();
			}

			await ExecuteVoyage();
			await HandleVoyageCompletion();
		}

		public async Task GoOpenWorldFishing()
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

			if ((Core.Me.MaxGP - Core.Me.CurrentGP) > FishingConstants.THALIAK_GP_THRESHOLD && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
			{
				ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
			}

			while (FishingManager.State != FishingState.PoleReady)
			{
				if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
				{
					// My testing has shown that there is always a 2.8f variance here, so remove that variance
					// May need future adjustment. Might just be an issue in latency.
					bite = (DateTime.Now - startedCast).TotalSeconds + FishingConstants.BITE_TIME_VARIANCE;

					Log($"Bite Time: {bite:F1}s");
					FishingManager.Hook();
				}

				FFXIV_Databinds.Instance.RefreshBait();
				await Coroutine.Yield();
			}
		}

		public async Task SwitchToJob(ClassJobType job)
		{
			if (Core.Me.CurrentJob == job) return;

			var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

			if (gearSets.Any(gs => gs.Class == job))
			{
				Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet");
				gearSets.First(gs => gs.Class == job).Activate();
				await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);
			}

			if (Core.Me.CurrentJob != job)
				Logging.Write(Colors.Red, $"[Ocean Trip] Could not change to FSH.");
		}

		/// <summary>
		/// Use the best available cordial using strategy pattern
		/// </summary>
		private async Task UseCordial()
		{
			await cordialStrategy.UseBestAvailableCordial();
		}

		public async Task LandRepair(int repairThreshold)
		{
			if (InventoryManager.EquippedItems.Where(r => r.IsFilled && r.Condition < repairThreshold).Count() > 0)
			{
				Logging.Write(Colors.Aqua, "Starting repair...");

				if (await NPCInteractionHelper.InteractWithMenderAndWaitForMenu())
				{
					NPCInteractionHelper.SelectIconStringSlot(1); // Repair option
					await NPCInteractionHelper.RepairAllEquipment();
				}

				Logging.Write(Colors.Aqua, "Repair complete!");
			}
		}

		/// <summary>
		/// Restock bait using the strategy pattern for cleaner, maintainable code
		/// </summary>
		public async Task RestockBait(int baitThreshold, uint baitCap)
		{
			await baitRestockStrategy.RestockBait(baitThreshold, baitCap);
		}

		public async Task LandSell(List<int> itemIds)
		{
			var itemsToSell = InventoryManager.FilledSlots.Where(bs => bs.IsSellable && itemIds.Contains((int)bs.RawItemId));
			if (itemsToSell.Count() != 0)
			{
				Log("Selling fish...");

				if (await NPCInteractionHelper.InteractWithMenderAndWaitForMenu())
				{
					NPCInteractionHelper.SelectIconStringSlot(0); // Shop option

					if (await NPCInteractionHelper.WaitForShopOpen())
					{
						foreach (var item in itemsToSell)
						{
							if (item.Value <= 18)
							{
								var name = item.Name;
								await CommonTasks.SellItem(item);
								await Coroutine.Wait(FishingConstants.ITEM_SELL_TIMEOUT_MS, () => !item.IsFilled || !item.Name.Equals(name));
								await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);
							}
						}

						await NPCInteractionHelper.CloseShop();
					}
				}

				Log("Fish selling complete!");
			}
		}

		public async Task EmptyScrips(int itemId, int scripThreshold)
		{
#if RB_DT
			SpecialCurrency currency = SpecialCurrency.PurpleGatherersScrips;
#else
			SpecialCurrency currency = SpecialCurrency.WhiteGatherersScrips;
#endif

			// TODO: Future enhancement - Support purchasing additional items with excess scrips (not just Hi-Cordials)
			if (SpecialCurrencyManager.GetCurrencyCount(currency) > scripThreshold)
			{
				Logging.Write(Colors.Aqua, $"[Ocean Trip] Purchasing {(int)SpecialCurrencyManager.GetCurrencyCount(currency) / 20} Hi-Cordials!");

				await PassTheTime.IdleLisbeth(itemId, (int)SpecialCurrencyManager.GetCurrencyCount(currency) / 20, "Exchange", "false", 0);
			}

		}

		private async Task WaitForCastLog()
		{
			//await Coroutine.Wait(FishingConstants.DIALOG_WINDOW_TIMEOUT_MS, () => (ChatCheck("cast your line", "cast your line") || ChatCheck("recast your line", "recast your line")));
			await Coroutine.Wait(FishingConstants.CAST_COMPLETION_TIMEOUT_MS, () => (FishingManager.State == FishingState.Reelin));
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

		/// <summary>
		/// Manage buffs and consumables (Cordials, Thaliak's Favor)
		/// </summary>
		private async Task ManageBuffsAndConsumables(bool spectraled)
		{
			// Check for spectral weather changes
			if (gameCache.CurrentWeatherId != Weather.Spectral)
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

			await Coroutine.Yield();

			// Should we Cordial?
			Log("Checking for Hi-Cordial Use.", OceanLogLevel.Debug);

			if (gameCache.NeedsGPRecovery(FishingConstants.CORDIAL_GP_THRESHOLD) && spectraled)
			{
				await UseCordial();
				await Coroutine.Yield();
			}
			else if (gameCache.NeedsGPRecovery(FishingConstants.CORDIAL_GP_THRESHOLD) && gameCache.CurrentGPPercent < FishingConstants.LOW_GP_PERCENT)
			{
				await UseCordial();
				await Coroutine.Yield();
			}

			Log("Done with Cordials.", OceanLogLevel.Debug);

			// Should we use Thaliak's Favor?
			Log("Checking if we need to use Thaliak's Favor", OceanLogLevel.Debug);

			if (spectraled && gameCache.NeedsGPRecovery(FishingConstants.THALIAK_GP_THRESHOLD) && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
			{
				Log("Using Thaliak's Favor!");
				ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
				await Coroutine.Yield();
			}
			else if (!spectraled && gameCache.NeedsGPRecovery(FishingConstants.THALIAK_GP_THRESHOLD) && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me) && Core.Player.Auras.Any(x => x.Id == CharacterAuras.AnglersArt && x.Value >= 7))
			{
				Log("Currently at >7 Angler's Art Stacks - Using Thaliak's Favor!");
				ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
				await Coroutine.Yield();
			}

			Log("Done with Thaliak's Favor.", OceanLogLevel.Debug);
		}

		/// <summary>
		/// Process caught fish and check for Identical Cast
		/// Returns true if Identical Cast was used and casting has started
		/// </summary>
		private async Task<bool> ProcessCaughtFish()
		{
			Log("Checking for a recently caught fish.", OceanLogLevel.Debug);

			// Cache LastFishCaught to avoid reading game memory multiple times (expensive operation)
			// Now uses the new Catch API when available for better reliability
			var currentFish = FishingLog.LastFishCaught;

			// Did we catch a fish? Let's log it.
			if (lastCaughtFish != currentFish && !caughtFishLogged)
			{
				lastCaughtFish = currentFish;
				caughtFish.Add(currentFish);
				caughtFishLogged = true;

				// Use Catch.FishName if available, otherwise fall back to gameCache lookup
				string fishName = FishingLog.LastFishName;
				if (string.IsNullOrEmpty(fishName))
					fishName = gameCache.GetItemName(currentFish);

				Log($"Caught {fishName}.");

				// Remove from missing fish list if needed
				if (FishingLog.MissingFish().Contains(currentFish))
					FishingLog.RemoveFish(currentFish);
			}

			Log("Done checking for a recently caught fish.", OceanLogLevel.Debug);

			//Identical Cast for Blue fish
			Log("Checking if we need to use Identical Cast.", OceanLogLevel.Debug);

			bool shouldIdenticalCast = false;

			// Check dictionary for standard identical cast fish
			if (FishingConstants.IdenticalCastTargets.TryGetValue(lastCaughtFish, out int requiredCount))
			{
				int currentCount = caughtFish.Count(x => x == lastCaughtFish);
				if (currentCount < requiredCount)
					shouldIdenticalCast = true;
			}
			// Special case for Funnel Shark (points mode only)
			else if (lastCaughtFish == OceanFish.FunnelShark
				&& (OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto || OceanTripNewSettings.Instance.FishPriority == FishPriority.Points)
				&& Core.Me.CurrentGP >= 700)
			{
				shouldIdenticalCast = true;
			}

			if (shouldIdenticalCast && ActionManager.CanCast(Actions.IdenticalCast, Core.Me) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
			{
				Log("Identical Cast!");
				ActionManager.DoAction(Actions.IdenticalCast, Core.Me);
				await WaitForCastLog();
				startedCast = DateTime.Now;

				Log("Done checking for Identical Cast.", OceanLogLevel.Debug);
				return true;  // Identical Cast was used, casting has started
			}

			Log("Done checking for Identical Cast.", OceanLogLevel.Debug);
			return false;  // No Identical Cast, proceed with normal flow
		}

		/// <summary>
		/// Select and apply the appropriate bait based on spectral status, location, time of day, and fish log
		/// </summary>
		private async Task SelectAndApplyBait(bool spectraled, string location, string timeOfDay, ulong baitId, ulong spectralbaitId, RouteWithFish currentRoute)
		{
			var context = new BaitSelectionContext
			{
				Location = location,
				TimeOfDay = timeOfDay,
				DefaultBaitId = spectraled ? spectralbaitId : baitId,
				CurrentRoute = currentRoute,
				MissingFish = FishingLog.MissingFish(),
				CaughtFish = caughtFish,
				FocusFishLog = FocusFishLog,
				CurrentWeather = gameCache.CurrentWeather
			};

			// Use achievement bait selector when in achievement mode
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
			{
				await achievementBaitSelector.SelectBait(context);
			}
			else if (spectraled)
			{
				await spectralBaitSelector.SelectBait(context);
			}
			else
			{
				await normalBaitSelector.SelectBait(context);
			}
		}

		private static bool PartyLeaderWaitConditions()
		{
			return PartyManager.VisibleMembers.Count() == PartyManager.AllMembers.Count();
		}

		/// <summary>
		/// Structured logging with log level support
		/// </summary>
		/// <param name="text">Message to log (supports format strings)</param>
		/// <param name="level">Log level (Always, Info, or Debug)</param>
		/// <param name="args">Format arguments</param>
		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info, params object[] args)
		{
			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text, args);
			Logging.Write(Colors.Aqua, msg);
		}
	}
}
