using ff14bot.Enums;
using ff14bot.Helpers;
using Newtonsoft.Json;
using OceanTripPlanner;
using OceanTripPlanner.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ocean_Trip.Definitions
{
	/// <summary>
	/// One row of the game's IKDPlayerMissionCondition sheet — describes what a mission slot
	/// is asking for and how many are needed. Row IDs are what Endeavor.Mission1Type/2Type/3Type
	/// return from raw memory, so lookups here don't depend on the client's UI language.
	/// </summary>
	public class IkdMissionCondition
	{
		public uint Id { get; set; }
		public string Text { get; set; }
		public int Count { get; set; }
	}

	public static class MissionDataCache
	{
		private static List<IkdMissionCondition> _cachedConditions;

		public static List<IkdMissionCondition> GetConditions()
		{
			if (_cachedConditions == null)
			{
				_cachedConditions = LoadConditions();
			}
			return _cachedConditions;
		}

		public static IkdMissionCondition GetById(uint id)
		{
			return GetConditions().FirstOrDefault(c => c.Id == id);
		}

		public static void InvalidateCache()
		{
			_cachedConditions = null;
		}

		/// <summary>
		/// Bite-strength missions ("Catch fish with a weak/strong/ferocious bite") are the one
		/// mission archetype directly targetable via Double/Triple Hook — a single hookset catches
		/// several fish sharing the same tug, multiplying mission progress. Category ("Catch sharks")
		/// and star-rating ("Catch fish rated ★★★ or higher") missions aren't classified here.
		/// </summary>
		public static TugType? GetRequiredTugType(string missionText)
		{
			if (string.IsNullOrEmpty(missionText))
				return null;

			if (missionText.Contains("(!!!)"))
				return TugType.Heavy;
			if (missionText.Contains("(!!)"))
				return TugType.Medium;
			if (missionText.Contains("(!)"))
				return TugType.Light;

			return null;
		}

		// Category-mission keyword (as it appears in ikdMissionConditions.json's Text) -> the
		// Fish.Achievement tag(s) it corresponds to. The mission JSON has no fish-ID list for these,
		// so this mapping was built by cross-checking each Achievement tag's actual fish names
		// against the mission wording (e.g. the "Boxfish" tag's fish — Crow Puffer, Pearl Bombfish,
		// etc. — are clearly the "Catch fugu" mission; "Mussel" tag fish are the "Catch shellfish" ones).
		private static readonly Dictionary<string, string[]> CategoryAchievementTags = new Dictionary<string, string[]>
		{
			["sharks"] = new[] { "Shark" },
			["crabs"] = new[] { "Crab" },
			["jellyfish"] = new[] { "Jellyfish" },
			["fugu"] = new[] { "Boxfish" },
			["shellfish"] = new[] { "Mussel" },
			["shrimp"] = new[] { "Shrimp" },
			["squid"] = new[] { "Squid" },
			["mantis shrimp"] = new[] { "Mantis" },
			["prehistoric wavekin"] = new[] { "Prehistoric" },
		};

		/// <summary>
		/// Category missions ("Catch sharks", "Catch fugu") are targetable via Double/Triple Hook
		/// the same way bite-strength missions are — a multi-catch hookset nets several fish of the
		/// matching category at once, if the biting fish's own DHBonus/THBonus supports it. Returns
		/// null for bite-strength or star-rating missions (star-rating still isn't classified here —
		/// Fish has no star-rating field to match against).
		///
		/// Returns the UNION of every matching keyword, not just the first — several missions are
		/// compound ("Catch jellyfish or crabs", "Catch shrimp or squid"), and both halves need to
		/// count. Keywords are checked longest-first with overlap tracking so "shrimp" (a literal
		/// substring of "mantis shrimp") can't hijack a match that should resolve to "mantis shrimp"
		/// instead — confirmed against every mission string in ikdMissionConditions.json.
		/// </summary>
		public static string[] GetRequiredAchievementTags(string missionText)
		{
			if (string.IsNullOrEmpty(missionText))
				return null;

			var matched = new List<string>();
			var consumedRanges = new List<(int Start, int End)>();

			foreach (var kvp in CategoryAchievementTags.OrderByDescending(k => k.Key.Length))
			{
				int index = missionText.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase);
				if (index < 0)
					continue;

				int end = index + kvp.Key.Length;
				if (consumedRanges.Any(r => index < r.End && end > r.Start))
					continue; // fully/partially covered by a longer keyword already matched — e.g. "shrimp" inside "mantis shrimp"

				consumedRanges.Add((index, end));
				matched.AddRange(kvp.Value);
			}

			return matched.Count > 0 ? matched.ToArray() : null;
		}

		private static List<IkdMissionCondition> LoadConditions()
		{
			try
			{
				var filePath = BotBasesResourceLocator.Resolve("Resources", "ikdMissionConditions.json");
				if (filePath == null)
				{
					throw new FileNotFoundException("The IKD mission conditions file was not found.");
				}

				var json = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<List<IkdMissionCondition>>(json);
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Error loading IKD mission conditions: {ex.Message}");
				return new List<IkdMissionCondition>();
			}
		}
	}
}
