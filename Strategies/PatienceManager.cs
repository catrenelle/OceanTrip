using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.Strategies
{
	/// <summary>
	/// Manages Patience buff application for fishing
	/// </summary>
	public class PatienceManager
	{
		private readonly bool _loggingEnabled;

		public PatienceManager(bool enableLogging = true)
		{
			_loggingEnabled = enableLogging;
		}

		/// <summary>
		/// Apply Patience or Patience II if not already active
		/// </summary>
		public async Task UsePatience()
		{
			if (ActionManager.CanCast(Actions.PatienceII, Core.Me) && !FishingManager.HasPatience)
			{
				Log($"Applying Patience II!", OceanLogLevel.Debug);
				ActionManager.DoAction(Actions.PatienceII, Core.Me);
			}
			else if (ActionManager.CanCast(Actions.Patience, Core.Me) && !FishingManager.HasPatience)
			{
				Log($"Applying Patience!", OceanLogLevel.Debug);
				ActionManager.DoAction(Actions.Patience, Core.Me);
			}

			await Coroutine.Yield();
		}

		/// <summary>
		/// Internal logging method
		/// </summary>
		private void Log(string text, OceanLogLevel level = OceanLogLevel.Info)
		{
			if (!_loggingEnabled)
				return;

			// Filter based on log level and settings
			if (level == OceanLogLevel.Debug && !OceanTripNewSettings.Instance.LoggingMode)
				return;

			var msg = string.Format("[Ocean Trip] " + text);
			Logging.Write(System.Windows.Media.Colors.Aqua, msg);
		}
	}
}
