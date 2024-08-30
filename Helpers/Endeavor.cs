using ff14bot.Managers;
using ff14bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using OceanTripPlanner.Definitions;
using System.Security.Policy;
using ProtoBuf.Grpc;

namespace Ocean_Trip
{
    public class Endeavor
    {
        internal static class Offsets
        {
#if !RB_DT
            internal const int statusOffset = 0x1CD4;
            internal const int zoneOffset = 0x1CD8;
#else
            internal const int statusOffset = 0x1DBC; 
            internal const int zoneOffset = 0x1DC0; 
#endif
        }

        private IntPtr DirectorPtr;

        public void CheckDirector()
        {
            // Are we on the boat?
            if ((WorldManager.RawZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby)
                && DirectorManager.ActiveDirector != null && (DirectorPtr == IntPtr.Zero || DirectorPtr != DirectorManager.ActiveDirector.Pointer))
            {
                DirectorPtr = DirectorManager.ActiveDirector.Pointer;
            }
        }

        public FishingStatus Status { 
            get 
            {
                if (DirectorPtr == IntPtr.Zero)
                    return FishingStatus.NotActive;
                else
                    return Core.Memory.Read<FishingStatus>(DirectorPtr + Offsets.statusOffset);
            }
        }

        // Should always return 0, 1, 2 if initialized, or 99 if uninitialized
        public uint CurrentZone
        {
            get
            {
                if (DirectorPtr == IntPtr.Zero)
                    return 99;
                else
                    return Core.Memory.Read<uint>(DirectorPtr + Offsets.zoneOffset);
            }
        }

        public bool shouldFish
        {
            get
            {
                switch (Status)
                {
                    case FishingStatus.Fishing:
                        return true;
                        break;
                    default:
                        return false;
                        break;
                }
            }
        }

        public bool waitingOnBoat
        {
            get 
            { 
                switch (Status)
                {
                    case FishingStatus.NotActive:
                    case FishingStatus.Finished:
                        return false;
                        break;
                    default:
                        return true;
                        break;
                }
            }
        }

        public enum FishingStatus : uint
        {
            WaitingForPlayers = 0,
            SwitchingZone = 1,
            Fishing = 2,
            NewZone = 3,
            Finished = 4,
            NotActive = 99,
        }
    }
}
