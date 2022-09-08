using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Timers;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using GreyMagic;
using OceanTripPlanner.RemoteWindows;
using OceanTripPlanner.Helpers;
using OceanTripPlanner.Definitions;
using TreeSharp;

namespace OceanTripPlanner
{
	public class OceanTrip : BotBase
	{
		private Composite _root;

		public OceanTrip()
		{
		}

		private static readonly Vector3[] fishSpots =
		{
			new Vector3(-7.541584f, 6.74677f, -7.7191f),
			new Vector3(-7.419403f, 6.73973f, -2.7815f),
			new Vector3(7.538965f, 6.745806f, -10.44607f),
			new Vector3(7.178741f, 6.749996f, -4.165483f),
			new Vector3(7.313677f, 6.711103f, -8.10146f),
			new Vector3(7.53893f, 6.745699f, 1.881091f)
		};
		private static readonly float[] headings = new[] { 4.622331f, 4.684318f, 1.569952f, 1.509215f, 1.553197f, 1.576235f };


		private readonly int[] oceanFish = new[] {
            OceanFish.GaladionGoby,
            OceanFish.RosyBream,
            OceanFish.TripodFish,
            OceanFish.GhoulBarracuda,
            OceanFish.LeopardEel,
            OceanFish.Sunfly,
            OceanFish.RhotanoWahoo,
            OceanFish.RhotanoSardine,
            OceanFish.DeepPlaice,
            OceanFish.Floefish,
            OceanFish.Megasquid,
            OceanFish.OschonsStone,
            OceanFish.Jasperhead,
            OceanFish.TarnishedShark,
            OceanFish.MarineBomb,
            OceanFish.MomoraMora,
            OceanFish.CrimsonMonkfish,
            OceanFish.ChromeHammerhead,
            OceanFish.OgreEel,
            OceanFish.TossedDagger,
            OceanFish.ShaggySeadragon,
            OceanFish.NetCrawler,
            OceanFish.CyanOctopus,
            OceanFish.Heavenswimmer,
            OceanFish.MerlthorButterfly,
            OceanFish.Gladius,
            OceanFish.DarkNautilus,
            OceanFish.Lampfish,
            OceanFish.MerlthorLobster,
            OceanFish.ElderDinichthys,
            OceanFish.Drunkfish,
            OceanFish.LittleLeviathan,
            OceanFish.Sabaton,
            OceanFish.ShootingStar,
            OceanFish.MermansMane,
            OceanFish.Heavenskey,
            OceanFish.GhostShark,
            OceanFish.QuicksilverBlade,
            OceanFish.NavigatorsPrint,
            OceanFish.CasketOyster,
            OceanFish.Fishmonger,
            OceanFish.MythrilSovereign,
            OceanFish.NimbleDancer,
            OceanFish.SeaNettle,
            OceanFish.GreatGrandmarlin,
            OceanFish.ShipwrecksSail,
            OceanFish.CharlatanSurvivor,
            OceanFish.HiAetherlouse,
            OceanFish.AzeymasSleeve,
            OceanFish.AethericSeadragon,
            OceanFish.CoralSeadragon,
            OceanFish.Roguesaurus,
            OceanFish.Aronnax,
            OceanFish.Sweeper,
            OceanFish.Silencer,
            OceanFish.DeepSeaEel,
            OceanFish.Executioner,
            OceanFish.WildUrchin,
            OceanFish.TrueBarramundi,
            OceanFish.ProdigalSon,
            OceanFish.Slipsnail,
            OceanFish.Hammerclaw,
            OceanFish.Coccosteus,
            OceanFish.BartholomewTheChopper,
            OceanFish.Prowler,
            OceanFish.Mopbeard,
            OceanFish.FloatingSaucer,
            OceanFish.Gugrusaurus,
            OceanFish.FunnelShark,
            OceanFish.TheFallenOne,
            OceanFish.SpectralMegalodon,
            OceanFish.SpectralDiscus,
            OceanFish.SpectralSeaBo,
            OceanFish.SpectralBass,
            OceanFish.Sothis,
            OceanFish.CoralManta,
            OceanFish.Stonescale,
            OceanFish.Elasmosaurus,
            OceanFish.TortoiseshellCrab,
            OceanFish.LadysCameo,
            OceanFish.MetallicBoxfish,
            OceanFish.GoobbueRay,
            OceanFish.Watermoura,
            OceanFish.KingCobrafish,
            OceanFish.MamahiMahi,
            OceanFish.LavandinRemora,
            OceanFish.SpectralButterfly,
            OceanFish.CieldalaesGeode,
            OceanFish.TitanshellCrab,
            OceanFish.MythrilBoxfish,
            OceanFish.MistbeardsCup,
            OceanFish.AnomalocarisSaron,
            OceanFish.FlamingEel,
            OceanFish.JetborneManta,
            OceanFish.DevilsSting,
            OceanFish.Callichthyid,
            OceanFish.MeanderingMora,
            OceanFish.Hafgufa,
            OceanFish.ThaliakCrab,
            OceanFish.StarOfTheDestroyer,
            OceanFish.TrueScad,
            OceanFish.BloodedWrasse,
            OceanFish.BloodpolishCrab,
            OceanFish.BlueStitcher,
            OceanFish.BloodfreshTuna,
            OceanFish.SunkenMask,
            OceanFish.SpectralEel,
            OceanFish.Bareface,
            OceanFish.OracularCrab,
            OceanFish.DravanianBream,
            OceanFish.Skaldminni,
            OceanFish.SerratedClam,
            OceanFish.BeatificVision,
            OceanFish.Exterminator,
            OceanFish.GoryTuna,
            OceanFish.Ticinepomis,
            OceanFish.QuartzHammerhead,
            OceanFish.SeafaringToad,
            OceanFish.CrowPuffer,
            OceanFish.StripOfRothlytKelp,
            OceanFish.LivingLantern,
            OceanFish.HoneycombFish,
            OceanFish.Godsbed,
            OceanFish.Lansquenet,
            OceanFish.ThavnairianShark,
            OceanFish.NephriteEel,
            OceanFish.Spectresaur,
            OceanFish.GinkgoFin,
            OceanFish.GarumJug,
            OceanFish.SmoothJaguar,
            OceanFish.RothlytMussel,
            OceanFish.LeviElver,
            OceanFish.PearlBombfish,
            OceanFish.Trollfish,
            OceanFish.Panoptes,
            OceanFish.CrepeSole,
            OceanFish.Knifejaw,
            OceanFish.Placodus
        };

		private static readonly int[] fishForSale = new int[]
		{
            OceanFish.GaladionGoby,
            OceanFish.GaladionChovy,
            OceanFish.RosyBream,
            OceanFish.TripodFish,
            OceanFish.Sunfly,
            OceanFish.TarnishedShark,
            OceanFish.TossedDagger,
            OceanFish.Jasperhead,
            OceanFish.MerlthorLobster,
            OceanFish.Heavenswimmer,
            OceanFish.GhoulBarracuda,
            OceanFish.LeopardEel,
            OceanFish.MarineBomb,
            OceanFish.MomoraMora,
            OceanFish.MerlthorButterfly,
            OceanFish.Gladius,
            OceanFish.RhotanoWahoo,
            OceanFish.RhotanoSardine,
            OceanFish.DeepPlaice,
            OceanFish.CrimsonMonkfish,
            OceanFish.Lampfish,
            OceanFish.OgreEel,
            OceanFish.CyanOctopus,
            OceanFish.ChromeHammerhead,
            OceanFish.Floefish,
            OceanFish.Megasquid,
            OceanFish.OschonsStone,
            OceanFish.LaNosceanJelly,
            OceanFish.ShaggySeadragon,
            OceanFish.NetCrawler,
            OceanFish.DarkNautilus,
            OceanFish.ElderDinichthys,
            OceanFish.Drunkfish,
            OceanFish.LittleLeviathan,
            OceanFish.Sabaton,
            OceanFish.ShootingStar,
            OceanFish.Hammerclaw,
            OceanFish.Heavenskey,
            OceanFish.GhostShark,
            OceanFish.QuicksilverBlade,
            OceanFish.NavigatorsPrint,
            OceanFish.CasketOyster,
            OceanFish.Fishmonger,
            OceanFish.MythrilSovereign,
            OceanFish.NimbleDancer,
            OceanFish.SeaNettle,
            OceanFish.GreatGrandmarlin,
            OceanFish.ShipwrecksSail,
            OceanFish.AzeymasSleeve,
            OceanFish.HiAetherlouse,
            OceanFish.FloatingSaucer,
            OceanFish.AethericSeadragon,
            OceanFish.CoralSeadragon,
            OceanFish.Roguesaurus,
            OceanFish.MermansMane,
            OceanFish.Sweeper,
            OceanFish.Silencer,
            OceanFish.DeepSeaEel,
            OceanFish.Executioner,
            OceanFish.WildUrchin,
            OceanFish.TrueBarramundi,
            OceanFish.Mopbeard,
            OceanFish.Slipsnail,
            OceanFish.Aronnax,
            OceanFish.Coccosteus,
            OceanFish.BartholomewTheChopper,
            OceanFish.Prowler,
            OceanFish.CharlatanSurvivor,
            OceanFish.ProdigalSon,
            OceanFish.Gugrusaurus,
            OceanFish.FunnelShark,
            OceanFish.SpectralMegalodon,
            OceanFish.SpectralDiscus,
            OceanFish.SpectralSeaBo,
            OceanFish.SpectralBass,
            OceanFish.TortoiseshellCrab,
            OceanFish.LadysCameo,
            OceanFish.MetallicBoxfish,
            OceanFish.GoobbueRay,
            OceanFish.Watermoura,
            OceanFish.KingCobrafish,
            OceanFish.MamahiMahi,
            OceanFish.LavandinRemora,
            OceanFish.SpectralButterfly,
            OceanFish.CieldalaesGeode,
            OceanFish.TitanshellCrab,
            OceanFish.MythrilBoxfish,
            OceanFish.MistbeardsCup,
            OceanFish.AnomalocarisSaron,
            OceanFish.FlamingEel,
            OceanFish.JetborneManta,
            OceanFish.DevilsSting,
            OceanFish.Callichthyid,
            OceanFish.MeanderingMora,
            OceanFish.ThaliakCrab,
            OceanFish.StarOfTheDestroyer,
            OceanFish.TrueScad,
            OceanFish.BloodedWrasse,
            OceanFish.BloodpolishCrab,
            OceanFish.BlueStitcher,
            OceanFish.BloodfreshTuna,
            OceanFish.SunkenMask,
            OceanFish.SpectralEel,
            OceanFish.Bareface,
            OceanFish.OracularCrab,
            OceanFish.DravanianBream,
            OceanFish.Skaldminni,
            OceanFish.SerratedClam,
            OceanFish.BeatificVision,
            OceanFish.Exterminator,
            OceanFish.GoryTuna,
            OceanFish.Ticinepomis,
            OceanFish.QuartzHammerhead,
            OceanFish.CrowPuffer,
            OceanFish.StripOfRothlytKelp,
            OceanFish.LivingLantern,
            OceanFish.HoneycombFish,
            OceanFish.Godsbed,
            OceanFish.Lansquenet,
            OceanFish.ThavnairianShark,
            OceanFish.NephriteEel,
            OceanFish.Spectresaur,
            OceanFish.GinkgoFin,
            OceanFish.GarumJug,
            OceanFish.SmoothJaguar,
            OceanFish.RothlytMussel,
            OceanFish.LeviElver,
            OceanFish.PearlBombfish,
            OceanFish.Trollfish,
            OceanFish.Panoptes,
            OceanFish.CrepeSole,
            OceanFish.Knifejaw
        };

		private static readonly string[] fullPattern = new[]{"BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN"};
		
		private static readonly Tuple<string, string>[] NS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("north", "Sunset")
		};
		private static readonly Tuple<string, string>[] NN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("north", "Night")
		};
		private static readonly Tuple<string, string>[] ND = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("north", "Day")
		};
		private static readonly Tuple<string, string>[] RS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("rhotano", "Sunset")
		};
		private static readonly Tuple<string, string>[] RN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("rhotano", "Night")
		};
		private static readonly Tuple<string, string>[] RD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("rhotano", "Day")
		};
		private static readonly Tuple<string, string>[] BS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("north", "Day"),
			new Tuple<string, string>("blood", "Sunset")
		};
		private static readonly Tuple<string, string>[] BN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("north", "Sunset"),
			new Tuple<string, string>("blood", "Night")
		};
		private static readonly Tuple<string, string>[] BD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("north", "Night"),
			new Tuple<string, string>("blood", "Day")
		};
		private static readonly Tuple<string, string>[] TS = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("rhotano", "Day"),
			new Tuple<string, string>("sound", "Sunset")
		};
		private static readonly Tuple<string, string>[] TN = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("rhotano", "Sunset"),
			new Tuple<string, string>("sound", "Night")
		};
		private static readonly Tuple<string, string>[] TD = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("rhotano", "Night"),
			new Tuple<string, string>("sound", "Day")
		};

		private static readonly Tuple<uint, Vector3>[] SummoningBells = new Tuple<uint, Vector3>[]
		{
			new Tuple<uint, Vector3>(Zones.LimsaLominsaLowerDecks, new Vector3(-123.888062f, 17.990356f, 21.469421f)),	// Limsa
			new Tuple<uint, Vector3>(Zones.Uldah,			new Vector3(148.91272f, 3.982544f, -44.205383f)),		// Ul'dah
			new Tuple<uint, Vector3>(Zones.OldGridania,		new Vector3(160.234863f, 15.671021f, -55.649719f)),		// Old Gridania (Gridania) 
			new Tuple<uint, Vector3>(Zones.MorDhona,		new Vector3(11.001709f, 28.976807f, -734.554077f)),		// Mor Dhona (Mor Dhona) 
			new Tuple<uint, Vector3>(Zones.Ishgard,			new Vector3(-151.171204f, -12.64978f, -11.764771f)),	// The Pillars (Ishgard) 
			new Tuple<uint, Vector3>(Zones.Idyllshire,		new Vector3(34.775269f, 208.148193f, -50.858398f)),		// Idyllshire (Dravania)  
			new Tuple<uint, Vector3>(Zones.Kugane,			new Vector3(19.394226f, 4.043579f, 53.025024f)),		// Kugane 
			new Tuple<uint, Vector3>(Zones.RhalgrsReach,	new Vector3(-57.633362f, -0.01532f, 49.30188f)),		// Rhalgr's Reach (Gyr Abania) 
			new Tuple<uint, Vector3>(Zones.Crystarium,		new Vector3(-69.840576f, -7.705872f, 123.491211f)),		// The Crystarium
			new Tuple<uint, Vector3>(Zones.Eulmore,			new Vector3(7.186951f, 83.17688f, 31.448853f))			// Eulmore 
		};

		private static Random rnd = new Random();
		private Stopwatch biteTimer = new Stopwatch();
		private bool doubleHooked = false;

		private bool RouteShown = false;

		private List<uint> missingFish = new List<uint>();

		static PatternFinder patternFinder = new PatternFinder(Core.Memory);
		static int HomeWorldOffset = patternFinder.Find("0F B7 81 ? ? ? ? 66 89 44 24 ? 48 8D 4C 24 ? Add 3 Read32").ToInt32();
		public static ushort HomeWorld = Core.Memory.NoCacheRead<ushort>(Core.Me.Pointer + HomeWorldOffset);

		System.Timers.Timer execute = new System.Timers.Timer();

		public override string Name => "Ocean Trip";
		public override PulseFlags PulseFlags => PulseFlags.All;

		public override bool IsAutonomous => true;
		public override bool RequiresProfile => false;

		public override Composite Root => _root;

		public override bool WantButton { get; } = true;

		private SettingsForm settings;
		public override void OnButtonPress()
		{
			if (settings == null || settings.IsDisposed)
				settings = new SettingsForm();
			try
			{
				settings.Show();
				settings.Activate();
			}
			catch
			{
			}
		}

		public override void Start()
		{
            RouteShown = false;
            
			TimeSpan timeLeftUntilFirstRun = TimeUntilNextBoat();
			if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
				execute.Interval = 100;
			else
				execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

			execute.Elapsed += new ElapsedEventHandler(KillLisbeth);
			execute.Start();


            Log("BotBase is initialized, beginning execution.");
			_root = new ActionRunCoroutine(r => Run());
		}

		private TimeSpan TimeUntilNextBoat()
		{
            TimeSpan stop = new TimeSpan();

            if ((DateTime.UtcNow.Hour % 2 == 0) && (DateTime.UtcNow.Minute > 10))
            {
                stop = new TimeSpan(DateTime.UtcNow.Hour + 2, 0, 0);
            }
            else
            {
                stop = new TimeSpan(DateTime.UtcNow.Hour + DateTime.UtcNow.Hour % 2, 0, 0);
            }

            TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;
			return timeLeftUntilFirstRun;
        }

		private async void KillLisbeth(object sender, ElapsedEventArgs e)
		{
			TimeSpan stop = new TimeSpan(DateTime.UtcNow.Hour + 2, 0, 0);

			var schedule = GetSchedule();

			if ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) 
				|| ((OceanTripSettings.Instance.FishPriority == FishPriority.FishLog) 
					&& ((missingFish.Contains((uint)OceanFish.Sothis) && (schedule == ND || schedule == RS)) 
						|| (missingFish.Contains((uint)OceanFish.CoralManta) && (schedule == RD || schedule == NS)) 
						|| (missingFish.Contains((uint)OceanFish.Stonescale) && (schedule == RS)) 
						|| (missingFish.Contains((uint)OceanFish.Elasmosaurus) && (schedule == ND)) 
						|| (missingFish.Contains((uint)OceanFish.Hafgufa) && (schedule == BS || schedule == TS)) 
						|| (missingFish.Contains((uint)OceanFish.SeafaringToad) && (schedule == BD)) 
						|| (missingFish.Contains((uint)OceanFish.Placodus) && (schedule == TS)))))
			{
                Log("Stop!");
                Lisbeth.StopGently();
                PassTheTime.freeToCraft = false;
			}
			else
			{
                Log("Not getting on the boat, no fish needed");
			}
			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;

			execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;
			execute.Start();
		}

		public override void Stop()
		{
			execute.Elapsed -= new ElapsedEventHandler(KillLisbeth);
			_root = null;
		}

		private async Task<bool> Run()
		{
			Navigator.PlayerMover = new SlideMover();
			Navigator.NavigationProvider = new ServiceNavigationProvider();

			await RefreshMissingFish();
            await OceanFishing();

			return true;
		}

		private async Task RefreshMissingFish()
		{
            Log("Obtaining current list of missing ocean fish.");
            missingFish = await GetFishLog();
            Log($"Total missing ocean fish: {missingFish.Count()}");
        }

        private async Task OceanFishing()
		{
			GetSchedule();
			if (WorldManager.RawZoneId != Zones.OceanFishing)
			{
				//missingFish = await GetFishLog();
				if (Core.Me.CurrentJob == ClassJobType.Fisher)
				{
					if (OceanTripSettings.Instance.ExchangeFish == ExchangeFish.Sell)
					{
						await LandSell(fishForSale);
					}
					else if (OceanTripSettings.Instance.ExchangeFish == ExchangeFish.Desynth)
					{
						await PassTheTime.DesynthOcean(fishForSale);
					}

					await Lisbeth.SelfRepairWithMenderFallback();
					await LandRepair(50);	
				}

				await RestockBait(150, 500);
				if (OceanTripSettings.Instance.EmptyScrips)
				{
					await EmptyScrips(12669, 1500);
				}

				if (OceanTripSettings.Instance.Venturing != Venturing.None)
				{
					await Retaining();
				}

                TimeSpan timeLeftUntilNextSpawn = TimeUntilNextBoat();
                Log($"Next boat is in {Math.Ceiling(timeLeftUntilNextSpawn.TotalMinutes)} minutes. Passing the time until then.");
                PassTheTime.freeToCraft = true;
				await PassTheTime.Craft();

                if (Core.Me.CurrentJob != ClassJobType.Fisher)
                {
                    await SwitchToJob(ClassJobType.Fisher);
                    Log("Switching to FSH class...");
                }
                await Lisbeth.SelfRepairWithMenderFallback();

				while (!((DateTime.UtcNow.Hour % 2 == 0) && (DateTime.UtcNow.Minute <= 10)))
				{
					await Coroutine.Sleep(1000);
					if (OceanTripSettings.Instance.Venturing != Venturing.None)
					{
						await Retaining();
					}
				}

				if (Core.Me.CurrentJob != ClassJobType.Fisher)
				{
                    await SwitchToJob(ClassJobType.Fisher);
					Log("Switching to FSH class...");
                }

                Log("Time to queue up for the boat!");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-410.1068f, 3.999944f, 74.89863f));


				uint edibleFood = 0;
				bool edibleFoodHQ = false;

				if (OceanTripSettings.Instance.OceanFood)
				{
					if (DataManager.GetItem((uint)FoodList.CrabCakes, true).ItemCount() >= 1)
					{
						edibleFood = (uint)FoodList.CrabCakes;
						edibleFoodHQ = true;
					}
					else if (DataManager.GetItem((uint)FoodList.CrabCakes, false).ItemCount() >= 1)
					{
						edibleFood = (uint)FoodList.CrabCakes;
						edibleFoodHQ = false;
					}
					else if (DataManager.GetItem((uint)FoodList.PepperedPopotoes, true).ItemCount() >= 1)
					{
						edibleFood = (uint)FoodList.PepperedPopotoes;
						edibleFoodHQ = true;
					}
					else if (DataManager.GetItem((uint)FoodList.PepperedPopotoes).ItemCount() >= 1)
					{
						edibleFood = (uint)FoodList.PepperedPopotoes;
						edibleFoodHQ = false;

                    }
					else
					{
						edibleFood = 0;
						edibleFoodHQ = false;
					}

					if (edibleFood > 0)
					{
							do
							{
								Log($"Eating {DataManager.GetItem(edibleFood, edibleFoodHQ).CurrentLocaleName}...");

								foreach (BagSlot slot in InventoryManager.FilledSlots)
								{
									if (slot.RawItemId == (uint)FoodList.PepperedPopotoes)
									{
										slot.UseItem();
									}
								}
								await Coroutine.Sleep(3000);

							} while (!Core.Player.Auras.Any(x => x.Value == CharacterAuras.FoodBuff));
							await Coroutine.Sleep(1000);
					}
                    else if (OceanTripSettings.Instance.OceanFood == true)
                    {
                        Log("Out of food!");
                    }
                }

                await GetOnBoat();
			}

			int spot = rnd.Next(6);
			var schedule = GetSchedule();
			int posOnSchedule = 0;

			if (!RouteShown)
			{
				Log("Route:");
				Log(schedule[posOnSchedule].Item1 + ", " + schedule[posOnSchedule].Item2);
				Log(schedule[posOnSchedule+1].Item1 + ", " + schedule[posOnSchedule+1].Item2);
				Log(schedule[posOnSchedule+2].Item1 + ", " + schedule[posOnSchedule+2].Item2);
				RouteShown = true;
			}
			
			while ((WorldManager.ZoneId == Zones.OceanFishing) && !ChatCheck("[NPCAnnouncements]", "measure your catch!"))
			{
				if (ChatCheck("[NPCAnnouncements]","southern Strait"))
				{
					Log($"Southern Merlthor, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.Krill, FishBait.ShrimpCageFeeder, "south", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","Galadion"))
				{
					Log($"Galadion Bay, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.PlumpWorm, FishBait.GlowWorm, "galadion", schedule[posOnSchedule].Item2, spot);		
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","northern Strait"))
				{
					Log($"Northern Merlthor, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.Ragworm, FishBait.HeavySteelJig, "north", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","Rhotano Sea"))
				{
					Log($"Rhotano Sea, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.Ragworm, FishBait.RatTail, "rhotano", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Cieldalaes"))
				{
					Log($"Cieldalaes, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.Ragworm, FishBait.SquidStrips, "ciel", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Bloodbrine"))
				{
					Log($"Bloodbrine, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.Krill, FishBait.PillBug, "blood", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Rothlyt Sound"))
				{
					Log($"Rothlyt Sound, {schedule[posOnSchedule].Item2}");
					await GoFish(FishBait.PlumpWorm, FishBait.Ragworm, "sound", schedule[posOnSchedule].Item2, spot);
					posOnSchedule++;
				}
				await Coroutine.Sleep(500);
			}


			AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
			if (windowByName != null)
			{
				await Coroutine.Sleep(12000);
				windowByName.SendAction(1, 3, 0);
				if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}

                await RefreshMissingFish();
				RouteShown = false;
            }

			await Coroutine.Sleep(1000);
		}

		private async Task ChangeBait(ulong baitId)
		{
			if ((baitId != FishingManager.SelectedBaitItemId) && (DataManager.GetItem((uint)baitId).ItemCount() > 20) && (DataManager.GetItem((uint)baitId).RequiredLevel <= Core.Me.ClassLevel))
			{
				AtkAddonControl baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				if(baitWindow == null)
				{
					ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
					await Coroutine.Sleep(200);
					baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				}							
				if(baitWindow != null)
				{
					baitWindow.SendAction(4, 0, 0, 0, 0, 0, 0, 1, baitId);
					Log($"Applied {DataManager.GetItem((uint) baitId).CurrentLocaleName}");
					await Coroutine.Sleep(200);
					ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
				}	
			}
		}

		private async Task GoFish(ulong baitId, ulong spectralbaitId, string location, string timeOfDay, int spot)
		{
			bool spectraled = false;
			Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
			while (fishSpots[spot].Distance2DSqr(Core.Me.Location) > 2)
			{
				Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
				await Coroutine.Sleep(100);
			}
			Navigator.PlayerMover.MoveStop();
			await Coroutine.Sleep(500);
			Core.Me.SetFacing(headings[spot]);
			await Coroutine.Sleep(1000);

			while ((WorldManager.ZoneId == Zones.OceanFishing) && !ChatCheck("[NPCAnnouncements]","Weigh the anchors") && !ChatCheck("[NPCAnnouncements]", "measure your catch!"))
			{
				if (WorldManager.CurrentWeatherId != Weather.Spectral) 
				{
					if (spectraled == true)
					{
						Log("Spectral over.");
						spectraled = false;
					}		
				}
				else
				{
					if (spectraled == false)
					{
						Log("Spectral popped!");
						spectraled = true;
					}		
				}

				if (FishingManager.AvailableMooch.Both == FishingManager.CanMoochAny && !ChatCheck("[2115]","Mooch II") && (Core.Me.CurrentGP < 500) && spectraled)
				{
					await UseCordial();
				}

				if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
				{
					//Identical Cast for Blue fish
					if ((ChatCheck("You land a","gugrusaurus") || ChatCheck("You land a","heavenskey") || ChatCheck("You land a", "grandmarlin")) && (Core.Me.CurrentGP >= DataManager.SpellCache[Actions.IdenticalCast].Cost) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
					{
						await Coroutine.Sleep(100);
						if (ActionManager.CanCast(Actions.IdenticalCast, Core.Me))
						{
							Log("Identical Cast!");
							ActionManager.DoAction(Actions.IdenticalCast, Core.Me);
						}
					}

					// Check for Mooch II before using Mooch
					if (ChatCheck("[2115]","Mooch II") && ActionManager.CanCast(268, Core.Me) && Core.Me.CurrentGP >= DataManager.SpellCache[Actions.MoochII].Cost && WorldManager.CurrentWeatherId == Weather.Spectral && spectraled)
					{
						Log("Mooch II!");
						ActionManager.DoAction(Actions.MoochII, Core.Me);
						biteTimer.Start();
					}
                    else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch && WorldManager.CurrentWeatherId == Weather.Spectral && spectraled)
                    {
                        Log("Mooch!");
                        FishingManager.Mooch();
                        biteTimer.Start();
                    }
                    else
                    {
						if (WorldManager.CurrentWeatherId == Weather.Spectral)
						{
							//Bait for Blue fish
							if (((location == "galadion") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Sothis)) || ((location == "south") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.CoralManta)) || ((location == "north") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Elasmosaurus)) || ((location == "rhotano") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Stonescale)) || ((location == "ciel") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Hafgufa)) || ((location == "blood") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.SeafaringToad)) || ((location == "sound") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Placodus)))
							{
								await ChangeBait(spectralbaitId);
							}
							else if (((location == "galadion") && (timeOfDay == "Sunset")) || ((location == "rhotano") && (timeOfDay == "Day")) || ((location == "north") && (timeOfDay == "Day")) || ((location == "south") && (timeOfDay == "Night")) || ((location == "ciel") && (timeOfDay == "Sunset")) || ((location == "blood") && (timeOfDay == "Sunset")))
							{
								await ChangeBait(FishBait.PlumpWorm); 
							}
							else if (((location == "south") && (timeOfDay == "Day")) || ((location == "rhotano") && (timeOfDay == "Night") && ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) || !missingFish.Contains((uint)OceanFish.Slipsnail))) || ((location == "north") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.TheFallenOne) && (OceanTripSettings.Instance.FishPriority == FishPriority.FishLog)) || ((location == "north") && (timeOfDay == "Night") && ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) || !missingFish.Contains((uint)OceanFish.BartholomewTheChopper))) || ((location == "galadion") && (timeOfDay == "Night")) || ((location == "ciel") && (timeOfDay == "Day")) || ((location == "blood") && (timeOfDay == "Night")) || ((location == "sound")))
							{
								await ChangeBait(FishBait.Krill); 
							}
							else if (((location == "galadion") && (timeOfDay == "Day")) || ((location == "south") && (timeOfDay == "Sunset")) || ((location == "north") && (timeOfDay == "Sunset")) || ((location == "north") && (timeOfDay == "Night")) || ((location == "rhotano") && (timeOfDay == "Night")) || ((location == "blood") && (timeOfDay == "Day")))
							{
								await ChangeBait(FishBait.Ragworm); 
							}
							else
							{
								await ChangeBait(spectralbaitId);
							}
						}
						else
						{
							//Cieldalaes Geode
							if (location == "ciel" && missingFish.Contains((uint)OceanFish.CieldalaesGeode))
							{
								if (!Core.Player.HasAura(CharacterAuras.FishersIntuition))
								{
									await ChangeBait(FishBait.Ragworm);
								}
								else
								{
									await ChangeBait(FishBait.Krill);
								}
							}

							//Ginkgo Fin
							else if (location == "sound" && missingFish.Contains((uint)OceanFish.GinkgoFin))
							{
								await ChangeBait(FishBait.Ragworm);
							}
							else
							{
								await ChangeBait(baitId);
							}
						}
						biteTimer.Start();
						FishingManager.Cast();	
					}
					await Coroutine.Sleep(50);
					doubleHooked = false;
				}

				while ((FishingManager.State != FishingState.PoleReady) && !ChatCheck("[NPCAnnouncements]","Weigh the anchors") && !ChatCheck("[NPCAnnouncements]", "measure your catch!"))
				{
					await Coroutine.Sleep(50);

					//Spectral popped, don't wait for normal fish
					if (WorldManager.CurrentWeatherId == Weather.Spectral && !spectraled)
					{
						Log("Spectral popped!");
						spectraled = true;
						if (FishingManager.CanHook)
						{
							FishingManager.Hook();
						}
					}
					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
					{
						biteTimer.Stop();
						Log($"Bite Time: {biteTimer.Elapsed.TotalSeconds:F1}s");
						if ((((location == "galadion") && 
									(((biteTimer.Elapsed.TotalSeconds >= 7) && (FishingManager.TugType != TugType.Medium) && (timeOfDay != "Night")) 
										|| ((biteTimer.Elapsed.TotalSeconds > 1) && (biteTimer.Elapsed.TotalSeconds <= 4) && (FishingManager.TugType == TugType.Medium)))) 
								|| ((location == "south") && 
									(((biteTimer.Elapsed.TotalSeconds >= 6) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Light)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 2) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Heavy) && (FishingManager.MoochLevel == 1)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 2) && (biteTimer.Elapsed.TotalSeconds <= 6) && (timeOfDay == "Night") && (FishingManager.TugType == TugType.Medium) && (FishingManager.MoochLevel == 1)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 4) && (biteTimer.Elapsed.TotalSeconds <= 7) && (timeOfDay == "Day") && (FishingManager.TugType == TugType.Medium)))) 
								|| ((location == "north") && 
									(((biteTimer.Elapsed.TotalSeconds >= 5) && (biteTimer.Elapsed.TotalSeconds <= 9) && (timeOfDay == "Night") && (FishingManager.TugType != TugType.Light)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 7) && (biteTimer.Elapsed.TotalSeconds <= 12) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Light)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 6) && (biteTimer.Elapsed.TotalSeconds <= 9) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Medium)))) 
								|| ((location == "rhotano") && 
									(((biteTimer.Elapsed.TotalSeconds >= 7) && (biteTimer.Elapsed.TotalSeconds <= 11) && (timeOfDay == "Night") && (FishingManager.TugType == TugType.Light)) 
										|| ((biteTimer.Elapsed.TotalSeconds >= 6) && (biteTimer.Elapsed.TotalSeconds <= 10) && (timeOfDay == "Day") && (FishingManager.TugType == TugType.Heavy))))) 
							&& (WorldManager.CurrentWeatherId == Weather.Spectral) && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && (Core.Me.CurrentGP >= DataManager.SpellCache[Actions.DoubleHook].Cost))
						{
							Log("Double Hook!");
							ActionManager.DoAction(Actions.DoubleHook, Core.Me);
							doubleHooked = true;
						}
						else if (FishingManager.HasPatience)
						{
							if (FishingManager.TugType == TugType.Light)
							{
								ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
							}
							else
							{
								ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
							}
						}
						else
						{
							FishingManager.Hook();
						}
						biteTimer.Reset();
					}
				}
			}

			spectraled = false;
			await Coroutine.Sleep(2000);
			//Log("Waiting for next stop...");
			if (FishingManager.State != FishingState.None)
			{
				ActionManager.DoAction(Actions.Quit, Core.Me);
			}
		}

		private static async Task GetOnBoat()
		{	
			var Dryskthota = GameObjectManager.GetObjectByNPCId(NPC.Dryskthota);

			if (Dryskthota != null)
			{
				Dryskthota.Interact();
				if (await Coroutine.Wait(5000, () => Talk.DialogOpen))
				{
					Talk.Next();
				}

				await Coroutine.Wait(5000, () => SelectString.IsOpen);

				if (SelectString.IsOpen)
				{
					SelectString.ClickSlot(0);
					await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
					SelectYesno.Yes();

					await Coroutine.Wait(1000000, () => ContentsFinderConfirm.IsOpen);

					await Coroutine.Yield();
					while (ContentsFinderConfirm.IsOpen)
					{
						DutyManager.Commence();
						await Coroutine.Yield();
						if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
						{
							await Coroutine.Yield();
							await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
						}
					}
					while (WorldManager.ZoneId != Zones.OceanFishing)
					{
						await Coroutine.Sleep(1000);
					}
					await Coroutine.Sleep(2500);
					Logging.Write(Colors.Aqua, "We're on the boat!");
				}
			}
		}

		private static async Task SwitchToJob(ClassJobType job)
		{
			if (Core.Me.CurrentJob == job) return;

			var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

			if (gearSets.Any(gs => gs.Class == job))
			{
				Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet");
				gearSets.First(gs => gs.Class == job).Activate();
				await Coroutine.Sleep(1000);
			}
		}		

		private async Task UseCordial()
		{
			await Coroutine.Sleep(500); // Sleep in case ability is in use

			uint cordial = 0;

			if (DataManager.GetItem(Cordials.HiCordial).ItemCount() > 0)
				cordial = Cordials.HiCordial;
			else if (DataManager.GetItem(Cordials.Cordial).ItemCount() > 0)
				cordial = Cordials.Cordial;
			else if (DataManager.GetItem(Cordials.WateredCordial).ItemCount() > 0)
				cordial = Cordials.WateredCordial;

			// Yay, we have a cordial!
			if (cordial > 0)
			{
				foreach (BagSlot slot in InventoryManager.FilledSlots)
				{
					if (slot.RawItemId == cordial)
					{
						slot.UseItem();
					}
				}
			}
			Logging.Write(Colors.Aqua, $"Used a {DataManager.GetItem(cordial).CurrentLocaleName}...");

            await Coroutine.Sleep(2000);
		}

		private static async Task LandRepair(int repairThreshold)
		{
			if (InventoryManager.EquippedItems.Where(r => r.IsFilled && r.Condition < repairThreshold).Count() > 0)
			{	
				Logging.Write(Colors.Aqua, "Starting repair...");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(1);
					await Coroutine.Wait(3000, () => Repair.IsOpen);
					if (Repair.IsOpen)
					{
						Repair.RepairAll();
						await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
						if (SelectYesno.IsOpen)
						{
							SelectYesno.ClickYes();
						}
						Repair.Close();
						await Coroutine.Wait(5000, () => !Repair.IsOpen);
					}
				}
				Logging.Write(Colors.Aqua, "Repair complete!");
			}
		}

		private async Task RestockBait(int baitThreshold, uint baitCap)
		{
			List<uint> itemsToBuy = new List<uint>();
			if (DataManager.GetItem(FishBait.Ragworm).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.Ragworm);
			}
			if (DataManager.GetItem(FishBait.Krill).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.Krill);
			}
			if (DataManager.GetItem(FishBait.PlumpWorm).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.PlumpWorm);
			}
			if (DataManager.GetItem(FishBait.RatTail).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.RatTail);
			}
			if (DataManager.GetItem(FishBait.GlowWorm).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.GlowWorm);
			}
			if (DataManager.GetItem(FishBait.HeavySteelJig).ItemCount() < 5)
			{
				itemsToBuy.Add(FishBait.HeavySteelJig);
			}
			if (DataManager.GetItem(FishBait.ShrimpCageFeeder).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.ShrimpCageFeeder);
			}
			if (DataManager.GetItem(FishBait.PillBug).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.PillBug);
			}
			if (DataManager.GetItem(FishBait.SquidStrips).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(FishBait.SquidStrips);
			}

			if (itemsToBuy.Any())
			{
				Log("Restocking bait...");
				if (itemsToBuy.Contains(FishBait.Ragworm) || itemsToBuy.Contains(FishBait.Krill) || itemsToBuy.Contains(FishBait.PlumpWorm))
				{
					await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
					await Coroutine.Sleep(1000);
					GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
					await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
					if (SelectIconString.IsOpen)
					{
						SelectIconString.ClickSlot(0);
						await Coroutine.Wait(5000, () => Shop.Open);
						foreach (uint item in itemsToBuy)
						{
							if ((item == FishBait.Ragworm) || (item == FishBait.Krill) || (item == FishBait.PlumpWorm))
							{
								Shop.Purchase(item, (baitCap - DataManager.GetItem(item).ItemCount()));
								await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
								SelectYesno.ClickYes();
							}
							await Coroutine.Sleep(1000);
						}
					}
					await Coroutine.Sleep(1000);
					Shop.Close();
					await Coroutine.Wait(5000, () => !Shop.Open);
				}
				if (itemsToBuy.Contains(FishBait.RatTail) || itemsToBuy.Contains(FishBait.GlowWorm) || itemsToBuy.Contains(FishBait.ShrimpCageFeeder) || itemsToBuy.Contains(FishBait.PillBug))
				{
					await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-247.6223f, 16.2f, 39.87407f));
					await Coroutine.Sleep(1000);
					GameObjectManager.GetObjectByNPCId(NPC.Syneyhil).Interact();
					await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
					if (SelectIconString.IsOpen)
					{
						SelectIconString.ClickSlot(2);
						await Coroutine.Wait(5000, () => Shop.Open);
						foreach (uint item in itemsToBuy)
						{
							if ((item == FishBait.RatTail) || (item == FishBait.GlowWorm) || (item == FishBait.ShrimpCageFeeder) || (item == FishBait.PillBug))
							{
								Shop.Purchase(item, (baitCap - DataManager.GetItem(item).ItemCount()));
								await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
								SelectYesno.ClickYes();
							}
							await Coroutine.Sleep(1000);
						}
					}
					await Coroutine.Sleep(1000);
					Shop.Close();
					await Coroutine.Wait(5000, () => !Shop.Open);
				}

				if (itemsToBuy.Contains(FishBait.HeavySteelJig) && Core.Me.Levels[ClassJobType.Goldsmith] >= 36)
				{
					await PassTheTime.IdleLisbeth((int)FishBait.HeavySteelJig, 10, "Goldsmith", "true", 0);
				}

				if (itemsToBuy.Contains(FishBait.SquidStrips))
				{
					await PassTheTime.IdleLisbeth((int)FishBait.SquidStrips, 300, "Exchange", "true", 0);
				}
				Log("Restocking bait complete");
			}
		}

		private async Task LandSell(int[] itemIds)
		{
			var itemsToSell = InventoryManager.FilledSlots.Where(bs => bs.IsSellable && itemIds.Contains((int)bs.RawItemId));
			if (itemsToSell.Count() != 0)
			{
				Log("Selling fish...");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(0);
					await Coroutine.Wait(5000, () => Shop.Open);
					foreach (var item in itemsToSell)
					{		
						if(item.Value <= 18)
						{
							var name = item.Name;
							await CommonTasks.SellItem(item);
							await Coroutine.Wait(10000, () => !item.IsFilled || !item.Name.Equals(name));
							await Coroutine.Sleep(1000);	
						}
					}
					Shop.Close();
					await Coroutine.Wait(2000, () => !Shop.Open);
				}
				Log("Fish selling complete!");	
			}
		}

		private async Task EmptyScrips(int itemId, int scripThreshold)
		{
			//TODO: Buy other stuff with scrip
			if (SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.WhiteGatherersScrips) > scripThreshold)
			{
				await PassTheTime.IdleLisbeth(itemId, (int)SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.WhiteGatherersScrips)/20, "Exchange", "false", 0);
			}
		}

		private async Task Retaining()
		{
			if(OceanTripSettings.Instance.VentureTime < DateTime.Now)
			{
				await Navigation.GetTo(SummoningBells[(int)OceanTripSettings.Instance.Venturing].Item1, SummoningBells[(int)OceanTripSettings.Instance.Venturing].Item2);

				foreach (var unit in GameObjectManager.GameObjects.OrderBy(r => r.Distance()))
				{
					if (unit.NpcId == 2000401 || unit.NpcId == 2000441)
					{
						unit.Interact();
						break;
					}
				}
				await Coroutine.Sleep(2000);
				string bell = Lua.GetReturnVal<string>(string.Format("local values = '' for key,value in pairs(_G) do if string.match(key, '{0}:') then return key;   end end return values;", "CmnDefRetainerBell")).Trim();
				int numOfRetainers = 0;

				if (bell.Length > 0)
				{
					numOfRetainers = Lua.GetReturnVal<int>(string.Format("return _G['{0}']:GetRetainerEmployedCount();", bell));
				}

				AtkAddonControl retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				while (retainerWindow == null)
				{
					await Coroutine.Sleep(1000);
					retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				}

				int count = 0;
				while (count < numOfRetainers)
				{
					retainerWindow = null;
					while (retainerWindow == null)
					{	
						await Coroutine.Sleep(1000);
						retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
					}
					retainerWindow.SendAction(2, 3UL, 2UL, 3UL, (ulong) count);
					if (await Coroutine.Wait(15000, () => Talk.DialogOpen))
					{
						Talk.Next();
					}
					if (await Coroutine.Wait(20000, () => SelectString.IsOpen))
					{
						if (SelectString.Lines().Contains("View venture report. (Complete)"))
						{
							SelectString.ClickLineEquals("View venture report. (Complete)");
							if (await Coroutine.Wait(20000, () => RetainerTaskResult.IsOpen))
							{
								RetainerTaskResult.Reassign();
								if (await Coroutine.Wait(10000, () => RetainerTaskAsk.IsOpen))
								{
									RetainerTaskAsk.Confirm();
									if (await Coroutine.Wait(10000, () => Talk.DialogOpen))
									{
										Talk.Next();
									}
								}
							}
						}
						await Coroutine.Wait(20000, () => SelectString.IsOpen);
						if(SelectString.Lines().Any(x => x.Contains("View venture report. (Complete on")))
						{
							Regex r = new Regex(@"(\d+[-.\/]\d+ \d+:\d+)");
							Match m = r.Match(SelectString.Lines().FirstOrDefault(x => x.Contains("View venture report. (Complete on")).ToString());
							if(m.Success)
							{
								DateTime ventureTime = DateTime.ParseExact(m.Value, "d/M H:mm", null);
								if ((ventureTime < OceanTripSettings.Instance.VentureTime && OceanTripSettings.Instance.VentureTime > DateTime.Now) 
									|| (ventureTime > OceanTripSettings.Instance.VentureTime && OceanTripSettings.Instance.VentureTime < DateTime.Now))
								{
									OceanTripSettings.Instance.VentureTime = ventureTime;
								}

							}
						}
						if (await Coroutine.Wait(20000, () => SelectString.IsOpen))
						{
							SelectString.ClickLineEquals("Quit.");
						}
						if (await Coroutine.Wait(10000, () => Talk.DialogOpen))
						{
							Talk.Next();
						}					
					}
					count++;
				}
				retainerWindow = null;
				while (retainerWindow == null)
				{	
					await Coroutine.Sleep(1000);
					retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				}
				retainerWindow.SendAction(1, 3UL, (ulong) uint.MaxValue);
				await Coroutine.Sleep(3000);
			}
		}

		private async Task<List<uint>> GetFishLog()
		{
			List<int> recordedFish = new List<int>();
			if (!FishGuide.IsOpen)
			{
				FishGuide.Toggle();
				await Coroutine.Wait(5000, () => FishGuide.IsOpen);
			}

			if (FishGuide.IsOpen)
			{
				for (int i = 33; i <= FishGuide.TabCount; i++)
				{
					FishGuide.ClickTab(i);
					await Coroutine.Sleep(10);
					var list = FishGuide.GetTabList();
					foreach (var fishy in list.Select(x => x.FishItem))
					{
						if (fishy != 0 && fishy != uint.MaxValue)
						{
							recordedFish.Add((int)fishy);
						}
					}
				}

				FishGuide.Toggle();
				await Coroutine.Wait(5000, () => !FishGuide.IsOpen);
			}

			// Convert the list to uint
			List<uint> newOceanFishList = oceanFish.Except(recordedFish).ToList().ConvertAll(x => (uint)x);
			return newOceanFishList;
		}

		private bool ChatCheck(string chattype, string chatmessage)
		{
			try
			{
				return GamelogManager.CurrentBuffer.Last(chatline => chatline.FullLine.Contains(chattype)).FullLine.Contains(chatmessage);
			}
			catch
			{
				return false;
			}
		}

		private Tuple<string, string>[] GetSchedule()
		{		
			int epoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			int twoHourChunk = ((epoch / 7200) + 88) % fullPattern.Length;

			switch (fullPattern[twoHourChunk])
			{
				case "NS":
					return NS;
				case "NN":
					return NN;
				case "ND":
					return ND;
				case "RS":
					return RS;
				case "RN":
					return RN;
				case "RD":
					return RD;
				case "BS":
					return BS;
				case "BN":
					return BN;
				case "BD":
					return BD;
				case "TS":
					return TS;
				case "TN":
					return TN;
				case "TD":
					return TD;
			}
			return null;
		}

		private void Log(string text, params object[] args)
		{
			var msg = string.Format("[" + Name + "] " + text, args);
			Logging.Write(Colors.Aqua, msg);
		}

	}
}