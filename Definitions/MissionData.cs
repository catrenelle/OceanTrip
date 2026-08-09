using ff14bot.Enums;
using ff14bot.Helpers;
using Newtonsoft.Json;
using OceanTripPlanner;
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

		private static List<IkdMissionCondition> LoadConditions()
		{
			try
			{
				var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };
				string filePath = null;

				foreach (var dir in possibleDirectories)
				{
					var potentialPath = Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "ikdMissionConditions.json");
					if (File.Exists(potentialPath))
					{
						filePath = potentialPath;
						break;
					}
				}

				if (filePath == null || !File.Exists(filePath))
				{
					throw new FileNotFoundException("The IKD mission conditions file was not found.", filePath);
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
