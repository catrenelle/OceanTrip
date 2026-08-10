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
			// Mutable (not const) so OceanFishingOffsetSync can self-update these at runtime if
			// aers' FFXIVClientStructs changes them upstream — see Helpers/OceanFishingOffsetSync.cs.
			// Values below are the last-known-good fallback, confirmed against
			// FFXIVClientStructs' InstanceContentOceanFishing struct layout as of patch 7.4.
			internal static int statusOffset = 0x1FBC;
			internal static int zoneOffset = 0x1FC0;

			// Duration = total length of the current stop, TimeOffset = elapsed time within it, both
			// in seconds — "time left at this stop" is Duration - TimeOffset. Used to gate GP-banking
			// (hold Thaliak's Favor/Hi-Cordial for a spectral-current burst) off once a current can no
			// longer occur this stop (inside the last 1:30).
			internal static int durationOffset = 0x1FC4;
			internal static int timeOffsetOffset = 0x1FC8;

			// Voyage results screen data — see Endeavor.VoyageResult / ReadVoyageResult(). AllResultSize
			// and LocalIndexInAllResult are single bytes; IndividualResult and LocalPlayerAllResult are
			// nested structs whose own field offsets are added in ReadVoyageResult() rather than pinned
			// here individually, since they're only ever read together.
			internal static int allResultSizeOffset = 0x23E8;
			internal static int localIndexInAllResultOffset = 0x23E9;
			internal static int individualResultOffset = 0x23EA;
			internal static int localPlayerAllResultOffset = 0x240C;
			internal static int allResultsOffset = 0x2434;

			internal static int mission1TypeOffset = 0x25C8;
			internal static int mission2TypeOffset = 0x25CC;
			internal static int mission3TypeOffset = 0x25D0;
			internal static int mission1ProgressOffset = 0x25D4;
			internal static int mission2ProgressOffset = 0x25D6;
			internal static int mission3ProgressOffset = 0x25D8;
#else
//			internal const int statusOffset = 0x2014; // Patch 7.25
//			internal const int zoneOffset = 0x2018;
			internal const int statusOffset = 0x1E14; // Patch 7.20
			internal const int zoneOffset = 0x1E18;
//			internal const int statusOffset = 0x1E0C; // Patch 7.10
//			internal const int zoneOffset = 0x1E10;
//			internal const int statusOffset = 0x1DBC; // Patch 7.00
//			internal const int zoneOffset = 0x1DC0;

			// Not independently confirmed on a live TC client — inferred by walking aers'
			// FFXIVClientStructs commit history (2026-08-09) for the InstanceContentOceanFishing
			// struct until Status/CurrentZone matched the Patch 7.20 values above exactly (commits
			// 628cf7b, 9966e4a, 0ad3f7e; 2025-03-25 to 2025-05-27 — all three agree, and all three
			// also agree with each other on Duration/TimeOffset below, same cross-check). Worst case
			// if this patch-window guess is wrong: Mission*Type resolves to an unrecognized row ID,
			// MissionDataCache.GetById() returns null, and mission tracking no-ops same as before.
			internal const int durationOffset = 0x1E1C;
			internal const int timeOffsetOffset = 0x1E20;

			// Same three-commit cross-check as Duration/TimeOffset above.
			internal const int allResultSizeOffset = 0x2240;
			internal const int localIndexInAllResultOffset = 0x2241;
			internal const int individualResultOffset = 0x2242;
			internal const int localPlayerAllResultOffset = 0x2264;
			internal const int allResultsOffset = 0x228C;

			internal const int mission1TypeOffset = 0x2420;
			internal const int mission2TypeOffset = 0x2424;
			internal const int mission3TypeOffset = 0x2428;
			internal const int mission1ProgressOffset = 0x242C;
			internal const int mission2ProgressOffset = 0x242E;
			internal const int mission3ProgressOffset = 0x2430;
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

		/// <summary>
		/// Seconds left at the current stop (Duration - TimeOffset, both read live), or null if that
		/// can't be determined right now — either the director isn't initialized yet, or the reading
		/// falls outside any plausible stop length (a stale/incorrect offset reads as some enormous or
		/// zero value rather than throwing, so this has to be sanity-checked rather than trusted
		/// blindly). Callers that gate behavior on this should treat null as "unknown" and fail toward
		/// whichever choice is safe to be wrong about, not assume a spectral current is imminent.
		/// </summary>
		public double? SecondsRemainingAtStop
		{
			get
			{
				if (DirectorPtr == IntPtr.Zero)
					return null;

				uint duration = Core.Memory.Read<uint>(DirectorPtr + Offsets.durationOffset);
				uint elapsed = Core.Memory.Read<uint>(DirectorPtr + Offsets.timeOffsetOffset);

				// Ocean Fishing stops are 15 minutes; anything wildly outside that range means the
				// offset (or its unit assumption) is wrong, not that the stop is actually that length.
				if (duration == 0 || duration > 3600)
					return null;

				return Math.Max(0, (double)duration - elapsed);
			}
		}

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

		// Row ID into the IKDPlayerMissionCondition sheet (0 = no mission in this slot / not yet loaded)
		public uint Mission1Type => ReadMissionUInt(Offsets.mission1TypeOffset);
		public uint Mission2Type => ReadMissionUInt(Offsets.mission2TypeOffset);
		public uint Mission3Type => ReadMissionUInt(Offsets.mission3TypeOffset);

		public ushort Mission1Progress => ReadMissionUShort(Offsets.mission1ProgressOffset);
		public ushort Mission2Progress => ReadMissionUShort(Offsets.mission2ProgressOffset);
		public ushort Mission3Progress => ReadMissionUShort(Offsets.mission3ProgressOffset);

		/// <summary>One boat passenger's line on the results screen leaderboard.</summary>
		public struct VoyagePlayerResult
		{
			public string Name;
			public int CaughtFish;
			public uint TotalPoints;
		}

		/// <summary>
		/// The voyage results screen, fully decoded — final score, bonuses earned (as row IDs into
		/// BonusDataCache), and placement among the up-to-10 tracked boat passengers. Read this once,
		/// while the IKDResult window is still up and before it's dismissed — the underlying memory is
		/// part of the director's own state, not the addon, but there's no guarantee it survives past
		/// voyage teardown once the window closes.
		/// </summary>
		public class VoyageResult
		{
			public uint TotalPoints;
			public uint ExperiencePoints;
			public ushort Scrip1Amount;
			public ushort Scrip2Amount;

			/// <summary>
			/// From LocalPlayerAllResult, not derived from TrackedPlayers[Placement] — the local
			/// player may not have placed in the top 10 on a large boat, in which case they're not in
			/// TrackedPlayers at all, but LocalPlayerAllResult is still populated as a direct copy.
			/// </summary>
			public int CaughtFish;

			/// <summary>Row IDs into BonusDataCache for every bonus earned this voyage (0 entries filtered out).</summary>
			public List<int> BonusIds = new List<int>();

			/// <summary>1-based rank among TrackedPlayers, or null if the local player wasn't in the tracked set.</summary>
			public int? Placement;

			/// <summary>Up to 10 passengers the results screen tracks, already sorted by the game (highest score first).</summary>
			public List<VoyagePlayerResult> TrackedPlayers = new List<VoyagePlayerResult>();
		}

		/// <summary>
		/// Reads and fully decodes the results-screen data. Returns null if the director isn't
		/// initialized (called too early/late relative to the results window being up).
		/// </summary>
		public VoyageResult ReadVoyageResult()
		{
			if (DirectorPtr == IntPtr.Zero)
				return null;

			var result = new VoyageResult
			{
				TotalPoints = Core.Memory.Read<uint>(DirectorPtr + Offsets.individualResultOffset + 0x2),
				ExperiencePoints = Core.Memory.Read<uint>(DirectorPtr + Offsets.individualResultOffset + 0xA),
				Scrip1Amount = Core.Memory.Read<ushort>(DirectorPtr + Offsets.individualResultOffset + 0xE),
				Scrip2Amount = Core.Memory.Read<ushort>(DirectorPtr + Offsets.individualResultOffset + 0x10),
				CaughtFish = Core.Memory.Read<ushort>(DirectorPtr + Offsets.localPlayerAllResultOffset + 0x2),
			};

			for (int i = 0; i < 16; i++)
			{
				byte bonusId = Core.Memory.Read<byte>(DirectorPtr + Offsets.individualResultOffset + 0x12 + i);
				if (bonusId != 0)
					result.BonusIds.Add(bonusId);
			}

			byte localIndex = Core.Memory.Read<byte>(DirectorPtr + Offsets.localIndexInAllResultOffset);

			// AllResultSize can read as garbage (or the array as not-yet-populated) before the results
			// window has actually finished calculating — cap at the array's real size (10) so a bad
			// reading can't walk off the end of it into unrelated memory.
			int trackedCount = Math.Min((int)Core.Memory.Read<byte>(DirectorPtr + Offsets.allResultSizeOffset), 10);

			for (int i = 0; i < trackedCount; i++)
			{
				int baseOffset = Offsets.allResultsOffset + i * 0x28;
				result.TrackedPlayers.Add(new VoyagePlayerResult
				{
					CaughtFish = Core.Memory.Read<ushort>(DirectorPtr + baseOffset + 0x2),
					TotalPoints = Core.Memory.Read<uint>(DirectorPtr + baseOffset + 0x4),
					Name = Core.Memory.ReadString(DirectorPtr + baseOffset + 0x8, Encoding.UTF8),
				});
			}

			if (localIndex < trackedCount)
				result.Placement = localIndex + 1;

			return result;
		}

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
