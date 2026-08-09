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
#if !RB_TC
			internal const int statusOffset = 0x1FBC; // Patch 7.4
			internal const int zoneOffset = 0x1FC0;

			// Confirmed against FFXIVClientStructs' InstanceContentOceanFishing struct layout
			internal const int mission1TypeOffset = 0x25C8;
			internal const int mission2TypeOffset = 0x25CC;
			internal const int mission3TypeOffset = 0x25D0;
			internal const int mission1ProgressOffset = 0x25D4;
			internal const int mission2ProgressOffset = 0x25D6;
			internal const int mission3ProgressOffset = 0x25D8;
#else
//			internal const int statusOffset = 0x2014; // Patch 7.25
//			internal const int zoneOffset = 0x2018;
			internal const int statusOffset = 0x1E14; // Patch 7.20
			internal const int zoneOffset = 0x1E18;
//			internal const int statusOffset = 0x1E0C; // Patch 7.10
//			internal const int zoneOffset = 0x1E10;
//			internal const int statusOffset = 0x1DBC; // Patch 7.00
//			internal const int zoneOffset = 0x1DC0;

			// Mission offsets are NOT verified for the TC client's older patch — the struct layout
			// may differ from the current-patch layout above. Mission tracking properties below
			// return 0 (unavailable) under RB_TC rather than guessing at unverified offsets.
#endif
		}

		private IntPtr DirectorPtr;

		public void CheckDirector()
		{
			// Are we on the boat?
			if ((WorldManager.RawZoneId == Zones.TheEndeavor || WorldManager.RawZoneId == Zones.TheEndeaver_Ruby || WorldManager.RawZoneId == Zones.TheEndeavor_Thavnair)
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

#if !RB_TC
		private uint ReadMissionUInt(int offset)
		{
			if (DirectorPtr == IntPtr.Zero)
				return 0;
			return Core.Memory.Read<uint>(DirectorPtr + offset);
		}

		private ushort ReadMissionUShort(int offset)
		{
			if (DirectorPtr == IntPtr.Zero)
				return 0;
			return Core.Memory.Read<ushort>(DirectorPtr + offset);
		}
#endif

		// Row ID into the IKDPlayerMissionCondition sheet (0 = no mission in this slot / not yet loaded)
		public uint Mission1Type =>
#if !RB_TC
			ReadMissionUInt(Offsets.mission1TypeOffset);
#else
			0;
#endif

		public uint Mission2Type =>
#if !RB_TC
			ReadMissionUInt(Offsets.mission2TypeOffset);
#else
			0;
#endif

		public uint Mission3Type =>
#if !RB_TC
			ReadMissionUInt(Offsets.mission3TypeOffset);
#else
			0;
#endif

		public ushort Mission1Progress =>
#if !RB_TC
			ReadMissionUShort(Offsets.mission1ProgressOffset);
#else
			0;
#endif

		public ushort Mission2Progress =>
#if !RB_TC
			ReadMissionUShort(Offsets.mission2ProgressOffset);
#else
			0;
#endif

		public ushort Mission3Progress =>
#if !RB_TC
			ReadMissionUShort(Offsets.mission3ProgressOffset);
#else
			0;
#endif

		public bool shouldFish
		{
			get
			{
				bool response;

				switch (Status)
				{
					case FishingStatus.Fishing:
						response = true;
						break;
					default:
						response = false;
						break;
				}

				return response;
			}
		}

		public bool waitingOnBoat
		{
			get 
			{
				bool response;

				switch (Status)
				{
					case FishingStatus.NotActive:
					case FishingStatus.Finished:
						response =  false;
						break;
					default:
						response = true;
						break;
				}

				return response;
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
