using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanTripPlanner.Definitions
{
    public static class OceanFish
    {
        public const int GaladionGoby = 28937;
        public const int GaladionChovy = 28938;
        public const int RosyBream = 28939;
        public const int TripodFish = 28940;
        public const int Sunfly = 28941;
        public const int RhotanoWahoo = 29728;
        public const int RhotanoSardine = 29729;
        public const int DeepPlaice = 29730;
        public const int Floefish = 29736;
        public const int Megasquid = 29737;
        public const int OschonsStone = 29738;
        public const int Jasperhead = 29719;
        public const int TarnishedShark = 28942;
        public const int TossedDagger = 29718;
        public const int GhoulBarracuda = 29722;
        public const int LeopardEel = 29723;
        public const int MarineBomb = 29724;
        public const int MomoraMora = 29725;
        public const int CrimsonMonkfish = 29731;
        public const int ChromeHammerhead = 29735;
        public const int OgreEel = 29733;
        public const int LaNosceanJelly = 29739;
        public const int ShaggySeadragon = 29740;
        public const int NetCrawler = 29741;
        public const int CyanOctopus = 29734;
        public const int Heavenswimmer = 29721;
        public const int MerlthorButterfly = 29726;
        public const int Gladius = 29727;
        public const int DarkNautilus = 29742;
        public const int Lampfish = 29732;
        public const int MerlthorLobster = 29720;
        public const int ElderDinichthys = 29743;
        public const int Drunkfish = 29744;
        public const int LittleLeviathan = 29745;
        public const int Sabaton = 29746;
        public const int ShootingStar = 29747;
        public const int MermansMane = 29766;
        public const int Heavenskey = 29749;
        public const int GhostShark = 29750;
        public const int QuicksilverBlade = 29751;
        public const int NavigatorsPrint = 29752;
        public const int CasketOyster = 29753;
        public const int Fishmonger = 29754;
        public const int MythrilSovereign = 29755;
        public const int NimbleDancer = 29756;
        public const int SeaNettle = 29757;
        public const int GreatGrandmarlin = 29758;
        public const int ShipwrecksSail = 29759;
        public const int CharlatanSurvivor = 29779;
        public const int HiAetherlouse = 29761;
        public const int AzeymasSleeve = 29760;
        public const int AethericSeadragon = 29763;
        public const int CoralSeadragon = 29764;
        public const int Roguesaurus = 29765;
        public const int Aronnax = 29775;
        public const int Sweeper = 29767;
        public const int Silencer = 29768;
        public const int DeepSeaEel = 29769;
        public const int Executioner = 29770;
        public const int WildUrchin = 29771;
        public const int TrueBarramundi = 29772;
        public const int ProdigalSon = 29780;
        public const int Slipsnail = 29774;
        public const int Hammerclaw = 29748;
        public const int Coccosteus = 29776;
        public const int BartholomewTheChopper = 29777;
        public const int Prowler = 29778;
        public const int Mopbeard = 29773;
        public const int FloatingSaucer = 29762;
        public const int Gugrusaurus = 29781;
        public const int FunnelShark = 29782;
        public const int TheFallenOne = 29783;
        public const int SpectralMegalodon = 29784;
        public const int SpectralDiscus = 29785;
        public const int SpectralSeaBo = 29786;
        public const int SpectralBass = 29787;
        public const int Sothis = 29788;
        public const int CoralManta = 29789;
        public const int Stonescale = 29790;
        public const int Elasmosaurus = 29791;
        public const int TortoiseshellCrab = 32055;
        public const int LadysCameo = 32056;
        public const int MetallicBoxfish = 32057;
        public const int GoobbueRay = 32058;
        public const int Watermoura = 32059;
        public const int KingCobrafish = 32060;
        public const int MamahiMahi = 32061;
        public const int LavandinRemora = 32062;
        public const int SpectralButterfly = 32063;
        public const int CieldalaesGeode = 32064;
        public const int TitanshellCrab = 32065;
        public const int MythrilBoxfish = 32066;
        public const int MistbeardsCup = 32067;
        public const int AnomalocarisSaron = 32068;
        public const int FlamingEel = 32069;
        public const int JetborneManta = 32070;
        public const int DevilsSting = 32071;
        public const int Callichthyid = 32072;
        public const int MeanderingMora = 32073;
        public const int Hafgufa = 32074;
        public const int ThaliakCrab = 32075;
        public const int StarOfTheDestroyer = 32076;
        public const int TrueScad = 32077;
        public const int BloodedWrasse = 32078;
        public const int BloodpolishCrab = 32079;
        public const int BlueStitcher = 32080;
        public const int BloodfreshTuna = 32081;
        public const int SunkenMask = 32082;
        public const int SpectralEel = 32083;
        public const int Bareface = 32084;
        public const int OracularCrab = 32085;
        public const int DravanianBream = 32086;
        public const int Skaldminni = 32087;
        public const int SerratedClam = 32088;
        public const int BeatificVision = 32089;
        public const int Exterminator = 32090;
        public const int GoryTuna = 32091;
        public const int Ticinepomis = 32092;
        public const int QuartzHammerhead = 32093;
        public const int SeafaringToad = 32094;
        public const int CrowPuffer = 32095;
        public const int StripOfRothlytKelp = 32096;
        public const int LivingLantern = 32097;
        public const int HoneycombFish = 32098;
        public const int Godsbed = 32099;
        public const int Lansquenet = 32100;
        public const int ThavnairianShark = 32101;
        public const int NephriteEel = 32102;
        public const int Spectresaur = 32103;
        public const int GinkgoFin = 32104;
        public const int GarumJug = 32105;
        public const int SmoothJaguar = 32106;
        public const int RothlytMussel = 32107;
        public const int LeviElver = 32108;
        public const int PearlBombfish = 32109;
        public const int Trollfish = 32110;
        public const int Panoptes = 32111;
        public const int CrepeSole = 32112;
        public const int Knifejaw = 32113;
        public const int Placodus = 32114;

        // Open Sirensong Sea
        public const int DeepshadeSardine = 40524;
        public const int SirensongMussel = 40522;
        public const int Arrowhead = 40523;
        public const int PinkShrimp = 40521;
        public const int SirensongMullet = 40525;
        public const int SelkiePuffer = 40526;
        public const int PoetsPipe = 40527;
        public const int MarineMatanga = 40528;
        public const int SpectralCoelacanth = 40529;
        public const int DuskShark = 40530;

        //Open Sirensong Sea (Spectral)
        public const int MermaidScale = 40531;
        public const int Broadhead = 40532;
        public const int VividPinkShrimp = 40533;
        public const int SunkenCoelacanth = 40534;
        public const int SirensSigh = 40535;
        public const int BlackjawedHelicoprion = 40536;
        public const int Impostopus = 40537;
        public const int JadeShrimp = 40538;
        public const int NymeiasWheel = 40539;
        public const int Taniwha = 40540;

        // Kugane Coast
        public const int RubyHerring = 40541;
        public const int WhirlpoolTurban = 40542;
        public const int LeopardPrawn = 40543;
        public const int SpearSquid = 40544;
        public const int FloatingLantern = 40545;
        public const int RubescentTatsunoko = 40546;
        public const int Hatatate = 40547;
        public const int SilentShark = 40548;
        public const int SpectralWrasse = 40549;
        public const int Mizuhiki = 40550;

        // Kugane Coase (Spectral)
        public const int SnappingKoban = 40551;
        public const int SilkweftPrawn = 40552;
        public const int StingfinTrevally = 40553;
        public const int SwordtipSquid = 40554;
        public const int Mailfish = 40555;
        public const int IdatensBolt = 40556;
        public const int MaelstromTurban = 40557;
        public const int Shoshitsuki = 40558;
        public const int Spadefish = 40559;
        public const int GlassDragon = 40560;

        // Open Ruby Sea
        public const int CrimsonKelp = 40561;
        public const int ReefSquid = 40562;
        public const int PinebarkFlounder = 40563;
        public const int MantleMoray = 40564;
        public const int BardedLobster = 40565;
        public const int ShisuiGoby = 40566;
        public const int Sanbaso = 40567;
        public const int VioletSentry = 40568;
        public const int SpectralSnakeEel = 40569;
        public const int HeavensentShark = 40570;

        // Open Ruby Sea (Spectral)
        public const int FleetingSquid = 40571;
        public const int BowbarbLobster = 40572;
        public const int PitchPickle = 40573;
        public const int SenbeiOctopus = 40574;
        public const int TentacaleThresher = 40575;
        public const int BekkoRockhugger = 40576;
        public const int YellowIris = 40577;
        public const int CrimsonSentry = 40578;
        public const int FlyingSquid = 40579;
        public const int HellsClaw = 40580;

        // Lower One River
        public const int CatchingCarp = 40581;
        public const int GarleanBluegill = 40582;
        public const int YanxianSoftshell = 40583;
        public const int PrincessSalmon = 40584;
        public const int Calligraph = 40585;
        public const int SingularShrimp = 40586;
        public const int BrocadeCarp = 40587;
        public const int YanxianSturgeon = 40588;
        public const int SpectralKotsuZetsu = 40589;
        public const int FishyShark = 40590;

        // Lower One River (Spectral) 
        public const int GensuiShrimp = 40591;
        public const int YatonoKami = 40592;
        public const int HeronsEel = 40593;
        public const int CrowshadowMussel = 40594;
        public const int YanxianGoby = 40595;
        public const int IridescentTrout = 40596;
        public const int UnNamazu = 40597;
        public const int Gakugyo = 40598;
        public const int GinrinGoshiki = 40599;
        public const int JewelofPlumSpring = 40600;
    }

    public static class FishBait
    {
        public static uint PillBug = 2587;
        public static uint RatTail = 2591;
        public static uint GlowWorm = 2603;
        public static uint ShrimpCageFeeder = 2613;
        public static uint HeavySteelJig = 2619;
        public static uint StoneflyNymph = 12704;
        public static uint Ragworm = 29714;
        public static uint Krill = 29715;
        public static uint PlumpWorm = 29716;
        public static uint SquidStrip = 27590;
        public static uint MackerelStrip = 36593;
    }
}
