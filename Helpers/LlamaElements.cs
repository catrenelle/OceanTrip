using ff14bot.Managers;
using ff14bot.RemoteWindows;
using ff14bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanTrip
{

    // Borrowered from nt153133 - https://github.com/nt153133/LlamaPlugins
    public static class LlamaElements
    {
#if !RB_DT
        private const int offset0 = 0x1CA;
        private const int offset2 = 0x160;
#else
        private const int offset0 = 0x1DA; 
        private const int offset2 = 0x170; 
#endif
        public static TwoInt[] ___Elements(string name)
        {

            AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName(name);
            if (windowByName != null)
            {
                ushort elementCount = ElementCount(name);

                IntPtr addr = Core.Memory.Read<IntPtr>(windowByName.Pointer + offset2);
                return Core.Memory.ReadArray<TwoInt>(addr, elementCount);
            }
            return null;
        }
        public static ushort ElementCount(string name)
        {

            AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName(name);
            if (windowByName != null)
            {
                return Core.Memory.Read<ushort>(windowByName.Pointer + offset0);
            }
            return 0;
        }
    }
}
