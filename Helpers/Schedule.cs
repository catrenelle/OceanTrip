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

namespace OceanTripPlanner
{
	public class Schedule
	{
		public string day { get; set; }
		public string time { get; set; }
		public string routeName { get; set; }
		public string routeTime { get; set; }
		public string objectives { get; set; }

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

			for (int i = 0; i < amount; i++)
			{
				var nextBoat = OceanTrip.TimeUntilNextBoat();
				DateTime time = DateTime.Now.AddMinutes((nextBoat.TotalMinutes - 120) + (i * 120));

				// Sometimes a mismatch can happen between when the timespan was captured and when the datetime is generated
				if (time.Minute == 59)
					time = time.AddMinutes(1);

				var schedule = Routes.GetSchedule(time, route);
				int posOnSchedule = 0;

				// Build the schedule!
				var entry = new Schedule();
				
				if (i == 0 || (time.ToString("hh:mm tt") == "12:00 AM" || time.ToString("hh:mm tt") == "01:00 AM"))
					entry.day = time.ToString("MM/dd");
				else
					entry.day = "";

				entry.time = time.ToString("hh:mm tt");
				entry.routeName = areaName(schedule[posOnSchedule + 2].Item1);
				entry.routeTime = schedule[posOnSchedule + 2].Item2;
				entry.objectives = scheduleObjectives(schedule);

				schedules.Add(entry);
			}

			return schedules;
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
