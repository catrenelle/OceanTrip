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

		// Last-logged progress per mission slot, to only log on change. ushort.MaxValue means
		// "not yet observed" so a real progress of 0 still logs once.
		private readonly ushort[] lastLoggedMissionProgress = { ushort.MaxValue, ushort.MaxValue, ushort.MaxValue };

		// Whether a spectral current has already occurred at the current stop. Reset on stop
		// transition (see the lastLoggedLocation check below); set once spectral is observed active.
		// Used to gate GP-banking — see ManageBuffsAndConsumables.
		private bool _hadSpectralThisStop;

		// Static mirror of _hadSpectralThisStop — see SpectralPityActive just below for why a
		// static bridge is used (CurrentRoutePageBehavior can't reach into a running instance).
		// Combined with knowing it's the last stop (CurrentRoutePageBehavior already tracks that
		// locally via Endeavor.CurrentZone), lets the UI show the "last stop, still chasing
		// spectral" notice — see NormalBaitSelector.NeedsSpectral's IsLastStop check.
		internal static bool HadSpectralThisStop { get; private set; }

		// Whether the stop we just LEFT never saw a spectral current — per the community-confirmed
		// pity rule, that makes the current stop's spectral run longer (3 min vs 2) with rising
		// trigger odds per spectral-fish catch, and it doesn't stack (recomputed fresh from
		// _hadSpectralThisStop on every transition, so repeatedly missing it just holds this at
		// true rather than escalating). Threaded into BaitSelectionContext so bait selectors can
		// prioritize converting it — see NormalBaitSelector.NeedsSpectral.
		private bool _spectralPityActive;

		// Static mirror of _spectralPityActive so CurrentRoutePageBehavior (which otherwise
		// re-derives all its state independently from Endeavor/GameStateCache/WorldManager rather
		// than reaching into a running OceanTrip instance) can show a "trying to trigger spectral"
		// notice on the Current Strategy panel. RebornBuddy only ever runs one BotBase instance at
		// a time, so a static bridge here is safe — set in lockstep with the instance field, never
		// read/written anywhere else.
		internal static bool SpectralPityActive { get; private set; }

		private bool ignoreBoat { get { if (OceanTripNewSettings.Instance.FishPriority == FishPriority.IgnoreBoat) { return true; } else { return false; } } }

		private static Random rnd = new Random();

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
		private BoatBoardingHandler boatBoardingHandler;
		private FishingSessionManager fishingSessionManager;
		private BaitRestockStrategy baitRestockStrategy;
		private CordialStrategy cordialStrategy;

		public override string Name => "Ocean Trip";

		public override PulseFlags PulseFlags => PulseFlags.All;

		public override bool IsAutonomous => true;
		public override bool RequiresProfile => false;

		public override Composite Root => _root;

		public override bool WantButton { get; } = true;

		public Ocean_Trip.Endeavor Endeavor;

		private GameStateCache gameCache => GameStateCache.Instance;

		/// <summary>
		/// Initialize shared resources used by both OnButtonPress and Start
		/// </summary>
		private void InitializeSharedResources()
		{
			Ocean_Trip.OceanFishingOffsetSync.RunAsync();

			FFXIV_Databinds.Instance.RefreshBait();
			FFXIV_Databinds.Instance.RefreshAchievements();

			// Pre-cache the fish data
			FishDataCache.GetFish();
			RouteDataCache.GetRoutesWithFish();

			if (Endeavor == null)
				Endeavor = new Ocean_Trip.Endeavor();
		}

		public override void OnButtonPress()
		{
			try
			{
				Ocean_Trip.UI.Wpf.ShellWindow.Show();
			}
			catch (Exception ex)
			{
				Logging.Write(Colors.Red, $"[Ocean Trip] WPF shell failed: {ex}");
			}

			InitializeSharedResources();
		}

		public override void Start()
		{
			// Ocean Fishing itself is gated behind a Fisher quest — without it, none of the Endeavor
			// memory reads or boat-boarding logic have anything valid to work against, so check this
			// before touching any of that rather than letting it fail confusingly mid-voyage-prep.
			if (!QuestLogManager.IsQuestCompleted(FishingConstants.OCEAN_FISHING_UNLOCK_QUEST_ID))
			{
				Log("Ocean Fishing isn't unlocked yet — complete the Fisher quest \"All the Fish in the Sea\" " +
					"(level 1, from Fhilsnoe at the Limsa Lominsa Lower Decks aftcastle) before running this BotBase.");
				TreeRoot.Stop("Ocean Fishing not unlocked — complete \"All the Fish in the Sea\" first.");
				return;
			}

			int fisherLevel = Core.Me.Levels[ClassJobType.Fisher];
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto
				&& fisherLevel < FishingConstants.LEVELING_MODE_LEVEL_CAP)
			{
				Log($"Fisher level {fisherLevel} is below {FishingConstants.LEVELING_MODE_LEVEL_CAP} — " +
					$"Automatic priority will focus on leveling (Krill/Ragworm/Plump Worm only, no lures) " +
					$"until you reach {FishingConstants.LEVELING_MODE_LEVEL_CAP}.");
			}

			TreeHooks.Instance.ClearAll();

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
			boatBoardingHandler = new BoatBoardingHandler();
			fishingSessionManager = new FishingSessionManager(gameCache, hookingStrategy);
			baitRestockStrategy = new BaitRestockStrategy();
			cordialStrategy = new CordialStrategy(gameCache);

			// Initialize timer with thread-safe lock
			lock (timerLock)
			{
				// Create new timer if null or disposed
				if (execute == null)
				{
					execute = new System.Timers.Timer();
					execute.AutoReset = false; // Only fire once, then manually restart
				}

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

			// Timer will be restarted after voyage completes in HandleVoyageCompletion()
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
					await LandSell(GetExchangeableFishIds());
				}
				else if (OceanTripNewSettings.Instance.ExchangeFish == ExchangeFish.Desynth)
				{
					await Coroutine.Sleep(FishingConstants.FISH_EXCHANGE_DELAY_MS);
					await PassTheTime.DesynthOcean(GetExchangeableFishIds());
				}

				//await Lisbeth.SelfRepairWithMenderFallback();
				await LandRepair(50);
			}

			FFXIV_Databinds.Instance.RefreshBait();
			FFXIV_Databinds.Instance.RefreshAchievements();

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
					Log("Switching to FSH class...");
					await SwitchToJob(ClassJobType.Fisher);
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
				Log("Switching to FSH class...");
				await SwitchToJob(ClassJobType.Fisher);
			}

			// Verify we're actually Fisher before proceeding to the boat
			if (Core.Me.CurrentJob != ClassJobType.Fisher)
			{
				Log("ERROR: Failed to switch to Fisher class. Cannot board boat. Waiting 5 seconds and retrying...");
				await Coroutine.Sleep(5000);
				await SwitchToJob(ClassJobType.Fisher);

				// Final check - if still not Fisher, log error and return
				if (Core.Me.CurrentJob != ClassJobType.Fisher)
				{
					Log($"CRITICAL: Unable to switch to Fisher (current: {Core.Me.CurrentJob}). Skipping boat boarding.");
					return;
				}
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

			// Auto-detect route from zone ID and sync the setting so achievement code reads the correct route
			string detectedRoute = WorldManager.RawZoneId == Zones.TheEndeavor ? "Indigo" : "Ruby";
			var settingRoute = OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Ruby ? "Ruby" : "Indigo";
			if (detectedRoute != settingRoute)
			{
				Log($"Route auto-detected as {detectedRoute} (UI setting was {settingRoute}) — updating setting to match");
				OceanTripNewSettings.Instance.FishingRoute = detectedRoute == "Ruby" ? FishingRoute.Ruby : FishingRoute.Indigo;
			}

			schedule = Routes.GetSchedule(route: detectedRoute);
			string location = "";
			string TimeOfDay = "";
			string lastLoggedLocation = "";

			// Warn if selected achievement has few fish available across this route
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
			{
				var focus = AchievementFishDataCache.GetCurrentAchievementFocus();
				if (focus != AchievementType.None)
				{
					int stopsWithFish = 0;
					for (int i = 0; i < 3; i++)
					{
						string stopLocation = schedule[i].Item1;
						string stopTime = schedule[i].Item2;
						// Day/Night/Sunset only ever gates spectral fish; weather isn't knowable this far
						// ahead of the stop, so normal fish are counted as possible regardless.
						var fish = AchievementFishDataCache.GetFishForLocation(stopLocation, focus)
							.Where(f => !f.SpectralFish || (f.TimeOfDayExclusion1 != stopTime && f.TimeOfDayExclusion2 != stopTime))
							.ToList();
						if (fish.Any())
							stopsWithFish++;
					}
					if (stopsWithFish == 0)
						Log($"WARNING: No {focus} fish available on this route! Consider switching to Points or Fish Log mode.");
					else if (stopsWithFish == 1)
						Log($"WARNING: {focus} fish only available at 1/3 stops on this route. Achievement progress will be limited.");
				}
			}

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

				ulong baitId = FishBait.Krill;
				ulong spectralbaitId = FishBait.Krill;

				location = schedule[Endeavor.CurrentZone].Item1;
				TimeOfDay = schedule[Endeavor.CurrentZone].Item2;

				if (String.IsNullOrEmpty(TimeOfDay))
					TimeOfDay = "Day";

				if (location != lastLoggedLocation)
				{
					// Carry pity forward from the stop we're leaving — but only on a genuine
					// transition, not the very first stop of the voyage (lastLoggedLocation still
					// empty here), where _hadSpectralThisStop's initial false would otherwise read
					// as "previous stop skipped it" with no previous stop to have skipped it at.
					if (!string.IsNullOrEmpty(lastLoggedLocation))
					{
						_spectralPityActive = !_hadSpectralThisStop;
						SpectralPityActive = _spectralPityActive;
						if (_spectralPityActive)
							Log("Spectral pity active — last stop never saw a current, this one runs longer.", OceanLogLevel.Debug);
					}
					else
					{
						// First stop of a fresh voyage. Unlike _hadSpectralThisStop (reset unconditionally
						// below on every stop transition), pity is only ever written inside the branch
						// above — so without this, a pity=true left over from the previous voyage's last
						// stop-transition would otherwise still be sitting in these fields and get shown
						// as active on round 1, where pity can't legitimately apply.
						_spectralPityActive = false;
						SpectralPityActive = false;
					}

					lastLoggedLocation = location;
					_hadSpectralThisStop = false;
					HadSpectralThisStop = false;
					string priorityMode = OceanTripNewSettings.Instance.FishPriority.ToString();
					string focusMode = FocusFishLog ? "Fish Log" : "Points";
					Log($"Zone: {Schedule.areaName(location)} ({location}), Time: {TimeOfDay}, Stop: {Endeavor.CurrentZone + 1}/3, Priority: {priorityMode} ({focusMode})");

					var zoneRoute = RouteDataCache.GetRoutesWithFish().FirstOrDefault(x => x.Route.RouteShortName == location);
					if (zoneRoute != null)
						Log($"Bait: Normal={gameCache.GetItemName((uint)zoneRoute.Route.NormalBait)}, Spectral={gameCache.GetItemName((uint)zoneRoute.Route.SpectralBait)}");
				}

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
					},
					RefreshBaitCallback = () => FFXIV_Databinds.Instance.RefreshBait(),
					ManageBuffsCallback = ManageBuffsAndConsumables,
					ProcessCaughtFishCallback = ProcessCaughtFish,
					PrizeCatchCallback = MaybeUsePrizeCatch,
					OnHookExecutedCallback = (logged) => { caughtFishLogged = logged; lastCaughtFish = 0; }
				};
				fishingContext.SelectAndApplyBaitCallback = async (spectraled) =>
				{
					var result = await SelectAndApplyBait(spectraled, location, TimeOfDay, baitId, spectralbaitId, currentRoute);
					fishingContext.SetShouldMooch(result.shouldMooch);
					fishingContext.SetChainCastTargetFishId(result.chainCastTargetFishId);
					fishingContext.SetChainMoochTargetFishId(result.chainMoochTargetFishId);
					fishingContext.SetMissionRequiredTugType(result.missionRequiredTugType);
					fishingContext.SetMissionRequiredAchievementTags(result.missionRequiredAchievementTags);
				};
				fishingContext.RefreshMissionStateCallback = () =>
				{
					fishingContext.SetMissionRequiredTugType(GetActiveMissionTugType());
					fishingContext.SetMissionRequiredAchievementTags(GetActiveMissionAchievementTags());
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

					// Read + log the actual results (score, bonuses, placement) now, while the
					// window's still up — this is the only point in the whole voyage where these are
					// knowable for certain, since bonus eligibility (spectral-catch counts, rare-fish
					// catches, etc.) isn't reliably predictable live.
					LogVoyageResult();

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

					PassTheTime.freeToCraft = true;
				}

				// Restart the timer for the next boat
				lock (timerLock)
				{
					if (execute != null)
					{
						TimeSpan timeLeftUntilNextRun = TimeUntilNextBoat();
						if (timeLeftUntilNextRun.TotalMilliseconds < 0)
							execute.Interval = 100;
						else
							execute.Interval = timeLeftUntilNextRun.TotalMilliseconds;

						execute.Start();
					}
				}

				await Coroutine.Sleep(FishingConstants.VOYAGE_COMPLETION_DELAY_MS);
			}
		}

		private static readonly string VoyageHistoryPath =
			Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripVoyageHistory.csv");

		/// <summary>
		/// Reads the just-finished voyage's results (score, bonuses, placement) from Endeavor and
		/// both logs a human-readable summary and appends a row to a persistent CSV so results are
		/// trackable over time, not just visible for the few seconds the results window is up.
		/// </summary>
		private void LogVoyageResult()
		{
			var result = Endeavor.ReadVoyageResult();
			if (result == null)
			{
				Log("Could not read voyage results (director not initialized) — skipping results log.", OceanLogLevel.Debug);
				return;
			}

			var bonuses = result.BonusIds
				.Select(id => BonusDataCache.GetById(id))
				.Where(b => b != null)
				.ToList();

			string placementText = result.Placement.HasValue
				? $"{Ordinal(result.Placement.Value)} of {result.TrackedPlayers.Count}"
				: "unranked (not in top 10)";

			Log($"Voyage complete: {result.TotalPoints} points, placed {placementText}, {result.CaughtFish} fish caught, {result.ExperiencePoints} XP.");

			if (bonuses.Count > 0)
				Log($"Bonuses earned: {string.Join(", ", bonuses.Select(b => $"{b.Objective} (+{b.BonusMultiplier - 100}%)"))}");
			else
				Log("Bonuses earned: none.");

			try
			{
				bool needsHeader = !File.Exists(VoyageHistoryPath);
				using (var writer = new StreamWriter(VoyageHistoryPath, append: true))
				{
					if (needsHeader)
						writer.WriteLine("Timestamp,Route,TotalPoints,Placement,TrackedPlayers,CaughtFish,ExperiencePoints,Scrip1,Scrip2,Bonuses");

					string bonusField = string.Join("; ", bonuses.Select(b => b.Objective));
					writer.WriteLine(string.Join(",",
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						OceanTripNewSettings.Instance.FishingRoute,
						result.TotalPoints,
						result.Placement?.ToString() ?? "",
						result.TrackedPlayers.Count,
						result.CaughtFish,
						result.ExperiencePoints,
						result.Scrip1Amount,
						result.Scrip2Amount,
						CsvQuote(bonusField)));
				}
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Failed to write voyage history log: {ex.Message}");
			}

			// Same "3rd/last stop represents the whole voyage" convention Helpers/Schedule.cs
			// already uses for the Schedule page's routeName/routeTime columns — recomputed fresh
			// here rather than threaded through from the main loop's local `schedule`/`TimeOfDay`
			// variables, since GetSchedule is a pure function of (time, route) and the voyage just
			// completed well within the same 2-hour slot.
			string fishingRoute = OceanTripNewSettings.Instance.FishingRoute.ToString();
			var schedule = Ocean_Trip.Definitions.Routes.GetSchedule(route: fishingRoute);
			string timeOfDay = schedule != null && schedule.Length >= 3 ? schedule[2].Item2 : null;
			string zoneName = schedule != null && schedule.Length >= 3 ? Schedule.areaName(schedule[2].Item1) : null;

			// Structured, readable-back sibling of the CSV log above — feeds the Result History
			// page's stats and last-10 table. Append() is self-contained (own try/catch), so no
			// outer guard needed here.
			Ocean_Trip.VoyageHistoryStore.Append(new Ocean_Trip.VoyageHistoryEntry
			{
				Timestamp = DateTime.Now,
				Route = fishingRoute,
				ZoneName = zoneName,
				TimeOfDay = timeOfDay,
				TotalPoints = result.TotalPoints,
				Placement = result.Placement,
				TrackedPlayerCount = result.TrackedPlayers.Count,
				CaughtFish = result.CaughtFish,
				ExperiencePoints = result.ExperiencePoints,
				Scrip1Amount = result.Scrip1Amount,
				Scrip2Amount = result.Scrip2Amount,
				Bonuses = bonuses.Select(b => new Ocean_Trip.VoyageBonusEntry { Id = b.Id, Name = b.Objective, Multiplier = b.BonusMultiplier }).ToList(),
			});
		}

		private static string CsvQuote(string field) => "\"" + (field ?? "").Replace("\"", "\"\"") + "\"";

		private static string Ordinal(int n)
		{
			if (n % 100 >= 11 && n % 100 <= 13)
				return $"{n}th";

			switch (n % 10)
			{
				case 1: return $"{n}st";
				case 2: return $"{n}nd";
				case 3: return $"{n}rd";
				default: return $"{n}th";
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

				if ((Core.Me.MaxGP - Core.Me.CurrentGP) > FishingConstants.THALIAK_GP_THRESHOLD && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
				{
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
				}

				// Cordial for GP recovery
				if (gameCache.NeedsGPRecovery(FishingConstants.CORDIAL_GP_THRESHOLD))
				{
					await UseCordial();
				}

				// Apply Patience if enabled and not already active
				if (OceanTripNewSettings.Instance.Patience != ShouldUsePatience.OnlyForSpecificFish && !FishingManager.HasPatience)
				{
					await patienceManager.UsePatience();
				}

				if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both)
				{
					FishingManager.Mooch();
				}
				else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
				{
					FishingManager.MoochTwo();
				}
				else
				{
					FishingManager.Cast();
				}
			}

			var openWorldLureStrategy = new LureStrategy(gameCache);
			openWorldLureStrategy.ResetForNewCast();
			bool hookExecuted = false;

			while (FishingManager.State != FishingState.PoleReady && FishingManager.State != FishingState.None)
			{
				// Apply lure while line is in water
				if (FishingManager.State != FishingState.Bite && FishingManager.State != FishingState.PoleReady)
				{
					var lureContext = new LureContext
					{
						Location = "",
						TimeOfDay = "",
						Spectraled = false,
						LastCastMooch = false,
						TargetFishId = 0,
						MissingFish = null
					};
					await openWorldLureStrategy.TryApplyLure(lureContext);
				}

				// Auto-accept collectable preservation dialog
				if (SelectYesno.IsOpen)
					SelectYesno.Yes();

				if (FishingManager.CanHook && FishingManager.State == FishingState.Bite && !hookExecuted)
				{
					hookExecuted = true;
					Log($"Bite Time: {FishingManager.TimeSinceCast.TotalSeconds + FishingConstants.BITE_TIMER_OFFSET:F1}s, Tug: {FishingManager.TugType}");

					if (FishingManager.HasPatience)
					{
						if (FishingManager.TugType == TugType.Light)
						{
							Log("Using Precision Hookset!");
							ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
						}
						else
						{
							Log("Using Powerful Hookset!");
							ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
						}
					}
					else
					{
						FishingManager.Hook();
					}
				}

				await Coroutine.Yield();
			}
		}

		public async Task SwitchToJob(ClassJobType job)
		{
			if (Core.Me.CurrentJob == job) return;

			// Close any open crafting windows that might block job switching
			if (CraftingLog.IsOpen)
			{
				CraftingLog.Close();
				await Coroutine.Wait(5000, () => !CraftingLog.IsOpen);
			}

			// Wait for any ongoing crafting to finish
			if (CraftingManager.IsCrafting)
			{
				Log("Waiting for crafting to finish before switching jobs...");
				await Coroutine.Wait(30000, () => !CraftingManager.IsCrafting);
			}

			var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

			if (gearSets.Any(gs => gs.Class == job))
			{
				Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet for {job}");
				gearSets.First(gs => gs.Class == job).Activate();

				// Wait for the job change to actually complete (up to 10 seconds)
				if (await Coroutine.Wait(10000, () => Core.Me.CurrentJob == job))
				{
					Log($"Successfully switched to {job}");
				}
				else
				{
					// Retry once if first attempt failed
					Log($"First job switch attempt failed, retrying...");
					await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);
					gearSets.First(gs => gs.Class == job).Activate();
					await Coroutine.Wait(10000, () => Core.Me.CurrentJob == job);
				}
			}

			if (Core.Me.CurrentJob != job)
				Logging.Write(Colors.Red, $"[Ocean Trip] Could not change to {job}. Current job: {Core.Me.CurrentJob}");
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
			var allowedBaits = OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Leveling
				? FishingConstants.LEVELING_ALLOWED_BAITS
				: null;

			await baitRestockStrategy.RestockBait(baitThreshold, baitCap, allowedBaits);
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
				if (WorldManager.RawZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby || WorldManager.RawZoneId == Zones.TheEndeavor_Thavnair)
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
			bool currentlySpectral = gameCache.CurrentWeatherId == Weather.Spectral;
			if (!currentlySpectral)
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
				_hadSpectralThisStop = true;
				HadSpectralThisStop = true;
			}

			await Coroutine.Yield();

			// Bank GP/Angler's Art instead of proactively spending it while still chasing this stop's
			// first spectral current — Thaliak's Favor and Hi-Cordial are worth far more spent in a
			// burst during the current than trickled out during ordinary fishing. That window closes
			// as soon as: spectral is currently active (spend freely — see below), a spectral current
			// has already happened at this stop, or under 90s remain (a current can't start that
			// close to the stop ending, so there's nothing left to bank for). SecondsRemainingAtStop
			// returning null (offset/unit assumption didn't pan out) fails toward still banking rather
			// than assuming a current is imminent when we can't actually tell.
			double? secondsRemaining = Endeavor.SecondsRemainingAtStop;
			bool spectralStillChasable = !_hadSpectralThisStop
				&& (secondsRemaining == null || secondsRemaining > FishingConstants.SPECTRAL_CUTOFF_SECONDS);
			bool shouldBankGp = spectralStillChasable && !currentlySpectral;

			// Even while banking, don't let GP run dangerously low — a Hi-Cordial here still leaves
			// plenty in reserve for the spectral burst, and this is the safety valve the pre-WPF-rewrite
			// Cordial logic had (LOW_GP_PERCENT) that banking otherwise bypasses entirely.
			bool criticallyLowGp = shouldBankGp
				&& gameCache.NeedsGPRecovery(FishingConstants.CORDIAL_GP_THRESHOLD)
				&& gameCache.CurrentGPPercent <= FishingConstants.LOW_GP_PERCENT;

			if (shouldBankGp && !criticallyLowGp)
			{
				Log("Banking GP for spectral current — skipping proactive Cordial/Thaliak's Favor.", OceanLogLevel.Debug);
			}
			else
			{
				// Should we Cordial?
				Log("Checking for Hi-Cordial Use.", OceanLogLevel.Debug);

				if (gameCache.NeedsGPRecovery(FishingConstants.CORDIAL_GP_THRESHOLD))
				{
					await UseCordial();
					await Coroutine.Yield();
				}

				Log("Done with Cordials.", OceanLogLevel.Debug);

				if (shouldBankGp)
				{
					// Still banking Angler's Art for the spectral burst — the Hi-Cordial above was
					// only an emergency GP top-up, not a signal to start spending Thaliak's Favor too.
					Log("Still banking Angler's Art for spectral current — skipping Thaliak's Favor.", OceanLogLevel.Debug);
					return;
				}

				// Should we use Thaliak's Favor?
				Log("Checking if we need to use Thaliak's Favor", OceanLogLevel.Debug);

				// No cooldown or cast time on this action, so drain banked Angler's Art stacks (cap 10,
				// 3 consumed per use) in a loop rather than firing once — otherwise GP recovery was
				// throttled to 150/cast even when several uses' worth of stacks were already banked.
				int thaliakUses = 0;
				while (gameCache.NeedsGPRecovery(FishingConstants.THALIAK_GP_THRESHOLD)
					&& ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me)
					&& thaliakUses < FishingConstants.THALIAK_MAX_CHAIN_USES)
				{
					Log("Using Thaliak's Favor!");
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
					await Coroutine.Wait(1000, () => !ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me));

					// Force a fresh GP read — RefreshIfNeeded's 100ms throttle would otherwise let the
					// loop re-check against the stale (pre-cast) GPDeficit and miscount remaining need.
					gameCache.Refresh();
					thaliakUses++;
				}

				Log("Done with Thaliak's Favor.", OceanLogLevel.Debug);
			}
		}

		/// <summary>
		/// "Blind" Prize Catch — cast before the line goes out (not chained off a confirmed bite via
		/// Identical Cast) so whatever bites next is guaranteed Large, covering a blind Double/Triple
		/// Hook. Gated to Spectral Current: outside it there's rarely enough point value on a single
		/// catch to justify 200 GP for a guaranteed-Large roll the way there is on spectral fish.
		/// Scoped to Points/Auto — Achievement mode cares about catch count per species, not Large
		/// status, so Prize Catch doesn't help it the way it helps score.
		/// </summary>
		private async Task MaybeUsePrizeCatch(bool spectraled)
		{
			if (!spectraled)
				return;

			var priority = OceanTripNewSettings.Instance.EffectiveFishPriority;
			if (priority != FishPriority.Points && priority != FishPriority.Auto)
				return;

			// Only worth committing to blind if there's GP left over afterward for the Double Hook
			// that actually cashes in the guaranteed-Large roll — otherwise Prize Catch's own 200 GP
			// just guarantees a Large fish off a plain Hook, which doesn't recoup the spend the way a
			// DH/TH catch does.
			if (Core.Me.CurrentGP < FishingConstants.PRIZE_CATCH_GP_COST + FishingConstants.DOUBLE_HOOK_GP_COST)
				return;

			if (!ActionManager.CanCast(Actions.PrizeCatch, Core.Me))
				return;

			Log("Using Prize Catch!");
			ActionManager.DoAction(Actions.PrizeCatch, Core.Me);
			await Coroutine.Wait(1000, () => !ActionManager.CanCast(Actions.PrizeCatch, Core.Me));
		}

		/// <summary>
		/// Process caught fish and check for Identical Cast
		/// Returns true if Identical Cast was used and casting has started
		/// </summary>
		private Task<bool> ProcessCaughtFish()
		{
			Log("Checking for a recently caught fish.", OceanLogLevel.Debug);

			// Cache LastFishCaught to avoid reading game memory multiple times (expensive operation)
			// Now uses the new Catch API when available for better reliability
			var currentFish = FishingLog.LastFishCaught;

			Log($"ProcessCaughtFish: currentFish={currentFish}, lastCaughtFish={lastCaughtFish}, caughtFishLogged={caughtFishLogged}", OceanLogLevel.Debug);

			LogMissionProgress();

			// Did we catch a fish? Let's log it.
			if (lastCaughtFish != currentFish && !caughtFishLogged)
			{
				lastCaughtFish = currentFish;
				caughtFish.Add(currentFish);
				caughtFishLogged = true;

				// Use Catch.FishName if available, fall back to gameCache, then fishList.json
				string fishName = FishingLog.LastFishName;
				if (string.IsNullOrEmpty(fishName))
					fishName = gameCache.GetItemName(currentFish);
				if (string.IsNullOrEmpty(fishName))
				{
					var fishData = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == (int)currentFish);
					fishName = fishData?.FishName ?? $"Unknown ({currentFish})";
				}

				Log($"Caught {fishName}.");

				// Remove from missing fish list if needed
				if (FishingLog.MissingFish().Contains(currentFish))
				{
					FishingLog.RemoveFish(currentFish);
					FishingLog.SaveMissingFishLog();
				}
			}

			Log("Done checking for a recently caught fish.", OceanLogLevel.Debug);

			//Identical Cast for Blue fish
			Log("Checking if we need to use Identical Cast.", OceanLogLevel.Debug);

			bool shouldIdenticalCast = false;
			bool comboPrizeCatch = false;

			// Leveling mode never chases anything via Identical Cast — chains below are all in
			// service of missing-fish/achievement/points goals a leveling character isn't pursuing,
			// and IC itself costs 350 GP that's better left for plain Hooksets while leveling.
			if (OceanTripNewSettings.Instance.EffectiveFishPriority != FishPriority.Leveling)
			{
			// Check dictionary for standard identical cast fish
			if (FishingConstants.IdenticalCastTargets.TryGetValue(lastCaughtFish, out int requiredCount))
			{
				int currentCount = caughtFish.Count(x => x == lastCaughtFish);
				if (currentCount < requiredCount)
					shouldIdenticalCast = true;
			}
			// Special case for Funnel Shark (points mode only)
			else if (lastCaughtFish == OceanFish.FunnelShark
				&& (OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Auto || OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Points)
				&& Core.Me.CurrentGP >= 700)
			{
				shouldIdenticalCast = true;
			}
			// DH-IC-PC-TH combo: the fish just caught independently clears the "worth Triple Hooking"
			// bar on its own economics (the same math HookingStrategy.IsPointsWorthDoubleHook applies
			// at bite time) — pin it via Identical Cast and guarantee the re-catch Large via Prize
			// Catch, instead of moving on to an unconfirmed cast. Reusing that exact threshold means
			// the next bite's independent DH/TH re-evaluation will also land on Triple Hook for this
			// same fish, so the combo doesn't go to waste on a plain Hook. Requires the full IC+PC+TH
			// round trip up front (not just per-step affordability) so we don't strand mid-combo.
			else if ((OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Auto || OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Points)
				&& gameCache.CurrentWeatherId == Weather.Spectral)
			{
				var comboFishData = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == (int)lastCaughtFish);
				if (comboFishData != null && comboFishData.THBonus > 1
					&& comboFishData.Points * comboFishData.THBonus > FishingConstants.TRIPLE_HOOK_GP_COST
					&& Core.Me.CurrentGP >= FishingConstants.IDENTICAL_CAST_GP_COST + FishingConstants.PRIZE_CATCH_GP_COST + FishingConstants.TRIPLE_HOOK_GP_COST)
				{
					shouldIdenticalCast = true;
					comboPrizeCatch = true;
				}
			}
			// Achievement mode: IC on any fish matching the target achievement
			else if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
			{
				var focus = AchievementFishDataCache.GetCurrentAchievementFocus();
				if (focus != AchievementType.None)
				{
					var caughtFishData = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == (int)lastCaughtFish);
					if (caughtFishData != null && !string.IsNullOrEmpty(caughtFishData.Achievement) &&
						AchievementFishDataCache.MapAchievementString(caughtFishData.Achievement) == focus)
					{
						shouldIdenticalCast = true;
					}
				}
			}
			} // EffectiveFishPriority != Leveling

			if (shouldIdenticalCast && ActionManager.CanCast(Actions.IdenticalCast, Core.Me) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
			{
				// Prize Catch before Identical Cast, matching the guide's ordering — Identical Cast is
				// what actually re-casts the line (see the "casting has started" comment below), so the
				// Large guarantee needs to already be up before that happens.
				if (comboPrizeCatch && ActionManager.CanCast(Actions.PrizeCatch, Core.Me))
				{
					Log("Using Prize Catch (combo)!");
					ActionManager.DoAction(Actions.PrizeCatch, Core.Me);
				}

				Log("Identical Cast!");
				ActionManager.DoAction(Actions.IdenticalCast, Core.Me);

				Log("Done checking for Identical Cast.", OceanLogLevel.Debug);
				return Task.FromResult(true);  // Identical Cast was used, casting has started
			}

			Log("Done checking for Identical Cast.", OceanLogLevel.Debug);
			return Task.FromResult(false);  // No Identical Cast, proceed with normal flow
		}

		/// <summary>
		/// Log live mission progress for all three slots, once per change. Reads raw memory
		/// (mission type IDs, not UI text), so this works regardless of client language.
		/// Unavailable under RB_TC — see Endeavor.cs for why.
		/// </summary>
		private void LogMissionProgress()
		{
			LogMissionSlot(0, Endeavor.Mission1Type, Endeavor.Mission1Progress);
			LogMissionSlot(1, Endeavor.Mission2Type, Endeavor.Mission2Progress);
			LogMissionSlot(2, Endeavor.Mission3Type, Endeavor.Mission3Progress);
		}

		private void LogMissionSlot(int slot, uint missionType, ushort progress)
		{
			if (missionType == 0 || progress == lastLoggedMissionProgress[slot])
				return;

			lastLoggedMissionProgress[slot] = progress;

			var condition = MissionDataCache.GetById(missionType);
			string description = condition != null
				? $"{condition.Text}: {progress}/{condition.Count}"
				: $"Unknown mission type {missionType} ({progress})";

			Log($"Mission {slot + 1}: {description}");
		}

		/// <summary>
		/// Select and apply the appropriate bait based on spectral status, location, time of day, and fish log
		/// </summary>
		private async Task<(bool shouldMooch, uint chainCastTargetFishId, uint chainMoochTargetFishId, TugType? missionRequiredTugType, string[] missionRequiredAchievementTags)> SelectAndApplyBait(bool spectraled, string location, string timeOfDay, ulong baitId, ulong spectralbaitId, RouteWithFish currentRoute)
		{
			// Force a fresh read (bait selection happens once per cast, not once per frame, so this
			// is cheap) rather than trusting whatever the last ambient RefreshIfNeeded() from the
			// previous bite-wait loop happened to catch — weather can change between casts.
			gameCache.Refresh();

			// Leveling mode bypasses NormalBaitSelector/SpectralBaitSelector entirely rather than
			// threading a "stay within these 3 baits" flag through their prereq-chain/points logic —
			// both selectors reference a dozen+ other bait types across their branches, and missing
			// even one there would leak a disallowed bait. Prefer the route's own curated bait when
			// it's already one of the 3 allowed (better bite rates for free); otherwise fall back to
			// Krill, matching the fallback FishingSessionManager already uses when route data is
			// missing entirely.
			if (OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Leveling)
			{
				uint routeBait = (uint)(spectraled ? spectralbaitId : baitId);
				uint levelingBait = FishingConstants.LEVELING_ALLOWED_BAITS.Contains(routeBait) ? routeBait : FishBait.Krill;

				await baitChanger.ChangeBait(levelingBait, $"Leveling mode — using {gameCache.GetItemName(levelingBait)}");
				return (false, 0, 0, null, null);
			}

			// Determine if target fish is available in this zone
			uint targetFishId = OceanTripNewSettings.Instance.TargetFishId;
			uint contextTargetFishId = 0;
			if (targetFishId != 0)
			{
				var targetFish = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == (int)targetFishId);
				if (targetFish != null && targetFish.RouteShortName == location)
					contextTargetFishId = targetFishId;
			}

			var context = new BaitSelectionContext
			{
				Location = location,
				TimeOfDay = timeOfDay,
				DefaultBaitId = spectraled ? spectralbaitId : baitId,
				CurrentRoute = currentRoute,
				MissingFish = FishingLog.MissingFish(),
				CaughtFish = caughtFish,
				FocusFishLog = FocusFishLog,
				CurrentWeather = gameCache.CurrentWeather,
				TargetFishId = contextTargetFishId,
				SpectralPityActive = _spectralPityActive,
				IsLastStop = Endeavor.CurrentZone >= 2,
				SpectralAlreadyTriggeredThisStop = _hadSpectralThisStop
			};

			// Use achievement bait selector when in achievement mode AND achievement fish exist here
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Achievements)
			{
				var focus = AchievementFishDataCache.GetCurrentAchievementFocus();
				var achievementFish = AchievementFishDataCache.GetFishForLocation(location, focus);
				if (achievementFish != null && achievementFish.Any())
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
			else if (spectraled)
			{
				await spectralBaitSelector.SelectBait(context);
			}
			else
			{
				await normalBaitSelector.SelectBait(context);
			}

			return (context.ShouldMooch, context.ChainCastTargetFishId, context.ChainMoochTargetFishId, GetActiveMissionTugType(), GetActiveMissionAchievementTags());
		}

		/// <summary>
		/// Bite-strength missions ("Catch fish with a weak/strong/ferocious bite") can be actively
		/// accelerated with Double/Triple Hook — checks all three mission slots and returns the tug
		/// type of the first one that's both a bite-strength mission and not yet complete.
		/// </summary>
		private TugType? GetActiveMissionTugType()
		{
			return CheckMissionSlotTugType(Endeavor.Mission1Type, Endeavor.Mission1Progress)
				?? CheckMissionSlotTugType(Endeavor.Mission2Type, Endeavor.Mission2Progress)
				?? CheckMissionSlotTugType(Endeavor.Mission3Type, Endeavor.Mission3Progress);
		}

		private TugType? CheckMissionSlotTugType(uint missionType, ushort progress)
		{
			if (missionType == 0)
				return null;

			var condition = MissionDataCache.GetById(missionType);
			if (condition == null || progress >= condition.Count)
				return null; // unknown mission, or already complete — nothing left to target

			return MissionDataCache.GetRequiredTugType(condition.Text);
		}

		/// <summary>
		/// Category missions ("Catch sharks", "Catch fugu") — same idea as GetActiveMissionTugType,
		/// but for the mission archetype matched by Fish.Achievement tag instead of tug type.
		/// </summary>
		private string[] GetActiveMissionAchievementTags()
		{
			return CheckMissionSlotAchievementTags(Endeavor.Mission1Type, Endeavor.Mission1Progress)
				?? CheckMissionSlotAchievementTags(Endeavor.Mission2Type, Endeavor.Mission2Progress)
				?? CheckMissionSlotAchievementTags(Endeavor.Mission3Type, Endeavor.Mission3Progress);
		}

		private string[] CheckMissionSlotAchievementTags(uint missionType, ushort progress)
		{
			if (missionType == 0)
				return null;

			var condition = MissionDataCache.GetById(missionType);
			if (condition == null || progress >= condition.Count)
				return null; // unknown mission, or already complete — nothing left to target

			return MissionDataCache.GetRequiredAchievementTags(condition.Text);
		}

		private static readonly HashSet<int> NeverExchangeFish = new HashSet<int>
		{
			OceanFish.JuniorJinbei
		};

		private List<int> GetExchangeableFishIds()
		{
			uint targetFish = OceanTripNewSettings.Instance.TargetFishId;
			return FishDataCache.GetFish()
				.Where(x => x.Rarity != "Rare" && !NeverExchangeFish.Contains(x.FishID) && x.FishID != (int)targetFish)
				.Select(x => x.FishID)
				.ToList();
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
