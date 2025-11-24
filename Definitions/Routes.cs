using ff14bot.Helpers;
using LlamaLibrary.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using OceanTripPlanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ocean_Trip.Definitions
{
	public class Routes
	{
		public string RouteName { get; set; }
		public string RouteShortName { get; set; }
		public uint NormalBait { get; set; }
		public uint SpectralBait { get; set; }
		public List<RouteFish> normal { get; set; }
		public List<RouteFish> spectral { get; set; }

		// Indigo
		public static readonly string[] fullPattern = new[] { "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN" };

		// Ruby
		public static readonly int[] ruby_fullPattern = new[]
		{
			1,2,3,4,5,6,1,2,3,4,5,6,
			2,3,4,5,6,1,2,3,4,5,6,1,
			3,4,5,6,1,2,3,4,5,6,1,2,
			4,5,6,1,2,3,4,5,6,1,2,3,
			5,6,1,2,3,4,5,6,1,2,3,4,
			6,1,2,3,4,5,6,1,2,3,4,5
		};

		// Indigo
		public static readonly Tuple<string, string>[] NS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("north", "Sunset")
		};
		public static readonly Tuple<string, string>[] NN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("north", "Night")
		};
		public static readonly Tuple<string, string>[] ND = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("north", "Day")
		};
		public static readonly Tuple<string, string>[] RS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("rhotano", "Sunset")
		};
		public static readonly Tuple<string, string>[] RN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("rhotano", "Night")
		};
		public static readonly Tuple<string, string>[] RD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("rhotano", "Day")
		};
		public static readonly Tuple<string, string>[] BS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("north", "Day"),
			new Tuple<string, string>("blood", "Sunset")
		};
		public static readonly Tuple<string, string>[] BN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("north", "Sunset"),
			new Tuple<string, string>("blood", "Night")
		};
		public static readonly Tuple<string, string>[] BD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("north", "Night"),
			new Tuple<string, string>("blood", "Day")
		};
		public static readonly Tuple<string, string>[] TS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("rhotano", "Day"),
			new Tuple<string, string>("sound", "Sunset")
		};
		public static readonly Tuple<string, string>[] TN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("rhotano", "Sunset"),
			new Tuple<string, string>("sound", "Night")
		};
		public static readonly Tuple<string, string>[] TD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("rhotano", "Night"),
			new Tuple<string, string>("sound", "Day")
		};

		// Ruby
		public static readonly Tuple<string, string>[] Ruby_RS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Night"),
			new Tuple<string, string>("kugane", "Day"),
			new Tuple<string, string>("rubysea", "Sunset")
		};

		public static readonly Tuple<string, string>[] Ruby_RN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Day"),
			new Tuple<string, string>("kugane", "Sunset"),
			new Tuple<string, string>("rubysea", "Night")
		};

		public static readonly Tuple<string, string>[] Ruby_RD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Sunset"),
			new Tuple<string, string>("kugane", "Night"),
			new Tuple<string, string>("rubysea", "Day")
		};

		public static readonly Tuple<string, string>[] Ruby_OS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Night"),
			new Tuple<string, string>("kugane", "Day"),
			new Tuple<string, string>("oneriver", "Sunset")
		};

		public static readonly Tuple<string, string>[] Ruby_ON = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Day"),
			new Tuple<string, string>("kugane", "Sunset"),
			new Tuple<string, string>("oneriver", "Night")
		};

		public static readonly Tuple<string, string>[] Ruby_OD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("sirensong", "Sunset"),
			new Tuple<string, string>("kugane", "Night"),
			new Tuple<string, string>("oneriver", "Day")
		};

		public static Tuple<string, string>[] GetSchedule(DateTime? time = null, string route = null)
		{
			int epoch;

			if (!time.HasValue)
				epoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			else
				epoch = (int)(time.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;




			if ((route is null && OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Ruby) || (route == "Ruby"))
			{
				// Thanks to https://millhio.re/oceancalculator2.html which was more accurate than what I was using. Translated the JS over to C#.
				int twoHourChunk = ((epoch / 7200) + 40) % Routes.ruby_fullPattern.Length;

				if (twoHourChunk >= ruby_fullPattern.Length)
					twoHourChunk = twoHourChunk - ruby_fullPattern.Length + 4;

				switch (ruby_fullPattern[twoHourChunk])
				{
					case 1:
						return Ruby_OD;
					case 2:
						return Ruby_RD;
					case 3:
						return Ruby_OS;
					case 4:
						return Ruby_RS;
					case 5:
						return Ruby_ON;
					case 6:
						return Ruby_RN;
				}
			}
			else
			{
				int twoHourChunk = ((epoch / 7200) + 88) % fullPattern.Length;

				switch (fullPattern[twoHourChunk])
				{
					case "NS": //Northern Strait
						return NS;
					case "NN":
						return NN;
					case "ND":
						return ND;
					case "RS": //Rhotano
						return RS;
					case "RN":
						return RN;
					case "RD":
						return RD;
					case "BS": //Bloodbrine
						return BS;
					case "BN":
						return BN;
					case "BD":
						return BD;
					case "TS": //Rothlyte
						return TS;
					case "TN":
						return TN;
					case "TD":
						return TD;
				}
			}

			return null;
		}
	}

	public class RouteFish
	{
		public int Fish1 { get; set; }
		public int Fish2 { get; set; }
		public int Fish3 { get; set; }
		public int Fish4 { get; set; }
		public int Fish5 { get; set; }
		public int Fish6 { get; set; }
		public int Fish7 { get; set; }
		public int Fish8 { get; set; }
		public int Fish9 { get; set; }
		public int Fish10 { get; set; }
	}

	public class RouteWithFish
	{
		public Routes Route { get; set; }
		public List<Fish> NormalFish { get; set; }
		public List<Fish> SpectralFish { get; set; }
	}

	public static class RouteDataCache
	{
		private static List<RouteWithFish> _cachedRoutesWithFish;

		public static List<RouteWithFish> GetRoutesWithFish()
		{
			if (_cachedRoutesWithFish == null)
			{
				_cachedRoutesWithFish = LoadRoutesWithFishData();
			}
			return _cachedRoutesWithFish;
		}

		public static void InvalidateCache()
		{
			_cachedRoutesWithFish = null;
		}

		private static List<RouteWithFish> LoadRoutesWithFishData()
		{
			try
			{
				var possibleDirectories = new[] { "OceanTrip", "Ocean Trip", "Ocean-Trip" };
				string filePath = null;

				foreach (var dir in possibleDirectories)
				{
					var potentialPath = Path.Combine(Environment.CurrentDirectory, "BotBases", dir, "Resources", "fishingRoutes.json");
					if (File.Exists(potentialPath))
					{
						filePath = potentialPath;
						break;
					}
				}

				if (filePath == null || !File.Exists(filePath))
				{
					throw new FileNotFoundException("The routes file was not found.", filePath);
				}

				var json = File.ReadAllText(filePath);
				var routes = JsonConvert.DeserializeObject<List<Routes>>(json);

				var fishList = FishDataCache.GetFish();

				return routes.Select(route => new RouteWithFish
				{
					Route = route,
					NormalFish = GetFishForRouteSegment(route.normal[0], fishList),
					SpectralFish = GetFishForRouteSegment(route.spectral[0], fishList)
				}).ToList();
			}
			catch (Exception ex)
			{
				// Log the exception or handle it as needed
				Console.WriteLine($"An error occurred while getting the routes list: {ex.Message}");
				return new List<RouteWithFish>(); // Return an empty list in case of error
			}
		}

		private static List<Fish> GetFishForRouteSegment(dynamic routeSegment, List<Fish> fishList)
		{
			var fishIds = new List<int?>
			{
				routeSegment.Fish1, routeSegment.Fish2, routeSegment.Fish3,
				routeSegment.Fish4, routeSegment.Fish5, routeSegment.Fish6,
				routeSegment.Fish7, routeSegment.Fish8, routeSegment.Fish9,
				routeSegment.Fish10
			};

			return fishIds.Select(fishId => fishList.FirstOrDefault(fish => fish.FishID == fishId)).ToList();
		}
	}
}
