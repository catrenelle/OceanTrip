using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Handles Modest/Ambitious Lure application during the bite-wait loop.
	/// Lures are post-cast actions applied while the line is in the water.
	/// </summary>
	public class LureStrategy
	{
		private readonly GameStateCache _gameCache;
		private bool _lureAppliedThisCast;
		private int _lureStacksApplied;

		// Auto-mode resolution is cast-invariant (depends only on location/time/target/achievement
		// focus, none of which change mid-cast) but was being recomputed via FishDataCache LINQ
		// queries on every tick of the bite-wait loop. Cache it per-cast instead.
		private uint? _cachedAutoLureAction;
		private bool _cachedAutoModeSkip;

		public LureStrategy(GameStateCache gameCache)
		{
			_gameCache = gameCache;
		}

		/// <summary>
		/// Total lure stacks applied on the current cast
		/// </summary>
		public int LureStacksApplied => _lureStacksApplied;

		/// <summary>
		/// Whether any lure was applied on the current cast (for bite timer adjustment)
		/// </summary>
		public bool LureAppliedThisCast => _lureAppliedThisCast;

		/// <summary>
		/// Reset state for a new cast. Call this when a new cast begins.
		/// </summary>
		public void ResetForNewCast()
		{
			_lureAppliedThisCast = false;
			_lureStacksApplied = 0;
			_cachedAutoLureAction = null;
			_cachedAutoModeSkip = false;
		}

		/// <summary>
		/// Attempt to apply lure during the bite-wait loop. Called each tick after casting.
		/// Returns immediately if lures aren't applicable or already done.
		/// </summary>
		public async Task TryApplyLure(LureContext context)
		{
			int maxStacks = OceanTripNewSettings.Instance.LureMaxStacks;
			if (_lureStacksApplied >= maxStacks)
				return;

			if (!ShouldUseLure(context))
				return;

			uint lureAction = GetLureAction(context);
			if (lureAction == 0)
				return;

			// Wait for line to settle before first lure application
			if (_lureStacksApplied == 0)
			{
				double elapsed = FishingManager.TimeSinceCast.TotalMilliseconds;
				if (elapsed < FishingConstants.LURE_POST_CAST_DELAY_MS)
					return;
			}

			if (!ActionManager.CanCast(lureAction, Core.Me))
				return;

			// Check for proc — stop stacking immediately if we got the guarantee
			if (HasLureProc())
			{
				Log($"Lure proc detected — stopping at {_lureStacksApplied} stack(s)");
				_lureStacksApplied = maxStacks;
				return;
			}

			// Check GP budget for this stack
			int gpCost = FishingConstants.LURE_GP_COSTS[_lureStacksApplied];
			if (Core.Me.CurrentGP < (uint)gpCost)
				return;

			// Reserve GP for DH/TH if we'll want it after the bite
			if (!HasEnoughGPForLureAndHook(context, gpCost))
				return;

			string lureName = lureAction == Actions.ModestLure ? "Modest Lure" : "Ambitious Lure";
			Log($"Using {lureName} (stack {_lureStacksApplied + 1}/{maxStacks}, {gpCost} GP)");

			ActionManager.DoAction(lureAction, Core.Me);
			_lureStacksApplied++;
			_lureAppliedThisCast = true;

			await Coroutine.Sleep(500);
		}

		private bool ShouldUseLure(LureContext context)
		{
			var pref = OceanTripNewSettings.Instance.LurePreference;
			if (pref == LurePreference.None)
				return false;

			// Leveling mode never uses lures — it's meant to be the simplest possible loop, and
			// lures spend GP a leveling character often can't spare.
			if (OceanTripNewSettings.Instance.EffectiveFishPriority == FishPriority.Leveling)
				return false;

			// Never during spectral — lures group all fast-biting spectral fish together,
			// making bite-time identification impossible
			if (context.Spectraled)
				return false;

			// Never during Fisher's Intuition — could lose rare Intuition fish
			if (Core.Player.HasAura(CharacterAuras.FishersIntuition))
				return false;

			// Never during mooch (game-enforced, but be safe)
			if (context.LastCastMooch)
				return false;

			// In Auto mode, skip when target hookset is dominant in the pool
			if (pref == LurePreference.Auto)
			{
				ResolveAutoModeIfNeeded(context);
				if (_cachedAutoLureAction == 0)
					return false;

				if (_cachedAutoModeSkip)
					return false;
			}

			return true;
		}

		private uint GetLureAction(LureContext context)
		{
			var pref = OceanTripNewSettings.Instance.LurePreference;

			if (pref == LurePreference.Modest)
				return Actions.ModestLure;

			if (pref == LurePreference.Ambitious)
				return Actions.AmbitiousLure;

			if (pref == LurePreference.Auto)
			{
				ResolveAutoModeIfNeeded(context);
				return _cachedAutoLureAction ?? 0;
			}

			return 0;
		}

		/// <summary>
		/// Resolve and cache the Auto-mode lure action (and dominant-hookset skip) once per cast.
		/// All inputs are cast-invariant, so recomputing this every bite-wait tick was wasted work.
		/// </summary>
		private void ResolveAutoModeIfNeeded(LureContext context)
		{
			if (_cachedAutoLureAction.HasValue)
				return;

			uint action = ResolveLureForAutoMode(context);
			_cachedAutoLureAction = action;

			if (action != 0)
			{
				TugType targetTug = action == Actions.ModestLure ? TugType.Light : TugType.Medium;
				_cachedAutoModeSkip = IsTargetHooksetDominant(context, targetTug);
			}
		}

		private uint ResolveLureForAutoMode(LureContext context)
		{
			var fishPriority = OceanTripNewSettings.Instance.FishPriority;

			// Achievement mode: match the achievement category's hookset
			if (fishPriority == FishPriority.Achievements)
			{
				var focus = AchievementFishDataCache.GetCurrentAchievementFocus();
				if (focus == AchievementType.None)
					return 0;

				var achievementFish = FishDataCache.GetFish()
					.Where(f => f.RouteShortName == context.Location &&
						!f.SpectralFish &&
						!string.IsNullOrEmpty(f.Achievement) &&
						AchievementFishDataCache.MapAchievementString(f.Achievement) == focus)
					.ToList();

				if (achievementFish.Any())
				{
					int lightCount = achievementFish.Count(f => f.BiteType == TugType.Light);
					return lightCount > achievementFish.Count / 2 ? Actions.ModestLure : Actions.AmbitiousLure;
				}

				// Achievement fish at this location are spectral-only — use Ambitious
				// to help pop spectral (trigger fish are always heavy tug)
				var spectralAchievementFish = FishDataCache.GetFish()
					.Any(f => f.RouteShortName == context.Location &&
						f.SpectralFish &&
						!string.IsNullOrEmpty(f.Achievement) &&
						AchievementFishDataCache.MapAchievementString(f.Achievement) == focus);

				if (spectralAchievementFish)
					return Actions.AmbitiousLure;

				return 0;
			}

			// Target fish mode: match the target fish's bite type
			if (context.TargetFishId != 0)
			{
				var targetFish = FishDataCache.GetFish().FirstOrDefault(f => f.FishID == (int)context.TargetFishId);
				if (targetFish != null)
					return targetFish.BiteType == TugType.Light ? Actions.ModestLure : Actions.AmbitiousLure;
			}

			// Fish Log mode: match missing fish at this zone
			if (fishPriority == FishPriority.FishLog || fishPriority == FishPriority.Auto)
			{
				var missingAtZone = FishDataCache.GetFish()
					.Where(f => f.RouteShortName == context.Location &&
						!f.SpectralFish &&
						context.MissingFish != null &&
						context.MissingFish.Contains((uint)f.FishID))
					.ToList();

				if (missingAtZone.Count == 1)
					return missingAtZone[0].BiteType == TugType.Light ? Actions.ModestLure : Actions.AmbitiousLure;
			}

			// Points mode: skip — lure value is marginal without a clear target
			return 0;
		}

		private bool IsTargetHooksetDominant(LureContext context, TugType targetTug)
		{
			// Non-spectral fish (already filtered to via !f.SpectralFish) are weather-gated only —
			// Day/Night/Sunset only ever applies to spectral fish.
			var zoneFish = FishDataCache.GetFish()
				.Where(f => f.RouteShortName == context.Location &&
					!f.SpectralFish &&
					f.WeatherExclusion1 != _gameCache.CurrentWeather &&
					f.WeatherExclusion2 != _gameCache.CurrentWeather)
				.ToList();

			if (zoneFish.Count == 0)
				return false;

			int matchingCount = zoneFish.Count(f => f.BiteType == targetTug);
			return (float)matchingCount / zoneFish.Count >= FishingConstants.LURE_SKIP_DOMINANT_THRESHOLD;
		}

		private bool HasLureProc()
		{
			return Core.Player.HasAura(CharacterAuras.ModestLureProc) ||
				Core.Player.HasAura(CharacterAuras.AmbitiousLureProc);
		}

		private bool HasEnoughGPForLureAndHook(LureContext context, int lureCost)
		{
			int reserve = FishingManager.HasPatience ? 100 : 400;
			return (int)Core.Me.CurrentGP - lureCost >= reserve;
		}

		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			Logging.Write(Colors.Aqua, $"[Ocean Trip] {text}");
		}
	}

	/// <summary>
	/// Context for lure application — passed from FishingSessionManager each tick
	/// </summary>
	public class LureContext
	{
		public string Location { get; set; }
		public string TimeOfDay { get; set; }
		public bool Spectraled { get; set; }
		public bool LastCastMooch { get; set; }
		public uint TargetFishId { get; set; }
		public System.Collections.Generic.HashSet<uint> MissingFish { get; set; }
	}
}
