using System;
using System.Collections.Generic;
using System.IO;
using ff14bot.Helpers;
using Newtonsoft.Json;

namespace Ocean_Trip
{
	/// <summary>
	/// Persists every completed voyage's result as a JSON array in the character settings folder —
	/// same JsonSettings.CharacterSettingsDirectory convention as OceanFishingOffsetSync's offset
	/// cache and FishingLog's missing-fish file. Written from OceanTrip.LogVoyageResult alongside
	/// its existing write-only CSV log; read back by the Result History page (and its nav button's
	/// visibility check). Captures every run ever recorded, not just what the page displays — the
	/// page itself decides how much of this to show.
	/// </summary>
	public static class VoyageHistoryStore
	{
		private static string HistoryPath =>
			Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripVoyageHistory.json");

		/// <summary>
		/// Cheap existence check for ShellWindow's nav-button visibility poll — the file is only
		/// created on the first Append and never deleted, so this is equivalent to "is there any
		/// history yet" without paying to parse the file on every tick.
		/// </summary>
		public static bool HasAny() => File.Exists(HistoryPath);

		public static List<VoyageHistoryEntry> LoadAll()
		{
			try
			{
				if (!File.Exists(HistoryPath))
					return new List<VoyageHistoryEntry>();

				return JsonConvert.DeserializeObject<List<VoyageHistoryEntry>>(File.ReadAllText(HistoryPath))
					?? new List<VoyageHistoryEntry>();
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Failed to load voyage history, showing no runs: {ex.Message}");
				return new List<VoyageHistoryEntry>();
			}
		}

		public static void Append(VoyageHistoryEntry entry)
		{
			try
			{
				var entries = LoadAll();
				entries.Add(entry);
				File.WriteAllText(HistoryPath, JsonConvert.SerializeObject(entries, Formatting.Indented));
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Failed to save voyage history: {ex.Message}");
			}
		}
	}

	public class VoyageHistoryEntry
	{
		public DateTime Timestamp { get; set; }
		public string Route { get; set; }
		public uint TotalPoints { get; set; }
		public int? Placement { get; set; }
		public int TrackedPlayerCount { get; set; }
		public int CaughtFish { get; set; }
		public uint ExperiencePoints { get; set; }
		public ushort Scrip1Amount { get; set; }
		public ushort Scrip2Amount { get; set; }
		public List<VoyageBonusEntry> Bonuses { get; set; } = new List<VoyageBonusEntry>();
	}

	/// <summary>One earned bonus, keeping its multiplier (not just the name) so Result History can
	/// show an aggregate bonus % without re-deriving it from BonusDataCache after the fact.</summary>
	public class VoyageBonusEntry
	{
		public string Name { get; set; }

		/// <summary>100 + the bonus percentage — matches IkdContentBonus.BonusMultiplier (120 = +20%).</summary>
		public int Multiplier { get; set; }
	}
}
