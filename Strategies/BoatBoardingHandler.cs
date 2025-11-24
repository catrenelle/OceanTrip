using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Handles the boat boarding sequence for Ocean Fishing
	/// </summary>
	public class BoatBoardingHandler
	{
		private readonly bool _loggingEnabled;

		public BoatBoardingHandler(bool enableLogging = true)
		{
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Execute the full boat boarding sequence
		/// </summary>
		public async Task BoardBoat(BoardingContext context)
		{
			var Dryskthota = GameObjectManager.GetObjectByNPCId(NPC.Dryskthota);

			// Ensure Fisher job
			if (Core.Me.CurrentJob != ClassJobType.Fisher)
			{
				await context.SwitchToJobCallback(ClassJobType.Fisher);
				Log("[Ocean Trip] Switching to FSH class...");
			}

			// Only interact if not in party or if party leader (not cross-realm)
			if (!PartyManager.IsInParty || (PartyManager.IsInParty && PartyManager.IsPartyLeader && !PartyManager.CrossRealm))
			{
				if (Dryskthota != null && Dryskthota.IsWithinInteractRange)
				{
					Log($"Interacting with Dryskthota.", OceanLogLevel.Debug);

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

						if (OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Indigo)
							Log($"Selecting Indigo Route.", OceanLogLevel.Debug);

						if (OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Ruby)
							Log($"Selecting Ruby Route.", OceanLogLevel.Debug);

						// Select route (0 = Indigo, 1 = Ruby)
						SelectString.ClickSlot((uint)OceanTripNewSettings.Instance.FishingRoute);

						Log($"Waiting for Yes/No dialog to appear for boarding confirmation.", OceanLogLevel.Debug);

						await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
						SelectYesno.Yes();

						Log($"Boat confirmed. We're now in the duty finder.", OceanLogLevel.Debug);
					}
				}
			}

			Log($"Waiting for Duty Finder.", OceanLogLevel.Debug);

			await Coroutine.Wait(1000000, () => ContentsFinderConfirm.IsOpen);

			await Coroutine.Yield();
			while (ContentsFinderConfirm.IsOpen)
			{
				Log($"Commencing Duty.", OceanLogLevel.Debug);

				DutyManager.Commence();
				await Coroutine.Yield();

				Log($"Waiting for loading screen.", OceanLogLevel.Debug);

				if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}

				Log($"Loading screen found.", OceanLogLevel.Debug);
			}
			while (!context.IsOnBoatCallback())
			{
				await Coroutine.Sleep(1000);
			}
			await Coroutine.Sleep(2500);
			Log("We're on the boat!");
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
	/// Context for boat boarding containing callbacks to main bot methods
	/// </summary>
	public class BoardingContext
	{
		public Func<ClassJobType, Task> SwitchToJobCallback { get; set; }
		public Func<bool> IsOnBoatCallback { get; set; }
	}
}
