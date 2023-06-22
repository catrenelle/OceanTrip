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
    }
}
