using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Ocean_Trip;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Bait selection strategy for spectral current fishing
	/// Handles blue fish prerequisites and Fisher's Intuition triggers
	/// </summary>
	public class SpectralBaitSelector : IBaitSelector
	{
		private readonly BaitChanger _baitChanger;
		private readonly PatienceManager _patienceManager;
		private readonly GameStateCache _gameCache;

		public SpectralBaitSelector(BaitChanger baitChanger, PatienceManager patienceManager, GameStateCache gameCache)
		{
			_baitChanger = baitChanger;
			_patienceManager = patienceManager;
			_gameCache = gameCache;
		}

		public async Task SelectBait(BaitSelectionContext context)
		{
			// Cache missing fish set to avoid repeated property access (called 17+ times in this method)
			var missingFish = context.MissingFish;
			var location = context.Location;
			var timeOfDay = context.TimeOfDay;
			var spectralbaitId = context.DefaultBaitId;
			var currentRoute = context.CurrentRoute;
			var caughtFish = context.CaughtFish;
			var focusFishLog = context.FocusFishLog;

			// Build Spectral Fish List
			var spectralFishToCatch = (currentRoute == null
				? new System.Collections.Generic.List<Fish>()
				: currentRoute.SpectralFish.Where(x => missingFish.Contains((uint)x.FishID)
					&& x.TimeOfDayExclusion1 != timeOfDay
					&& x.TimeOfDayExclusion2 != timeOfDay).ToList());

			// Check if we need to use Patience
			if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience
				|| OceanTripNewSettings.Instance.Patience == ShouldUsePatience.SpectralOnly)
			{
				await _patienceManager.UsePatience();
			}

			// Bait for Blue fish with Fisher's Intuition
			if (Core.Player.HasAura(CharacterAuras.FishersIntuition) &&
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
				))
			{
				caughtFish.Clear();
				await _baitChanger.ChangeBait(spectralbaitId, $"Switching bait to {_gameCache.GetItemName((uint)spectralbaitId)} to catch a blue fish now that we have intuition.");
			}
			// Blue fish prerequisite handling
			else if ((location == "galadion") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Sothis) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Heavenskey) < 2) // Needs 2 Heavenskey. Use Ragworm to catch.
				{
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.Heavenskey)}");
				}
				else if (!caughtFish.Contains(OceanFish.NavigatorsPrint)) // Requires 1 Navigators Print.
				{
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.NavigatorsPrint)}");
				}
			}
			else if ((location == "south") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.CoralManta) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.GreatGrandmarlin) < 2) // Needs 2 Great Grandmarlin. Mooch from Hi-Aetherlouse.
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.GreatGrandmarlin)} via mooching {_gameCache.GetItemName((uint)OceanFish.HiAetherlouse)}.");
				}
			}
			else if ((location == "north") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Elasmosaurus) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Gugrusaurus) < 3) // Needs 3 Gugrusaurus
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.Gugrusaurus)}");
				}
			}
			else if ((location == "rhotano") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Stonescale) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.CrimsonMonkfish) < 2) // Needs 2 Crimson Monkfish
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.CrimsonMonkfish)}");
				}
			}
			else if ((location == "ciel") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Hafgufa) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.JetborneManta) < 2) // Needs 2 Jetborne Manta
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.JetborneManta)}");
				}
				else if (!caughtFish.Contains(OceanFish.MistbeardsCup)) // Needs 1 Mistbeard's Cup
				{
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.MistbeardsCup)}");
				}
			}
			else if ((location == "blood") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.SeafaringToad) && focusFishLog)
			{
				// This will help increase the chances of catching Seafaring Toad.
				await _patienceManager.UsePatience();

				// Catch 3 Beatific Vision to trigger intuition
				await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.BeatificVision)}");
			}
			else if ((location == "sound") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Placodus) && focusFishLog)
			{
				await _patienceManager.UsePatience();

				// Use Ragworm to catch Rothlyt Mussel, then Mooch to Trollfish to trigger intuition.
				await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.RothlytMussel)} to mooch into {_gameCache.GetItemName((uint)OceanFish.Trollfish)}");
			}
			else if ((location == "sirensong") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Taniwha) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.SunkenCoelacanth) < 3) // Needs 3 Sunken Coelacanth
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.SunkenCoelacanth)}");
				}
			}
			else if ((location == "kugane") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.GlassDragon) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Shoshitsuki) < 2) // Needs Shoshitsuki
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.Shoshitsuki)}");
				}
				else
				{
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.SnappingKoban)} to mooch into {_gameCache.GetItemName((uint)OceanFish.GlassDragon)}");
				}
			}
			else if ((location == "rubysea") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.HellsClaw) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.FlyingSquid) < 1) // Needs 1x Flying Squid
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.FlyingSquid)}");
				}
				else if (caughtFish.Count(x => x == OceanFish.FleetingSquid) < 2) // Needs 2x Fleeting Squid
				{
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.FleetingSquid)}");
				}
			}
			else if ((location == "oneriver") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.JewelofPlumSpring) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.YanxianGoby) < 2) // Needs 2x Yanxian Goby
				{
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.YanxianGoby)}");
				}
				else if (caughtFish.Count(x => x == OceanFish.GensuiShrimp) < 1) // Needs 1x Gensui Shrimp
				{
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.GensuiShrimp)}");
				}
			}
			// Fallback to favorite bait for spectral fish
			else if (focusFishLog && spectralFishToCatch.Any(x => x.FavoriteBait == FishBait.PlumpWorm) && PassTheTime.inventoryCount((int)FishBait.PlumpWorm) > 0)
			{
				await _baitChanger.ChangeBait(FishBait.PlumpWorm);
			}
			else if (focusFishLog && spectralFishToCatch.Any(x => x.FavoriteBait == FishBait.Krill) && PassTheTime.inventoryCount((int)FishBait.Krill) > 0)
			{
				await _baitChanger.ChangeBait(FishBait.Krill);
			}
			else if (focusFishLog && spectralFishToCatch.Any(x => x.FavoriteBait == FishBait.Ragworm) && PassTheTime.inventoryCount((int)FishBait.Ragworm) > 0)
			{
				await _baitChanger.ChangeBait(FishBait.Ragworm);
			}
			// Location/time based default spectral bait
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
				await _baitChanger.ChangeBait(FishBait.PlumpWorm);
			}
			else if (
				((location == "south") && (timeOfDay == "Day"))
				|| ((location == "rhotano") && (timeOfDay == "Night")
					&& (!focusFishLog || !missingFish.Contains((uint)OceanFish.Slipsnail)))
				|| ((location == "north") && (timeOfDay == "Sunset")
					&& missingFish.Contains((uint)OceanFish.TheFallenOne)
					&& focusFishLog)
				|| ((location == "north") && (timeOfDay == "Night")
					&& (!focusFishLog
					|| !missingFish.Contains((uint)OceanFish.BartholomewTheChopper)))
				|| ((location == "galadion") && (timeOfDay == "Night"))
				|| ((location == "ciel") && (timeOfDay == "Day" || timeOfDay == "Night"))
				|| ((location == "blood") && (timeOfDay == "Night"))
				|| ((location == "sound"))
				|| ((location == "kugane"))
				|| ((location == "sirensong"))
				|| ((location == "oneriver"))
			)
			{
				await _baitChanger.ChangeBait(FishBait.Krill);
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
				await _baitChanger.ChangeBait(FishBait.Ragworm);
			}
			else
			{
				await _baitChanger.ChangeBait(spectralbaitId);
			}
		}
	}
}
