using System;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using ff14bot.Helpers;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace OceanTripPlanner
{

	public enum LisbethFood : int
	{
		StoneSoup = 4717,
		SeafoodStew = 12865,
		SeafoodStewHQ = 1012865,
		ChiliCrabHQ = 1030482,
		TsaitouVounouHQ = 1036060,
		CalamariRipieni = 37282,
		CalamariRipieniHQ = 1037282,

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
		Points,
		Auto,
		IgnoreBoat
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

	public enum FullGPAction : uint
	{
		Chum,
		DoubleHook,
		None
	}

	public enum OceanFood : int
	{
        PepperedPopotoes = 27870,
		CrabCakes = 30481,
		None = 0
	}

	public enum LisbethPotionCrafting : int
	{
		Grade7 = 1,
		Grade6 = 2,
		None = 0
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

		[DisplayName("Refresh Retainers")]
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

		private bool _ResumeOrder;
		[Setting]

		[DisplayName("Resume Lisbeth Order")]
		[Description("Resume last order before making new ones. This will resume any unfinished Lisbeth order, even if started from somewhere else.")]
		[Category("Idle Stuff")]

		[DefaultValueAttribute(false)]
		public bool ResumeOrder
		{
			get { return _ResumeOrder; }
			set
			{
				if (_ResumeOrder != value)
				{
					_ResumeOrder = value;
					Save();
				}
			}
		}

		private bool _CustomOrder;
		[Setting]

		[DisplayName("Custom Lisbeth Order")]
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


        private FullGPAction _FullGPAction;
        [Setting]

        [DisplayName("Full GP Action")]
        [Description("What to do when your current GP is at its maximum. Does not apply during Spectral events. Double Hook will not work if Patience is in use.")]
        [Category("Ocean Fishing")]

        [DefaultValueAttribute(FullGPAction.Chum)]
        public FullGPAction FullGPAction
        {
            get { return _FullGPAction; }
            set
            {
                if (_FullGPAction != value)
                {
                    _FullGPAction = value;
                    Save();
                }
            }
        }

        private bool _LateBoatQueue;
        [Setting]

        [DisplayName("Late Queue")]
        [Description("Queues for the boat at 13 minutes if true instead of within 10 minutes. Potential for more points.")]
        [Category("Ocean Fishing")]

        [DefaultValueAttribute(false)]
        public bool LateBoatQueue
        {
            get { return _LateBoatQueue; }
            set
            {
                if (_LateBoatQueue != value)
                {
                    _LateBoatQueue = value;
                    Save();
                    //TimeSpan timeLeftUntilNextSpawn = OceanTrip.TimeUntilNextBoat();
                    //if (timeLeftUntilNextSpawn.TotalMinutes < 1)
                    //    Logging.Write(Colors.Aqua, $"[Ocean Trip] Late Boat Queue setting changed! The boat is ready to be boarded!");
                    //else
                    //    Logging.Write(Colors.Aqua, $"[Ocean Trip] Late Boat Queue setting changed! Next boat is in {Math.Ceiling(timeLeftUntilNextSpawn.TotalMinutes)} minutes.")
                }
            }
        }
        
		private OceanFood _OceanFood;
		[Setting]

		[DisplayName("Use Ocean Fishing Food")]
		[Description("What food item do you want to use for fishing? HQ food will be used first if you have it.")]
		[Category("Ocean Fishing")]

		[DefaultValueAttribute(OceanFood.None)]
		public OceanFood OceanFood
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

		private ExchangeFish _ExchangeFish;
		[Setting]

		[DisplayName("Exchange Fish")]
		[Description("What to do with fish caught by ocean fishing (won't dispose of blue fish).")]
		[Category("Ocean Fishing")]

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

		[DisplayName("Use Crafting Food")]
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

		[DisplayName("Fishing Priority")]
		[Description("Prioritize fish log completion or points while ocean fishing. Setting to Auto will attempt to complete the fish log if any fish are available before switching to Points mode. Setting to Fish Log will skip the upcoming boat if you have no missing fish.")]
		[Category("Ocean Fishing")]

		[DefaultValueAttribute(FishPriority.Auto)]
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

		private int _BaitRestockThreshold;
		[Setting]

		[DisplayName("Bait Restock Threshold")]
		[Description("How low must your bait be before we restock?")]
		[Category("Ocean Fishing")]

		[DefaultValueAttribute(100)]
		public int BaitRestockThreshold
		{
			get { return _BaitRestockThreshold; }
			set
			{
				if (_BaitRestockThreshold != value)
				{
					_BaitRestockThreshold = value;
					Save();
				}
			}
		}

        private int _BaitRestockAmount;
        [Setting]

        [DisplayName("Bait Restock Amount")]
        [Description("How much bait do we need to buy when restocking?")]
        [Category("Ocean Fishing")]

        [DefaultValueAttribute(500)]
        public int BaitRestockAmount
        {
            get { return _BaitRestockAmount; }
            set
            {
                if (_BaitRestockAmount != value)
                {
                    _BaitRestockAmount = value;
                    Save();
                }
            }
        }

        private LisbethPotionCrafting _CraftPotions;
		[Setting]

		[DisplayName("Craft Raid Potions")]
		[Description("Use Lisbeth to craft various (lvl90) potions while waiting for the boat.")]
		[Category("Idle Stuff")]
		
		[DefaultValueAttribute(LisbethPotionCrafting.Grade6)]
		public LisbethPotionCrafting CraftPotions
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

		[DisplayName("Craft Raid Food")]
		[Description("Use Lisbeth to craft various (lvl90) food while waiting for the boat.")]
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

		private bool _GetMateria;
		[Setting]

		[DisplayName("Purchase Materia")]
		[Description("Use Lisbeth to buy IX and X materia while waiting for the boat.")]
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

		[DisplayName("Refill Crafter Scrips")]
		[Description("Use Lisbeth to refill White and Purple Crafter Scrips while waiting for the boat.")]
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

		[DisplayName("Purchase Hi-Cordials")]
		[Description("Buy Cordials when White Gatherer Scrips are close to cap. Purchasing will stop when scrips are at 1500 or less.")]
		[Category("Idle Stuff")]

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

        private bool _OpenWorldFishing;
        [Setting]

        [DisplayName("Assisted Fishing")]
        [Description("Allows the bot to do basic fishing for you in the open world. You must start the cast to put you into Pole Ready, and manually use quit to stop the bot.")]
        [Category("Open World Fishing")]

        [DefaultValueAttribute(false)]
        public bool OpenWorldFishing
        {
            get { return _OpenWorldFishing; }
            set
            {
                if (_OpenWorldFishing != value)
                {
                    _OpenWorldFishing = value;
                    Save();
                }
            }
        }
    }
}