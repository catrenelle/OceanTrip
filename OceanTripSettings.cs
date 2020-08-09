using System;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using ff14bot.Helpers;

namespace OceanTripPlanner
{
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

        private bool _Venturing;
        [Setting]

		[DisplayName("Retaining")]
		[Description("Go to Idyllshire to refresh ventures.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(true)]
        public bool Venturing
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
		[Description("Desynth rose gold cogs for Fieldcraft IIIs.")]
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
		
		private FishPriority _FishPriority;
        [Setting]

		[DisplayName("Fish Priority")]
		[Description("Prioritize fish log completion or points while ocean fishing.")]
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
		
		private bool _CraftToSell;
        [Setting]

		[DisplayName("Craft To Sell")]
		[Description("Use Lisbeth to craft various (lvl80) stuff while waiting for the boat.")]
		[Category("Idle Stuff")]
		
        [DefaultValueAttribute(true)]
        public bool CraftToSell
        {
            get { return _CraftToSell; }
            set
            {
                if (_CraftToSell != value)
                {
                    _CraftToSell = value;
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