using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
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
					|| ((location == "unnamed") && (timeOfDay == "Sunset"))
					|| ((location == "thavnair") && (timeOfDay == "Night"))
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
					context.ChainCastTargetFishId = (uint)OceanFish.Heavenskey;
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.Heavenskey)}");
				}
				else if (!caughtFish.Contains(OceanFish.NavigatorsPrint)) // Requires 1 Navigators Print.
				{
					context.ChainCastTargetFishId = (uint)OceanFish.NavigatorsPrint;
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.NavigatorsPrint)}");
				}
			}
			else if ((location == "south") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.CoralManta) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.GreatGrandmarlin) < 2) // Needs 2 Great Grandmarlin. Mooch from Hi-Aetherlouse.
				{
					context.ShouldMooch = true;
					context.ChainCastTargetFishId = (uint)OceanFish.HiAetherlouse;
					context.ChainMoochTargetFishId = (uint)OceanFish.GreatGrandmarlin;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.GreatGrandmarlin)} via mooching {_gameCache.GetItemName((uint)OceanFish.HiAetherlouse)}.");
				}
			}
			else if ((location == "north") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Elasmosaurus) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Gugrusaurus) < 3) // Needs 3 Gugrusaurus
				{
					context.ChainCastTargetFishId = (uint)OceanFish.Gugrusaurus;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.Gugrusaurus)}");
				}
			}
			else if ((location == "rhotano") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Stonescale) && focusFishLog)
			{
				// Stonescale's real Intuition prereq is Deep-sea Eel (per IntuitionPrereqs in
				// fishList.json), not Crimson Monkfish — that's actually Sabaton's prereq (a
				// separate, normal/non-spectral fish already handled generically by
				// NormalBaitSelector's own IntuitionPrereqs walk). Walk the prereq list the same
				// way NormalBaitSelector does, so this stays correct if the data ever changes.
				var stonescale = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == OceanFish.Stonescale);
				var prereq = stonescale?.IntuitionPrereqs?.FirstOrDefault(p => !p.IsMooch && caughtFish.Count(x => x == (uint)p.FishID) < p.Count);
				if (prereq != null)
				{
					var prereqFish = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == prereq.FishID);
					if (prereqFish != null)
					{
						context.ChainCastTargetFishId = (uint)prereq.FishID;
						var caught = caughtFish.Count(x => x == (uint)prereq.FishID);
						await _baitChanger.ChangeBait(prereqFish.FavoriteBait, $"Switching bait to {_gameCache.GetItemName(prereqFish.FavoriteBait)} in order to catch {caught}/{prereq.Count}x {prereqFish.FishName} (Intuition prereq for {stonescale.FishName})");
					}
				}
			}
			else if ((location == "ciel") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Hafgufa) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.JetborneManta) < 2) // Needs 2 Jetborne Manta
				{
					context.ChainCastTargetFishId = (uint)OceanFish.JetborneManta;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.JetborneManta)}");
				}
				else if (!caughtFish.Contains(OceanFish.MistbeardsCup)) // Needs 1 Mistbeard's Cup
				{
					context.ChainCastTargetFishId = (uint)OceanFish.MistbeardsCup;
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.MistbeardsCup)}");
				}
			}
			else if ((location == "blood") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.SeafaringToad) && focusFishLog)
			{
				// This will help increase the chances of catching Seafaring Toad.
				await _patienceManager.UsePatience();

				// Catch 3 Beatific Vision to trigger intuition
				context.ChainCastTargetFishId = (uint)OceanFish.BeatificVision;
				await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.BeatificVision)}");
			}
			else if ((location == "sound") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Placodus) && focusFishLog)
			{
				await _patienceManager.UsePatience();

				// Use Ragworm to catch Rothlyt Mussel, then Mooch to Trollfish to trigger intuition.
				context.ShouldMooch = true;
				context.ChainCastTargetFishId = (uint)OceanFish.RothlytMussel;
				context.ChainMoochTargetFishId = (uint)OceanFish.Trollfish;
				await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.RothlytMussel)} to mooch into {_gameCache.GetItemName((uint)OceanFish.Trollfish)}");
			}
			else if ((location == "sirensong") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Taniwha) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.SunkenCoelacanth) < 3) // Needs 3 Sunken Coelacanth
				{
					context.ChainCastTargetFishId = (uint)OceanFish.SunkenCoelacanth;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.SunkenCoelacanth)}");
				}
			}
			else if ((location == "kugane") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.GlassDragon) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Shoshitsuki) < 2) // Needs Shoshitsuki
				{
					context.ChainCastTargetFishId = (uint)OceanFish.Shoshitsuki;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.Shoshitsuki)}");
				}
				else
				{
					context.ShouldMooch = true;
					context.ChainCastTargetFishId = (uint)OceanFish.SnappingKoban;
					context.ChainMoochTargetFishId = (uint)OceanFish.GlassDragon;
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.SnappingKoban)} to mooch into {_gameCache.GetItemName((uint)OceanFish.GlassDragon)}");
				}
			}
			else if ((location == "rubysea") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.HellsClaw) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.FlyingSquid) < 1) // Needs 1x Flying Squid
				{
					context.ChainCastTargetFishId = (uint)OceanFish.FlyingSquid;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.FlyingSquid)}");
				}
				else if (caughtFish.Count(x => x == OceanFish.FleetingSquid) < 2) // Needs 2x Fleeting Squid
				{
					context.ChainCastTargetFishId = (uint)OceanFish.FleetingSquid;
					await _baitChanger.ChangeBait(FishBait.PlumpWorm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.PlumpWorm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.FleetingSquid)}");
				}
			}
			else if ((location == "oneriver") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.JewelofPlumSpring) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.YanxianGoby) < 2) // Needs 2x Yanxian Goby
				{
					context.ChainCastTargetFishId = (uint)OceanFish.YanxianGoby;
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 2x {_gameCache.GetItemName((uint)OceanFish.YanxianGoby)}");
				}
				else if (caughtFish.Count(x => x == OceanFish.GensuiShrimp) < 1) // Needs 1x Gensui Shrimp
				{
					context.ChainCastTargetFishId = (uint)OceanFish.GensuiShrimp;
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 1x {_gameCache.GetItemName((uint)OceanFish.GensuiShrimp)}");
				}
			}
#if !RB_TC
			else if ((location == "unnamed") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Akupara) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.CieldalaesRoosterfish) < 2) // Needs 2x Cieldalaes Roosterfish (mooch from Captain's Pen)
				{
					context.ShouldMooch = true;
					context.ChainCastTargetFishId = (uint)OceanFish.CaptainsPen;
					context.ChainMoochTargetFishId = (uint)OceanFish.CieldalaesRoosterfish;
					await _baitChanger.ChangeBait(FishBait.Krill, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Krill)} in order to catch {_gameCache.GetItemName((uint)OceanFish.CaptainsPen)} to mooch into {_gameCache.GetItemName((uint)OceanFish.CieldalaesRoosterfish)}");
				}
			}
			else if ((location == "thavnair") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Manasvin) && focusFishLog)
			{
				if (caughtFish.Count(x => x == OceanFish.Satrapsaurus) < 3) // Needs 3x Satrapsaurus
				{
					context.ChainCastTargetFishId = (uint)OceanFish.Satrapsaurus;
					await _baitChanger.ChangeBait(FishBait.Ragworm, $"Switching bait to {_gameCache.GetItemName((uint)FishBait.Ragworm)} in order to catch 3x {_gameCache.GetItemName((uint)OceanFish.Satrapsaurus)}");
				}
			}
#endif
			else
			{
				var availableSpectralFish = currentRoute?.SpectralFish
					.Where(f => f.TimeOfDayExclusion1 != timeOfDay
						&& f.TimeOfDayExclusion2 != timeOfDay)
					.ToList() ?? new System.Collections.Generic.List<Fish>();

				uint spectralBait = 0;
				string baitReason = null;

				if (focusFishLog)
				{
					spectralBait = BaitRanker.SelectBaitForMissingFish(availableSpectralFish, missingFish);
					if (spectralBait != 0)
					{
						var targetedFish = availableSpectralFish
							.Where(f => missingFish.Contains((uint)f.FishID) && !f.RequiresIntuition && f.FavoriteBait == spectralBait)
							.Select(f => f.FishName);
						baitReason = $"Spectral — targeting missing fish: {string.Join(", ", targetedFish)}";
					}
				}

				if (spectralBait == 0)
				{
					spectralBait = BaitRanker.SelectBaitForPoints(availableSpectralFish);
					if (spectralBait != 0)
						baitReason = "Spectral — optimizing for points";
				}

				if (spectralBait == 0)
					spectralBait = (uint)spectralbaitId;

				await _baitChanger.ChangeBait(spectralBait, baitReason);
			}
		}
	}
}
