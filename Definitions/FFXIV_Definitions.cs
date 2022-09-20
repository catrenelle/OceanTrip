using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanTripPlanner.Definitions
{
    public static class Actions
    {
        public const uint MoochII = 268;
        public const uint DoubleHook = 269;
        public const uint OpenCloseBaitMenu = 288;
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
    }

    public static class CharacterAuras
    {
        public static uint WellFed = 48;
        public static uint FishersIntuition = 568;
        public static uint AnglersArt = 2778;
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
        PurpleCraftersScrips = 33913
    }

    public static class FoodList
    {
        public static int PepperedPopotoes = 27870;
        public static int CrabCakes = 30481;
        public static int TsaiTouVounou = 36060;
        public static int PumpkinRatatouille = 36069;
        public static int ArchonBurger = 36067;
        public static int PumpkinPotage = 36070;
        public static int ThavnairianChai = 36074;
    }

    public static class NPC
    {
        public const uint Syneyhil = 1003254;
        public const uint Dryskthota = 1005421;
        public const uint LimsaFishingMerchantMender = 1005422;
    }

    public static class Potions
    {
        public static int Grade6TinctureStrength = 36109;
        public static int Grade6TinctureDexterity = 36110;
        public static int Grade6TinctureIntelligence = 36112;
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
        public const uint Ishgard = 419;
        public const uint Idyllshire = 478;
        public const uint Kugane = 628;
        public const uint RhalgrsReach = 635;
        public const uint Crystarium = 819;
        public const uint Eulmore = 820;
        public const uint TheEndeavor = 900;
    }
}
