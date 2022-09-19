using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.RemoteWindows;

namespace OceanTrip
{
    public static class FishingLog
    {
        private static string name = "IKDFishingLog";
        private static int elementCount => LlamaElements.ElementCount(name);
        private static TwoInt[] Elements => LlamaElements.___Elements(name);

        public static string AreaName 
        { 
            get 
            {
                if (elementCount > 0)
                {
                    return Core.Memory.ReadString((IntPtr)Elements[2].Data, Encoding.UTF8);
                }

                return ""; 
            } 
        }

        public static int Points
        {
            get 
            {
                if (elementCount > 0)
                {
                    return Elements[7].TrimmedData;
                }

                return 0;
            }
        }
    }
}
