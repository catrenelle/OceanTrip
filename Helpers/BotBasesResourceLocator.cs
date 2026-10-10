using System;
using System.IO;
using System.Linq;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// RebornBuddy installs this bot base under BotBases/ using whichever of three directory-name
	/// variants the user picked at install time — "OceanTrip", "Ocean Trip", or "Ocean-Trip". Every
	/// place that reads a bundled resource (fish/route/mission JSON, XAML, icons/images) needs to
	/// probe all three to find the one actually in use.
	/// </summary>
	public static class BotBasesResourceLocator
	{
		private static readonly string[] PossibleDirectories = { "OceanTrip", "Ocean Trip", "Ocean-Trip" };

		/// <summary>
		/// Returns the first existing path under BotBases/&lt;variant&gt;/&lt;subPathSegments&gt; across
		/// the three directory-name variants, or null if none of them has the file.
		/// </summary>
		public static string Resolve(params string[] subPathSegments)
		{
			foreach (var dir in PossibleDirectories)
			{
				var candidate = subPathSegments.Aggregate(
					Path.Combine(Environment.CurrentDirectory, "BotBases", dir),
					Path.Combine);

				if (File.Exists(candidate))
					return candidate;
			}

			return null;
		}
	}
}
