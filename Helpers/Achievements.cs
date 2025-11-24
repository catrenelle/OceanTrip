using Buddy.Coroutines;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ocean_Trip.Helpers
{
	internal class Achievements
	{
		// This is from nt153133
		// Achievements do not load unless you open the achievement window
		public static async Task OpenWindow()
		{
			if (!Achievement.Instance.IsOpen)
			{
				AgentAchievement.Instance.Toggle();
				await Coroutine.Wait(2000, () => AgentAchievement.Instance.Status != 0);
				await Coroutine.Wait(10000, () => AgentAchievement.Instance.Status == 0);

				if (Achievement.Instance.IsOpen)
				{
					Achievement.Instance.Close(); ;
					await Coroutine.Wait(10000, () => !Achievement.Instance.IsOpen);
				}
			}
		}
	}
}
