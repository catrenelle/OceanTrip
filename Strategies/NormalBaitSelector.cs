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
	/// Bait selection strategy for normal (non-spectral) fishing
	/// Handles Fisher's Intuition and favorite bait preferences
	/// </summary>
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
			// Cache missing fish set to avoid repeated property access
			var missingFish = context.MissingFish;
			var location = context.Location;
			var timeOfDay = context.TimeOfDay;
			var baitId = context.DefaultBaitId;
			var currentRoute = context.CurrentRoute;
			var focusFishLog = context.FocusFishLog;

			// Cache current weather to avoid repeated API calls in LINQ query
			string currentWeather = context.CurrentWeather;

			// Build Normal Fish List
			var normalFishToCatch = (currentRoute == null
				? new System.Collections.Generic.List<Fish>()
				: currentRoute.NormalFish.Where(x => missingFish.Contains((uint)x.FishID)
					&& x.TimeOfDayExclusion1 != timeOfDay
					&& x.TimeOfDayExclusion2 != timeOfDay
					&& x.WeatherExclusion1 != currentWeather
					&& x.WeatherExclusion2 != currentWeather)
				.ToList());

			if (OceanTripNewSettings.Instance.Patience == ShouldUsePatience.AlwaysUsePatience)
				await _patienceManager.UsePatience();

			// Deal with Intuition fish first... if we have the intution buff
			if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "galadion" || location == "rhotano" || location == "ciel" || location == "blood" || location == "rubysea"))
				await _baitChanger.ChangeBait(FishBait.Krill);
			else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && ((location == "south" && ((currentWeather != "Wind" && currentWeather != "Gales"))) || location == "sirensong" || location == "oneriver"))
				await _baitChanger.ChangeBait(FishBait.PlumpWorm);
			else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "sound" || location == "north" || location == "kugane"))
				await _baitChanger.ChangeBait(FishBait.Ragworm);

			// Deal with all the rest - prefer favorite bait for missing fish
			else if (focusFishLog && normalFishToCatch.Any(x => x.FavoriteBait == FishBait.Krill))
				await _baitChanger.ChangeBait(FishBait.Krill);
			else if (focusFishLog && normalFishToCatch.Any(x => x.FavoriteBait == FishBait.PlumpWorm))
				await _baitChanger.ChangeBait(FishBait.PlumpWorm);
			else if (focusFishLog && normalFishToCatch.Any(x => x.FavoriteBait == FishBait.Ragworm))
				await _baitChanger.ChangeBait(FishBait.Ragworm);
			// Location-based defaults
			else if (location == "galadion" || location == "rhotano" || location == "sound" || location == "oneriver" || location == "sirensong")
				await _baitChanger.ChangeBait(FishBait.PlumpWorm);
			else if (location == "south" || location == "blood")
				await _baitChanger.ChangeBait(FishBait.Krill);
			else if (location == "north" || location == "ciel" || location == "kugane" || location == "rubysea")
				await _baitChanger.ChangeBait(FishBait.Ragworm);
			else
				await _baitChanger.ChangeBait(baitId);

			// Should we use Chum?
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
	}
}
