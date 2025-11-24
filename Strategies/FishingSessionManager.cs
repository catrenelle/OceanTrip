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
		private readonly bool _loggingEnabled;

		public FishingSessionManager(GameStateCache gameCache, HookingStrategy hookingStrategy, bool enableLogging = true)
		{
			_gameCache = gameCache;
			_hookingStrategy = hookingStrategy;
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Execute a complete fishing session for one zone
		/// </summary>
		public async Task ExecuteFishingSession(FishingSessionContext context)
		{
			bool spectraled = false;

			// Handle food consumption at start of session
			await ConsumeFood(context);

			while (context.ShouldContinueFishingCallback())
			{
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
					// Process caught fish and check for Identical Cast
					bool identicalCastUsed = await context.ProcessCaughtFishCallback();

					// If Identical Cast was used, skip mooch/bait selection (cast already started)
					if (!identicalCastUsed)
					{
						// Check for Mooch before using Mooch II
						Log("Checking for Mooch before moving into bait checks.", OceanLogLevel.Debug);
						if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both)
						{
							Log("Using Mooch!");
							FishingManager.Mooch();
							context.SetLastCastMooch(true);
							await context.WaitForCastLogCallback();
							context.SetStartedCast(DateTime.Now);
						}
						else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
						{
							Log("Using Mooch II!");
							FishingManager.MoochTwo();
							context.SetLastCastMooch(true);
							await context.WaitForCastLogCallback();
							context.SetStartedCast(DateTime.Now);
						}
						else
						{
							// Select and apply bait based on current conditions
							await context.SelectAndApplyBaitCallback(spectraled);

							Log("Casting and then waiting for chat message about cast!", OceanLogLevel.Debug);

							FishingManager.Cast();
							await context.WaitForCastLogCallback();
							context.SetStartedCast(DateTime.Now);
							context.SetLastCastMooch(false);
						}
					}

					await Coroutine.Yield();
				}

				while ((FishingManager.State != FishingState.PoleReady) && context.ShouldContinueFishingCallback())
				{
					// Refresh cache to detect spectral changes immediately during bite wait
					_gameCache.RefreshIfNeeded();

					//Spectral popped, don't wait for normal fish
					if (_gameCache.CurrentWeatherId == Weather.Spectral && !spectraled)
					{
						Log("Spectral popped!");
						spectraled = true;

						if (FishingManager.CanHook)
							FishingManager.Hook();
					}

					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
					{
						var hookContext = new HookContext
						{
							BiteElapsedSeconds = (DateTime.Now - context.GetStartedCast()).TotalSeconds + FishingConstants.BITE_TIME_VARIANCE,
							Spectraled = spectraled,
							Location = context.Location,
							TimeOfDay = context.TimeOfDay,
							CurrentRoute = context.CurrentRoute,
							LastCastMooch = context.GetLastCastMooch()
						};
						hookContext.SetHookExecutedCallback(context.OnHookExecutedCallback);
						await _hookingStrategy.ExecuteHook(hookContext);
						context.SetLastCastMooch(false);
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
		public Func<Task> WaitForCastLogCallback { get; set; }
		public Func<bool, Task> SelectAndApplyBaitCallback { get; set; }
		public Action<bool> OnHookExecutedCallback { get; set; }

		// State management
		private DateTime _startedCast;
		private bool _lastCastMooch;

		public DateTime GetStartedCast() => _startedCast;
		public void SetStartedCast(DateTime value) => _startedCast = value;
		public bool GetLastCastMooch() => _lastCastMooch;
		public void SetLastCastMooch(bool value) => _lastCastMooch = value;
	}
}
