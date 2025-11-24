using System;
using System.IO;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Resume previous Lisbeth orders from saved state
	/// </summary>
	public class ResumeLisbethActivity : IIdleActivity
	{
		public int Priority => 10; // Execute first
		public string Name => "Resume Lisbeth";

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!OceanTripNewSettings.Instance.resumeLisbeth)
				return;

			try
			{
				// Use character settings directory path since WorldHelper.HomeWorldId may not be available
				var settingsDir = Path.Combine("Settings", $"{Core.Me.Name}");
				var resumePath = Path.Combine(settingsDir, "lisbeth-resume.json");

				if (!context.IsFreeToCraft() || !File.Exists(resumePath))
					return;

				var resumeData = File.ReadAllText(resumePath);
				if (resumeData != "[]")
				{
					context.LogCallback("Resuming last Lisbeth order.");
					await Lisbeth.ExecuteOrders(resumeData);
				}
			}
			catch
			{
				context.LogCallback("Encountered an error with lisbeth-resume.json, ignoring the file/setting.");
			}
		}
	}
}
