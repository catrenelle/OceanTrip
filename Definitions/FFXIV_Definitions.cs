using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OceanTripPlanner.Definitions
{
	/// <summary>
	/// Log levels for structured logging throughout the application
	/// </summary>
	public enum OceanLogLevel
	{
		/// <summary>Always logged - critical information and errors</summary>
		Always = 0,

		/// <summary>Informational messages - logged by default</summary>
		Info = 1,

		/// <summary>Debug/verbose messages - logged only when LoggingMode is enabled</summary>
		Debug = 2
	}

	public static class Defaults
	{
		// Replaces the old Heavensward-patch (3.1/3.55) Purple Gatherers' Scrip items (Dinosaur
		// Leather, Sphalerite, Royal Mistletoe, Cloud Cotton Boll, Cloud Mythril Ore, Stormcloud
		// Cotton Boll) — confirmed 2026-08-12 against the live exchange that the Gatherers' Scrip
		// "Materials" category is just Aethersand now (already its own card/list, see aethersands
		// below), so that raw-ore/leather/cloth category has no current-tier equivalent to swap in.
		// The full current Orange Crafters' Scrip "Materials" list took its place instead.
		public static readonly int[] materials = new int[]
		{
			Material.CondensedSolution,
			Material.RumplessChicken,
			Material.BrownCardamom,
			Material.WildCoffeeBeans,
			Material.NavelOrange,
			Material.RoyalLobster,
			Material.Cassava,
			Material.SplendidMateLeaves,
			Material.AjiAmarillo,
			Material.QuesoFresco,
			Material.WoolbackLoin,
			Material.FlintCorn,
			Material.TuraliPlum,
			Material.RroneekMilk,
			Material.RockFistPopoto,
			Material.Quahog
		};

		// Everborn/Everdeep (pre-Endwalker) and Endstone/Endwood/Endtide/Earthbreak (Endwalker) are
		// all gone from the current Orange Gatherers' Scrip exchange — confirmed 2026-08-12 against
		// the live in-game exchange window, which lists exactly Levinchrome, Sungilt, Mythloam,
		// Mythroot, and Mythbrine. Levinchrome and Sungilt were missing entirely from this list
		// despite being current (7.0/7.3) and used by recipes this bot already crafts (Grade 4
		// Gemdraughts, Moqueca — see Defaults.raidpotions/raidfood).
		public static readonly int[] aethersands = new int[]
		{
			Material.LevinchromeAethersand,
			Material.SungiltAethersand,
			Material.MythloamAethersand,
			Material.MythrootAethersand,
			Material.MythbrineAethersand,
		};

		// Moqueca (Crit/Det) replaced the old Piety food (Broccoli and Spinach Saute) here — Piety
		// isn't a stat current savage BiS food chases, while Moqueca is the current cross-role
		// "crit/det" pick per job consumables guides. Introduced in 7.05, so it's already correct
		// for RB_TC's 7.2 client too — no ifdef split needed like raidpotions' Gemdraught tier.
		public static readonly int[] raidfood = new int[]
		{
			FoodList.CreamyAlpacaPasta,
			FoodList.Moqueca,
			FoodList.VegetableSoup,
			FoodList.MesquiteSoup,
		};

		public static readonly int[] raidpotions = new int[]
		{
#if (RB_DT && !RB_TC)
			// Global client, patch 7.55 — Grade 4 is the current tier (introduced 7.4).
			Potions.Grade4GemdraughtStrength,
			Potions.Grade4GemdraughtDexterity,
			Potions.Grade4GemdraughtIntelligence,
			Potions.Grade4GemdraughtMind,
#elif (RB_DT && RB_TC)
			// RB_TC client is separately patched and still on 7.2 (see Routes.cs/CHANGELOG.txt) —
			// Grade 4 didn't exist yet there, so it stays on the Grade 2 tier that was current then.
			Potions.Grade2GemdraughtStrength,
			Potions.Grade2GemdraughtDexterity,
			Potions.Grade2GemdraughtIntelligence,
			Potions.Grade2GemdraughtMind,
#else
			Potions.Grade8TinctureStrength,
			Potions.Grade8TinctureDexterity,
			Potions.Grade8TinctureIntelligence,
			Potions.Grade8TinctureMind,
#endif
		};

		public static readonly int[] materiaxii = new int[]
		{
			Materia.CrafterCompetenceXII,
			Materia.CrafterCunningXII,
			Materia.CrafterCommandXII,
			Materia.GatherGuerdonXII,
			Materia.GatherGuileXII,
			Materia.GatherGraspXII
		};

		public static readonly int[] materiaxi = new int[]
		{
			Materia.CrafterCompetenceXI,
			Materia.CrafterCunningXI,
			Materia.CrafterCommandXI,
			Materia.GatherGuerdonXI,
			Materia.GatherGuileXI,
			Materia.GatherGraspXI
		};

		public static readonly int[] materiax = new int[]
		{
			Materia.CrafterCompetenceX,
			Materia.CrafterCunningX,
			Materia.CrafterCommandX,
			Materia.GatherGuerdonX,
			Materia.GatherGuileX,
			Materia.GatherGraspX
		};

		public static readonly int[] materiaix = new int[]
		{
			Materia.CrafterCompetenceIX,
			Materia.CrafterCunningIX,
			Materia.CrafterCommandIX,
			Materia.GatherGuerdonIX,
			Materia.GatherGuileIX,
			Materia.GatherGraspIX
		};

		public static readonly int[] materiaviii = new int[]
		{
			Materia.CrafterCompetenceVIII,
			Materia.CrafterCunningVIII,
			Materia.CrafterCommandVIII,
			Materia.GatherGuerdonVIII,
			Materia.GatherGuileVIII,
			Materia.GatherGraspVIII
		};

		public static readonly int[] materiavii = new int[]
		{
			Materia.CrafterCompetenceVII,
			Materia.CrafterCunningVII,
			Materia.CrafterCommandVII,
			Materia.GatherGuerdonVII,
			Materia.GatherGuileVII,
			Materia.GatherGraspVII
		};

		public static readonly int[] materiavi = new int[]
		{
			Materia.CrafterCompetenceVI,
			Materia.CrafterCunningVI,
			Materia.CrafterCommandVI,
			Materia.GatherGuerdonVI,
			Materia.GatherGuileVI,
			Materia.GatherGraspVI
		};


		public static readonly int[] materiav = new int[]
		{
			Materia.CrafterCompetenceV,
			Materia.CrafterCunningV,
			Materia.CrafterCommandV,
			Materia.GatherGuerdonV,
			Materia.GatherGuileV,
			Materia.GatherGraspV
		};

		public static readonly int[] materiaiv = new int[]
		{
			Materia.CrafterCompetenceIV,
			Materia.CrafterCunningIV,
			Materia.CrafterCommandIV,
			Materia.GatherGuerdonIV,
			Materia.GatherGuileIV,
			Materia.GatherGraspIV
		};
	}

	public static class Achievement
	{
		public const int Mantas = 2756;
		public const int Octopods = 2563;
		public const int Sharks = 2564;
		public const int Jellyfish = 2565;
		public const int Seadragons = 2566;
		public const int Balloons = 2754;
		public const int Crabs = 2755;
		public const int Indigo5kPoints = 2560;
		public const int Indigo10kPoints = 2561;
		public const int Indigo16kPoints = 2562;
		public const int Indigo20kPoints = 2759;

		public const int Shrimp = 3269;
		public const int Shellfish = 3267;
		public const int Squid = 3268;
		public const int MantisShrimp = 3976;
		public const int Prehistoric = 3975;
		public const int Ruby5kPoints = 3264;
		public const int Ruby10kPoints = 3265;
		public const int Ruby16kPoints = 3266;
		public const int Ruby20kPoints = 3974;

		public const int Overall100kPoints = 2558;
		public const int Overall500kPoints = 2559;
		public const int Overall1mPoints = 2757;
		public const int Overall3mPoints = 2758;
	}

	public static class Actions
	{
		public const uint MoochII = 268;
		public const uint DoubleHook = 269;
		public const uint Cast = 289;
		public const uint Mooch = 297;
		public const uint Quit = 299;
		public const uint PowerfulHookset = 4103;
		public const uint Patience = 4102;
		public const uint Chum = 4104;
		public const uint PatienceII = 4106;
		public const uint PrecisionHookset = 4179;
		public const uint IdenticalCast = 4596;
		public const uint ThaliaksFavor = 26804;

		/// <summary>
		/// "Lights up the tip of your fishing rod." Toggle, FSH Lv1, 0 GP — purely cosmetic (helps
		/// see the bobber at night). Verified via XIVAPI Action row 2135. Applies NO status aura, so
		/// its on/off state isn't observable — it must be fired once and tracked, never re-checked.
		/// </summary>
		public const uint CastLight = 2135;

		/// <summary>
		/// Guarantees the next catch is Large (2x points), until you catch something or quit fishing.
		/// Verified via XIVAPI Action 26806: Name "Prize Catch", ClassJobLevel 81, GP cost 200.
		/// </summary>
		public const uint PrizeCatch = 26806;

		public const uint TripleHook = 27523;
		public const uint ModestLure = 37595;
		public const uint AmbitiousLure = 37594;

		/// <summary>
		/// "Abandons fishing but keeps your equipment at the ready" (GP cost 0, level 1, Fisher-only).
		/// Verified via XIVAPI Action sheet row 37047. Lets us bail out of an unwanted bite
		/// immediately instead of letting it sit until the bite times out on its own.
		/// </summary>
		public const uint Rest = 37047;
	}

	public static class CharacterAuras
	{
		public static uint WellFed = 48;
		public static uint FishersIntuition = 568;
		public static uint Chum = 763;
		public static uint AnglersFortune = 850;
		public static uint AnglersArt = 2778;
		public static uint ModestLureProc = 4082;
		public static uint AmbitiousLureProc = 4083;
	}

	public static class Crystals 
	{
		public static int FireShard = 2;
		public static int IceShard = 3;
		public static int WindShard = 4;
		public static int EarthShard = 5;
		public static int LightningShard = 6;
		public static int WaterShard = 7;
		public static int FireCrystal = 8;
		public static int IceCrystal = 9;
		public static int WindCrystal = 10;
		public static int EarthCrystal = 11;
		public static int LightningCrystal = 12;
		public static int WaterCrystal = 13;
		public static int FireCluster = 14;
		public static int IceCluster = 15;
		public static int WindCluster = 16;
		public static int EarthCluster = 17;
		public static int LightningCluster = 18;
		public static int WaterCluster = 19;
	}

	public static class Cordials
	{
		public static uint Cordial = 6141;
		public static uint HiCordial = 12669;
		public static uint WateredCordial = 16911;
	}

	public enum Currency : uint
	{
		WhiteCraftersScrips = 25199,
		PurpleCraftersScrips = 33913,
		OrangeCraftersScrips = 41784
	}

	public static class FoodList
	{
		public static int PepperedPopotoes = 27870;
		public static int CrabCakes = 30481;
		public static int YakowMoussaka = 36054;
		public static int TsaiTouVounou = 36060;

		public static int PumpkinRatatouille = 36069;
		public static int ArchonBurger = 36067;
		public static int PumpkinPotage = 36070;
		public static int ThavnairianChai = 36074;

		public static int CalamariRipieni = 37282;

		public static int SunsetCarrotNibbles = 38263;
		public static int CarrotPudding = 38264;
		public static int GarleanPizza = 38268;
		public static int MelonPie = 38261;

		public static int HoneyedDragonfruit = 39869;
		public static int BabaGhanoush = 39871;
		public static int BakedEggplant = 39872;
		public static int CaviarCanapes = 39876;


		public static int NasiGoreng = 44078;
		public static int RroneekSteak = 44091;

		public static int CreamyAlpacaPasta = 44087;
		public static int BroccoliSpinachSaute = 44090;
		public static int VegetableSoup = 44096;
		public static int MesquiteSoup = 44098;

		/// <summary>Determination/Critical Hit/Vitality — introduced 7.05, still the current
		/// BiS "crit/det" food as of 7.55 per job consumables guides.</summary>
		public static int Moqueca = 44178;
	}

	public static class Materia
	{

		public static int GatherGuerdonIV = 5687;
		public static int GatherGuileIV = 5692;
		public static int GatherGraspIV = 5697;

		public static int CrafterCompetenceIV = 5702;
		public static int CrafterCunningIV = 5707;
		public static int CrafterCommandIV = 5712;

		public static int GatherGuerdonV = 5688;
		public static int GatherGuileV = 5693;
		public static int GatherGraspV = 5698;

		public static int CrafterCompetenceV = 5703;
		public static int CrafterCunningV = 5708;
		public static int CrafterCommandV = 5713;

		public static int GatherGuerdonVI = 18022;
		public static int GatherGuileVI = 18023;
		public static int GatherGraspVI = 18024;

		public static int CrafterCompetenceVI = 18025;
		public static int CrafterCunningVI = 18026;
		public static int CrafterCommandVI = 18027;


		public static int GatherGuerdonVII = 25191;
		public static int GatherGuileVII = 25192;
		public static int GatherGraspVII = 25193;

		public static int CrafterCompetenceVII = 25194;
		public static int CrafterCunningVII = 25195;
		public static int CrafterCommandVII = 25196;
		
		public static int GatherGuerdonVIII = 26732;
		public static int GatherGuileVIII = 26733;
		public static int GatherGraspVIII = 26734;
		
		public static int CrafterCompetenceVIII = 26735;
		public static int CrafterCunningVIII = 26736;
		public static int CrafterCommandVIII = 26737;
		
		public static int GatherGuerdonIX = 33922;
		public static int GatherGuileIX = 33923;
		public static int GatherGraspIX = 33924;
		
		public static int CrafterCompetenceIX = 33925;
		public static int CrafterCunningIX = 33926;
		public static int CrafterCommandIX = 33927;
		
		public static int GatherGuerdonX = 33935;
		public static int GatherGuileX = 33936;
		public static int GatherGraspX = 33937;
		
		public static int CrafterCompetenceX = 33938;
		public static int CrafterCunningX = 33939;
		public static int CrafterCommandX = 33940;

		public static int GatherGuerdonXI = 41762;
		public static int GatherGuileXI = 41763;
		public static int GatherGraspXI = 41764;

		public static int CrafterCompetenceXI = 41765;
		public static int CrafterCunningXI = 41766;
		public static int CrafterCommandXI = 41767;

		public static int GatherGuerdonXII = 41775;
		public static int GatherGuileXII = 41776;
		public static int GatherGraspXII = 41777;

		public static int CrafterCompetenceXII = 41778;
		public static int CrafterCunningXII = 41779;
		public static int CrafterCommandXII = 41780;
	}

	public static class Material
	{
		public static int DinosaurLeather = 13745;
		public static int Sphalerite = 13750;
		public static int RoyalMistletoe = 13752;
		public static int CloudCottonBoll = 13753;
		public static int CloudMythrilOre = 17570;
		public static int StormcloudCottonBoll = 17571;
		public static int DusklightAethersand = 20013;
		public static int DawnlightAethersand = 20014;
		public static int EverbrightAethersand = 20015;
		public static int EverbornAethersand = 20016;
		public static int EverdeepAethersand = 20017;
		public static int EndstoneAethersand = 36224;
		public static int EndwoodAethersand = 36225;
		public static int EndtideAethersand = 36226;
		public static int ImmutableSolution = 37284;
		public static int EarthbreakAethersand = 38936;

		public static int MythloamAethersand = 44036;
		public static int MythrootAethersand = 44037;
		public static int MythbrineAethersand = 44038;

		/// <summary>7.3 — used in dozens of current level-100 recipes across all 8 crafting classes
		/// (all four Grade 4 Gemdraughts, All i Pebre, Synthetic Dark Matter, Aspected Aether items,
		/// Courtly Lover's gear). Confirmed in the current Orange Gatherers' Scrip exchange.</summary>
		public static int LevinchromeAethersand = 46246;

		/// <summary>7.0 — used in Optical Nanofiber (-> Everseekers gear) and Moqueca (see
		/// Defaults.raidfood). Confirmed in the current Orange Gatherers' Scrip exchange.</summary>
		public static int SungiltAethersand = 44035;

		/// <summary>125 Orange Crafters' Scrip — the Dawntrail-tier successor to ImmutableSolution
		/// (125 Purple Crafters' Scrip, Endwalker). Used in Optical Nanofiber -> Everseekers gear,
		/// the current tier of high-difficulty crafts.</summary>
		public static int CondensedSolution = 44848;

		// The rest of the Orange Crafters' Scrip "Materials" exchange, confirmed 2026-08-12 against
		// the live in-game exchange window (Materials/Misc subcategory) rather than guessed from
		// naming convention. Cheap (10-15 scrip) Culinarian ingredients; Brown Cardamom in
		// particular is a real Moqueca ingredient (see Defaults.raidfood).
		public static int RumplessChicken = 44170;
		public static int BrownCardamom = 44171;
		public static int WildCoffeeBeans = 44172;
		public static int NavelOrange = 44173;
		public static int RoyalLobster = 44174;

		public static int Cassava = 45990;
		public static int SplendidMateLeaves = 45991;
		public static int AjiAmarillo = 45992;
		public static int QuesoFresco = 45993;
		public static int WoolbackLoin = 45994;

		public static int FlintCorn = 49229;
		public static int TuraliPlum = 49230;
		public static int RroneekMilk = 49231;
		public static int RockFistPopoto = 49232;
		public static int Quahog = 49233;
	}

	public static class NPC
	{
		public const uint IndependentMerchantCoerthasWesternHighlands = 1011228;
		public const uint Syneyhil = 1003254;
		public const uint Dryskthota = 1005421;
		public const uint LimsaFishingMerchantMender = 1005422;
		public const uint Vernarth = 1027241;
	}

	public static class Potions
	{
		public static int Grade6TinctureStrength = 36109;
		public static int Grade6TinctureDexterity = 36110;
		public static int Grade6TinctureIntelligence = 36112;
		public static int Grade6TinctureMind = 36113;

		public static int Grade7TinctureStrength = 37840;
		public static int Grade7TinctureDexterity = 37841;
		public static int Grade7TinctureIntelligence = 37843;
		public static int Grade7TinctureMind = 37844;

		public static int Grade8TinctureStrength = 39727;
		public static int Grade8TinctureDexterity = 39728;
		public static int Grade8TinctureIntelligence = 39730;
		public static int Grade8TinctureMind = 39731;

		public static int Grade1GemdraughtStrength = 44157;
		public static int Grade1GemdraughtDexterity = 44158;
		public static int Grade1GemdraughtIntelligence = 44160;
		public static int Grade1GemdraughtMind = 44161;

		public static int Grade2GemdraughtStrength = 44162;
		public static int Grade2GemdraughtDexterity = 44163;
		public static int Grade2GemdraughtIntelligence = 44165;
		public static int Grade2GemdraughtMind = 44166;

		public static int Grade4GemdraughtStrength = 49234;
		public static int Grade4GemdraughtDexterity = 49235;
		public static int Grade4GemdraughtIntelligence = 49237;
		public static int Grade4GemdraughtMind = 49238;
	}

	public static class Weather
	{
		public const uint Spectral = 145;
	}

	public static class Zones
	{
		public const uint LimsaLominsaLowerDecks = 129;
		public const uint Uldah = 131;
		public const uint OldGridania = 133;
		public const uint MorDhona = 156;
		public const uint CoerthasWesternHighlands = 397;
		public const uint Ishgard = 419;
		public const uint Idyllshire = 478;
		public const uint Kugane = 628;
		public const uint RhalgrsReach = 635;
		public const uint Crystarium = 819;
		public const uint Eulmore = 820;

		public const uint TheEndeavor = 900;
		public const uint TheEndeaver_Ruby = 1163;
		public const uint TheEndeavor_Thavnair = 1163; // Shares Ruby Endeavor instance; update if 7.5 uses a separate zone
	}
}
