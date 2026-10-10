using System;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Centralized boat schedule calculation logic
	/// </summary>
	public static class BoatScheduleCalculator
	{
		/// <summary>
		/// Calculate the next boat departure time based on queue settings
		/// </summary>
		/// <param name="lateQueue">Whether to use late queue timing (13-15 min) vs early (0-13 min)</param>
		/// <returns>TimeSpan representing the next boat departure time</returns>
		public static TimeSpan GetNextBoatDepartureTime(bool lateQueue)
		{
			int queueMinute = lateQueue ? FishingConstants.LATE_QUEUE_MINUTE : FishingConstants.EARLY_QUEUE_MINUTE;
			DateTime now = DateTime.UtcNow;
			int currentHour = now.Hour;
			int currentMinute = now.Minute;

			// Boats depart every 2 hours on even hours
			bool isEvenHour = currentHour % 2 == 0;

			// Determine if we've missed the current boat window
			bool missedCurrentBoat = isEvenHour &&
				((currentMinute > 12 && !lateQueue) ||
				 (currentMinute > (FishingConstants.LATE_QUEUE_END_MINUTE - 1) && lateQueue));

			int departureHour = missedCurrentBoat
				? currentHour + 2  // Next boat is 2 hours away
				: currentHour + (currentHour % 2);  // Next boat this hour or next even hour

			return new TimeSpan(departureHour, queueMinute, 0);
		}

		/// <summary>
		/// Calculate time remaining until the next boat
		/// </summary>
		/// <param name="lateQueue">Whether to use late queue timing</param>
		/// <returns>TimeSpan representing time until next boat</returns>
		public static TimeSpan TimeUntilNextBoat(bool lateQueue)
		{
			TimeSpan nextDeparture = GetNextBoatDepartureTime(lateQueue);
			TimeSpan timeRemaining = nextDeparture - DateTime.UtcNow.TimeOfDay;
			return timeRemaining;
		}
	}
}
