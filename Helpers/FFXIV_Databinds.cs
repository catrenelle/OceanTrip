using ff14bot.Managers;
using OceanTripPlanner;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using ff14bot.Helpers;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using OceanTripPlanner.Definitions;
using ff14bot;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;
using System.Drawing;
using Ocean_Trip.Definitions;
using Ocean_Trip;
using LlamaLibrary.Properties;
using System.Windows;

namespace OceanTripPlanner
{
	public class FFXIV_Databinds : Ocean_Trip.Helpers.BindableBase
	{
		private static FFXIV_Databinds _databinds;

		public static FFXIV_Databinds Instance
		{
			get { return _databinds ?? (_databinds = new FFXIV_Databinds()); }
		}

		private string _ragwormCount;
		public string ragwormCount { get => _ragwormCount; set => SetProperty(ref _ragwormCount, value); }

		private string _krillCount;
		public string krillCount { get => _krillCount; set => SetProperty(ref _krillCount, value); }

		private string _plumpwormCount;
		public string plumpwormCount { get => _plumpwormCount; set => SetProperty(ref _plumpwormCount, value); }

		private string _rattailCount;
		public string rattailCount { get => _rattailCount; set => SetProperty(ref _rattailCount, value); }

		private string _glowwormCount;
		public string glowwormCount { get => _glowwormCount; set => SetProperty(ref _glowwormCount, value); }

		private string _heavysteeljigCount;
		public string heavysteeljigCount { get => _heavysteeljigCount; set => SetProperty(ref _heavysteeljigCount, value); }

		private string _shrimpcagefeederCount;
		public string shrimpcagefeederCount { get => _shrimpcagefeederCount; set => SetProperty(ref _shrimpcagefeederCount, value); }

		private string _pillbugCount;
		public string pillbugCount { get => _pillbugCount; set => SetProperty(ref _pillbugCount, value); }

		private string _squidstripCount;
		public string squidstripCount { get => _squidstripCount; set => SetProperty(ref _squidstripCount, value); }

		private string _mackerelstripCount;
		public string mackerelstripCount { get => _mackerelstripCount; set => SetProperty(ref _mackerelstripCount, value); }

		private string _stoneflynymphCount;
		public string stoneflynymphCount { get => _stoneflynymphCount; set => SetProperty(ref _stoneflynymphCount, value); }

		// Indigo
		private bool _achievementMantas;
		public bool achievementMantas { get => _achievementMantas; set => SetProperty(ref _achievementMantas, value); }
		private bool _achievementOctopods;
		public bool achievementOctopods { get => _achievementOctopods; set => SetProperty(ref _achievementOctopods, value); }
		private bool _achievementSharks;
		public bool achievementSharks { get => _achievementSharks; set => SetProperty(ref _achievementSharks, value); }
		private bool _achievementJellyfish;
		public bool achievementJellyfish { get => _achievementJellyfish; set => SetProperty(ref _achievementJellyfish, value); }
		private bool _achievementSeadragons;
		public bool achievementSeadragons { get => _achievementSeadragons; set => SetProperty(ref _achievementSeadragons, value); }
		private bool _achievementBalloons;
		public bool achievementBalloons { get => _achievementBalloons; set => SetProperty(ref _achievementBalloons, value); }
		private bool _achievementCrabs;
		public bool achievementCrabs { get => _achievementCrabs; set => SetProperty(ref _achievementCrabs, value); }


		private bool _achievement5kindigo;
		public bool achievement5kindigo { get => _achievement5kindigo; set => SetProperty(ref _achievement5kindigo, value); }
		private bool _achievement10kindigo;
		public bool achievement10kindigo { get => _achievement10kindigo; set => SetProperty(ref _achievement10kindigo, value); }
		private bool _achievement16kindigo;
		public bool achievement16kindigo { get => _achievement16kindigo; set => SetProperty(ref _achievement16kindigo, value); }
		private bool _achievement20kindigo;
		public bool achievement20kindigo { get => _achievement20kindigo; set => SetProperty(ref _achievement20kindigo, value); }


		// Ruby
		private bool _achievementShrimp;
		public bool achievementShrimp { get => _achievementShrimp; set => SetProperty(ref _achievementShrimp, value); }
		private bool _achievementShellfish;
		public bool achievementShellfish { get => _achievementShellfish; set => SetProperty(ref _achievementShellfish, value); }
		private bool _achievementSquid;
		public bool achievementSquid { get => _achievementSquid; set => SetProperty(ref _achievementSquid, value); }


		private bool _achievement5kruby;
		public bool achievement5kruby { get => _achievement5kruby; set => SetProperty(ref _achievement5kruby, value); }
		private bool _achievement10kruby;
		public bool achievement10kruby { get => _achievement10kruby; set => SetProperty(ref _achievement10kruby, value); }
		private bool _achievement16kruby;
		public bool achievement16kruby { get => _achievement16kruby; set => SetProperty(ref _achievement16kruby, value); }

		// Overall
		private bool _achievement100koverall;
		public bool achievement100koverall { get => _achievement100koverall; set => SetProperty(ref _achievement100koverall, value); }
		private bool _achievement500koverall;
		public bool achievement500koverall { get => _achievement500koverall; set => SetProperty(ref _achievement500koverall, value); }
		private bool _achievement1moverall;
		public bool achievement1moverall { get => _achievement1moverall; set => SetProperty(ref _achievement1moverall, value); }
		private bool _achievement3moverall;
		public bool achievement3moverall { get => _achievement3moverall; set => SetProperty(ref _achievement3moverall, value); }



		// Schedule Screen Feesh
		private string _r1title;
		public string r1title { get => _r1title; set => SetProperty(ref _r1title, value); }
		private Image _r1tod;
		public Image r1tod { get => _r1tod; set => SetProperty(ref _r1tod, value); }
		private Image _r1weather;
		public Image r1weather { get => _r1weather; set => SetProperty(ref _r1weather, value); }
		private Image _r1nbait;
		public Image r1nbait { get => _r1nbait; set => SetProperty(ref _r1nbait, value); }
		private Image _r1sbait;
		public Image r1sbait { get => _r1sbait; set => SetProperty(ref _r1sbait, value); }
		private Image _r1n1_icon;
		public Image r1n1_icon { get => _r1n1_icon; set => SetProperty(ref _r1n1_icon, value); }
		private string _r1n1_name;
		public string r1n1_name { get => _r1n1_name; set => SetProperty(ref _r1n1_name, value); }
		private Image _r1n2_icon;
		public Image r1n2_icon { get => _r1n2_icon; set => SetProperty(ref _r1n2_icon, value); }
		private string _r1n2_name;
		public string r1n2_name { get => _r1n2_name; set => SetProperty(ref _r1n2_name, value); }
		private Image _r1n3_icon;
		public Image r1n3_icon { get => _r1n3_icon; set => SetProperty(ref _r1n3_icon, value); }
		private string _r1n3_name;
		public string r1n3_name { get => _r1n3_name; set => SetProperty(ref _r1n3_name, value); }
		private Image _r1n4_icon;
		public Image r1n4_icon { get => _r1n4_icon; set => SetProperty(ref _r1n4_icon, value); }
		private string _r1n4_name;
		public string r1n4_name { get => _r1n4_name; set => SetProperty(ref _r1n4_name, value); }
		private Image _r1n5_icon;
		public Image r1n5_icon { get => _r1n5_icon; set => SetProperty(ref _r1n5_icon, value); }
		private string _r1n5_name;
		public string r1n5_name { get => _r1n5_name; set => SetProperty(ref _r1n5_name, value); }
		private Image _r1n6_icon;
		public Image r1n6_icon { get => _r1n6_icon; set => SetProperty(ref _r1n6_icon, value); }
		private string _r1n6_name;
		public string r1n6_name { get => _r1n6_name; set => SetProperty(ref _r1n6_name, value); }
		private Image _r1n7_icon;
		public Image r1n7_icon { get => _r1n7_icon; set => SetProperty(ref _r1n7_icon, value); }
		private string _r1n7_name;
		public string r1n7_name { get => _r1n7_name; set => SetProperty(ref _r1n7_name, value); }
		private Image _r1n8_icon;
		public Image r1n8_icon { get => _r1n8_icon; set => SetProperty(ref _r1n8_icon, value); }
		private string _r1n8_name;
		public string r1n8_name { get => _r1n8_name; set => SetProperty(ref _r1n8_name, value); }
		private Image _r1n9_icon;
		public Image r1n9_icon { get => _r1n9_icon; set => SetProperty(ref _r1n9_icon, value);   }
		private string _r1n9_name;
		public string r1n9_name { get => _r1n9_name; set => SetProperty(ref _r1n9_name, value); }
		private Image _r1n10_icon;
		public Image r1n10_icon { get => _r1n10_icon; set => SetProperty(ref _r1n10_icon, value); }
		private string _r1n10_name;
		public string r1n10_name { get => _r1n10_name; set => SetProperty(ref _r1n10_name, value); }
		private Image _r1s1_icon;
		public Image r1s1_icon { get => _r1s1_icon; set => SetProperty(ref _r1s1_icon, value); }
		private string _r1s1_name;
		public string r1s1_name { get => _r1s1_name; set => SetProperty(ref _r1s1_name, value); }
		private Image _r1s2_icon;
		public Image r1s2_icon { get => _r1s2_icon; set => SetProperty(ref _r1s2_icon, value); }
		private string _r1s2_name;
		public string r1s2_name { get => _r1s2_name; set => SetProperty(ref _r1s2_name, value); }
		private Image _r1s3_icon;
		public Image r1s3_icon { get => _r1s3_icon; set => SetProperty(ref _r1s3_icon, value); }
		private string _r1s3_name;
		public string r1s3_name { get => _r1s3_name; set => SetProperty(ref _r1s3_name, value); }
		private Image _r1s4_icon;
		public Image r1s4_icon { get => _r1s4_icon; set => SetProperty(ref _r1s4_icon, value); }
		private string _r1s4_name;
		public string r1s4_name { get => _r1s4_name; set => SetProperty(ref _r1s4_name, value); }
		private Image _r1s5_icon;
		public Image r1s5_icon { get => _r1s5_icon; set => SetProperty(ref _r1s5_icon, value); }
		private string _r1s5_name;
		public string r1s5_name { get => _r1s5_name; set => SetProperty(ref _r1s5_name, value); }
		private Image _r1s6_icon;
		public Image r1s6_icon { get => _r1s6_icon; set => SetProperty(ref _r1s6_icon, value); }
		private string _r1s6_name;
		public string r1s6_name { get => _r1s6_name; set => SetProperty(ref _r1s6_name, value); }
		private Image _r1s7_icon;
		public Image r1s7_icon { get => _r1s7_icon; set => SetProperty(ref _r1s7_icon, value); }
		private string _r1s7_name;
		public string r1s7_name { get => _r1s7_name; set => SetProperty(ref _r1s7_name, value); }
		private Image _r1s8_icon;
		public Image r1s8_icon { get => _r1s8_icon; set => SetProperty(ref _r1s8_icon, value); }
		private string _r1s8_name;
		public string r1s8_name { get => _r1s8_name; set => SetProperty(ref _r1s8_name, value); }
		private Image _r1s9_icon;
		public Image r1s9_icon { get => _r1s9_icon; set => SetProperty(ref _r1s9_icon, value); }
		private string _r1s9_name;
		public string r1s9_name { get => _r1s9_name; set => SetProperty(ref _r1s9_name, value); }
		private Image _r1s10_icon;
		public Image r1s10_icon { get => _r1s10_icon; set => SetProperty(ref _r1s10_icon, value); }
		private string _r1s10_name;
		public string r1s10_name { get => _r1s10_name; set => SetProperty(ref _r1s10_name, value); }



		public void RefreshBait()
		{
			var baitMappings = new Dictionary<string, uint>
			{
				{ nameof(ragwormCount), FishBait.Ragworm },
				{ nameof(krillCount), FishBait.Krill },
				{ nameof(plumpwormCount), FishBait.PlumpWorm },
				{ nameof(rattailCount), FishBait.RatTail },
				{ nameof(glowwormCount), FishBait.GlowWorm },
				{ nameof(heavysteeljigCount), FishBait.HeavySteelJig },
				{ nameof(shrimpcagefeederCount), FishBait.ShrimpCageFeeder },
				{ nameof(pillbugCount), FishBait.PillBug },
				{ nameof(squidstripCount), FishBait.SquidStrip },
				{ nameof(mackerelstripCount), FishBait.MackerelStrip },
				{ nameof(stoneflynymphCount), FishBait.StoneflyNymph }
			};

			foreach (var mapping in baitMappings)
			{
				var property = this.GetType().GetProperty(mapping.Key);
				if (property != null)
				{
					var itemCount = DataManager.GetItem((uint)mapping.Value, false).ItemCount().ToString();
					property.SetValue(this, itemCount);
				}
			}
		}

		public void RefreshAchievements()
		{
			var achievementMappings = new Dictionary<string, Func<bool>>
			{
				{ nameof(achievementMantas), () => Achievements.HasAchievement(Definitions.Achievement.Mantas) },
				{ nameof(achievementOctopods), () => Achievements.HasAchievement(Definitions.Achievement.Octopods) },
				{ nameof(achievementSharks), () => Achievements.HasAchievement(Definitions.Achievement.Sharks) },
				{ nameof(achievementJellyfish), () => Achievements.HasAchievement(Definitions.Achievement.Jellyfish) },
				{ nameof(achievementSeadragons), () => Achievements.HasAchievement(Definitions.Achievement.Seadragons) },
				{ nameof(achievementBalloons), () => Achievements.HasAchievement(Definitions.Achievement.Balloons) },
				{ nameof(achievementCrabs), () => Achievements.HasAchievement(Definitions.Achievement.Crabs) },
				{ nameof(achievement5kindigo), () => Achievements.HasAchievement(Definitions.Achievement.Indigo5kPoints) },
				{ nameof(achievement10kindigo), () => Achievements.HasAchievement(Definitions.Achievement.Indigo10kPoints) },
				{ nameof(achievement16kindigo), () => Achievements.HasAchievement(Definitions.Achievement.Indigo16kPoints) },
				{ nameof(achievement20kindigo), () => Achievements.HasAchievement(Definitions.Achievement.Indigo20kPoints) },
				{ nameof(achievementShrimp), () => Achievements.HasAchievement(Definitions.Achievement.Shrimp) },
				{ nameof(achievementShellfish), () => Achievements.HasAchievement(Definitions.Achievement.Shellfish) },
				{ nameof(achievementSquid), () => Achievements.HasAchievement(Definitions.Achievement.Squid) },
				{ nameof(achievement5kruby), () => Achievements.HasAchievement(Definitions.Achievement.Ruby5kPoints) },
				{ nameof(achievement10kruby), () => Achievements.HasAchievement(Definitions.Achievement.Ruby10kPoints) },
				{ nameof(achievement16kruby), () => Achievements.HasAchievement(Definitions.Achievement.Ruby16kPoints) },
				{ nameof(achievement100koverall), () => Achievements.HasAchievement(Definitions.Achievement.Overall100kPoints) },
				{ nameof(achievement500koverall), () => Achievements.HasAchievement(Definitions.Achievement.Overall500kPoints) },
				{ nameof(achievement1moverall), () => Achievements.HasAchievement(Definitions.Achievement.Overall1mPoints) },
				{ nameof(achievement3moverall), () => Achievements.HasAchievement(Definitions.Achievement.Overall3mPoints) }
			};

			foreach (var mapping in achievementMappings)
			{
				var property = this.GetType().GetProperty(mapping.Key);
				if (property != null)
				{
					property.SetValue(this, mapping.Value());
				}
			}
		}

		public void RefreshCurrentRoute(int routeId = 0)
		{
			if (routeId < 0 || routeId > 2)
				routeId = 0;

			var schedule = Routes.GetSchedule();
			var routeWithFish = RouteDataCache.GetRoutesWithFish().FirstOrDefault(x => x.Route.RouteShortName == schedule[routeId].Item1);

			if (routeWithFish == null)
				return;

			r1title = routeWithFish.Route.RouteName + " - " + schedule[0].Item2;
			/*
			r1tod = TimeOfDayImage(schedule[0].Item2);

			for (int i = 0; i < 10; i++)
			{
				// Update normal fish details
				if (i < routeWithFish.NormalFish.Count)
				{
					var normalFish = routeWithFish.NormalFish[i];
					UpdateFishDetails(normalFish, out var normalName, out var normalIcon);
					this.GetType().GetProperty($"r1n{i + 1}_name").SetValue(this, normalName);
					this.GetType().GetProperty($"r1n{i + 1}_icon").SetValue(this, normalIcon);
				}

				// Update spectral fish details
				if (i < routeWithFish.SpectralFish.Count)
				{
					var spectralFish = routeWithFish.SpectralFish[i];
					UpdateFishDetails(spectralFish, out var spectralName, out var spectralIcon);
					this.GetType().GetProperty($"r1s{i + 1}_name").SetValue(this, spectralName);
					this.GetType().GetProperty($"r1s{i + 1}_icon").SetValue(this, spectralIcon);
				}
			}
			*/
		}

		private void UpdateFishDetails(Fish fish, out string name, out Image icon)
		{
			name = DataManager.GetItem((uint)fish.FishID).CurrentLocaleName;
			icon = UIElements.getIconImage(fish.IconX, fish.IconY);
		}

		private Image TimeOfDayImage(string timeOfDay)
		{
			Image img;

			switch (timeOfDay) 
			{
				case "Day":
					img = Ocean_Trip.Properties.Resources.day;
					break;
				case "Sunset":
					img = Ocean_Trip.Properties.Resources.sunset;
					break;
				case "Night":
					img = Ocean_Trip.Properties.Resources.night;
					break;
				default:
					img = null;
					break;
			}

			return img;
		}
	}
}
