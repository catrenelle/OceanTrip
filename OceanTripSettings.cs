using System;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using ff14bot.Helpers;

namespace OceanTripPlanner
{

    public enum LisbethFood : int
    {
        StoneSoup = 4717,
        SeafoodStew = 12865,
        SeafoodStewHQ = 1012865,
        None = 0
    }

    public enum ExchangeFish : uint
	{
		Sell,
		Desynth,
		None
	}
	
	public enum FishPriority : uint
	{
		FishLog,
		Points
	}

    public enum Venturing : uint
    {
        Limsa,
        Uldah,
        Gridania,
        MorDohna,
        Ishgard,
        Idyllshire,
        Kugane,
        RhalgrsReach,
        Crystarium,
        Eulmore,
        None
    }

    internal class OceanTripSettings : JsonSettings
    {
        private static OceanTripSettings _settings;

        public static OceanTripSettings Instance
        {
            get { return _settings ?? (_settings = new OceanTripSettings()); }
        }

        public OceanTripSettings()
            : base(Path.Combine(CharacterSettingsDirectory, "OceanTripSettings.json"))
        {
        }

        private Venturing _Venturing;
        [Setting]

		[DisplayName("Retaining")]
		[Description("Go to a summoning bell and refresh ventures.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(Venturing.None)]
        public Venturing Venturing
        {
            get { return _Venturing; }
            set
            {
                if (_Venturing != value)
                {
                    _Venturing = value;
                    Save();
                }
            }
        }

        private bool _CustomOrder;
        [Setting]

        [DisplayName("Custom Order")]
        [Description("Use your own Lisbeth json order while waiting for the boat. Save it as \"BoatOrder.json\" in your RebornBuddy root folder")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(false)]
        public bool CustomOrder
        {
            get { return _CustomOrder; }
            set
            {
                if (_CustomOrder != value)
                {
                    _CustomOrder = value;
                    Save();
                }
            }
        }

        private bool _OceanFood;
        [Setting]

		[DisplayName("Ocean Food")]
		[Description("Craft and use Peppered Popotos for ocean fishing.")]
		[Category("Ocean")]

        [DefaultValueAttribute(true)]
        public bool OceanFood
        {
            get { return _OceanFood; }
            set
            {
                if (_OceanFood != value)
                {
                    _OceanFood = value;
                    Save();
                }
            }
        }

		private bool _FieldcraftIII;
        [Setting]

		[DisplayName("Fieldcraft III")]
		[Description("Craft & Desynth rose gold cogs for Fieldcraft IIIs.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(true)]
        public bool FieldcraftIII
        {
            get { return _FieldcraftIII; }
            set
            {
                if (_FieldcraftIII != value)
                {
                    _FieldcraftIII = value;
                    Save();
                }
            }
        }

		private ExchangeFish _ExchangeFish;
        [Setting]

		[DisplayName("Exchange Fish")]
		[Description("What to do with fish caught by ocean fishing.")]
		[Category("Ocean")]

        [DefaultValueAttribute(ExchangeFish.Sell)]
        public ExchangeFish ExchangeFish
        {
            get { return _ExchangeFish; }
            set
            {
                if (_ExchangeFish != value)
                {
                    _ExchangeFish = value;
                    Save();
                }
            }
        }

        private LisbethFood _LisbethFood;
        [Setting]

        [DisplayName("Lisbeth Food")]
        [Description("Food to use while crafting.")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(LisbethFood.None)]
        public LisbethFood LisbethFood
        {
            get { return _LisbethFood; }
            set
            {
                if (_LisbethFood != value)
                {
                    _LisbethFood = value;
                    Save();
                }
            }
        }

        private FishPriority _FishPriority;
        [Setting]

		[DisplayName("Fish Priority")]
		[Description("Prioritize fish log completion or points while ocean fishing. This will skip the upcoming boat if you have the blue fish from it.")]
		[Category("Ocean")]

        [DefaultValueAttribute(FishPriority.Points)]
        public FishPriority FishPriority
        {
            get { return _FishPriority; }
            set
            {
                if (_FishPriority != value)
                {
                    _FishPriority = value;
                    Save();
                }
            }
        }
		
		private bool _CraftPotions;
        [Setting]

		[DisplayName("Craft Potions")]
		[Description("Use Lisbeth to craft various (lvl80) potions while waiting for the boat.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(true)]
        public bool CraftPotions
        {
            get { return _CraftPotions; }
            set
            {
                if (_CraftPotions != value)
                {
                    _CraftPotions = value;
                    Save();
                }
            }
        }

        private bool _CraftFood;
        [Setting]

        [DisplayName("Craft Food")]
        [Description("Use Lisbeth to craft various (lvl80) food while waiting for the boat.")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(true)]
        public bool CraftFood
        {
            get { return _CraftFood; }
            set
            {
                if (_CraftFood != value)
                {
                    _CraftFood = value;
                    Save();
                }
            }
        }

        private bool _CraftGear;
        [Setting]

        [DisplayName("Craft Gear")]
        [Description("Use Lisbeth to craft various Aesthete stuff while waiting for the boat (won't buy tome items).")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(true)]
        public bool CraftGear
        {
            get { return _CraftGear; }
            set
            {
                if (_CraftGear != value)
                {
                    _CraftGear = value;
                    Save();
                }
            }
        }

        private bool _GetMateria;
        [Setting]

        [DisplayName("Get Materia")]
        [Description("Use Lisbeth to buy VII and VIII materias while waiting for the boat.")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(true)]
        public bool GetMateria
        {
            get { return _GetMateria; }
            set
            {
                if (_GetMateria != value)
                {
                    _GetMateria = value;
                    Save();
                }
            }
        }

        private bool _RefillScrips;
        [Setting]

        [DisplayName("Refill Scrips")]
        [Description("Use Lisbeth to refill Yellow and White Crafter Scrips while waiting for the boat.")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(true)]
        public bool RefillScrips
        {
            get { return _RefillScrips; }
            set
            {
                if (_RefillScrips != value)
                {
                    _RefillScrips = value;
                    Save();
                }
            }
        }

        private bool _EmptyScrips;
        [Setting]

        [DisplayName("Empty Gatherer Scrips")]
        [Description("Buy Cordials when Yellow Gatherer Scrips are close to cap.")]
        [Category("Ocean")]

        [DefaultValueAttribute(true)]
        public bool EmptyScrips
        {
            get { return _EmptyScrips; }
            set
            {
                if (_EmptyScrips != value)
                {
                    _EmptyScrips = value;
                    Save();
                }
            }
        }

        private bool _CraftMats;
        [Setting]

        [DisplayName("Craft Mats")]
        [Description("Use Lisbeth to craft various (lvl80) stuff while waiting for the boat.")]
        [Category("Idle Stuff")]

        [DefaultValueAttribute(true)]
        public bool CraftMats
        {
            get { return _CraftMats; }
            set
            {
                if (_CraftMats != value)
                {
                    _CraftMats = value;
                    Save();
                }
            }
        }

        private bool _GatherShards;
        [Setting]

		[DisplayName("Gather Shards")]
		[Description("Use Lisbeth to gather shards and crystals while waiting for the boat.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(true)]
        public bool GatherShards
        {
            get { return _GatherShards; }
            set
            {
                if (_GatherShards != value)
                {
                    _GatherShards = value;
                    Save();
                }
            }
        }

        private DateTime _VentureTime;
        [Setting]

		[Browsable(false)]
		
        public DateTime VentureTime
        {
            get { return _VentureTime; }
            set
            {
                if (_VentureTime != value)
                {
                    _VentureTime = value;
                    Save();
                }
            }
        }
    }
}