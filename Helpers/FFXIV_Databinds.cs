using ff14bot.Managers;
using OceanTripPlanner;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using ff14bot.Helpers;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using OceanTripPlanner.Definitions;
using ff14bot;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;

namespace OceanTripPlanner
{
    public class FFXIV_Databinds : INotifyPropertyChanged
    {
        private static FFXIV_Databinds _databinds;

        public static FFXIV_Databinds Instance
        {
            get { return _databinds ?? (_databinds = new FFXIV_Databinds()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string _ragwormCount;
        public string ragwormCount { get => _ragwormCount; set { _ragwormCount = value; NotifyPropertyChanged("ragwormCount"); } }

        private string _krillCount;
        public string krillCount { get => _krillCount; set { _krillCount = value; NotifyPropertyChanged("krillCount"); } }

        private string _plumpwormCount;
        public string plumpwormCount { get => _plumpwormCount; set { _plumpwormCount = value; NotifyPropertyChanged("plumpwormCount"); } }

        private string _rattailCount;
        public string rattailCount { get => _rattailCount; set { _rattailCount = value; NotifyPropertyChanged("rattailCount"); } }

        private string _glowwormCount;
        public string glowwormCount { get => _glowwormCount; set { _glowwormCount = value; NotifyPropertyChanged("glowwormCount"); } }

        private string _heavysteeljigCount;
        public string heavysteeljigCount { get => _heavysteeljigCount; set { _heavysteeljigCount = value; NotifyPropertyChanged("heavysteeljigCount"); } }

        private string _shrimpcagefeederCount;
        public string shrimpcagefeederCount { get => _shrimpcagefeederCount; set { _shrimpcagefeederCount = value; NotifyPropertyChanged("shrimpcagefeederCount"); } }

        private string _pillbugCount;
        public string pillbugCount { get => _pillbugCount; set { _pillbugCount = value; NotifyPropertyChanged("pillbugCount"); } }

        private string _squidstripCount;
        public string squidstripCount { get => _squidstripCount; set { _squidstripCount = value; NotifyPropertyChanged("squidstripCount"); } }

        private string _mackerelstripCount;
        public string mackerelstripCount { get => _mackerelstripCount; set { _mackerelstripCount = value; NotifyPropertyChanged("mackerelstripCount"); } }

        private string _stoneflynymphCount;
        public string stoneflynymphCount { get => _stoneflynymphCount; set { _stoneflynymphCount = value; NotifyPropertyChanged("stoneflynymphCount"); } }

        // Indigo
        private bool _achievementMantas;
        public bool achievementMantas { get => _achievementMantas; set { _achievementMantas = value; NotifyPropertyChanged("achievementMantas"); } }
        private bool _achievementOctopods;
        public bool achievementOctopods { get => _achievementOctopods; set { _achievementOctopods = value; NotifyPropertyChanged("achievementOctopods"); } }
        private bool _achievementSharks;
        public bool achievementSharks { get => _achievementSharks; set { _achievementSharks = value; NotifyPropertyChanged("achievementSharks"); } }
        private bool _achievementJellyfish;
        public bool achievementJellyfish { get => _achievementJellyfish; set { _achievementJellyfish = value; NotifyPropertyChanged("achievementJellyfish"); } }
        private bool _achievementSeadragons;
        public bool achievementSeadragons { get => _achievementSeadragons; set { _achievementSeadragons = value; NotifyPropertyChanged("achievementSeadragons"); } }
        private bool _achievementBalloons;
        public bool achievementBalloons { get => _achievementBalloons; set { _achievementBalloons = value; NotifyPropertyChanged("achievementBalloons"); } }
        private bool _achievementCrabs;
        public bool achievementCrabs { get => _achievementCrabs; set { _achievementCrabs = value; NotifyPropertyChanged("achievementCrabs"); } }


        private bool _achievement5kindigo;
        public bool achievement5kindigo { get => _achievement5kindigo; set { _achievement5kindigo = value; NotifyPropertyChanged("achievement5kindigo"); } }
        private bool _achievement10kindigo;
        public bool achievement10kindigo { get => _achievement10kindigo; set { _achievement10kindigo = value; NotifyPropertyChanged("achievement10kindigo"); } }
        private bool _achievement16kindigo;
        public bool achievement16kindigo { get => _achievement16kindigo; set { _achievement16kindigo = value; NotifyPropertyChanged("achievement16kindigo"); } }
        private bool _achievement20kindigo;
        public bool achievement20kindigo { get => _achievement20kindigo; set { _achievement20kindigo = value; NotifyPropertyChanged("achievement20kindigo"); } }


        // Ruby
        private bool _achievementShrimp;
        public bool achievementShrimp { get => _achievementShrimp; set { _achievementShrimp = value; NotifyPropertyChanged("achievementShrimp"); } }
        private bool _achievementShellfish;
        public bool achievementShellfish { get => _achievementShellfish; set { _achievementShellfish = value; NotifyPropertyChanged("achievementShellfish"); } }
        private bool _achievementSquid;
        public bool achievementSquid { get => _achievementSquid; set { _achievementSquid = value; NotifyPropertyChanged("achievementSquid"); } }


        private bool _achievement5kruby;
        public bool achievement5kruby { get => _achievement5kruby; set { _achievement5kruby = value; NotifyPropertyChanged("achievement5kruby"); } }
        private bool _achievement10kruby;
        public bool achievement10kruby { get => _achievement10kruby; set { _achievement10kruby = value; NotifyPropertyChanged("achievement10kruby"); } }
        private bool _achievement16kruby;
        public bool achievement16kruby { get => _achievement16kruby; set { _achievement16kruby = value; NotifyPropertyChanged("achievement16kruby"); } }

        // Overall
        private bool _achievement100koverall;
        public bool achievement100koverall { get => _achievement100koverall; set { _achievement100koverall = value; NotifyPropertyChanged("achievement100koverall"); } }
        private bool _achievement500koverall;
        public bool achievement500koverall { get => _achievement500koverall; set { _achievement500koverall = value; NotifyPropertyChanged("achievement500koverall"); } }
        private bool _achievement1moverall;
        public bool achievement1moverall { get => _achievement1moverall; set { _achievement1moverall = value; NotifyPropertyChanged("achievement1moverall"); } }
        private bool _achievement3moverall;
        public bool achievement3moverall { get => _achievement3moverall; set { _achievement3moverall = value; NotifyPropertyChanged("achievement3moverall"); } }


        public void RefreshBait()
        {
            ragwormCount = DataManager.GetItem((uint)FishBait.Ragworm, false).ItemCount().ToString();
            krillCount = DataManager.GetItem((uint)FishBait.Krill, false).ItemCount().ToString();
            plumpwormCount = DataManager.GetItem((uint)FishBait.PlumpWorm, false).ItemCount().ToString();
            rattailCount = DataManager.GetItem((uint)FishBait.RatTail, false).ItemCount().ToString();
            glowwormCount = DataManager.GetItem((uint)FishBait.GlowWorm, false).ItemCount().ToString();
            heavysteeljigCount = DataManager.GetItem((uint)FishBait.HeavySteelJig, false).ItemCount().ToString();
            shrimpcagefeederCount = DataManager.GetItem((uint)FishBait.ShrimpCageFeeder, false).ItemCount().ToString();
            pillbugCount = DataManager.GetItem((uint)FishBait.PillBug, false).ItemCount().ToString();
            squidstripCount = DataManager.GetItem((uint)FishBait.SquidStrip, false).ItemCount().ToString();
            mackerelstripCount = DataManager.GetItem((uint)FishBait.MackerelStrip, false).ItemCount().ToString();
            stoneflynymphCount = DataManager.GetItem((uint)FishBait.StoneflyNymph, false).ItemCount().ToString();

            return;
        }

        public void RefreshAchievements()
        {
            achievementMantas = Achievements.HasAchievement(Definitions.Achievement.Mantas);
            achievementOctopods = Achievements.HasAchievement(Definitions.Achievement.Octopods);
            achievementSharks = Achievements.HasAchievement(Definitions.Achievement.Sharks);
            achievementJellyfish = Achievements.HasAchievement(Definitions.Achievement.Jellyfish);
            achievementSeadragons = Achievements.HasAchievement(Definitions.Achievement.Seadragons);
            achievementBalloons = Achievements.HasAchievement(Definitions.Achievement.Balloons);
            achievementCrabs = Achievements.HasAchievement(Definitions.Achievement.Crabs);
            achievement5kindigo = Achievements.HasAchievement(Definitions.Achievement.Indigo5kPoints);
            achievement10kindigo = Achievements.HasAchievement(Definitions.Achievement.Indigo10kPoints);
            achievement16kindigo = Achievements.HasAchievement(Definitions.Achievement.Indigo16kPoints);
            achievement20kindigo = Achievements.HasAchievement(Definitions.Achievement.Indigo20kPoints);

            achievementShrimp = Achievements.HasAchievement(Definitions.Achievement.Shrimp);
            achievementShellfish = Achievements.HasAchievement(Definitions.Achievement.Shellfish);
            achievementSquid = Achievements.HasAchievement(Definitions.Achievement.Squid);
            achievement5kruby = Achievements.HasAchievement(Definitions.Achievement.Ruby5kPoints);
            achievement10kruby = Achievements.HasAchievement(Definitions.Achievement.Ruby10kPoints);
            achievement16kruby = Achievements.HasAchievement(Definitions.Achievement.Ruby16kPoints);

            achievement100koverall = Achievements.HasAchievement(Definitions.Achievement.Overall100kPoints);
            achievement500koverall = Achievements.HasAchievement(Definitions.Achievement.Overall500kPoints);
            achievement1moverall = Achievements.HasAchievement(Definitions.Achievement.Overall1mPoints);
            achievement3moverall = Achievements.HasAchievement(Definitions.Achievement.Overall3mPoints);

            return;
        }
    }
}
