using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OceanTrip;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner
{
	public class Schedule
	{
		public string day { get; set; }
		public string time { get; set; }
		public string routeName { get; set; }
		public string routeTime { get; set; }
		public string objectives { get; set; }

		// Run highlights, computed in GetSchedules from the run's fish (see RunFishStats).
		public bool hasMissingFish { get; set; }
		public string missingFishNames { get; set; }
		public bool bestForPoints { get; set; }
		public int pointsScore { get; set; }

		public Schedule() { }

		public Schedule(string day, string time, string routeName, string routeTime, string objectives)
		{
			this.day = day;
			this.time = time;
			this.routeName = routeName;
			this.routeTime = routeTime;
			this.objectives = objectives;
		}

		public static List<Schedule> GetSchedules(int amount, string route=null)
		{
			// Error Checks
			if (amount <= 0 || amount >= 50)
				return null;
			// Passed Error Checks


			List<Schedule> schedules = new List<Schedule>();

			// Hoisted out of the per-run loop — both are cached lookups, but there's no reason to
			// re-fetch them 18 times. MissingFish can be null before the log has been read once.
			var missingFish = global::OceanTrip.FishingLog.MissingFish() ?? new HashSet<uint>();
			var allFish = FishDataCache.GetFish();

			var nextBoat = OceanTrip.TimeUntilNextBoat();

			// Row 0 of the raw walk is the most-recent boat (the -120 offset), which is only worth
			// showing while it's still boardable. Walk forward, skipping any run whose boarding window
			// has already closed, until we've collected `amount` upcoming runs. The +4 cap keeps a
			// pathological clock/timespan state from looping forever.
			for (int i = 0; schedules.Count < amount && i < amount + 4; i++)
			{
				DateTime time = DateTime.Now.AddMinutes((nextBoat.TotalMinutes - 120) + (i * 120));

				// Sometimes a mismatch can happen between when the timespan was captured and when the datetime is generated
				if (time.Minute == 59)
					time = time.AddMinutes(1);

				// Drop boats that have already sailed. Registration closes at :15 past a run's even
				// departure hour (LATE_QUEUE_END_MINUTE) — after that it can't be boarded, so it has no
				// place at the top of an "upcoming runs" list. e.g. by 11:00 the 10:00 boat is long gone
				// and the first row should be 12:00. Floor to the hour first so this holds regardless of
				// the early (:00) vs late (:13) queue-minute the time was generated at.
				DateTime boardingClosesAt = time.AddMinutes(-time.Minute).AddMinutes(FishingConstants.LATE_QUEUE_END_MINUTE);
				if (DateTime.Now > boardingClosesAt)
					continue;

				var schedule = Routes.GetSchedule(time, route);
				int posOnSchedule = 0;

				// Build the schedule!
				var entry = new Schedule();

				// First VISIBLE row (after any skips) carries the date, as do the day-rollover rows.
				if (schedules.Count == 0 || (time.ToString("hh:mm tt") == "12:00 AM" || time.ToString("hh:mm tt") == "01:00 AM"))
					entry.day = time.ToString("MM/dd");
				else
					entry.day = "";

				entry.time = time.ToString("hh:mm tt");
				entry.routeName = areaName(schedule[posOnSchedule + 2].Item1);
				entry.routeTime = schedule[posOnSchedule + 2].Item2;
				entry.objectives = scheduleObjectives(schedule);

				var (hasMissing, missingNames, score) = RunFishStats(schedule, missingFish, allFish);
				entry.hasMissingFish = hasMissing;
				entry.missingFishNames = missingNames;
				entry.pointsScore = score;

				schedules.Add(entry);
			}

			// "Best for points" is relative to the runs actually shown: flag every run within 10% of the
			// top score. When one route rotation has standout high-value (blue-fish) runs they alone light
			// up; when the shown runs are all comparable, they all qualify — which is itself accurate.
			if (schedules.Count > 0)
			{
				int maxScore = schedules.Max(s => s.pointsScore);
				int threshold = (int)(maxScore * 0.9);
				foreach (var s in schedules)
					s.bestForPoints = maxScore > 0 && s.pointsScore >= threshold;
			}

			return schedules;
		}


		/// <summary>
		/// Per-run fish highlights: whether the run can catch any still-uncaught Fish Log entry, the
		/// names of those fish (for the tooltip), and a points-potential score. Score = sum over the
		/// three stops of the single highest-value fish available at each — a cheap proxy that rewards
		/// runs stacked with blue/high-value fish. Weather isn't knowable this far ahead, so normal
		/// fish are all counted as possible; spectral fish are still time-of-day gated (that IS known).
		/// </summary>
		private static (bool hasMissing, string missingNames, int pointsScore) RunFishStats(
			Tuple<string, string>[] schedule, HashSet<uint> missingFish, List<Fish> allFish)
		{
			var missingNames = new List<string>();
			var missingSeen = new HashSet<uint>();
			int score = 0;

			for (int i = 0; i <= 2; i++)
			{
				string loc = schedule[i].Item1;
				string tod = schedule[i].Item2;

				var stopFish = allFish.Where(f => f.RouteShortName == loc &&
					(!f.SpectralFish || (f.TimeOfDayExclusion1 != tod && f.TimeOfDayExclusion2 != tod))).ToList();

				if (stopFish.Count > 0)
					score += stopFish.Max(f => f.Points);

				foreach (var f in stopFish)
				{
					if (missingFish.Contains((uint)f.FishID) && missingSeen.Add((uint)f.FishID))
						missingNames.Add(f.FishName);
				}
			}

			return (missingNames.Count > 0, string.Join(", ", missingNames), score);
		}

		public static string areaName(string shortname)
		{
			string name;

			switch (shortname)
			{
				case "south":
					name = "Southern Strait of Merlthor";
					break;
				case "galadion":
					name = "Galadion Bay";
					break;
				case "north":
					name = "Northern Strait of Merlthor";
					break;
				case "rhotano":
					name = "Rhotano Sea";
					break;
				case "ciel":
					name = "Cieldalaes";
					break;
				case "blood":
					name = "Bloodbrine Sea";
					break;
				case "sound":
					name = "Rothlyt Sound";
					break;
				case "sirensong":
					name = "Sirensong Sea";
					break;
				case "kugane":
					name = "Kugane";
					break;
				case "rubysea":
					name = "Ruby Sea";
					break;
				case "oneriver":
					name = "One River";
					break;
#if !RB_TC
				case "unnamed":
					name = "Unnamed Margin";
					break;
				case "thavnair":
					name = "Thavnairian Coast";
					break;
#endif
				default:
					name = shortname;
					break;
			}

			return name;
		}

		public static string scheduleObjectives(Tuple<string, string>[] schedule)
		{
			List<string> objectives = new List<string>();
			List<string> blueFish = new List<string>();

			// Checking for Indigo achievements
			int mantas = 0;
			int octopods = 0;
			int sharks = 0;
			int jellyfish = 0;
			int seadragons = 0;
			int balloons = 0;
			int crabs = 0;


			// Check for Shellfish
			if ((schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Night"
						&& schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Day"
						&& schedule[2].Item1 == "oneriver" && schedule[2].Item2 == "Sunset")
					|| (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Day"
						&& schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Sunset"
						&& schedule[2].Item1 == "oneriver" && schedule[2].Item2 == "Night"))
			{
				objectives.Add("Shellfish");
			}

			// Check for Squid
			if ((schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Day"
						&& schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Sunset"
						&& schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Night")
					|| (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Sunset"
						&& schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Night"
						&& schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Day")
					|| (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Night"
						&& schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Day"
						&& schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Sunset"))
			{
				objectives.Add("Squid");
			}

			if (schedule[0].Item1 == "sirensong" || schedule[0].Item1 == "kugane" || schedule[0].Item1 == "rubysea" || schedule[0].Item1 == "oneriver")
				objectives.Add("Shrimp");

#if !RB_TC
			if (schedule[0].Item1 == "unnamed")
			{
				if (schedule[2].Item2 == "Day")
					objectives.Add("Prehistoric");
				else
					objectives.Add("Mantis");
			}
#endif


			for (int i = 0; i <= 2; i++)
			{
				string area = schedule[i].Item1;
				string tod = schedule[i].Item2;

				switch (area)
				{
					case "south":
						if (tod == "Sunset")
							jellyfish++;
						if (tod == "Night")
						{
							blueFish.Add("Coral Manta");
							seadragons++;
						}
						break;
					case "galadion":
						if (tod == "Sunset")
						{
							octopods++;
							sharks++;
						}
						if (tod == "Night")
							blueFish.Add("Sothis");
						break;
					case "north":
						if (tod == "Day")
							blueFish.Add("Elasmosaurus");
						if (tod == "Sunset")
							seadragons++;
						if (tod == "Night")
						{
							crabs++;
							octopods++;
						}
						break;
					case "rhotano":
						if (tod == "Day")
							sharks++;
						if (tod == "Sunset")
						{
							blueFish.Add("Stonescale");
							balloons++;
						}
						if (tod == "Night")
						{
							jellyfish++;
							balloons++;
						}
						break;
					case "ciel":
						if (tod == "Day")
						{
							mantas++;
							balloons++;
						 }
						if (tod == "Sunset")
						{
							balloons++;
							crabs++;
							mantas++;
						}
						if (tod == "Night")
							blueFish.Add("Hafgufa");
						break;
					case "blood":
						if (tod == "Day")
						{
							blueFish.Add("Seafaring Toad");
							crabs++;
						}
						if (tod == "Night")
							mantas++;
						break;
					case "sound":
						if (tod == "Day")
						{
							balloons++;
							mantas++;
						}
						if (tod == "Sunset")
							blueFish.Add("Placodus");
						if (tod == "Night")
							balloons++;
						break;
					case "sirensong":
						if (tod == "Day")
							blueFish.Add("Taniwha");
						break;
					case "kugane":
						if (tod == "Night")
							blueFish.Add("Glass Dragon");
						break;
					case "rubysea":
						if (tod == "Sunset")
							blueFish.Add("Hells' Claw");
						break;
					case "oneriver":
						if (tod == "Day")
							blueFish.Add("Jewel of Plum Spring");
						break;
#if !RB_TC
					case "unnamed":
						if (schedule[2].Item2 == "Day")
							blueFish.Add("Akupara");
						break;
					case "thavnair":
						if (tod == "Night")
							blueFish.Add("Manasvin");
						break;
#endif
					default:
						break;
				}
			}

			if (blueFish.Count < 2)
			{
				if (mantas >= 2)
					objectives.Add("Mantas");
				if (octopods >= 2)
					objectives.Add("Octopods");
				if (sharks >= 2)
					objectives.Add("Sharks");
				if (jellyfish >= 2)
					objectives.Add("Jellyfish");
				if (seadragons >= 2)
					objectives.Add("Seadragons");
				if (balloons >= 2)
					objectives.Add("Balloons");
				if (crabs >= 2)
					objectives.Add("Crabs");
			}

			objectives.AddRange(blueFish);

			if (objectives.Count > 2 && objectives.Contains("Shrimp"))
				objectives.Remove("Shrimp");

			if (objectives.Count == 0)
				return "";
			else
				return string.Join(", ", objectives);
		}
	}
}
