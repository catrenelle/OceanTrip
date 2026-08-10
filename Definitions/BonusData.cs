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
	/// One row of the game's IKDContentBonus sheet — a score-multiplier bonus objective shown on the
	/// voyage results screen. Row IDs are what IndividualResultStruct's Bonuses array holds (see
	/// Endeavor.VoyageResult), so lookups here don't depend on the client's UI language.
	/// </summary>
	public class IkdContentBonus
	{
		public int Id { get; set; }
		public string Objective { get; set; }
		public string Requirement { get; set; }

		/// <summary>100 + the bonus percentage — e.g. 120 means +20%. 100 (row 0) means no bonus.</summary>
		public int BonusMultiplier { get; set; }
	}

	public static class BonusDataCache
	{
		private static List<IkdContentBonus> _cachedBonuses;

		public static List<IkdContentBonus> GetBonuses()
		{
			if (_cachedBonuses == null)
				_cachedBonuses = LoadBonuses();
			return _cachedBonuses;
		}

		public static IkdContentBonus GetById(int id)
		{
			return GetBonuses().FirstOrDefault(b => b.Id == id);
		}

		private static List<IkdContentBonus> LoadBonuses()
		{
			try
			{
				var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };
				string filePath = null;

				foreach (var dir in possibleDirectories)
				{
					var potentialPath = Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "ikdContentBonus.json");
					if (File.Exists(potentialPath))
					{
						filePath = potentialPath;
						break;
					}
				}

				if (filePath == null || !File.Exists(filePath))
					throw new FileNotFoundException("The IKD content bonus file was not found.", filePath);

				var json = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<List<IkdContentBonus>>(json);
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Error loading IKD content bonus data: {ex.Message}");
				return new List<IkdContentBonus>();
			}
		}
	}
}
