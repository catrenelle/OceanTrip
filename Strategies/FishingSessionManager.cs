using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using Ocean_Trip.Definitions;
using OceanTrip;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Manages a complete fishing session for one zone/stop on the ocean voyage
	/// </summary>
	public class FishingSessionManager
	{
		private readonly GameStateCache _gameCache;
		private readonly HookingStrategy _hookingStrategy;
		private readonly LureStrategy _lureStrategy;
		private readonly bool _loggingEnabled;

		// Self-owned bite clock. We deliberately do NOT use FishingManager.TimeSinceCast for bite
		// timing: RB resets it only inside FishingManager.Cast() (spell 289) -- never on Mooch,
		// Identical Cast, or the client's automatic re-cast during Spectral Current. In spectral the
		// base Cast isn't castable, so FishingManager.Cast() silently no-ops while the client keeps
		// re-casting, leaving TimeSinceCast to climb monotonically across the whole window and
		// misreport bite times. Instead we snapshot the time at every line-out (each cast / mooch /
		// Identical Cast) and, on the bite, take (UtcNow - _castStartUtc). The outer loop still runs a
		// cast cycle per catch during spectral, so we re-stamp every line-out and each bite is timed
		// from ITS OWN cast, to ~0.2-0.5s.
		private DateTime _castStartUtc;
		private bool _awaitingLineOut;

		// The line is out and settled, waiting for a bite (post-splashdown) -- as opposed to the cast
		// animation (PoleOut), reeling, or pole-ready. The FIRST of these observed after arming is the
		// real "bobber landed" moment we want to time a bite from.
		private static bool IsWaitingForBite(FishingState st) =>
			st == FishingState.Waitin || st == FishingState.NormalFishing || st == FishingState.LureFishing;

		// Arm the line-out latch. _castStartUtc is stamped now as a safe fallback (this cast/hook moment);
		// if a settled waiting state is then observed before the bite, LatchLineOut refines it to the true
		// splashdown. Armed by EVERY line-out event -- each explicit cast/mooch/Identical Cast AND every
		// hook -- so a seamless same-bait re-cast (Spectral Current) still re-bases and can never collapse
		// multiple catches into one giant cast, nor go stale if the settled state is never sampled.
		private void ArmLineOut()
		{
			_castStartUtc = DateTime.UtcNow;
			_awaitingLineOut = true;
		}

		// Refine the bite clock to the instant the line actually settles in the water. Idempotent per arm
		// (clears the latch), so it only moves the start forward once, at splashdown.
		private void LatchLineOut()
		{
			if (_awaitingLineOut && IsWaitingForBite(FishingManager.State))
			{
				_castStartUtc = DateTime.UtcNow;
				_awaitingLineOut = false;
				Log($"Line settled (state {FishingManager.State}); bite clock started.", OceanLogLevel.Debug);
			}
		}

		public FishingSessionManager(GameStateCache gameCache, HookingStrategy hookingStrategy, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_hookingStrategy = hookingStrategy;
			_lureStrategy = new LureStrategy(gameCache);
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Execute a complete fishing session for one zone
		/// </summary>
		public async Task ExecuteFishingSession(FishingSessionContext context)
		{
			bool spectraled = false;
			bool hookExecuted = false;

			// Fresh bite clock for this session
			ArmLineOut();

			// Handle food consumption at start of session
			await ConsumeFood(context);

			while (context.ShouldContinueFishingCallback())
			{
				LatchLineOut();

				// Fast path: If pole is ready, skip expensive overhead and jump straight to casting
				if (FishingManager.State == FishingState.PoleReady)
				{
					// Minimal cache refresh
					_gameCache.RefreshIfNeeded();
					// Don't update spectraled here - let ManageBuffsCallback detect the change
				}
				else
				{
					// Full preparation path for first cast or between zones
					_gameCache.RefreshIfNeeded();
					if (FishingManager.State == FishingState.None)
					{
						context.RefreshUICallback();
					}

					// Just in case you're already standing in a fishing spot. IE: Restarting botbase/rebornbuddy
					if (!ActionManager.CanCast(Actions.Cast, Core.Me) && FishingManager.State == FishingState.None)
					{
						await MoveToFishingSpot(context.Spot);
					}

					context.RefreshBaitCallback();
				}

				// Manage buffs and consumables (IMPORTANT: Always check, not just on first cast!)
				// This will detect and log spectral changes
				await context.ManageBuffsCallback(spectraled);

				// Update spectraled status after management (allows ManageBuffsCallback to detect changes)
				spectraled = (_gameCache.CurrentWeatherId == Weather.Spectral);

				if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
				{
					hookExecuted = false;
					_lureStrategy.ResetForNewCast();
					// Process caught fish and check for Identical Cast
					bool identicalCastUsed = await context.ProcessCaughtFishCallback();

					// Identical Cast re-casts the line inside ProcessCaughtFish (a raw DoAction that does
					// not reset TimeSinceCast); arm our own line-out latch so its bite is timed from here.
					if (identicalCastUsed)
						ArmLineOut();

					// If Identical Cast was used, skip mooch/bait selection (cast already started)
					if (!identicalCastUsed)
					{
						// Only mooch when the bait selector indicated a mooch chain is active
						Log("Checking for Mooch before moving into bait checks.", OceanLogLevel.Debug);
						if (context.GetShouldMooch() && (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both))
						{
							Log("Using Mooch!");
							context.RefreshMissionStateCallback?.Invoke();
							FishingManager.Mooch();
							ArmLineOut();
							context.SetLastCastMooch(true);
							context.SetShouldMooch(false);
						}
						else if (context.GetShouldMooch() && FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
						{
							Log("Using Mooch II!");
							context.RefreshMissionStateCallback?.Invoke();
							FishingManager.MoochTwo();
							ArmLineOut();
							context.SetLastCastMooch(true);
							context.SetShouldMooch(false);
						}
						else
						{
							// Select and apply bait for the current conditions.
							await context.SelectAndApplyBaitCallback(spectraled);

							// Prize Catch before Cast, not after a bite — it's a pre-commit buff
							// ("next catch is Large") that has to be up before the line goes out to
							// cover a blind Double/Triple Hook. Mooch bites don't go through this
							// branch, matching the guide's advice that spending it on a Mooch is
							// rarely worthwhile.
							await context.PrizeCatchCallback(spectraled);

							Log("Casting!", OceanLogLevel.Debug);

							FishingManager.Cast();
							ArmLineOut();
							context.SetLastCastMooch(false);
						}
					}

					await Coroutine.Yield();
				}

				while ((FishingManager.State != FishingState.PoleReady) && context.ShouldContinueFishingCallback())
				{

					// Refine the bite clock to the real splashdown (see ArmLineOut/LatchLineOut).
					LatchLineOut();

					// Refresh cache to detect spectral changes immediately during bite wait
					_gameCache.RefreshIfNeeded();

					//Spectral popped, don't wait for normal fish
					if (_gameCache.CurrentWeatherId == Weather.Spectral && !spectraled)
					{
						Log("Spectral popped!");
						spectraled = true;

						if (FishingManager.CanHook)
						{
							FishingManager.Hook();
							hookExecuted = true;
							context.OnHookExecutedCallback?.Invoke(false);
							ArmLineOut();

							// As on the main hook path, wait for the catch to resolve and log it here —
							// a spectral-pop catch may never return cleanly to PoleReady, so relying on
							// ProcessCaughtFish alone would drop it.
							await Coroutine.Wait(FishingConstants.POST_HOOK_SETTLE_TIMEOUT_MS,
								() => FishingManager.State == FishingState.PoleReady
									|| FishingManager.State == FishingState.None
									|| IsWaitingForBite(FishingManager.State));
							context.LogCaughtFishCallback?.Invoke();
						}
					}

					// Apply lure while line is in water, before a bite occurs
					if (FishingManager.State != FishingState.Bite && FishingManager.State != FishingState.PoleReady)
					{
						var lureContext = new LureContext
						{
							Location = context.Location,
							TimeOfDay = context.TimeOfDay,
							Spectraled = spectraled,
							LastCastMooch = context.GetLastCastMooch(),
							TargetFishId = OceanTripNewSettings.Instance.TargetFishId,
							MissingFish = FishingLog.MissingFish()
						};
						await _lureStrategy.TryApplyLure(lureContext);
					}

					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite && !hookExecuted)
					{
						hookExecuted = true;
						// Time this line has actually been in the water, measured from splashdown on our own clock
						// (see ArmLineOut/LatchLineOut) -- not FishingManager.TimeSinceCast, which never resets
						// on mooch / Identical Cast / the Spectral auto-recast and so accumulates across a window.
						double biteTime = Math.Max(0, (DateTime.UtcNow - _castStartUtc).TotalSeconds);
						Log($"Bite clock: {biteTime:F2}s (settled anchor)", OceanLogLevel.Debug);
						var hookContext = new HookContext
						{
							BiteElapsedSeconds = biteTime + FishingConstants.LANDED_BITE_OFFSET,
							Spectraled = spectraled,
							Location = context.Location,
							TimeOfDay = context.TimeOfDay,
							CurrentRoute = context.CurrentRoute,
							LastCastMooch = context.GetLastCastMooch(),
							ChainCastTargetFishId = context.GetChainCastTargetFishId(),
							ChainMoochTargetFishId = context.GetChainMoochTargetFishId(),
							MissionRequiredTugType = context.GetMissionRequiredTugType(),
							MissionRequiredAchievementTags = context.GetMissionRequiredAchievementTags()
						};
						hookContext.SetHookExecutedCallback(context.OnHookExecutedCallback);
						await _hookingStrategy.ExecuteHook(hookContext);
						context.SetLastCastMooch(false);
						ArmLineOut();

						// Force the state machine to advance past the catch before we loop back to re-cast:
						// returns the instant we reach PoleReady (or the line is already back out), so it costs
						// no more than the natural reel-in but stops a fast Spectral re-deploy from blurring the
						// reel/PoleReady transition past our poll tick (which hid the anchor and starved bait
						// changes of their tackle-box window). Predicate also accepts the line-out states so an
						// aliased-over PoleReady can't hang us until the timeout.
						bool settled = await Coroutine.Wait(FishingConstants.POST_HOOK_SETTLE_TIMEOUT_MS,
							() => FishingManager.State == FishingState.PoleReady
								|| FishingManager.State == FishingState.None
								|| IsWaitingForBite(FishingManager.State));
						Log($"Post-hook settle: {(settled ? "advanced" : "timed out")} at state {FishingManager.State}", OceanLogLevel.Debug);

						// Log this catch now, at its own resolution point. ProcessCaughtFish (the other
						// catch-logging site) only runs when the pole returns to PoleReady at the top of
						// the outer loop; in Spectral Current the line auto-redeploys and that often
						// doesn't happen between catches, so without this the mid-spectral catches (and
						// their caughtFish prereq entries) were dropped. Idempotent via the catch
						// signature, so the eventual PoleReady call won't double-log it.
						context.LogCaughtFishCallback?.Invoke();
					}

					await Coroutine.Yield();
				}
			}

			// Cleanup after session
			spectraled = false;
			await Coroutine.Yield();

			//Log("Waiting for next stop...");
			if (FishingManager.State != FishingState.None)
			{
				ActionManager.DoAction(Actions.Quit, Core.Me);
			}
		}

		/// <summary>
		/// Handle food consumption at start of fishing session
		/// </summary>
		private async Task ConsumeFood(FishingSessionContext context)
		{
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
						Log($"Eating {_gameCache.GetItemName(edibleFood, edibleFoodHQ)}...");

						foreach (BagSlot slot in InventoryManager.FilledSlots)
						{
							if (slot.RawItemId == (uint)edibleFood)
							{
								slot.UseItem();
							}
						}
						await Coroutine.Sleep(3000);

					} while (!Core.Player.Auras.Any(x => x.Id == CharacterAuras.WellFed));
					await Coroutine.Yield();
				}
				else
				{
					Log($"Out of {_gameCache.GetItemName(food, false)} to eat!");
				}
			}
		}

		/// <summary>
		/// Move to the designated fishing spot
		/// </summary>
		private async Task MoveToFishingSpot(int spot)
		{
			//Navigator.PlayerMover.MoveTowards(FishingConstants.FishSpots[spot]);
			while (FishingConstants.FishSpots[spot].Distance2DSqr(Core.Me.Location) > 2)
			{
				Navigator.PlayerMover.MoveTowards(FishingConstants.FishSpots[spot]);
				await Coroutine.Yield();
			}
			Navigator.PlayerMover.MoveStop();
			await Coroutine.Sleep(300);
			Core.Me.SetFacing(FishingConstants.Headings[spot]);

			await Coroutine.Yield();
		}

		/// <summary>
		/// Internal logging method
		/// </summary>
		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text);
			Logging.Write(Colors.Aqua, msg);
		}
	}

	/// <summary>
	/// Context for fishing session containing all necessary state and callbacks
	/// </summary>
	public class FishingSessionContext
	{
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public int Spot { get; set; }
		public RouteWithFish CurrentRoute { get; set; }
		public ulong BaitId { get; set; }
		public ulong SpectralBaitId { get; set; }

		// Callbacks to main bot methods
		public Func<bool> ShouldContinueFishingCallback { get; set; }
		public Action RefreshUICallback { get; set; }
		public Action RefreshBaitCallback { get; set; }
		public Func<bool, Task> ManageBuffsCallback { get; set; }
		public Func<Task<bool>> ProcessCaughtFishCallback { get; set; }
		public Func<bool, Task> SelectAndApplyBaitCallback { get; set; }
		public Func<bool, Task> PrizeCatchCallback { get; set; }
		public Action<bool> OnHookExecutedCallback { get; set; }

		/// <summary>
		/// Logs a newly landed fish right after a hook resolves. Called per hook from the fishing loop so
		/// Spectral-Current catches — where the pole rarely returns to PoleReady between catches, so
		/// ProcessCaughtFish isn't reached — still get logged and counted into the caughtFish prereq
		/// list. Idempotent via the catch signature, so it never double-logs with ProcessCaughtFish.
		/// </summary>
		public Func<bool> LogCaughtFishCallback { get; set; }

		/// <summary>
		/// Spends GP-recovery consumables (Cordial + Thaliak's Favor) per hook. Same fix as
		/// LogCaughtFishCallback: ManageBuffsCallback only runs once per cast cycle at the top of the
		/// outer loop, which a continuous Spectral Current starves, so the GP banked for the spectral
		/// burst was never spent. Self-gated on GP need, so calling it every hook is cheap when GP is fine.
		/// </summary>
		public Func<Task> ManageGpConsumablesCallback { get; set; }

		/// <summary>
		/// Re-reads active mission tug-type/achievement-tag requirements from Endeavor and stores
		/// them on this context. SelectAndApplyBaitCallback already does this as part of bait
		/// selection, but the mooch-continuation path (FishingSessionManager) skips that callback
		/// entirely, so it needs this lighter equivalent to avoid hooking against a stale mission
		/// snapshot from before the mooch's source cast.
		/// </summary>
		public Action RefreshMissionStateCallback { get; set; }

		// State management
		private bool _lastCastMooch;
		private bool _shouldMooch;
		private uint _chainCastTargetFishId;
		private uint _chainMoochTargetFishId;
		private TugType? _missionRequiredTugType;
		private string[] _missionRequiredAchievementTags;

		public bool GetLastCastMooch() => _lastCastMooch;
		public void SetLastCastMooch(bool value) => _lastCastMooch = value;
		public bool GetShouldMooch() => _shouldMooch;
		public void SetShouldMooch(bool value) => _shouldMooch = value;
		public uint GetChainCastTargetFishId() => _chainCastTargetFishId;
		public void SetChainCastTargetFishId(uint value) => _chainCastTargetFishId = value;
		public uint GetChainMoochTargetFishId() => _chainMoochTargetFishId;
		public void SetChainMoochTargetFishId(uint value) => _chainMoochTargetFishId = value;
		public TugType? GetMissionRequiredTugType() => _missionRequiredTugType;
		public void SetMissionRequiredTugType(TugType? value) => _missionRequiredTugType = value;
		public string[] GetMissionRequiredAchievementTags() => _missionRequiredAchievementTags;
		public void SetMissionRequiredAchievementTags(string[] value) => _missionRequiredAchievementTags = value;
	}
}
