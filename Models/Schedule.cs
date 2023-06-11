using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OceanTrip;

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

        public static List<Schedule> GetSchedules(int amount)
        {
            // Error Checks
            if (amount <= 0 || amount >= 50)
                return null;
            // Passed Error Checks


            List<Schedule> schedules = new List<Schedule>();

            for (int i = 0; i < amount; i++)
            {
                var nextBoat = OceanTrip.TimeUntilNextBoat();
                DateTime time = DateTime.Now.AddMinutes(nextBoat.TotalMinutes + (i * 120));

                // Sometimes a mismatch can happen between when the timespan was captured and when the datetime is generated
                if (time.Minute == 59)
                    time = time.AddMinutes(1);

                var schedule = OceanTrip.GetSchedule(time);
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

            //// Check for Shellfish
            //if ((schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Night"
            //            && schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Day"
            //            && schedule[2].Item1 == "oneriver" && schedule[2].Item2 == "Sunset")
            //        || (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Day"
            //            && schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Sunset"
            //            && schedule[2].Item1 == "oneriver" && schedule[2].Item2 == "Night"))
            //{
            //    objectives.Add("Shellfish");
            //}

            //// Check for Squid
            //if ((schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Day"
            //            && schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Sunset"
            //            && schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Night")
            //        || (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Sunset"
            //            && schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Night"
            //            && schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Day")
            //        || (schedule[0].Item1 == "sirensong" && schedule[0].Item2 == "Night"
            //            && schedule[1].Item1 == "kugane" && schedule[1].Item2 == "Day"
            //            && schedule[2].Item1 == "rubysea" && schedule[2].Item2 == "Sunset"))
            //{
            //    objectives.Add("Squid");
            //}

            //if (schedule[0].Item1 == "sirensong" || schedule[0].Item1 == "kugane" || schedule[0].Item1 == "rubysea" || schedule[0].Item1 == "oneriver")
            //    objectives.Add("Shrimp");


            for (int i = 0; i <= 2; i++)
            {
                string area = schedule[i].Item1;
                string tod = schedule[i].Item2;

                switch (area)
                {
                    case "south":
                        if (tod == "Night")
                            objectives.Add("Coral Manta");
                        break;
                    case "galadion":
                        if (tod == "Night")
                            objectives.Add("Sothis");
                        break;
                    case "north":
                        if (tod == "Day")
                            objectives.Add("Elasmosaurus");
                        break;
                    case "rhotano":
                        if (tod == "Sunset")
                            objectives.Add("Stonescale");
                        break;
                    case "ciel":
                        if (tod == "Night")
                            objectives.Add("Hafgufa");
                        break;
                    case "blood":
                        if (tod == "Day")
                            objectives.Add("Seafaring Toad");
                        break;
                    case "sound":
                        if (tod == "Sunset")
                            objectives.Add("Placodus");
                        break;
                    case "sirensong":
                        if (tod == "Day")
                            objectives.Add("Taniwha");
                        break;
                    case "kugane":
                        if (tod == "Night")
                            objectives.Add("Glass Dragon");
                        break;
                    case "rubysea":
                        if (tod == "Sunset")
                            objectives.Add("Hells' Claw");
                        break;
                    case "oneriver":
                        if (tod == "Day")
                            objectives.Add("Jewel of Plum Spring");
                        break;
                    default:
                        break;
                }
            }

            //if (objectives.Count > 2 && objectives.Contains("Shrimp"))
            //    objectives.Remove("Shrimp");

            if (objectives.Count == 0)
                return "";
            else
                return string.Join(", ", objectives);
        }
    }
}
