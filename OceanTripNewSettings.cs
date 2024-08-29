using System;
using System.Configuration;
using System.IO;
using System.ComponentModel;
using ff14bot.Helpers;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace OceanTripPlanner
{
    public enum LoggingMode : int
    {
        Normal = 0,
        Verbose = 1
    }

    public enum FishPriority : uint
    {
        FishLog,
        Points,
        Auto,
        IgnoreBoat,
        Achievements
    }

    public enum OceanFood : int
    {
        NasiGoreng = 44078,
        CrabCakes = 30481,
        None = 0
    }

    public enum FishingRoute : int
    {
        Indigo = 0,
        Ruby = 1
    }

    public enum FullGPAction : uint
    {
        Chum,
        DoubleHook,
        None
    }

    public enum ShouldUsePatience : int
    {
        OnlyForSpecificFish = 0,
        AlwaysUsePatience = 1,
        SpectralOnly = 2
    }

    public enum ExchangeFish : uint
    {
        Sell,
        Desynth,
        None
    }

    internal class OceanTripNewSettings : JsonSettings, INotifyPropertyChanged
	{
		private static OceanTripNewSettings _settings;

		public static OceanTripNewSettings Instance
		{
			get { return _settings ?? (_settings = new OceanTripNewSettings()); }
		}

		public OceanTripNewSettings()
			: base(Path.Combine(CharacterSettingsDirectory, "OceanTripNewSettings.json"))
		{
		}

        private bool _material1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material1
        {
            get { return _material1; }
            set
            {
                if (_material1 != value)
                {
                    _material1 = value;
                    Save();
                    NotifyPropertyChanged("material1");
                }
            }
        }

        private bool _material2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material2
        {
            get { return _material2; }
            set
            {
                if (_material2 != value)
                {
                    _material2 = value;
                    Save();
                    NotifyPropertyChanged("material2");
                }
            }
        }

        private bool _material3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material3
        {
            get { return _material3; }
            set
            {
                if (_material3 != value)
                {
                    _material3 = value;
                    Save();
                    NotifyPropertyChanged("material3");
                }
            }
        }

        private bool _material4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material4
        {
            get { return _material4; }
            set
            {
                if (_material4 != value)
                {
                    _material4 = value;
                    Save();
                    NotifyPropertyChanged("material4");
                }
            }
        }

        private bool _material5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material5
        {
            get { return _material5; }
            set
            {
                if (_material5 != value)
                {
                    _material5 = value;
                    Save();
                    NotifyPropertyChanged("material5");
                }
            }
        }

        private bool _material6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material6
        {
            get { return _material6; }
            set
            {
                if (_material6 != value)
                {
                    _material6 = value;
                    Save();
                    NotifyPropertyChanged("material6");
                }
            }
        }

        private bool _material7;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool material7
        {
            get { return _material7; }
            set
            {
                if (_material7 != value)
                {
                    _material7 = value;
                    Save();
                    NotifyPropertyChanged("material7");
                }
            }
        }

        private bool _aethersand1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand1
        {
            get { return _aethersand1; }
            set
            {
                if (_aethersand1 != value)
                {
                    _aethersand1 = value;
                    Save();
                    NotifyPropertyChanged("aethersand1");
                }
            }
        }

        private bool _aethersand2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand2
        {
            get { return _aethersand2; }
            set
            {
                if (_aethersand2 != value)
                {
                    _aethersand2 = value;
                    Save();
                    NotifyPropertyChanged("aethersand2");
                }
            }
        }
        private bool _aethersand3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand3
        {
            get { return _aethersand3; }
            set
            {
                if (_aethersand3 != value)
                {
                    _aethersand3 = value;
                    Save();
                    NotifyPropertyChanged("aethersand3");
                }
            }
        }
        private bool _aethersand4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand4
        {
            get { return _aethersand4; }
            set
            {
                if (_aethersand4 != value)
                {
                    _aethersand4 = value;
                    Save();
                    NotifyPropertyChanged("aethersand4");
                }
            }
        }
        private bool _aethersand5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand5
        {
            get { return _aethersand5; }
            set
            {
                if (_aethersand5 != value)
                {
                    _aethersand5 = value;
                    Save();
                    NotifyPropertyChanged("aethersand5");
                }
            }
        }
        private bool _aethersand6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand6
        {
            get { return _aethersand6; }
            set
            {
                if (_aethersand6 != value)
                {
                    _aethersand6 = value;
                    Save();
                    NotifyPropertyChanged("aethersand6");
                }
            }
        }
        private bool _aethersand7;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand7
        {
            get { return _aethersand7; }
            set
            {
                if (_aethersand7 != value)
                {
                    _aethersand7 = value;
                    Save();
                    NotifyPropertyChanged("aethersand7");
                }
            }
        }
        private bool _aethersand8;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand8
        {
            get { return _aethersand8; }
            set
            {
                if (_aethersand8 != value)
                {
                    _aethersand8 = value;
                    Save();
                    NotifyPropertyChanged("aethersand8");
                }
            }
        }

        private bool _aethersand9;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool aethersand9
        {
            get { return _aethersand9; }
            set
            {
                if (_aethersand9 != value)
                {
                    _aethersand9 = value;
                    Save();
                    NotifyPropertyChanged("aethersand9");
                }
            }
        }

        private bool _food1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool food1
        {
            get { return _food1; }
            set
            {
                if (_food1 != value)
                {
                    _food1 = value;
                    Save();
                    NotifyPropertyChanged("food1");
                }
            }
        }

        private bool _food2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool food2
        {
            get { return _food2; }
            set
            {
                if (_food2 != value)
                {
                    _food2 = value;
                    Save();
                    NotifyPropertyChanged("food2");
                }
            }
        }

        private bool _food3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool food3
        {
            get { return _food3; }
            set
            {
                if (_food3 != value)
                {
                    _food3 = value;
                    Save();
                    NotifyPropertyChanged("food3");
                }
            }
        }

        private bool _food4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool food4
        {
            get { return _food4; }
            set
            {
                if (_food4 != value)
                {
                    _food4 = value;
                    Save();
                    NotifyPropertyChanged("food4");
                }
            }
        }

        private bool _potion1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool potion1
        {
            get { return _potion1; }
            set
            {
                if (_potion1 != value)
                {
                    _potion1 = value;
                    Save();
                    NotifyPropertyChanged("potion1");
                }
            }
        }

        private bool _potion2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool potion2
        {
            get { return _potion2; }
            set
            {
                if (_potion2 != value)
                {
                    _potion2 = value;
                    Save();
                    NotifyPropertyChanged("potion2");
                }
            }
        }

        private bool _potion3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool potion3
        {
            get { return _potion3; }
            set
            {
                if (_potion3 != value)
                {
                    _potion3 = value;
                    Save();
                    NotifyPropertyChanged("potion3");
                }
            }
        }

        private bool _potion4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool potion4
        {
            get { return _potion4; }
            set
            {
                if (_potion4 != value)
                {
                    _potion4 = value;
                    Save();
                    NotifyPropertyChanged("potion4");
                }
            }
        }

        private bool _firecrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool firecrystal
        {
            get { return _firecrystal; }
            set
            {
                if (_firecrystal != value)
                {
                    _firecrystal = value;
                    Save();
                    NotifyPropertyChanged("firecrystal");
                }
            }
        }

        private bool _icecrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool icecrystal
        {
            get { return _icecrystal; }
            set
            {
                if (_icecrystal != value)
                {
                    _icecrystal = value;
                    Save();
                    NotifyPropertyChanged("icecrystal");
                }
            }
        }

        private bool _windcrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool windcrystal
        {
            get { return _windcrystal; }
            set
            {
                if (_windcrystal != value)
                {
                    _windcrystal = value;
                    Save();
                    NotifyPropertyChanged("windcrystal");
                }
            }
        }

        private bool _earthcrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool earthcrystal
        {
            get { return _earthcrystal; }
            set
            {
                if (_earthcrystal != value)
                {
                    _earthcrystal = value;
                    Save();
                    NotifyPropertyChanged("earthcrystal");
                }
            }
        }

        private bool _lightningcrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool lightningcrystal
        {
            get { return _lightningcrystal; }
            set
            {
                if (_lightningcrystal != value)
                {
                    _lightningcrystal = value;
                    Save();
                    NotifyPropertyChanged("lightningcrystal");
                }
            }
        }

        private bool _watercrystal;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool watercrystal
        {
            get { return _watercrystal; }
            set
            {
                if (_watercrystal != value)
                {
                    _watercrystal = value;
                    Save();
                    NotifyPropertyChanged("watercrystal");
                }
            }
        }


        private bool _materiaxii1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii1
        {
            get { return _materiaxii1; }
            set
            {
                if (_materiaxii1 != value)
                {
                    _materiaxii1 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii1");
                }
            }
        }


        private bool _materiaxii2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii2
        {
            get { return _materiaxii2; }
            set
            {
                if (_materiaxii2 != value)
                {
                    _materiaxii2 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii2");
                }
            }
        }

        private bool _materiaxii3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii3
        {
            get { return _materiaxii3; }
            set
            {
                if (_materiaxii3 != value)
                {
                    _materiaxii3 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii3");
                }
            }
        }

        private bool _materiaxii4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii4
        {
            get { return _materiaxii4; }
            set
            {
                if (_materiaxii4 != value)
                {
                    _materiaxii4 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii4");
                }
            }
        }

        private bool _materiaxii5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii5
        {
            get { return _materiaxii5; }
            set
            {
                if (_materiaxii5 != value)
                {
                    _materiaxii5 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii5");
                }
            }
        }

        private bool _materiaxii6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxii6
        {
            get { return _materiaxii6; }
            set
            {
                if (_materiaxii6 != value)
                {
                    _materiaxii6 = value;
                    Save();
                    NotifyPropertyChanged("materiaxii6");
                }
            }
        }

        private bool _materiaxi1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi1
        {
            get { return _materiaxi1; }
            set
            {
                if (_materiaxi1 != value)
                {
                    _materiaxi1 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi1");
                }
            }
        }


        private bool _materiaxi2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi2
        {
            get { return _materiaxi2; }
            set
            {
                if (_materiaxi2 != value)
                {
                    _materiaxi2 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi2");
                }
            }
        }

        private bool _materiaxi3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi3
        {
            get { return _materiaxi3; }
            set
            {
                if (_materiaxi3 != value)
                {
                    _materiaxi3 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi3");
                }
            }
        }

        private bool _materiaxi4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi4
        {
            get { return _materiaxi4; }
            set
            {
                if (_materiaxi4 != value)
                {
                    _materiaxi4 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi4");
                }
            }
        }

        private bool _materiaxi5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi5
        {
            get { return _materiaxi5; }
            set
            {
                if (_materiaxi5 != value)
                {
                    _materiaxi5 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi5");
                }
            }
        }

        private bool _materiaxi6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaxi6
        {
            get { return _materiaxi6; }
            set
            {
                if (_materiaxi6 != value)
                {
                    _materiaxi6 = value;
                    Save();
                    NotifyPropertyChanged("materiaxi6");
                }
            }
        }

        private bool _materiax1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax1
        {
            get { return _materiax1; }
            set
            {
                if (_materiax1 != value)
                {
                    _materiax1 = value;
                    Save();
                    NotifyPropertyChanged("materiax1");
                }
            }
        }


        private bool _materiax2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax2
        {
            get { return _materiax2; }
            set
            {
                if (_materiax2 != value)
                {
                    _materiax2 = value;
                    Save();
                    NotifyPropertyChanged("materiax2");
                }
            }
        }

        private bool _materiax3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax3
        {
            get { return _materiax3; }
            set
            {
                if (_materiax3 != value)
                {
                    _materiax3 = value;
                    Save();
                    NotifyPropertyChanged("materiax3");
                }
            }
        }

        private bool _materiax4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax4
        {
            get { return _materiax4; }
            set
            {
                if (_materiax4 != value)
                {
                    _materiax4 = value;
                    Save();
                    NotifyPropertyChanged("materiax4");
                }
            }
        }

        private bool _materiax5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax5
        {
            get { return _materiax5; }
            set
            {
                if (_materiax5 != value)
                {
                    _materiax5 = value;
                    Save();
                    NotifyPropertyChanged("materiax5");
                }
            }
        }

        private bool _materiax6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiax6
        {
            get { return _materiax6; }
            set
            {
                if (_materiax6 != value)
                {
                    _materiax6 = value;
                    Save();
                    NotifyPropertyChanged("materiax6");
                }
            }
        }



        private bool _materiaix1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix1
        {
            get { return _materiaix1; }
            set
            {
                if (_materiaix1 != value)
                {
                    _materiaix1 = value;
                    Save();
                    NotifyPropertyChanged("materiaix1");
                }
            }
        }


        private bool _materiaix2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix2
        {
            get { return _materiaix2; }
            set
            {
                if (_materiaix2 != value)
                {
                    _materiaix2 = value;
                    Save();
                    NotifyPropertyChanged("materiaix2");
                }
            }
        }

        private bool _materiaix3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix3
        {
            get { return _materiaix3; }
            set
            {
                if (_materiaix3 != value)
                {
                    _materiaix3 = value;
                    Save();
                    NotifyPropertyChanged("materiaix3");
                }
            }
        }

        private bool _materiaix4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix4
        {
            get { return _materiaix4; }
            set
            {
                if (_materiaix4 != value)
                {
                    _materiaix4 = value;
                    Save();
                    NotifyPropertyChanged("materiaix4");
                }
            }
        }

        private bool _materiaix5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix5
        {
            get { return _materiaix5; }
            set
            {
                if (_materiaix5 != value)
                {
                    _materiaix5 = value;
                    Save();
                    NotifyPropertyChanged("materiaix5");
                }
            }
        }

        private bool _materiaix6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaix6
        {
            get { return _materiaix6; }
            set
            {
                if (_materiaix6 != value)
                {
                    _materiaix6 = value;
                    Save();
                    NotifyPropertyChanged("materiaix6");
                }
            }
        }


        private bool _materiaviii1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii1
        {
            get { return _materiaviii1; }
            set
            {
                if (_materiaviii1 != value)
                {
                    _materiaviii1 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii1");
                }
            }
        }


        private bool _materiaviii2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii2
        {
            get { return _materiaviii2; }
            set
            {
                if (_materiaviii2 != value)
                {
                    _materiaviii2 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii2");
                }
            }
        }

        private bool _materiaviii3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii3
        {
            get { return _materiaviii3; }
            set
            {
                if (_materiaviii3 != value)
                {
                    _materiaviii3 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii3");
                }
            }
        }

        private bool _materiaviii4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii4
        {
            get { return _materiaviii4; }
            set
            {
                if (_materiaviii4 != value)
                {
                    _materiaviii4 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii4");
                }
            }
        }

        private bool _materiaviii5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii5
        {
            get { return _materiaviii5; }
            set
            {
                if (_materiaviii5 != value)
                {
                    _materiaviii5 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii5");
                }
            }
        }

        private bool _materiaviii6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaviii6
        {
            get { return _materiaviii6; }
            set
            {
                if (_materiaviii6 != value)
                {
                    _materiaviii6 = value;
                    Save();
                    NotifyPropertyChanged("materiaviii6");
                }
            }
        }

        private bool _materiavii1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii1
        {
            get { return _materiavii1; }
            set
            {
                if (_materiavii1 != value)
                {
                    _materiavii1 = value;
                    Save();
                    NotifyPropertyChanged("materiavii1");
                }
            }
        }


        private bool _materiavii2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii2
        {
            get { return _materiavii2; }
            set
            {
                if (_materiavii2 != value)
                {
                    _materiavii2 = value;
                    Save();
                    NotifyPropertyChanged("materiavii2");
                }
            }
        }

        private bool _materiavii3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii3
        {
            get { return _materiavii3; }
            set
            {
                if (_materiavii3 != value)
                {
                    _materiavii3 = value;
                    Save();
                    NotifyPropertyChanged("materiavii3");
                }
            }
        }

        private bool _materiavii4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii4
        {
            get { return _materiavii4; }
            set
            {
                if (_materiavii4 != value)
                {
                    _materiavii4 = value;
                    Save();
                    NotifyPropertyChanged("materiavii4");
                }
            }
        }

        private bool _materiavii5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii5
        {
            get { return _materiavii5; }
            set
            {
                if (_materiavii5 != value)
                {
                    _materiavii5 = value;
                    Save();
                    NotifyPropertyChanged("materiavii5");
                }
            }
        }

        private bool _materiavii6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavii6
        {
            get { return _materiavii6; }
            set
            {
                if (_materiavii6 != value)
                {
                    _materiavii6 = value;
                    Save();
                    NotifyPropertyChanged("materiavii6");
                }
            }
        }

        private bool _materiavi1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi1
        {
            get { return _materiavi1; }
            set
            {
                if (_materiavi1 != value)
                {
                    _materiavi1 = value;
                    Save();
                    NotifyPropertyChanged("materiavi1");
                }
            }
        }


        private bool _materiavi2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi2
        {
            get { return _materiavi2; }
            set
            {
                if (_materiavi2 != value)
                {
                    _materiavi2 = value;
                    Save();
                    NotifyPropertyChanged("materiavi2");
                }
            }
        }

        private bool _materiavi3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi3
        {
            get { return _materiavi3; }
            set
            {
                if (_materiavi3 != value)
                {
                    _materiavi3 = value;
                    Save();
                    NotifyPropertyChanged("materiavi3");
                }
            }
        }

        private bool _materiavi4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi4
        {
            get { return _materiavi4; }
            set
            {
                if (_materiavi4 != value)
                {
                    _materiavi4 = value;
                    Save();
                    NotifyPropertyChanged("materiavi4");
                }
            }
        }

        private bool _materiavi5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi5
        {
            get { return _materiavi5; }
            set
            {
                if (_materiavi5 != value)
                {
                    _materiavi5 = value;
                    Save();
                    NotifyPropertyChanged("materiavi5");
                }
            }
        }

        private bool _materiavi6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiavi6
        {
            get { return _materiavi6; }
            set
            {
                if (_materiavi6 != value)
                {
                    _materiavi6 = value;
                    Save();
                    NotifyPropertyChanged("materiavi6");
                }
            }
        }

        private bool _materiav1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav1
        {
            get { return _materiav1; }
            set
            {
                if (_materiav1 != value)
                {
                    _materiav1 = value;
                    Save();
                    NotifyPropertyChanged("materiav1");
                }
            }
        }


        private bool _materiav2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav2
        {
            get { return _materiav2; }
            set
            {
                if (_materiav2 != value)
                {
                    _materiav2 = value;
                    Save();
                    NotifyPropertyChanged("materiavi2");
                }
            }
        }

        private bool _materiav3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav3
        {
            get { return _materiav3; }
            set
            {
                if (_materiav3 != value)
                {
                    _materiav3 = value;
                    Save();
                    NotifyPropertyChanged("materiav3");
                }
            }
        }

        private bool _materiav4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav4
        {
            get { return _materiav4; }
            set
            {
                if (_materiav4 != value)
                {
                    _materiav4 = value;
                    Save();
                    NotifyPropertyChanged("materiav4");
                }
            }
        }

        private bool _materiav5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav5
        {
            get { return _materiav5; }
            set
            {
                if (_materiav5 != value)
                {
                    _materiav5 = value;
                    Save();
                    NotifyPropertyChanged("materiav5");
                }
            }
        }

        private bool _materiav6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiav6
        {
            get { return _materiav6; }
            set
            {
                if (_materiav6 != value)
                {
                    _materiav6 = value;
                    Save();
                    NotifyPropertyChanged("materiav6");
                }
            }
        }

        private bool _materiaiv1;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv1
        {
            get { return _materiaiv1; }
            set
            {
                if (_materiaiv1 != value)
                {
                    _materiaiv1 = value;
                    Save();
                    NotifyPropertyChanged("materiaiv1");
                }
            }
        }


        private bool _materiaiv2;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv2
        {
            get { return _materiaiv2; }
            set
            {
                if (_materiaiv2 != value)
                {
                    _materiaiv2 = value;
                    Save();
                    NotifyPropertyChanged("materiaivi2");
                }
            }
        }

        private bool _materiaiv3;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv3
        {
            get { return _materiaiv3; }
            set
            {
                if (_materiaiv3 != value)
                {
                    _materiaiv3 = value;
                    Save();
                    NotifyPropertyChanged("materiaiv3");
                }
            }
        }

        private bool _materiaiv4;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv4
        {
            get { return _materiaiv4; }
            set
            {
                if (_materiaiv4 != value)
                {
                    _materiaiv4 = value;
                    Save();
                    NotifyPropertyChanged("materiaiv4");
                }
            }
        }

        private bool _materiaiv5;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv5
        {
            get { return _materiaiv5; }
            set
            {
                if (_materiaiv5 != value)
                {
                    _materiaiv5 = value;
                    Save();
                    NotifyPropertyChanged("materiaiv5");
                }
            }
        }

        private bool _materiaiv6;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool materiaiv6
        {
            get { return _materiaiv6; }
            set
            {
                if (_materiaiv6 != value)
                {
                    _materiaiv6 = value;
                    Save();
                    NotifyPropertyChanged("materiaiv6");
                }
            }
        }

        private int _selectedMateriaIndex;
        [Setting]
        [DefaultValueAttribute(0)]
        public int selectedMateriaIndex
        {
            get { return _selectedMateriaIndex; }
            set
            {
                if (_selectedMateriaIndex != value)
                {
                    _selectedMateriaIndex = value;
                    Save();
                    NotifyPropertyChanged("selectedMateriaIndex");
                }
            }
        }

        private bool _useCraftingFood;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool useCraftingFood
        {
            get { return _useCraftingFood; }
            set
            {
                if (_useCraftingFood != value)
                {
                    _useCraftingFood = value;
                    Save();
                    NotifyPropertyChanged("useCraftingFood");
                }
            }
        }

        private bool _refillScrips;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool refillScrips
        {
            get { return _refillScrips; }
            set
            {
                if (_refillScrips != value)
                {
                    _refillScrips = value;
                    Save();
                    NotifyPropertyChanged("refillScrips");
                }
            }
        }

        private bool _purchaseHiCordials;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool purchaseHiCordials
        {
            get { return _purchaseHiCordials; }
            set
            {
                if (_purchaseHiCordials != value)
                {
                    _purchaseHiCordials = value;
                    Save();
                    NotifyPropertyChanged("purchaseHiCordials");
                }
            }
        }


        private bool _customBoatOrders;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool customBoatOrders
        {
            get { return _customBoatOrders; }
            set
            {
                if (_customBoatOrders != value)
                {
                    _customBoatOrders = value;
                    Save();
                    NotifyPropertyChanged("customBoatOrders");
                }
            }
        }

        private bool _resumeLisbeth;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool resumeLisbeth
        {
            get { return _resumeLisbeth; }
            set
            {
                if (_resumeLisbeth != value)
                {
                    _resumeLisbeth = value;
                    Save();
                    NotifyPropertyChanged("resumeLisbeth");
                }
            }
        }

        private bool _LoggingMode;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool LoggingMode
        {
            get { return _LoggingMode; }
            set
            {
                if (_LoggingMode != value)
                {
                    _LoggingMode = value;
                    Save();
                    NotifyPropertyChanged("loggingMode");
                }
            }
        }

        private int _BaitRestockThreshold;
        [Setting]
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
                    NotifyPropertyChanged("baitRestockThreshold");
                }
            }
        }

        private int _BaitRestockAmount;
        [Setting]
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
                    NotifyPropertyChanged("baitRestockAmount");
                }
            }
        }

        private bool _OpenWorldFishing;
        [Setting]
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
                    NotifyPropertyChanged("openWorldFishing");
                }
            }
        }

        private FishPriority _FishPriority;
        [Setting]
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
                    NotifyPropertyChanged("fishPriority");
                }
            }
        }

        private bool _LateBoatQueue;
        [Setting]
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
                    NotifyPropertyChanged("lateBoatQueue");
                }
            }
        }

        private bool _OceanFood;
        [Setting]
        [DefaultValueAttribute(false)]
        public bool OceanFood
        {
            get { return _OceanFood; }
            set
            {
                if (_OceanFood != value)
                {
                    _OceanFood = value;
                    Save();
                    NotifyPropertyChanged("OceanFood");
                }
            }
        }

        private FishingRoute _FishingRoute;
        [Setting]
        [DefaultValueAttribute(FishingRoute.Indigo)]
        public FishingRoute FishingRoute
        {
            get { return _FishingRoute; }
            set
            {
                if (_FishingRoute != value)
                {
                    _FishingRoute = value;
                    Save();
                    NotifyPropertyChanged("fishingRoute");
                }
            }
        }

        private FullGPAction _FullGPAction;
        [Setting]
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
                    NotifyPropertyChanged("fullGPAction");
                }
            }
        }

        private ShouldUsePatience _Patience;
        [Setting]
        [DefaultValueAttribute(ShouldUsePatience.OnlyForSpecificFish)]
        public ShouldUsePatience Patience
        {
            get { return _Patience; }
            set
            {
                if (_Patience != value)
                {
                    _Patience = value;
                    Save();
                    NotifyPropertyChanged("usePatience");
                }
            }
        }

        private ExchangeFish _ExchangeFish;
        [Setting]
        [DefaultValueAttribute(ExchangeFish.Desynth)]
        public ExchangeFish ExchangeFish
        {
            get { return _ExchangeFish; }
            set
            {
                if (_ExchangeFish != value)
                {
                    _ExchangeFish = value;
                    Save();
                    NotifyPropertyChanged("exchangeFish");
                }
            }
        }

        // Necessary for UI Winforms Databinding
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}