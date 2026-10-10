using System.IO;
using System.Threading.Tasks;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner.IdleActivities
{
	/// <summary>
	/// Execute custom boat orders from BoatOrder.json
	/// </summary>
	public class CustomBoatOrderActivity : IIdleActivity
	{
		public int Priority => 20; // Execute after resume
		public string Name => "Custom Boat Orders";

		public async Task ExecuteAsync(IdleActivityContext context)
		{
			if (!OceanTripNewSettings.Instance.customBoatOrders)
				return;

			try
			{
				if (!context.IsFreeToCraft() || !File.Exists("BoatOrder.json"))
					return;

				await Lisbeth.ExecuteOrders(File.ReadAllText("BoatOrder.json"));
			}
			catch
			{
				context.LogCallback("Encountered error reading BoatOrder.json, ignoring the file.");
			}
		}
	}
}
