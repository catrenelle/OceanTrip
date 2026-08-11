using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTrip;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	public class NormalBaitSelector : IBaitSelector
	{
		private readonly BaitChanger _baitChanger;
		private readonly PatienceManager _patienceManager;
		private readonly GameStateCache _gameCache;

		public NormalBaitSelector(BaitChanger baitChanger, PatienceManager patienceManager, GameStateCache gameCache)
		{
			_baitChanger = baitChanger;
			_patienceManager = patienceManager;
			_gameCache = gameCache;
		}

		public async Task SelectBait(BaitSelectionContext context)
		{
			var missingFish = context.MissingFish;
			var currentRoute = context.CurrentRoute;
			var timeOfDay = context.TimeOfDay;
			var focusFishLog = context.FocusFishLog;
			var caughtFish = context.CaughtFish;
			string currentWeather = context.CurrentWeather;

			var availableNormalFish = currentRoute?.NormalFish
				.Where(f => f.TimeOfDayExclusion1 != timeOfDay
					&& f.TimeOfDayExclusion2 != timeOfDay
					&& f.WeatherExclusion1 != currentWeather
					&& f.WeatherExclusion2 != currentWeather)
				.ToList() ?? new List<Fish>();

			if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience)
				await _patienceManager.UsePatience();

			uint selectedBait = 0;
			string baitReason = null;

			// Step 0: Target fish override — use target fish's bait when in its zone
			if (context.TargetFishId != 0)
			{
				var targetFish = availableNormalFish.FirstOrDefault(f => f.FishID == (int)context.TargetFishId);
				if (targetFish != null)
				{
					selectedBait = targetFish.FavoriteBait;
					baitReason = $"Target fish mode — using {_gameCache.GetItemName(targetFish.FavoriteBait)} for {targetFish.FishName}";
				}
			}

			// Step 1: Intuition buff active — use highest-points fish bait (always the Intuition fish)
			if (selectedBait == 0 && Core.Player.HasAura(CharacterAuras.FishersIntuition))
			{
				var topFish = availableNormalFish.OrderByDescending(f => f.Points).FirstOrDefault();
				if (topFish != null)
				{
					selectedBait = topFish.FavoriteBait;
					baitReason = $"Fisher's Intuition active — targeting {topFish.FishName} ({topFish.Points} pts)";
				}
			}

			// Step 1.5: Goal needs spectral — use bait for the spectral trigger fish
			if (selectedBait == 0 && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
			{
				var spectralTrigger = availableNormalFish.FirstOrDefault(f => f.CausesSpectral);
				if (spectralTrigger != null)
				{
					string spectralReason = NeedsSpectral(context, missingFish);
					if (spectralReason != null)
					{
						selectedBait = spectralTrigger.FavoriteBait;
						baitReason = $"Popping spectral — {spectralReason}";
					}
				}
			}

			if (selectedBait == 0 && focusFishLog)
			{
				// Step 2: Chase missing Intuition fish prereqs
				var missingIntuitionFish = availableNormalFish
					.Where(f => f.RequiresIntuition && missingFish.Contains((uint)f.FishID))
					.OrderByDescending(f => f.Points)
					.FirstOrDefault();

				if (missingIntuitionFish?.IntuitionPrereqs != null)
				{
					foreach (var prereq in missingIntuitionFish.IntuitionPrereqs)
					{
						if (!prereq.IsMooch && caughtFish.Count(x => x == (uint)prereq.FishID) < prereq.Count)
						{
							var prereqFish = availableNormalFish.FirstOrDefault(f => f.FishID == prereq.FishID);
							if (prereqFish != null)
							{
								selectedBait = prereqFish.FavoriteBait;
								context.ChainCastTargetFishId = (uint)prereq.FishID;
								var caught = caughtFish.Count(x => x == (uint)prereq.FishID);
								baitReason = $"Targeting {caught}/{prereq.Count}x {prereqFish.FishName} (prereq for missing {missingIntuitionFish.FishName})";
								break;
							}
						}
					}

					// Mooch-type prereqs: the fish to mooch FROM isn't tracked in IntuitionPrereqs
					// at all (only the mooch target and count are), so these can't be resolved
					// generically. Only the chains confirmed against the community spreadsheet /
					// known game mechanics are hardcoded here.
					if (selectedBait == 0)
					{
						var moochPrereq = missingIntuitionFish.IntuitionPrereqs
							.FirstOrDefault(p => p.IsMooch && caughtFish.Count(x => x == (uint)p.FishID) < p.Count);

						if (moochPrereq != null)
						{
							uint moochSourceFishId = 0;
							if (moochPrereq.FishID == OceanFish.ElderDinichthys)
								moochSourceFishId = (uint)OceanFish.TossedDagger; // Shooting Star chain
							else if (moochPrereq.FishID == OceanFish.Gladius)
								moochSourceFishId = (uint)OceanFish.GhoulBarracuda; // Little Leviathan chain
							else if (moochPrereq.FishID == OceanFish.SilentShark)
								moochSourceFishId = (uint)OceanFish.LeopardPrawn; // Mizuhiki chain — repeats (needs 2x Silent Shark)

							if (moochSourceFishId != 0)
							{
								var sourceFish = availableNormalFish.FirstOrDefault(f => f.FishID == (int)moochSourceFishId);
								if (sourceFish != null)
								{
									context.ShouldMooch = true;
									selectedBait = sourceFish.FavoriteBait;
									context.ChainCastTargetFishId = moochSourceFishId;
									context.ChainMoochTargetFishId = (uint)moochPrereq.FishID;
									var moochTargetName = _gameCache.GetItemName((uint)moochPrereq.FishID);
									baitReason = $"Switching bait to {_gameCache.GetItemName(sourceFish.FavoriteBait)} in order to catch 1x {sourceFish.FishName} to mooch into {moochTargetName} (prereq for missing {missingIntuitionFish.FishName})";
								}
							}
						}
					}
				}

				// Step 3: Other missing fish, rarity-first
				if (selectedBait == 0)
				{
					selectedBait = BaitRanker.SelectBaitForMissingFish(availableNormalFish, missingFish);
					if (selectedBait != 0)
					{
						var targetedFish = availableNormalFish
							.Where(f => missingFish.Contains((uint)f.FishID) && !f.RequiresIntuition && f.FavoriteBait == selectedBait)
							.Select(f => f.FishName);
						baitReason = $"Targeting missing fish: {string.Join(", ", targetedFish)}";
					}
				}
			}

			// Step 4: Fallback — points optimization
			if (selectedBait == 0)
			{
				selectedBait = BaitRanker.SelectBaitForPoints(availableNormalFish);
				if (selectedBait != 0)
					baitReason = "Optimizing for points";
			}

			if (selectedBait == 0)
				selectedBait = (uint)context.DefaultBaitId;

			if (!await _baitChanger.ChangeBait(selectedBait, baitReason))
				context.ClearChainTargets();

			// Chum handling
			if (_gameCache.MaxGP >= FishingConstants.FULL_GP_BUFFER
				&& (_gameCache.GPDeficit <= FishingConstants.FULL_GP_BUFFER)
				&& OceanTripNewSettings.Instance.FullGPAction == FullGPAction.Chum)
			{
				if (ActionManager.CanCast(Actions.Chum, Core.Me))
				{
					_baitChanger.Log("Triggering Full GP Action to keep regen going - Chum!");
					ActionManager.DoAction(Actions.Chum, Core.Me);
				}
			}
		}

		/// <summary>
		/// Returns a reason string if spectral should be prioritized, null otherwise.
		/// Achievement mode spectral popping is handled in AchievementBaitSelector.
		/// </summary>
		private string NeedsSpectral(BaitSelectionContext context, HashSet<uint> missingFish)
		{
			var location = context.Location;
			var allFish = FishDataCache.GetFish();

			// Fish Log / Auto mode: check if missing fish at this zone are spectral
			if (context.FocusFishLog && missingFish.Count > 0)
			{
				var missingHere = allFish
					.Where(f => f.RouteShortName == location && missingFish.Contains((uint)f.FishID))
					.ToList();
				if (missingHere.Any())
				{
					int spectralMissing = missingHere.Count(f => f.SpectralFish);
					int normalMissing = missingHere.Count(f => !f.SpectralFish);
					if (spectralMissing > 0 && normalMissing == 0)
						return $"all {spectralMissing} missing fish here are spectral";
					if (spectralMissing > normalMissing)
						return $"most missing fish here are spectral ({spectralMissing} spectral vs {normalMissing} normal)";
				}
			}

			// Points/Auto mode: spectral fish are worth more points
			if (OceanTripNewSettings.Instance.FishPriority == FishPriority.Points || OceanTripNewSettings.Instance.FishPriority == FishPriority.Auto)
			{
				var spectralFish = allFish
					.Where(f => f.RouteShortName == location && f.SpectralFish)
					.ToList();
				var normalFish = allFish
					.Where(f => f.RouteShortName == location && !f.SpectralFish && !f.CausesSpectral)
					.ToList();
				if (spectralFish.Any() && normalFish.Any() && spectralFish.Average(f => f.Points) > normalFish.Average(f => f.Points))
					return "spectral fish are worth more points";
			}

			return null;
		}
	}
}
