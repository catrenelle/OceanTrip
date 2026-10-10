using System;
using System.Collections.Generic;
using System.Linq;

namespace Ocean_Trip.UI.Wpf
{
	/// <summary>
	/// Lets the Schedule page fake being on a specific route/stop so CurrentRoutePageBehavior can
	/// be exercised and iterated on without actually being on the Ocean Fishing boat in-game.
	/// Plain static state shared between the two pages — both run on the same UI dispatcher thread,
	/// so no synchronization is needed.
	/// </summary>
	public static class RouteSimulation
	{
		public static bool Enabled { get; set; }
		public static string Route { get; set; } = "Indigo";
		public static int Leg { get; set; } = 0;

		public class SimulatedMission
		{
			public uint MissionId;
			public ushort Progress;
		}

		public static List<SimulatedMission> Missions { get; private set; } = new List<SimulatedMission>();

		/// <summary>
		/// Picks 3 random mission conditions from the real ikdMissionConditions.json pool (the same
		/// data Endeavor.Mission1/2/3Type resolve against live), each with a random in-progress
		/// count, so Current Route's Missions card and DH/TH mission badges can be exercised without
		/// actually being on a voyage. Progress is capped at Count-1 so a simulated mission never
		/// starts out already complete.
		///
		/// Groups by Text first, then picks 3 distinct groups — the JSON has several difficulty-tier
		/// duplicates of the same archetype as separate rows (e.g. "Catch fish with a weak bite (!)"
		/// appears 7 times with different Count values), and a real voyage never shows that same
		/// archetype twice, so sampling rows directly could otherwise land on 2-3 copies of it at once.
		/// </summary>
		public static void RerollMissions()
		{
			var conditions = Ocean_Trip.Definitions.MissionDataCache.GetConditions();
			if (conditions == null || conditions.Count == 0)
			{
				Missions = new List<SimulatedMission>();
				return;
			}

			var random = new Random();
			Missions = conditions
				.GroupBy(c => c.Text)
				.OrderBy(_ => random.Next())
				.Take(3)
				.Select(group =>
				{
					var condition = group.ElementAt(random.Next(group.Count()));
					return new SimulatedMission
					{
						MissionId = condition.Id,
						Progress = (ushort)random.Next(0, Math.Max(1, condition.Count)),
					};
				})
				.ToList();
		}
	}
}
