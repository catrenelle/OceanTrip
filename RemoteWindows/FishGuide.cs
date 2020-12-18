using System;
using System.Runtime.InteropServices;
using ff14bot;
using ff14bot.Managers;

namespace OceanTripPlanner.RemoteWindows
{
	public static class FishGuide
	{
		internal static IntPtr Vtable;
		internal static int TabSlotCount;
		internal static int TabStart;
		public const int TabCount = 40;
		public static int AgentId;
		public static IntPtr Pointer => AgentModule.GetAgentInterfaceById(AgentId).Pointer;

		public static AtkAddonControl WindowByName => RaptureAtkUnitManager.GetWindowByName("FishGuide");
		public static bool IsOpen => WindowByName != null;

		static FishGuide()
		{
			using (var pf = new GreyMagic.PatternFinder(Core.Memory))
			{
				Vtable = pf.Find("Search 48 8D 05 ? ? ? ? BA ? ? ? ? 48 89 03 48 8D 05 ? ? ? ? Add 3 TraceRelative");
				TabStart = pf.Find("Search 48 8D 43 ? 88 93 ? ? ? ? Add 3 Read8").ToInt32();
				TabSlotCount = pf.Find("Search 8D 4A ? 66 89 93 ? ? ? ? 48 89 93 ? ? ? ? Add 2 Read8").ToInt32();
				AgentId = AgentModule.FindAgentIdByVtable(Vtable);
			}
		}

		public static FishGuideItem[] GetTabList()
		{
			using (Core.Memory.TemporaryCacheState(enabledTemporarily: false))
			{
				return Core.Memory.ReadArray<FishGuideItem>(Pointer + TabStart - 0x6, TabSlotCount); //.Select(x => x.FishItem) as List<uint>;
			}
		}

		public static void ClickTab(int index)
		{
			AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("FishGuide");
			windowByName.SendAction(2, 3, 8, 3, (ulong)index);
		}

		public static void Toggle()
		{
			AgentModule.ToggleAgentInterfaceById(AgentId);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 0x8)]
	public struct FishGuideItem
	{
		public uint FishItem;
		public uint Unknown;
	}
}