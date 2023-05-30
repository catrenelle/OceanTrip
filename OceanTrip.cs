﻿using System;
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
using OceanTripPlanner.Helpers;
using OceanTripPlanner.Definitions;
using TreeSharp;
using OceanTrip;
using LlamaLibrary;
using LlamaLibrary.RemoteAgents;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace OceanTripPlanner
{
	public class OceanTrip : BotBase
	{
        //public override Composite Root => _root;
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


		private List<uint> caughtFish;
		private uint lastCaughtFish = 0;
		private bool caughtFishLogged = false;

		private bool ignoreBoat { get { if (OceanTripSettings.Instance.FishPriority == FishPriority.IgnoreBoat) { return true; } else { return false; } } }

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
            OceanFish.Placodus,
            OceanFish.DeepshadeSardine,
            OceanFish.SirensongMussel,
            OceanFish.Arrowhead,
            OceanFish.PinkShrimp,
            OceanFish.SirensongMullet,
            OceanFish.SelkiePuffer,
            OceanFish.PoetsPipe,
            OceanFish.MarineMatanga,
            OceanFish.SpectralCoelacanth,
            OceanFish.DuskShark,
            OceanFish.MermaidScale,
            OceanFish.Broadhead,
            OceanFish.VividPinkShrimp,
            OceanFish.SunkenCoelacanth,
            OceanFish.SirensSigh,
            OceanFish.BlackjawedHelicoprion,
            OceanFish.Impostopus,
            OceanFish.JadeShrimp,
            OceanFish.NymeiasWheel,
            OceanFish.Taniwha,
            OceanFish.RubyHerring,
            OceanFish.WhirlpoolTurban,
            OceanFish.LeopardPrawn,
            OceanFish.SpearSquid,
            OceanFish.FloatingLantern,
            OceanFish.RubescentTatsunoko,
            OceanFish.Hatatate,
            OceanFish.SilentShark,
            OceanFish.SpectralWrasse,
            OceanFish.Mizuhiki,
            OceanFish.SnappingKoban,
            OceanFish.SilkweftPrawn,
            OceanFish.StingfinTrevally,
            OceanFish.SwordtipSquid,
            OceanFish.Mailfish,
            OceanFish.IdatensBolt,
            OceanFish.MaelstromTurban,
            OceanFish.Shoshitsuki,
            OceanFish.Spadefish,
            OceanFish.GlassDragon,
            OceanFish.CrimsonKelp,
            OceanFish.ReefSquid,
            OceanFish.PinebarkFlounder,
            OceanFish.MantleMoray,
            OceanFish.BardedLobster,
            OceanFish.ShisuiGoby,
            OceanFish.Sanbaso,
            OceanFish.VioletSentry,
            OceanFish.SpectralSnakeEel,
            OceanFish.HeavensentShark,
            OceanFish.FleetingSquid,
            OceanFish.BowbarbLobster,
            OceanFish.PitchPickle,
            OceanFish.SenbeiOctopus,
            OceanFish.TentacaleThresher,
            OceanFish.BekkoRockhugger,
            OceanFish.YellowIris,
            OceanFish.CrimsonSentry,
            OceanFish.FlyingSquid,
            OceanFish.HellsClaw,
            OceanFish.CatchingCarp,
            OceanFish.GarleanBluegill,
            OceanFish.YanxianSoftshell,
            OceanFish.PrincessSalmon,
            OceanFish.Calligraph,
            OceanFish.SingularShrimp,
            OceanFish.BrocadeCarp,
            OceanFish.YanxianSturgeon,
            OceanFish.SpectralKotsuZetsu,
            OceanFish.FishyShark,
            OceanFish.GensuiShrimp,
            OceanFish.YatonoKami,
            OceanFish.HeronsEel,
            OceanFish.CrowshadowMussel,
            OceanFish.YanxianGoby,
            OceanFish.IridescentTrout,
            OceanFish.UnNamazu,
            OceanFish.Gakugyo,
            OceanFish.GinrinGoshiki,
            OceanFish.JewelofPlumSpring
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
            OceanFish.Knifejaw,
			OceanFish.DeepshadeSardine,
			OceanFish.SirensongMussel,
			OceanFish.Arrowhead,
			OceanFish.PinkShrimp,
			OceanFish.SirensongMullet,
			OceanFish.SelkiePuffer,
			OceanFish.PoetsPipe,
			OceanFish.MarineMatanga,
			OceanFish.SpectralCoelacanth,
			OceanFish.DuskShark,
            OceanFish.MermaidScale,
            OceanFish.Broadhead,
            OceanFish.VividPinkShrimp,
            OceanFish.SunkenCoelacanth,
            OceanFish.SirensSigh,
            OceanFish.BlackjawedHelicoprion,
            OceanFish.Impostopus,
            OceanFish.JadeShrimp,
            OceanFish.NymeiasWheel,
            OceanFish.RubyHerring,
			OceanFish.WhirlpoolTurban,
			OceanFish.LeopardPrawn,
			OceanFish.SpearSquid,
			OceanFish.FloatingLantern,
			OceanFish.RubescentTatsunoko,
			OceanFish.Hatatate,
			OceanFish.SilentShark,
			OceanFish.SpectralWrasse,
			OceanFish.Mizuhiki,
            OceanFish.SnappingKoban,
            OceanFish.SilkweftPrawn,
            OceanFish.StingfinTrevally,
            OceanFish.SwordtipSquid,
            OceanFish.Mailfish,
            OceanFish.IdatensBolt,
            OceanFish.MaelstromTurban,
            OceanFish.Shoshitsuki,
            OceanFish.Spadefish,
            OceanFish.CrimsonKelp,
            OceanFish.ReefSquid,
			OceanFish.PinebarkFlounder,
			OceanFish.MantleMoray,
			OceanFish.BardedLobster,
			OceanFish.ShisuiGoby,
			OceanFish.Sanbaso,
			OceanFish.VioletSentry,
			OceanFish.SpectralSnakeEel,
			OceanFish.HeavensentShark,
			OceanFish.FleetingSquid,
			OceanFish.BowbarbLobster,
			OceanFish.PitchPickle,
			OceanFish.SenbeiOctopus,
			OceanFish.TentacaleThresher,
			OceanFish.BekkoRockhugger,
			OceanFish.YellowIris,
			OceanFish.CrimsonSentry,
			OceanFish.FlyingSquid,
            OceanFish.CatchingCarp,
			OceanFish.GarleanBluegill,
			OceanFish.YanxianSoftshell,
			OceanFish.PrincessSalmon,
			OceanFish.Calligraph,
			OceanFish.SingularShrimp,
			OceanFish.BrocadeCarp,
			OceanFish.YanxianSturgeon,
			OceanFish.SpectralKotsuZetsu,
			OceanFish.FishyShark,
			OceanFish.GensuiShrimp,
			OceanFish.YatonoKami,
			OceanFish.HeronsEel,
			OceanFish.CrowshadowMussel,
			OceanFish.YanxianGoby,
			OceanFish.IridescentTrout,
			OceanFish.UnNamazu,
			OceanFish.Gakugyo,
			OceanFish.GinrinGoshiki
        };

		// Indigo
		private static readonly string[] fullPattern = new[]{"BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "BS", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "TS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "NS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "RS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "BN", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "TN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "NN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "RN", "BD", "TD", "ND", "RD", "BS", "TS", "NS", "RS", "BN", "TN", "NN"};

		// Ruby
		/*private static readonly string[] ruby_fullPattern = new[] {
			"OD", "RD", "OS", "RS", "ON", "RN", "OD", "RD", "OS", "RS", "RN", // Rotation 1
			"OD", "RD", "OS", "RS", "ON", "RN", "OD", "RD", "OS", "RS", "ON" // Rotation 2
		};*/
		private static readonly int[] ruby_fullPattern = new[]
		{
            1,2,3,4,5,6,1,2,3,4,5,6,
			2,3,4,5,6,1,2,3,4,5,6,1,
			3,4,5,6,1,2,3,4,5,6,1,2,
			4,5,6,1,2,3,4,5,6,1,2,3,
			5,6,1,2,3,4,5,6,1,2,3,4,
			6,1,2,3,4,5,6,1,2,3,4,5
        };



		// Indigo
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

        // Ruby
        private static readonly Tuple<string, string>[] Ruby_RS = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Night"),
            new Tuple<string, string>("kugane", "Day"),
            new Tuple<string, string>("rubysea", "Sunset")
        };

        private static readonly Tuple<string, string>[] Ruby_RN = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Day"),
            new Tuple<string, string>("kugane", "Sunset"),
            new Tuple<string, string>("rubysea", "Night")
        };

        private static readonly Tuple<string, string>[] Ruby_RD = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Sunset"),
            new Tuple<string, string>("kugane", "Night"),
            new Tuple<string, string>("rubysea", "Day")
        };

        private static readonly Tuple<string, string>[] Ruby_OS = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Night"),
            new Tuple<string, string>("kugane", "Day"),
            new Tuple<string, string>("oneriver", "Sunset")
        };

        private static readonly Tuple<string, string>[] Ruby_ON = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Day"),
            new Tuple<string, string>("kugane", "Sunset"),
            new Tuple<string, string>("oneriver", "Night")
        };

        private static readonly Tuple<string, string>[] Ruby_OD = new Tuple<string, string>[3]
        {
            new Tuple<string, string>("sirensong", "Sunset"),
            new Tuple<string, string>("kugane", "Night"),
            new Tuple<string, string>("oneriver", "Day")
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

		private bool RouteShown = false;

		private List<uint> missingFish = new List<uint>();
		private bool missingFishRefreshed = false;
		private Tuple<string, string>[] schedule;
		private bool lastCastMooch = false;

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

		private static SettingsForm settings;
		public override void OnButtonPress()
		{
			if (settings == null || settings.IsDisposed)
				settings = new SettingsForm();

			try
			{
				// Temporary. Future item to come. Currently not working and is a proof of concept. :)
				settings.tempHideRouteInformationTab();

                settings.Show();
				settings.Activate();
			}
			catch
			{
			}
		}

		public override void Start()
		{
			TreeHooks.Instance.ClearAll();

            Log("Initializing OceanTrip Settings.");
            if (settings == null || settings.IsDisposed)
                settings = new SettingsForm();

			Log("OceanTrip Settings Loaded.");

            caughtFish = new List<uint>();
			lastCaughtFish = 0;
			caughtFishLogged = false;

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

		public static TimeSpan TimeUntilNextBoat()
		{
            TimeSpan stop = new TimeSpan();
            int min = (OceanTripSettings.Instance.LateBoatQueue ? 13 : 0);


            if ((DateTime.UtcNow.Hour % 2 == 0) 
					&& ((DateTime.UtcNow.Minute > 12 && !OceanTripSettings.Instance.LateBoatQueue) || (DateTime.UtcNow.Minute > 14 && OceanTripSettings.Instance.LateBoatQueue)))
            {
                stop = new TimeSpan(DateTime.UtcNow.Hour + 2, min, 0);
            }
            else
            {
                stop = new TimeSpan(DateTime.UtcNow.Hour + DateTime.UtcNow.Hour % 2, min, 0);
            }

            TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;
			return timeLeftUntilFirstRun;
        }

		private async void KillLisbeth(object sender, ElapsedEventArgs e)
		{
			TimeSpan stop =  new TimeSpan((DateTime.UtcNow.Hour % 2 == 0 ? DateTime.UtcNow.Hour + 2 : DateTime.UtcNow.Hour), (OceanTripSettings.Instance.LateBoatQueue ? 13 : 0), 0); //TimeUntilNextBoat();


            schedule = GetSchedule();

			if (!ignoreBoat)
			{
				if ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog)
						|| (FocusFishLog && missingFish.Count() > 0))
				{
					//Log("Stopping Lisbeth!");
					Lisbeth.StopGently();
					PassTheTime.freeToCraft = false;
				}
				else
				{
					Log("Not getting on the boat, no fish needed");
				}
			}

			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;

            if (timeLeftUntilFirstRun.TotalMilliseconds < 0)
                execute.Interval = 100;
            else
                execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;

			execute.Start();
		}

		public override void Stop()
		{
			execute.Elapsed -= new ElapsedEventHandler(KillLisbeth);
			_root = null;

			Navigator.NavigationProvider = new NullProvider();
			Navigator.Clear();
		}

		private async Task<bool> Run()
		{
			Navigator.PlayerMover = new SlideMover();
			Navigator.NavigationProvider = new ServiceNavigationProvider();
			missingFishRefreshed = false;
			caughtFish.Clear();

			if (missingFish.Count == 0)
				if (File.Exists(Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt")))
				{
					FishingLog.LoadMissingFishLog(out missingFish);
                    Log($"[Ocean Trip] Missing Fish Loaded! Cache shows a total of {missingFish.Count()} missing fish.");
                    missingFishRefreshed = true;
				}
				else
					await RefreshMissingFish();
            
			await OceanFishing();

			return true;
		}

		private async Task RefreshMissingFish()
		{
			if (!missingFishRefreshed)
			{
				try
				{
					Log("Obtaining current list of missing ocean fish.");
					missingFish = await FishingLog.GetFishLog(oceanFish);

					List<string> missingFishNames = new List<string>();
					foreach (var fish in missingFish)
						missingFishNames.Add(DataManager.GetItem(fish).EnglishName);

					settings.updateMissingFish(missingFishNames);
					missingFishNames.Clear();

					Log($"Total missing ocean fish: {missingFish.Count()}");

                    FishingLog.SaveMissingFishLog(missingFish);
				}
				catch (Exception e)
				{
					if (OceanTripSettings.Instance.FishPriority != FishPriority.IgnoreBoat)
					{
						OceanTripSettings.Instance.FishPriority = FishPriority.Points;
						Logging.Write(Colors.Red, "[OceanTrip] Cannot obtain list of Missing Fish! Defaulting OceanTrip to points mode.");
					}
					else
                        Logging.Write(Colors.Red, "[OceanTrip] Cannot obtain list of Missing Fish! Not a huge deal due to being set to Ignore Boat.");

                    Logging.Write(Colors.Red, "[OceanTrip] Exception Message: " + e.Message);
					Logging.Write(Colors.Red, "[OceanTrip] Stack Trace: " + e.StackTrace);
					settings.updateMissingFish(null);
				}

				missingFishRefreshed = true;
			}
        }

        private async Task OceanFishing()
		{
			//GetSchedule();
			if (WorldManager.RawZoneId != Zones.TheEndeavor && WorldManager.RawZoneId != Zones.TheEndeaver_Ruby)
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

				if (OceanTripSettings.Instance.BaitRestockThreshold > 10 && OceanTripSettings.Instance.BaitRestockAmount > 30)
					await RestockBait(OceanTripSettings.Instance.BaitRestockThreshold, (uint)OceanTripSettings.Instance.BaitRestockAmount);
				else
					Log("Bait Restock Threshold or Restock Amount is set too low. Skipping bait restock. If you are missing the required baits for ocean fishing, the bot may not operate properly.");

			
				if (OceanTripSettings.Instance.EmptyScrips)
				{
					await EmptyScrips((int)Cordials.HiCordial, 1500);
				}

				if (OceanTripSettings.Instance.Venturing != Venturing.None)
				{
					await Retaining();
				}


                if (!File.Exists(Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt")))
                {
                    await RefreshMissingFish();
                    missingFishRefreshed = true;
                }
 
				if (!ignoreBoat)
				{
					TimeSpan timeLeftUntilNextSpawn = TimeUntilNextBoat();
					if (timeLeftUntilNextSpawn.TotalMinutes < 1)
					{
						Log($"The boat is ready to be boarded!");
						PassTheTime.freeToCraft = false;
					}
					else
					{
						Log($"Next boat is in {Math.Ceiling(timeLeftUntilNextSpawn.TotalMinutes)} minutes. Passing the time until then.");
						PassTheTime.freeToCraft = true;
					}
				}
				else
					PassTheTime.freeToCraft = true;

                await PassTheTime.Craft();

				if (!ignoreBoat)
				{
					if (Core.Me.CurrentJob != ClassJobType.Fisher)
					{
						await SwitchToJob(ClassJobType.Fisher);
						Log("Switching to FSH class...");
					}
				}

				await Lisbeth.SelfRepairWithMenderFallback();

				// LongBoatQueue = true = 13-15 Minutes
				// LongBoatQueue = false = 10-13 minutes
				while (!((DateTime.UtcNow.Hour % 2 == 0) &&
						((DateTime.UtcNow.Minute < 13 && !OceanTripSettings.Instance.LateBoatQueue)
						|| (DateTime.UtcNow.Minute >= 13 && DateTime.UtcNow.Minute < 15 && OceanTripSettings.Instance.LateBoatQueue)))
						|| ignoreBoat)
				{
					await Coroutine.Sleep(1000);
					if (OceanTripSettings.Instance.Venturing != Venturing.None)
					{
						await Retaining();
					}

					if (OceanTripSettings.Instance.OpenWorldFishing &&  FishingManager.State != FishingState.None && Core.Me.CurrentJob == ClassJobType.Fisher)
                    {
						await GoOpenWorldFishing();
					}
				}

                if (FishingManager.State != FishingState.None)
                    ActionManager.DoAction(Actions.Quit, Core.Me);

                if (Core.Me.CurrentJob != ClassJobType.Fisher)
				{
                    await SwitchToJob(ClassJobType.Fisher);
					Log("Switching to FSH class...");
                }

                Log("Time to queue up for the boat!");
				await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-410.1068f, 3.999944f, 74.89863f));


				uint edibleFood = 0;
				bool edibleFoodHQ = false;

				if (OceanTripSettings.Instance.OceanFood != OceanFood.None)
				{
					if (DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood, true).ItemCount() >= 1)
					{
						edibleFood = (uint)OceanTripSettings.Instance.OceanFood;
						edibleFoodHQ = true;
					}
					else if (DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood, false).ItemCount() >= 1)
					{
						edibleFood = (uint)OceanTripSettings.Instance.OceanFood;
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
									if (slot.RawItemId == (uint)edibleFood)
									{
										slot.UseItem();
									}
								}
								await Coroutine.Sleep(3000);

							} while (!Core.Player.Auras.Any(x => x.Id == CharacterAuras.WellFed));
							await Coroutine.Sleep(1000);
					}
                    else 
                    {
                        Log($"Out of {DataManager.GetItem((uint)OceanTripSettings.Instance.OceanFood, false).CurrentLocaleName} to eat!");
                    }
                }

                await GetOnBoat();
			}

			int spot = rnd.Next(6);
			schedule = GetSchedule();
			int posOnSchedule = 0;
			string TimeOfDay = "";

            if (!RouteShown)
			{
				Log("Route:");
				Log(schedule[posOnSchedule].Item1 + ", " + schedule[posOnSchedule].Item2);
				Log(schedule[posOnSchedule+1].Item1 + ", " + schedule[posOnSchedule+1].Item2);
				Log(schedule[posOnSchedule+2].Item1 + ", " + schedule[posOnSchedule+2].Item2);
				RouteShown = true;
			}
			
			while ((WorldManager.ZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby) && !ChatCheck("[NPCAnnouncements]", "measure your catch!"))
			{
				if (!String.IsNullOrEmpty(FishingLog.AreaName))
				{ 
					// Reset for this round
					caughtFish.Clear();
					lastCaughtFish = 0;
					caughtFishLogged = false;
				}

                // English
                // Deutsch
                // Francais
                // 日本語
                // 中文
                // 한국어
                if (FishingLog.AreaName.Contains("Southern Strait") || FishingLog.AreaName.Contains("Merlthorstraße (Süd)")
					|| FishingLog.AreaName.Contains("Détroit sud de Merlthor") || FishingLog.AreaName.Contains("メルトール海峡南")
					|| FishingLog.AreaName.Contains("梅尔托尔海峡南") || FishingLog.AreaName.Contains("멜토르 해협 남쪽"))
				{
					TimeOfDay = GetBoatTimeOfDay(schedule, "south");
					Log($"Southern Merlthor, {TimeOfDay}");
					await GoFish(FishBait.Krill, FishBait.ShrimpCageFeeder, "south", TimeOfDay, spot);
				}
				else if (FishingLog.AreaName.Contains("Galadion") || FishingLog.AreaName.Contains("Galadion-Bucht")
					|| FishingLog.AreaName.Contains("Baie de Galadion") || FishingLog.AreaName.Contains("ガラディオン湾")
					|| FishingLog.AreaName.Contains("加拉迪翁湾") || FishingLog.AreaName.Contains("갈라디온 만"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "galadion");
                    Log($"Galadion Bay, {TimeOfDay}");
					await GoFish(FishBait.PlumpWorm, FishBait.GlowWorm, "galadion", TimeOfDay, spot);		
				}
                else if (FishingLog.AreaName.Contains("Northern Strait") || FishingLog.AreaName.Contains("Merlthorstraße (Nord)")
					|| FishingLog.AreaName.Contains("Détroit nord de Merlthor") || FishingLog.AreaName.Contains("メルトール海峡北")
					|| FishingLog.AreaName.Contains("梅尔托尔海峡北") || FishingLog.AreaName.Contains("멜토르 해협 북쪽"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "north");
                    Log($"Northern Merlthor, {TimeOfDay}");
					await GoFish(FishBait.Ragworm, FishBait.HeavySteelJig, "north", TimeOfDay, spot);
				}
                else if (FishingLog.AreaName.Contains("Rhotano Sea") || FishingLog.AreaName.Contains("Rhotano-See")
					|| FishingLog.AreaName.Contains("Mer de Rhotano") || FishingLog.AreaName.Contains("ロータノ海")
					|| FishingLog.AreaName.Contains("罗塔诺海") || FishingLog.AreaName.Contains("로타노 해"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "rhotano");
                    Log($"Rhotano Sea, {TimeOfDay}");
					await GoFish(FishBait.Ragworm, FishBait.RatTail, "rhotano", TimeOfDay, spot);
				}
                else if (FishingLog.AreaName.Contains("Cieldalaes") || FishingLog.AreaName.Contains("Cieldaläen")
					|| FishingLog.AreaName.Contains("Cieldalaes") || FishingLog.AreaName.Contains("シェルダレー諸島")
					|| FishingLog.AreaName.Contains("谢尔达莱群岛") || FishingLog.AreaName.Contains("시엘달레 제도"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "ciel");
                    Log($"Cieldalaes, {TimeOfDay}");
					await GoFish(FishBait.Ragworm, FishBait.SquidStrip, "ciel", TimeOfDay, spot);
				}
                else if (FishingLog.AreaName.Contains("Bloodbrine") || FishingLog.AreaName.Contains("Schwerblütiges")
					|| FishingLog.AreaName.Contains("Mer Pourpre") || FishingLog.AreaName.Contains("緋汐海")
					|| FishingLog.AreaName.Contains("绯汐海") || FishingLog.AreaName.Contains("붉은물결 바다"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "blood");
                    Log($"Bloodbrine, {TimeOfDay}");
					await GoFish(FishBait.Krill, FishBait.PillBug, "blood", TimeOfDay, spot);
				}
                else if (FishingLog.AreaName.Contains("Rothlyt Sound") || FishingLog.AreaName.Contains("Rothlyt-Meerbusen")
					|| FishingLog.AreaName.Contains("Golfe de Rothlyt") || FishingLog.AreaName.Contains("ロズリト湾")
					|| FishingLog.AreaName.Contains("罗斯利特湾") || FishingLog.AreaName.Contains("로들리트 만"))
				{
                    TimeOfDay = GetBoatTimeOfDay(schedule, "sound");
                    Log($"Rothlyt Sound, {TimeOfDay}");
					await GoFish(FishBait.PlumpWorm, FishBait.Ragworm, "sound", TimeOfDay, spot);
				}
                else if (FishingLog.AreaName.Contains("Sirensong"))
                {
                    TimeOfDay = GetBoatTimeOfDay(schedule, "sirensong");
                    Log($"Sirensong Sea, {TimeOfDay}");
                    await GoFish(FishBait.Ragworm, FishBait.MackerelStrip, "sirensong", TimeOfDay, spot);
                }
                else if (FishingLog.AreaName.Contains("Kugane"))
                {
                    TimeOfDay = GetBoatTimeOfDay(schedule, "kugane");
                    Log($"Kugane, {TimeOfDay}");
                    await GoFish(FishBait.Ragworm, FishBait.Ragworm, "kugane", TimeOfDay, spot);
                }
                else if (FishingLog.AreaName.Contains("Ruby Sea"))
                {
                    TimeOfDay = GetBoatTimeOfDay(schedule, "rubysea");
                    Log($"The Ruby Sea, {TimeOfDay}");
                    await GoFish(FishBait.Ragworm, FishBait.SquidStrip, "rubysea", TimeOfDay, spot);
                }
                else if (FishingLog.AreaName.Contains("One River"))
                {
                    TimeOfDay = GetBoatTimeOfDay(schedule, "oneriver");
                    Log($"The One River, {TimeOfDay}");
                    await GoFish(FishBait.PlumpWorm, FishBait.StoneflyNymph, "oneriver", TimeOfDay, spot);
                }
                else
                {
					if (!String.IsNullOrEmpty(FishingLog.AreaName))
						Log($"Cannot determine location: {FishingLog.AreaName}");
				}

				await Coroutine.Sleep(2000);
			}


			AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
			if (windowByName != null)
			{
                // This is super sloppy as we have to rely on a bunch of sleeps right now.
                await Coroutine.Sleep(12000); 

				// What if the player already clicked the button and we're now loading or something else? This will potentially CRASH the client. Look into refining this later.
               	windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
				if (windowByName != null)
					windowByName.SendAction(1, 3, 0);
				
				if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}

				RouteShown = false;
				missingFishRefreshed = false;
                PassTheTime.freeToCraft = true;
            }

			await Coroutine.Sleep(2000);
		}

		private string GetBoatTimeOfDay(Tuple<string, string>[] schedule, string area)
		{
			string TimeOfDay = schedule.FirstOrDefault(x => x.Item1 == area).Item2;

			if (String.IsNullOrEmpty(TimeOfDay))
				TimeOfDay = "Day";

			return TimeOfDay;
		}

        private async Task ChangeBait(ulong baitId)
		{
			if ((baitId != FishingManager.SelectedBaitItemId) && (DataManager.GetItem((uint)baitId).RequiredLevel <= Core.Me.ClassLevel))
			{
				AtkAddonControl baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				if(baitWindow == null)
				{
					ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
					await Coroutine.Sleep(400);
					baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				}							
				if(baitWindow != null)
				{
					baitWindow.SendAction(4, 0, 0, 0, 0, 0, 0, 1, baitId);
					Log($"Applied {DataManager.GetItem((uint) baitId).CurrentLocaleName}");
					await Coroutine.Sleep(400);
					ActionManager.DoAction(Actions.OpenCloseBaitMenu, GameObjectManager.LocalPlayer);
				}	
			}
		}

		private async Task GoOpenWorldFishing()
		{
			if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
			{
                if ((Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
                {
                    ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
                }

                if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
                {
                    FishingManager.MoochTwo();
					biteTimer.Start();
                }
                else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch)
                {
                    FishingManager.Mooch();
                    biteTimer.Start();
                }

                FishingManager.Cast();
                biteTimer.Start();
            }

            if ((Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
            {
                ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
            }

            while (FishingManager.State != FishingState.PoleReady)
			{
                if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
				{
                    biteTimer.Stop();
                    Log($"Bite Time: {biteTimer.Elapsed.TotalSeconds:F1}s");
                    FishingManager.Hook();
                    biteTimer.Reset();
                }

                await Coroutine.Sleep(100);
            }
        }

        private async Task GoFish(ulong baitId, ulong spectralbaitId, string location, string timeOfDay, int spot)
		{
			bool spectraled = false;

            // Just in case you're already standing in a fishing spot. IE: Restarting botbase/rebornbuddy			
			if (!ActionManager.CanCast(Actions.Cast, Core.Me) && FishingManager.State == FishingState.None) 
            {
                Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
				while (fishSpots[spot].Distance2DSqr(Core.Me.Location) > 2)
				{
					Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
					await Coroutine.Sleep(100);
				}
				Navigator.PlayerMover.MoveStop();
				await Coroutine.Sleep(500);
				Core.Me.SetFacing(headings[spot]);
			}

			await Coroutine.Sleep(1000);


			while ((WorldManager.ZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby) && !ChatCheck("[NPCAnnouncements]","Weigh the anchors") && !ChatCheck("[NPCAnnouncements]", "measure your catch!"))
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

				// Should we Cordial?
				if ((Core.Me.MaxGP - Core.Me.CurrentGP) >= 400 && spectraled)
					await UseCordial();
				else if ((Core.Me.MaxGP - Core.Me.CurrentGP) >= 400 && Core.Me.CurrentGPPercent < 25.00)
					await UseCordial();

				// Should we use Thaliak's Favor?
				if (spectraled && (Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me))
				{
					Log("Using Thaliak's Favor!");
					ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
					await Coroutine.Sleep(100);
				}
				else if (!spectraled && (Core.Me.MaxGP - Core.Me.CurrentGP) > 200 && ActionManager.CanCast(Actions.ThaliaksFavor, Core.Me) && Core.Player.Auras.Any(x => x.Id == CharacterAuras.AnglersArt && x.Value >= 10))
				{
                    Log("Currently at 10 Angler's Art Stacks - Using Thaliak's Favor!");
                    ActionManager.DoAction(Actions.ThaliaksFavor, Core.Me);
                    await Coroutine.Sleep(100);
                }

                if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
				{
                    // Did we catch a fish? Let's log it.
                    if (ChatCheck("You land", "measuring") && !caughtFishLogged)
                    {
                        lastCaughtFish = FishingLog.LastFishCaught;
                        caughtFish.Add(FishingLog.LastFishCaught);
						caughtFishLogged = true;

						if (missingFish.Contains(FishingLog.LastFishCaught))
						{
							missingFish.Remove(FishingLog.LastFishCaught);
							FishingLog.SaveMissingFishLog(missingFish);
						}
                    }

					//Identical Cast for Blue fish
					if (((lastCaughtFish == OceanFish.Gugrusaurus && caughtFish.Where(x => x == OceanFish.Gugrusaurus).Count() < 3)
							|| (lastCaughtFish == OceanFish.Heavenskey && caughtFish.Where(x => x == OceanFish.Heavenskey).Count() < 2)
							|| (lastCaughtFish == OceanFish.GreatGrandmarlin && caughtFish.Where(x => x == OceanFish.GreatGrandmarlin).Count() < 2)
							|| (lastCaughtFish == OceanFish.CrimsonMonkfish && caughtFish.Where(x => x == OceanFish.CrimsonMonkfish).Count() < 2)
							|| (lastCaughtFish == OceanFish.JetborneManta && caughtFish.Where(x => x == OceanFish.JetborneManta).Count() < 2)
							|| (lastCaughtFish == OceanFish.BeatificVision && caughtFish.Where(x => x == OceanFish.BeatificVision).Count() < 3)
							|| (lastCaughtFish == OceanFish.YanxianGoby && caughtFish.Where(x => x == OceanFish.YanxianGoby).Count() < 2)
							|| (lastCaughtFish == OceanFish.CatchingCarp && caughtFish.Where(x => x == OceanFish.CatchingCarp).Count() < 3)
							|| (lastCaughtFish == OceanFish.FleetingSquid && caughtFish.Where(x => x == OceanFish.FleetingSquid).Count() < 2)
							|| (lastCaughtFish == OceanFish.CrimsonKelp && caughtFish.Where(x => x == OceanFish.CrimsonKelp).Count() < 3)
							|| (lastCaughtFish == OceanFish.Shoshitsuki && caughtFish.Where(x => x == OceanFish.Shoshitsuki).Count() < 2)
							|| (lastCaughtFish == OceanFish.SilentShark && caughtFish.Where(x => x == OceanFish.SilentShark).Count() < 2)
							|| (lastCaughtFish == OceanFish.SunkenCoelacanth && caughtFish.Where(x => x == OceanFish.SunkenCoelacanth).Count() < 3)
							|| (lastCaughtFish == OceanFish.PoetsPipe && caughtFish.Where(x => x == OceanFish.PoetsPipe).Count() < 2)
                        )
						&& (ActionManager.CanCast(Actions.IdenticalCast, Core.Me)) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))
					{
						if (ActionManager.CanCast(Actions.IdenticalCast, Core.Me))
						{
							Log("Identical Cast!");
							lastCastMooch = false;
                            ActionManager.DoAction(Actions.IdenticalCast, Core.Me);
                            biteTimer.Start();
                        }
					}

					// Check for Mooch II before using Mooch
					if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.MoochTwo)
					{
						Log("Using Mooch II!");
						FishingManager.MoochTwo();
						lastCastMooch = true;
                        biteTimer.Start();
					}
                    else if (FishingManager.CanMoochAny == FishingManager.AvailableMooch.Mooch || FishingManager.CanMoochAny == FishingManager.AvailableMooch.Both)
                    {
                        Log("Using Mooch!");
                        FishingManager.Mooch();
						lastCastMooch = true;
                        biteTimer.Start();
                    }
                    else
                    {
						if (spectraled)
						{
							//Bait for Blue fish
							if (
									Core.Player.HasAura(CharacterAuras.FishersIntuition) &&
									(
										((location == "galadion") && (timeOfDay == "Night"))
										|| ((location == "south") && (timeOfDay == "Night"))
										|| ((location == "north") && (timeOfDay == "Day"))
										|| ((location == "rhotano") && (timeOfDay == "Sunset"))
										|| ((location == "ciel") && (timeOfDay == "Night"))
										|| ((location == "blood") && (timeOfDay == "Day"))
										|| ((location == "sound") && (timeOfDay == "Sunset"))
										|| ((location == "sirensong") && (timeOfDay == "Day"))
										|| ((location == "kugane") && (timeOfDay == "Night"))
										|| ((location == "rubysea") && (timeOfDay == "Sunset"))
										|| ((location == "oneriver") && (timeOfDay == "Day"))
									)
							)
							{
								caughtFish.Clear();

								await ChangeBait(spectralbaitId);
							}
							else if ((location == "galadion") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Sothis) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Heavenskey).Count() < 2) // Needs 2 Heavenskey. Use Ragworm to catch.
									await ChangeBait(FishBait.Ragworm);
								else if (!caughtFish.Contains(OceanFish.NavigatorsPrint)) // Requires 1 Navigators Print.
									await ChangeBait(FishBait.Krill);
							}
							else if ((location == "south") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.CoralManta) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.GreatGrandmarlin).Count() < 2) // Needs 2 Great Grandmarlin. Mooch from Hi-Aetherlouse.
									await ChangeBait(FishBait.PlumpWorm);
							}
							else if ((location == "north") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Elasmosaurus) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Gugrusaurus).Count() < 3) // Needs 3 Gugrusaurus
									await ChangeBait(FishBait.PlumpWorm);
							}
							else if ((location == "rhotano") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Stonescale) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.CrimsonMonkfish).Count() < 2) // Needs 2 Crimson Monkfish
									await ChangeBait(FishBait.PlumpWorm);
							}
							else if ((location == "ciel") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.Hafgufa) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.JetborneManta).Count() < 2) // Needs 2 Jetborne Manta
									await ChangeBait(FishBait.PlumpWorm);
								else if (!caughtFish.Contains(OceanFish.MistbeardsCup)) // Needs 1 Mistbeard's Cup
									await ChangeBait(FishBait.Krill);
							}
							else if ((location == "blood") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.SeafaringToad) && FocusFishLog)
							{
								// This will help increase the chances of catching Seafaring Toad.
								UsePatience();

								// Catch 3 Beatific Vision to trigger intuition
								await ChangeBait(FishBait.Krill);
							}
							else if ((location == "sound") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.Placodus) && FocusFishLog)
							{
								UsePatience();

								// Use Ragworm to catch Rothlyt Mussel, then Mooch to Trollfish to trigger intuition.
								await ChangeBait(FishBait.Ragworm);
							}
							else if ((location == "sirensong") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.Taniwha) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.SunkenCoelacanth).Count() < 3) // Needs 3 Sunken Coelacanth
									await ChangeBait(FishBait.PlumpWorm);
							}
							else if ((location == "kugane") && (timeOfDay == "Night") && missingFish.Contains((uint)OceanFish.GlassDragon) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.Shoshitsuki).Count() < 2) // Needs Shoshitsuki
									await ChangeBait(FishBait.PlumpWorm);
								else
									await ChangeBait(FishBait.Krill); // Try to get Snapping Koban to mooch Glass Dragon
							}
							else if ((location == "rubysea") && (timeOfDay == "Sunset") && missingFish.Contains((uint)OceanFish.HellsClaw) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.FlyingSquid).Count() < 1) // Needs 1x Flying Squid
									await ChangeBait(FishBait.PlumpWorm);
								else if (caughtFish.Where(x => x == OceanFish.FleetingSquid).Count() < 2) // Needs 2x Fleeting Squid
									await ChangeBait(FishBait.PlumpWorm);
							}
							else if ((location == "oneriver") && (timeOfDay == "Day") && missingFish.Contains((uint)OceanFish.JewelofPlumSpring) && FocusFishLog)
							{
								if (caughtFish.Where(x => x == OceanFish.YanxianGoby).Count() < 2) // Needs 2x Yanxian Goby
									await ChangeBait(FishBait.Ragworm);
								else if (caughtFish.Where(x => x == OceanFish.GensuiShrimp).Count() < 1) // Needs 1x Gensui Shrimp
									await ChangeBait(FishBait.Ragworm);
							}
							else if (
									FocusFishLog &&
									(((location == "galadion") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.QuicksilverBlade) || missingFish.Contains((uint)OceanFish.FunnelShark)))
									|| ((location == "galadion") && (missingFish.Contains((uint)OceanFish.Fishmonger) || missingFish.Contains((uint)OceanFish.GhostShark)))
									|| ((location == "south") && (missingFish.Contains((uint)OceanFish.HiAetherlouse) || missingFish.Contains((uint)OceanFish.ShipwrecksSail)))
									|| ((location == "north") && (missingFish.Contains((uint)OceanFish.Coccosteus) || missingFish.Contains((uint)OceanFish.Gugrusaurus)))
									|| ((location == "rhotano") && (missingFish.Contains((uint)OceanFish.DeepSeaEel)))
									|| ((location == "rhotano") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.Sweeper) || missingFish.Contains((uint)OceanFish.Executioner)))
									|| ((location == "ciel") && (missingFish.Contains((uint)OceanFish.JetborneManta)))
									|| ((location == "ciel") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.Callichthyid)))
									|| ((location == "ciel") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.MeanderingMora)))
									|| ((location == "blood") && (missingFish.Contains((uint)OceanFish.GoryTuna) || missingFish.Contains((uint)OceanFish.Ticinepomis) || missingFish.Contains((uint)OceanFish.QuartzHammerhead)))
									|| ((location == "sound") && (missingFish.Contains((uint)OceanFish.SmoothJaguar)))
									|| ((location == "sound") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.Panoptes))))

									// Ruby Route
									|| ((location == "sirensong") && missingFish.Contains((uint)OceanFish.Broadhead) && timeOfDay != "Night")
									|| ((location == "sirensong") && (missingFish.Contains((uint)OceanFish.SunkenCoelacanth) || (missingFish.Contains((uint)OceanFish.Taniwha)) && timeOfDay == "Day"))
									|| ((location == "sirensong") && missingFish.Contains((uint)OceanFish.BlackjawedHelicoprion) && timeOfDay == "Night")
                                    || ((location == "kugane") && missingFish.Contains((uint)OceanFish.SwordtipSquid))
                                    || ((location == "kugane") && missingFish.Contains((uint)OceanFish.Shoshitsuki))
                                    || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.FleetingSquid))
                                    || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.TentacaleThresher))
                                    || ((location == "rubysea") && timeOfDay == "Night" && missingFish.Contains((uint)OceanFish.CrimsonSentry))
                                    || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.FlyingSquid))
                                    || ((location == "oneriver") && missingFish.Contains((uint)OceanFish.YatonoKami))
                                    || ((location == "oneriver") && missingFish.Contains((uint)OceanFish.HeronsEel))
                                    || ((location == "oneriver") && timeOfDay == "Night" && missingFish.Contains((uint)OceanFish.Gakugyo))
                            )
                            {
								await ChangeBait(FishBait.PlumpWorm);
							}
							else if (
								FocusFishLog &&
								(
									((location == "galadion") && (missingFish.Contains((uint)OceanFish.NavigatorsPrint) || missingFish.Contains((uint)OceanFish.MermansMane)))
									|| ((location == "south") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.MythrilSovereign)))
									|| ((location == "south") && (missingFish.Contains((uint)OceanFish.CharlatanSurvivor) || missingFish.Contains((uint)OceanFish.AzeymasSleeve)))
									|| ((location == "north") && (missingFish.Contains((uint)OceanFish.Hammerclaw)))
									|| ((location == "north") && (timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.Mopbeard)))
									|| ((location == "north") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.TheFallenOne)))
									|| ((location == "rhotano") && (missingFish.Contains((uint)OceanFish.Aronnax) || missingFish.Contains((uint)OceanFish.TrueBarramundi) || missingFish.Contains((uint)OceanFish.ProdigalSon)))
									|| ((location == "rhotano") && (timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.FloatingSaucer)))
									|| ((location == "ciel") && (missingFish.Contains((uint)OceanFish.AnomalocarisSaron) || missingFish.Contains((uint)OceanFish.TitanshellCrab) || missingFish.Contains((uint)OceanFish.MistbeardsCup)))
									|| ((location == "ciel") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.DevilsSting)))
									|| ((location == "ciel") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.FlamingEel)))
									|| ((location == "blood") && (missingFish.Contains((uint)OceanFish.DravanianBream) || missingFish.Contains((uint)OceanFish.BeatificVision)))
									|| ((location == "blood") && (timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.Skaldminni)))
									|| ((location == "sound") && (missingFish.Contains((uint)OceanFish.LeviElver) || missingFish.Contains((uint)OceanFish.Knifejaw)))
									|| ((location == "sound") && (timeOfDay == "Day" || timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.PearlBombfish)))

									// Ruby Route
									|| ((location == "sirensong" && timeOfDay == "Night" && missingFish.Contains((uint)OceanFish.VividPinkShrimp)))
									|| ((location == "sirensong") && (missingFish.Contains((uint)OceanFish.Impostopus) || missingFish.Contains((uint)OceanFish.JadeShrimp)))
									|| ((location == "sirensong") && timeOfDay != "Night" && missingFish.Contains((uint)OceanFish.NymeiasWheel))
									|| ((location == "kugane") && missingFish.Contains((uint)OceanFish.SnappingKoban))
                                    || ((location == "kugane") && timeOfDay == "Day" && missingFish.Contains((uint)OceanFish.StingfinTrevally))
                                    || ((location == "kugane") && missingFish.Contains((uint)OceanFish.IdatensBolt))
                                    || ((location == "kugane") && timeOfDay == "Sunset" && missingFish.Contains((uint)OceanFish.Spadefish))
                                    || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.FleetingSquid))
                                    || ((location == "rubysea") && timeOfDay == "Night" && missingFish.Contains((uint)OceanFish.BekkoRockhugger))
                                    || ((location == "oneriver") && timeOfDay == "Sunset" && missingFish.Contains((uint)OceanFish.IridescentTrout))
                                    || ((location == "oneriver") && timeOfDay == "Night" && missingFish.Contains((uint)OceanFish.UnNamazu))
                                    || ((location == "oneriver") && timeOfDay == "Sunset" && missingFish.Contains((uint)OceanFish.GinrinGoshiki))
                                )
                            )
							{
								await ChangeBait(FishBait.Krill);
							}
							else if (
								FocusFishLog &&
								(((location == "galadion") && (missingFish.Contains((uint)OceanFish.Heavenskey)))
								|| ((location == "galadion") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.CasketOyster) || missingFish.Contains((uint)OceanFish.NimbleDancer)))
								|| ((location == "south") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.SeaNettle)))
								|| ((location == "north") && (missingFish.Contains((uint)OceanFish.Prowler) || missingFish.Contains((uint)OceanFish.WildUrchin)))
								|| ((location == "north") && (timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.BartholomewTheChopper)))
								|| ((location == "north") && (timeOfDay == "Sunset") && (missingFish.Contains((uint)OceanFish.CoralSeadragon)))
								|| ((location == "rhotano") && (missingFish.Contains((uint)OceanFish.Silencer)))
								|| ((location == "rhotano") && (timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.Slipsnail)))
								|| ((location == "ciel") && (missingFish.Contains((uint)OceanFish.MythrilBoxfish)))
								|| ((location == "blood") && (missingFish.Contains((uint)OceanFish.SerratedClam)))
								|| ((location == "blood") && (timeOfDay == "Day") && (missingFish.Contains((uint)OceanFish.OracularCrab) || missingFish.Contains((uint)OceanFish.Exterminator)))
								|| ((location == "sound") && (missingFish.Contains((uint)OceanFish.RothlytMussel) || missingFish.Contains((uint)OceanFish.CrepeSole)))
								|| ((location == "sound") && (timeOfDay == "Day" || timeOfDay == "Night") && (missingFish.Contains((uint)OceanFish.GarumJug))))

								// Ruby Route
                                || ((location == "sirensong") && missingFish.Contains((uint)OceanFish.MermaidScale))
                                || ((location == "sirensong") && missingFish.Contains((uint)OceanFish.SirensSigh))
                                || ((location == "kugane") && missingFish.Contains((uint)OceanFish.SilkweftPrawn))
                                || ((location == "kugane") && timeOfDay == "Sunset" && missingFish.Contains((uint)OceanFish.Mailfish))
                                || ((location == "kugane") && timeOfDay != "Night" && missingFish.Contains((uint)OceanFish.MaelstromTurban))
                                || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.BowbarbLobster))
                                || ((location == "rubysea") && timeOfDay == "Day" && missingFish.Contains((uint)OceanFish.PitchPickle))
                                || ((location == "rubysea") && missingFish.Contains((uint)OceanFish.SenbeiOctopus))
                                || ((location == "rubysea") && timeOfDay == "Day" && missingFish.Contains((uint)OceanFish.YellowIris))
                                || ((location == "oneriver") && missingFish.Contains((uint)OceanFish.GensuiShrimp))
                                || ((location == "oneriver") && missingFish.Contains((uint)OceanFish.CrowshadowMussel))
                                || ((location == "oneriver") && missingFish.Contains((uint)OceanFish.YanxianGoby))
                            )
                            {
								await ChangeBait(FishBait.Ragworm);
							}
							else if (
										((location == "galadion") && (timeOfDay == "Sunset"))
										|| ((location == "rhotano") && (timeOfDay == "Day"))
										|| ((location == "north") && (timeOfDay == "Day"))
										|| ((location == "south") && (timeOfDay == "Night"))
										|| ((location == "ciel") && (timeOfDay == "Sunset"))
										|| ((location == "blood") && (timeOfDay == "Sunset"))
                                        || ((location == "rubysea"))
                            )
                            {
								await ChangeBait(FishBait.PlumpWorm);
							}
							else if (
										((location == "south") && (timeOfDay == "Day"))
										|| ((location == "rhotano") && (timeOfDay == "Night")
											&& (!FocusFishLog || !missingFish.Contains((uint)OceanFish.Slipsnail)))
										|| ((location == "north") && (timeOfDay == "Sunset")
											&& missingFish.Contains((uint)OceanFish.TheFallenOne)
											&& FocusFishLog)
										|| ((location == "north") && (timeOfDay == "Night")
											&& (!FocusFishLog
                                            || !missingFish.Contains((uint)OceanFish.BartholomewTheChopper)))
										|| ((location == "galadion") && (timeOfDay == "Night"))
										|| ((location == "ciel") && (timeOfDay == "Day" || timeOfDay == "Night"))
										|| ((location == "blood") && (timeOfDay == "Night"))
										|| ((location == "sound"))
                                        || ((location == "kugane"))
                                        || ((location == "sirensong"))
                                        || ((location == "oneriver"))
                            )
                            {
								await ChangeBait(FishBait.Krill);
							}
							else if (
										((location == "galadion") && (timeOfDay == "Day"))
										|| ((location == "south") && (timeOfDay == "Sunset"))
										|| ((location == "north") && (timeOfDay == "Sunset"))
										|| ((location == "north") && (timeOfDay == "Sunset"))
										|| ((location == "rhotano") && (timeOfDay == "Sunset"))
										|| ((location == "blood") && (timeOfDay == "Day"))
							)
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
							// Deal with Intuition fish first... if we have the intution buff
							if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "galadion" || location == "rhotano" || location == "ciel" || location == "blood" || location == "rubysea"))
								await ChangeBait(FishBait.Krill);
							else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && ((location == "south" && ((WorldManager.CurrentWeather != "Wind" && WorldManager.CurrentWeather != "Gales"))) || location == "sirensong" || location == "oneriver"))
								await ChangeBait(FishBait.PlumpWorm);
							else if (Core.Player.HasAura(CharacterAuras.FishersIntuition) && (location == "sound" || location == "north" || location == "kugane"))
								await ChangeBait(FishBait.Ragworm);

							// Deal with all the rest
							else if (FocusFishLog && (
										(location == "galadion" && missingFish.Contains((uint)OceanFish.Jasperhead) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog"))
										|| (location == "galadion" && (missingFish.Contains((uint)OceanFish.CyanOctopus) || missingFish.Contains((uint)OceanFish.RosyBream) || missingFish.Contains((uint)OceanFish.GaladionChovy)))
										|| (location == "south" && ((WorldManager.CurrentWeather != "Wind" && WorldManager.CurrentWeather != "Gales") && !Core.Player.HasAura(CharacterAuras.FishersIntuition) && missingFish.Contains((uint)OceanFish.LittleLeviathan)))
										|| (location == "north" && missingFish.Contains((uint)OceanFish.TripodFish) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog"))
										|| (location == "north" && (missingFish.Contains((uint)OceanFish.MerlthorLobster) || missingFish.Contains((uint)OceanFish.NetCrawler)))
										|| (location == "rhotano" && (missingFish.Contains((uint)OceanFish.DeepPlaice) && WorldManager.CurrentWeather != "Dust Storms"))
										|| (location == "rhotano" && missingFish.Contains((uint)OceanFish.RhotanoWahoo) && WorldManager.CurrentWeather != "Heat Waves")
										|| (location == "rhotano" && missingFish.Contains((uint)OceanFish.DarkNautilus))
										|| (location == "ciel" && missingFish.Contains((uint)OceanFish.Watermoura) && (WorldManager.CurrentWeather != "Thunderstorms"))
										|| (location == "ciel" && (missingFish.Contains((uint)OceanFish.LavandinRemora) || missingFish.Contains((uint)OceanFish.TortoiseshellCrab)))
										|| (location == "blood" && (missingFish.Contains((uint)OceanFish.BlueStitcher) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")))
                                        || (location == "sound" && missingFish.Contains((uint)OceanFish.LivingLantern) && WorldManager.CurrentWeather != "Thunder")
                                        || (location == "sound" && (missingFish.Contains((uint)OceanFish.NephriteEel) || missingFish.Contains((uint)OceanFish.ThavnairianShark)))
										|| (location == "sirensong" && (missingFish.Contains((uint)OceanFish.Arrowhead) || missingFish.Contains((uint)OceanFish.PinkShrimp)))
										|| (location == "sirensong" && (WorldManager.CurrentWeather != "Thunderstorms") && missingFish.Contains(OceanFish.SirensongMullet))
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.RubyHerring) && WorldManager.CurrentWeather != "Rain"))
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.RubescentTatsunoko)) && WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.SpectralWrasse)) && WorldManager.CurrentWeather != "Clear Skies")
										|| (location == "rubysea" && (missingFish.Contains((uint)OceanFish.ReefSquid) || missingFish.Contains((uint)OceanFish.MantleMoray)))
										|| (location == "rubysea" && (missingFish.Contains((uint)OceanFish.SpectralSnakeEel)) && WorldManager.CurrentWeather != "Clear Skies")
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.GarleanBluegill)) && WorldManager.CurrentWeather != "Rain" && WorldManager.CurrentWeather != "Showers")
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.YanxianSoftshell)))
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.BrocadeCarp)))
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.SpectralKotsuZetsu)) && WorldManager.CurrentWeather != "Clear Skies")
                                    ))
								await ChangeBait(FishBait.Krill);

							else if (FocusFishLog && (
									(location == "galadion" && missingFish.Contains((uint)OceanFish.TarnishedShark) && WorldManager.CurrentWeather != "Showers")
									|| (location == "galadion" && (missingFish.Contains((uint)OceanFish.LeopardEel) && (WorldManager.CurrentWeather != "Rain" && WorldManager.CurrentWeather != "Showers")))
									|| (location == "north" && (missingFish.Contains((uint)OceanFish.Megasquid) || missingFish.Contains((uint)OceanFish.OschonsStone)))
									|| (location == "rhotano" && missingFish.Contains((uint)OceanFish.OgreEel) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog"))
									|| (location == "rhotano" && missingFish.Contains((uint)OceanFish.Sabaton) && !Core.Player.HasAura(CharacterAuras.FishersIntuition)) // To catch Sabaton, need 2 Crimson Monkfish
									|| (location == "ciel" && missingFish.Contains((uint)OceanFish.KingCobrafish) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog"))
									|| (location == "ciel" && (missingFish.Contains((uint)OceanFish.GoobbueRay) || missingFish.Contains((uint)OceanFish.MamahiMahi)))
									|| (location == "blood" && missingFish.Contains((uint)OceanFish.BloodedWrasse) && WorldManager.CurrentWeather != "Showers")
									|| (location == "blood" && missingFish.Contains((uint)OceanFish.BloodfreshTuna))
									|| (location == "sound" && missingFish.Contains((uint)OceanFish.Lansquenet) && WorldManager.CurrentWeather != "Thunderstorms")
									|| (location == "sirensong" && missingFish.Contains((uint)OceanFish.DeepshadeSardine) && WorldManager.CurrentWeather != "Thunderstorms" && WorldManager.CurrentWeather != "Rain")
									|| (location == "sirensong" && missingFish.Contains((uint)OceanFish.MarineMatanga))
									|| (location == "sirensong" && missingFish.Contains((uint)OceanFish.SpectralCoelacanth) && WorldManager.CurrentWeather != "Clear Skies")
									|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.LeopardPrawn) || missingFish.Contains((uint)OceanFish.SpearSquid)))
									|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.FloatingLantern) && WorldManager.CurrentWeather != "Showers"))
                                    || (location == "kugane" && (missingFish.Contains((uint)OceanFish.SilentShark))) // Mooch from Leopard Prawn!
									|| (location == "rubysea" && (missingFish.Contains((uint)OceanFish.Sanbaso)) && WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")
									|| (location == "rubysea" && (missingFish.Contains((uint)OceanFish.VioletSentry)))
									|| (location == "rubysea" && (missingFish.Contains((uint)OceanFish.SpectralSnakeEel)) && WorldManager.CurrentWeather != "Clear Skies")
                                    || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.Calligraph)) && WorldManager.CurrentWeather != "Showers")
                                    || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.YanxianSturgeon)))
                                    || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.SpectralKotsuZetsu)) && WorldManager.CurrentWeather != "Cloud Skies")
                                ))
								await ChangeBait(FishBait.PlumpWorm);

							else if (FocusFishLog && (
									(location == "galadion" && (missingFish.Contains((uint)OceanFish.GaladionGoby) || missingFish.Contains((uint)OceanFish.Heavenswimmer)))
										|| (location == "south" && (WorldManager.CurrentWeather != "Clouds" || WorldManager.CurrentWeather != "Fog") && missingFish.Contains((uint)OceanFish.ShaggySeadragon))
										|| (location == "south" && (missingFish.Contains((uint)OceanFish.MerlthorButterfly) || missingFish.Contains((uint)OceanFish.Sunfly) || missingFish.Contains((uint)OceanFish.LaNosceanJelly)))
										|| (location == "north" && missingFish.Contains((uint)OceanFish.ShootingStar))
										|| (location == "north" && (missingFish.Contains((uint)OceanFish.Floefish) && (WorldManager.CurrentWeather != "Blizzards" && WorldManager.CurrentWeather != "Snow")))
										|| (location == "rhotano" && (missingFish.Contains((uint)OceanFish.RhotanoSardine) || missingFish.Contains((uint)OceanFish.Lampfish)))
										|| (location == "ciel" && missingFish.Contains((uint)OceanFish.LadysCameo) && (WorldManager.CurrentWeather != "Thunder"))
										|| (location == "ciel" && missingFish.Contains((uint)OceanFish.CieldalaesGeode) && !Core.Player.HasAura(CharacterAuras.FishersIntuition)) // 3 Metallic Boxfish required to get Cieldalaes Geode
										|| (location == "blood" && missingFish.Contains((uint)OceanFish.StarOfTheDestroyer) && WorldManager.CurrentWeather != "Rain")
										|| (location == "blood" && (missingFish.Contains((uint)OceanFish.ThaliakCrab) || missingFish.Contains((uint)OceanFish.BloodpolishCrab)))
										|| (location == "blood" && (missingFish.Contains((uint)OceanFish.Bareface) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))) // Need Bareface but doesn't have intuition - Catch Sunken Mask using Ragworm
                                        || (location == "sound" && (missingFish.Contains((uint)OceanFish.Godsbed) && (WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")))
                                        || (location == "sound" && (missingFish.Contains((uint)OceanFish.CrowPuffer) || missingFish.Contains((uint)OceanFish.HoneycombFish)))
                                        || (location == "sound" && (missingFish.Contains((uint)OceanFish.GinkgoFin) && !Core.Player.HasAura(CharacterAuras.FishersIntuition))) // Needs Ginkgo Fin but doesn't have intuition - Use Ragworm to get 3 Rothlyt Kelp
										|| (location == "sirensong" && missingFish.Contains((uint)OceanFish.SirensongMussel))
										|| (location == "sirensong" && missingFish.Contains((uint)OceanFish.SelkiePuffer) && WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")
										|| (location == "sirensong" && (missingFish.Contains((uint)OceanFish.PoetsPipe) || missingFish.Contains((uint)OceanFish.DuskShark))) // Dusk Shark needs 2x Poet's Pipe for Intuition
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.WhirlpoolTurban)))
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.LeopardPrawn)))
										|| (location == "kugane" && (missingFish.Contains((uint)OceanFish.Hatatate)))
                                        || (location == "rubyseas" && (missingFish.Contains((uint)OceanFish.CrimsonKelp)))
                                        || (location == "rubyseas" && (missingFish.Contains((uint)OceanFish.PinebarkFlounder)) && WorldManager.CurrentWeather != "Wind" && WorldManager.CurrentWeather != "Gales" && WorldManager.CurrentWeather != "Thunder")
                                        || (location == "rubyseas" && (missingFish.Contains((uint)OceanFish.BardedLobster)))
                                        || (location == "rubyseas" && (missingFish.Contains((uint)OceanFish.ShisuiGoby)))
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.CatchingCarp)))
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.PrincessSalmon)) && WorldManager.CurrentWeather != "Clouds" && WorldManager.CurrentWeather != "Fog")
                                        || (location == "oneriver" && (missingFish.Contains((uint)OceanFish.SingularShrimp)))
                                ))
								await ChangeBait(FishBait.Ragworm);

							else if (location == "galadion" || location == "rhotano" || location == "sound" || location == "oneriver")
								await ChangeBait(FishBait.PlumpWorm);
							else if (location == "south" || location == "blood")
								await ChangeBait(FishBait.Krill);
							else if (location == "north" || location == "ciel" || location == "sirensong" || location == "kugane" || location == "rubysea")
								await ChangeBait(FishBait.Ragworm);
							else
								await ChangeBait(baitId);


                            // Should we use Chum?
                            if (Core.Me.MaxGP >= 100 && ((Core.Me.MaxGP - Core.Me.CurrentGP) <= 100) && OceanTripSettings.Instance.FullGPAction == FullGPAction.Chum)
                            {
                                if (ActionManager.CanCast(Actions.Chum, Core.Me))
                                {
                                    Log("Triggering Full GP Action to keep regen going - Chum!");
                                    ActionManager.DoAction(Actions.Chum, Core.Me);
                                }
                            }
                        }

						FishingManager.Cast();
                        biteTimer.Start();
						lastCastMooch = false;

                    }

					await Coroutine.Sleep(50);
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
                            biteTimer.Stop();
							biteTimer.Reset();
                        }
                    }

					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
					{
						biteTimer.Stop();
						bool doubleHook = false;
						bool shouldMooch = false;
						int biteElapsed = (int)Math.Floor(biteTimer.Elapsed.TotalSeconds);

						Log($"Bite Time: {biteTimer.Elapsed.TotalSeconds:F1}s");

						// Made this more readable.
						if (location == "galadion" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto)) //Galadion Bay					
                        {
							if (timeOfDay == "Day")
							{
								// Day - ! tug at 8+ seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 7)
									doubleHook = true;

								// Day - !! tug at 2 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed <= 3)
									doubleHook = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - !!! tug
								if (FishingManager.TugType == TugType.Heavy)
									doubleHook = true;

								// Sunset - !! tug at 2 or 6 seconds
								if ( FishingManager.TugType == TugType.Medium && ((biteElapsed >= 1 && biteElapsed <= 3) || (biteElapsed >= 5 && biteElapsed <= 7)))
									doubleHook = true;
							}

							if (timeOfDay == "Night")
							{
								// Night - !! tug at 2 or 4 seconds
								if (FishingManager.TugType == TugType.Medium && ((biteElapsed >= 1 && biteElapsed <= 3) || (biteElapsed >= 3 && biteElapsed <= 5)))
									doubleHook = true;
							}
						}

						if (location == "south" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto)) //Southern Merlthor
                        {
							if (timeOfDay == "Day")
							{
								// Day - !!! tug
								if (FishingManager.TugType == TugType.Heavy)
									doubleHook = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - ! tug at 8+ seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 7)
									doubleHook = true;

								// Sunset - Any fish after a Mooch
								if (lastCastMooch)
									doubleHook = true;
							}

							if (timeOfDay == "Night")
							{
								// Night - Any fish after a mooch
								if (lastCastMooch)
									doubleHook = true;
							}
						}

						if (location == "north" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
                                // Day - !!! tug
                                if (FishingManager.TugType == TugType.Heavy)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Sunset")
							{
                                // Sunset - ! tug at 8+ seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 7)
                                    doubleHook = true;

                                // Sunset - !!! tug
                                if (FishingManager.TugType == TugType.Heavy)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Night")
							{
                                // Night - !! tug at 6+ seconds
                                if (FishingManager.TugType == TugType.Medium && biteElapsed >= 5)
                                    doubleHook = true;

                                // Night - !!! tug
                                if (FishingManager.TugType == TugType.Heavy)
                                    doubleHook = true;
                            }
                        }

						if (location == "rhotano" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
								// Day - !!! tug
								if (FishingManager.TugType == TugType.Heavy)
									doubleHook = true;

								// Day - !! tug at 6 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 5 && biteElapsed <= 7)
									doubleHook = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - ! tug at 4-6 seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 3 && biteElapsed <= 7)
									doubleHook = true;
							}

							if (timeOfDay == "Night")
							{
								// ! tug at 7+ seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 6)
									doubleHook = true;

								// ! tug at 4-6 seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 3 && biteElapsed <= 7)
									doubleHook = true;
							}
						}

						if (location == "ciel" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
								// Day - !! tug at 6+ seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 5)
									doubleHook = true;

								// Day - !! tug at 2-3 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 1 && biteElapsed <= 4)
									doubleHook = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - !! tug at 7+ seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 6)
									doubleHook = true;

								// Sunset - !! tug at 2-3 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 1 && biteElapsed <= 4)
									doubleHook = true;
							}

							if (timeOfDay == "Night")
							{
								// Night - !! tug at 2-3 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 1 && biteElapsed <= 4)
									doubleHook = true;
							}
						}

						if (location == "blood" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
								// Day - ! tug at 6+ seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 5)
									doubleHook = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - !!! tug at 6+ seconds
								if (FishingManager.TugType == TugType.Heavy && biteElapsed >= 5)
									doubleHook = true;

								// Sunset - !! tug at 2 seconds 
								if (FishingManager.TugType == TugType.Medium && biteElapsed <= 3)
									doubleHook = true;
							}

							if (timeOfDay == "Night")
							{
								// Night - !! tug at 6+ seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 6)
									doubleHook = true;

								// Night - !! tug at 2 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed <= 3)
									doubleHook = true;
							}
						}

						if (location == "sound" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
								// Day - !! tug at 5 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 4 && biteElapsed <= 6)
									doubleHook = true;

								// Day - !! mooch at 6+ seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 5)
									shouldMooch = true;
							}

							if (timeOfDay == "Sunset")
							{
								// Sunset - !! tug at 8 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 7 && biteElapsed <= 9)
									doubleHook = true;

								// Sunset - ! mooch at 3-5 seconds
								if (FishingManager.TugType == TugType.Light && biteElapsed >= 2 && biteElapsed <= 6)
									shouldMooch = true;
							}

							if (timeOfDay == "Night")
							{
								// Night - !! tug at 5 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 4 && biteElapsed <= 6)
									doubleHook = true;

								// Night - !! tug at 8 seconds
								if (FishingManager.TugType == TugType.Medium && biteElapsed >= 7 && biteElapsed <= 9)
									doubleHook = true;
							}
						}

						// Ruby Route
						if (location == "sirensong" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
						{
							if (timeOfDay == "Day")
							{
                                // Day - ! tug at 13-20 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 13 && biteElapsed <= 20)
                                    doubleHook = true;

                                // Day - ! tug at 10+ seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 10)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Sunset")
                            {
                                // Day - ! tug at 13-20 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 13 && biteElapsed <= 20)
                                    doubleHook = true;

                                // Sunset - ! tug at 10+ seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 10)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Night")
                            {
                                // Day - ! tug at 13-20 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 13 && biteElapsed <= 20)
                                    doubleHook = true;

                                // Night - ! tug at 10+ seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 10)
                                    doubleHook = true;
                            }
                        }

                        if (location == "kugane" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
                        {
                            if (timeOfDay == "Day")
                            {
                                // Day - ! tug at 9-13 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 8 && biteElapsed <= 14)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Sunset")
                            {
                                // Sunset - ! tug at 9-13 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 8 && biteElapsed <= 14)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Night")
                            {
                                // Night - ! tug at 9-13 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 8 && biteElapsed <= 14)
                                    doubleHook = true;
                            }
                        }

                        if (location == "rubysea" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
                        {
                            if (timeOfDay == "Day")
                            {
                                // Day - ! tug at 10-18 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 9 && biteElapsed <= 19)
                                    doubleHook = true;

                                // Day - ! tug at 20-30 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 19 && biteElapsed <= 31)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Sunset")
                            {
                                // Sunset - ! tug at 10-18 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 9 && biteElapsed <= 19)
                                    doubleHook = true;

                                // Sunset - ! tug at 20-30 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 19 && biteElapsed <= 31)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Night")
                            {
                                // Night - ! tug at 10-18 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 9 && biteElapsed <= 19)
                                    doubleHook = true;

                                // Night - ! tug at 20-30 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 19 && biteElapsed <= 31)
                                    doubleHook = true;
                            }
                        }

                        if (location == "oneriver" && (OceanTripSettings.Instance.FishPriority == FishPriority.Points || OceanTripSettings.Instance.FishPriority == FishPriority.Auto))
                        {
                            if (timeOfDay == "Day")
                            {
                                // Day - ! tug at 19-28 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 18 && biteElapsed <= 29)
                                    doubleHook = true;

                                // Day - !! tug at 16-17 seconds
                                if (FishingManager.TugType == TugType.Medium && biteElapsed >= 15 && biteElapsed <= 18)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Sunset")
                            {
                                // Sunset - ! tug at 19-28 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 18 && biteElapsed <= 29)
                                    doubleHook = true;

                                // Sunset - !! tug at 16-17 seconds
                                if (FishingManager.TugType == TugType.Medium && biteElapsed >= 15 && biteElapsed <= 18)
                                    doubleHook = true;
                            }

                            if (timeOfDay == "Night")
                            {
                                // Night - ! tug at 19-28 seconds
                                if (FishingManager.TugType == TugType.Light && biteElapsed >= 18 && biteElapsed <= 29)
                                    doubleHook = true;

                                // Night - !! tug at 16-17 seconds
                                if (FishingManager.TugType == TugType.Medium && biteElapsed >= 15 && biteElapsed <= 18)
                                    doubleHook = true;
                            }
                        }

                        if (doubleHook && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && spectraled)
						{
							Log("Using Double Hook!");
							ActionManager.DoAction(Actions.DoubleHook, Core.Me);
							lastCastMooch = false;
						}
						else if (FishingManager.HasPatience)
						{
							if (FishingManager.TugType == TugType.Light)
							{
								ActionManager.DoAction(Actions.PrecisionHookset, Core.Me);
                                lastCastMooch = false;
                            }
                            else
							{
								ActionManager.DoAction(Actions.PowerfulHookset, Core.Me);
                                lastCastMooch = false;
                            }
                        }
						else
						{
							if (!spectraled && Core.Me.MaxGP >= 500 && ((Core.Me.MaxGP - Core.Me.CurrentGP) <= 100) && ActionManager.CanCast(Actions.DoubleHook, Core.Me) && OceanTripSettings.Instance.FullGPAction == FullGPAction.DoubleHook)
							{
								Log("Triggering Full GP Action to keep regen going - Double Hook!");
								ActionManager.DoAction(Actions.DoubleHook, Core.Me);
							}
							else
								FishingManager.Hook();


							lastCastMooch = false;
                        }

                        biteTimer.Reset();
						caughtFishLogged = false;

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

			if (!PartyManager.IsInParty || (PartyManager.IsInParty && PartyManager.IsPartyLeader && !PartyManager.CrossRealm))
			{
                // Wait for party members to be nearby - Thanks zzi and nt153133!
                await Coroutine.Wait(TimeSpan.FromMinutes(30), PartyLeaderWaitConditions);

                if (Dryskthota != null && Dryskthota.IsWithinInteractRange)
				{
					Dryskthota.Interact();
					if (await Coroutine.Wait(5000, () => Talk.DialogOpen))
					{
						Talk.Next();
					}

					await Coroutine.Wait(5000, () => SelectString.IsOpen);

                    // Click ready to board
                    if (SelectString.IsOpen)
					{ 
						SelectString.ClickSlot(0);

                        await Coroutine.Sleep(1000); // Sleep for a second

                        // Select route (0 = Indigo, 1 = Ruby)
                        SelectString.ClickSlot((uint)OceanTripSettings.Instance.FishingRoute);

                        await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                        SelectYesno.Yes();
					}
				}
			}

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
            while (WorldManager.ZoneId != Zones.TheEndeavor && WorldManager.RawZoneId != Zones.TheEndeaver_Ruby)
            {
                await Coroutine.Sleep(1000);
            }
            await Coroutine.Sleep(2500);
            Logging.Write(Colors.Aqua, "We're on the boat!");
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
			uint cordial = 0;

			if (DataManager.GetItem(Cordials.HiCordial).ItemCount() > 0)
				cordial = Cordials.HiCordial;
			else if (DataManager.GetItem(Cordials.Cordial).ItemCount() > 0)
				cordial = Cordials.Cordial;
			else if (DataManager.GetItem(Cordials.WateredCordial).ItemCount() > 0)
				cordial = Cordials.WateredCordial;

			// Yay, we have a cordial!
			if (cordial > 0 && InventoryManager.FilledSlots.Any(x => x.RawItemId == cordial))
			{
				var slot = InventoryManager.FilledSlots.First(x => x.RawItemId == cordial);
                await Coroutine.Sleep(2000);

                if (slot.UseItem())
					Logging.Write(Colors.Aqua, $"[Ocean Trip] Used a {DataManager.GetItem(cordial).CurrentLocaleName}!");

				await Coroutine.Sleep(100);
			}
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
			List<uint> baitList = new List<uint>();

			baitList.Add(FishBait.Ragworm);
			baitList.Add(FishBait.Krill);
			baitList.Add(FishBait.PlumpWorm);
			baitList.Add(FishBait.RatTail);
			baitList.Add(FishBait.GlowWorm);
			baitList.Add(FishBait.HeavySteelJig);
			baitList.Add(FishBait.ShrimpCageFeeder);
			baitList.Add(FishBait.PillBug);
            baitList.Add(FishBait.SquidStrip);
			baitList.Add(FishBait.MackerelStrip);

            foreach (var bait in baitList)
			{
				var baitFound = InventoryManager.FilledSlots.FirstOrDefault(x => x.RawItemId == bait);

				if ((baitFound != null && ((bait == FishBait.HeavySteelJig && baitFound.Count < 5) || (bait != FishBait.HeavySteelJig && baitFound.Count < baitThreshold)))
					|| baitFound is null)
					itemsToBuy.Add(bait);
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

				if (itemsToBuy.Contains(FishBait.SquidStrip))
				{
					await PassTheTime.IdleLisbeth((int)FishBait.SquidStrip, 300, "Exchange", "true", 0);
				}

				if (itemsToBuy.Contains(FishBait.MackerelStrip))
				{
                    await PassTheTime.IdleLisbeth((int)FishBait.MackerelStrip, 300, "Exchange", "true", 0);
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

		private bool FocusFishLog
		{
			get
			{
				if (OceanTripSettings.Instance.FishPriority == FishPriority.FishLog || OceanTripSettings.Instance.FishPriority == FishPriority.Auto)
					return true;

				return false;
			}
		}

		private void UsePatience()
		{
            if (ActionManager.CanCast(Actions.PatienceII, Core.Me) && !FishingManager.HasPatience)
                ActionManager.DoAction(Actions.PatienceII, Core.Me);
            else if (ActionManager.CanCast(Actions.Patience, Core.Me) && !FishingManager.HasPatience)
                ActionManager.DoAction(Actions.Patience, Core.Me);
        }

        public static Tuple<string, string>[] GetSchedule(DateTime? time = null)
		{
			int epoch;

            if (!time.HasValue)
                epoch = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            else
                epoch = (int)(time.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            if (OceanTripSettings.Instance.FishingRoute == FishingRoute.Ruby)
			{
                // Thanks to https://millhio.re/oceancalculator2.html which was more accurate than what I was using. Translated the JS over to C#.
                int twoHourChunk = ((epoch / 7200) + 40) % ruby_fullPattern.Length;

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

        private static bool PartyLeaderWaitConditions()
        {
            return PartyManager.VisibleMembers.Count() == PartyManager.AllMembers.Count();
        }

        private void Log(string text, params object[] args)
		{
			var msg = string.Format("[Ocean Trip] " + text, args);
			Logging.Write(Colors.Aqua, msg);
		}
	}
}