using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing;
using TreeSharp;

namespace OceanTripPlanner.Helpers
{
	public static class Navigation
	{
		public static readonly WaitTimer waitTimer_0 = new WaitTimer(new TimeSpan(0, 0, 0, 15));
		internal static async Task<Queue<NavGraph.INode>> GenerateNodes(uint ZoneId, Vector3 xyz)
		{
			return await NavGraph.GetPathAsync((uint)ZoneId, xyz);
		}

		public static async Task<bool> GetTo(uint ZoneId, Vector3 XYZ)
		{            
			var path = await GenerateNodes(ZoneId, XYZ );
			
			if (path == null && WorldManager.ZoneId != ZoneId)
			{
				if (WorldManager.AetheryteIdsForZone(ZoneId).Length >= 1)
				{
					var AE = WorldManager.AetheryteIdsForZone(ZoneId).OrderBy(i => i.Item2.DistanceSqr(XYZ)).First();
					WorldManager.TeleportById(AE.Item1);
					await Coroutine.Wait(20000, () => WorldManager.ZoneId == AE.Item1);
					await Coroutine.Sleep(2000);
					return await GetTo(ZoneId, XYZ);
				}
				else
				{
					return false;
				}
			}

			if (path == null)
			{
				bool result = await FlightorMove(XYZ);
				Navigator.Stop();
				return result;
			}
			
			if (path.Count < 1)
			{
				LogCritical($"Couldn't get a path to {XYZ} on {ZoneId}, Stopping.");
				return false;
			}
			
			object object_0 = new object();
			var composite =  NavGraph.NavGraphConsumer(j => path);

			while (path.Count > 0)
			{
				composite.Start(object_0);
				await Coroutine.Yield();
				while (composite.Tick(object_0) == RunStatus.Running)
				{
					await Coroutine.Yield();
				}
				composite.Stop(object_0);
				await Coroutine.Yield();
			}
			
			Navigator.Stop();

			return Navigator.InPosition(Core.Me.Location, XYZ, 3);
		}
		
		public static void LogCritical(string text)
		{
			Logging.Write(Colors.OrangeRed, text);
		}
		
		internal static async Task<bool> FlightorMove(Vector3 loc)
		{
			var moving = MoveResult.GeneratingPath;
			while (!(moving == MoveResult.Done ||
					 moving == MoveResult.ReachedDestination ||
					 moving == MoveResult.Failed ||
					 moving == MoveResult.Failure ||
					 moving == MoveResult.PathGenerationFailed))
			{
				moving = Flightor.MoveTo(new FlyToParameters(loc));

				await Coroutine.Yield();
			}

			return moving == MoveResult.ReachedDestination;
		}
	}
}