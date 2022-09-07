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
    }

    public static class FishBait
    {
        public static uint PillBug = 2587;
        public static uint RatTail = 2591;
        public static uint GlowWorm = 2603;
        public static uint ShrimpCageFeeder = 2613;
        public static uint HeavySteelJig = 2619;
        public static uint Ragworm = 29714;
        public static uint Krill = 29715;
        public static uint PlumpWorm = 29716;
        public static uint StripOfJerkedOvim = 27590; 
    }

    public static class Cordials
    {
        public static uint Cordial = 6141;
        public static uint HiCordial = 12669;
        public static uint WateredCordial = 16911;
    }

    public static class CharacterAuras
    {
        public static int FoodBuff = 10419;
    }
}
