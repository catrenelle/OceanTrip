using System;
using ff14bot.Helpers;

namespace Ocean_Trip
{
	/// <summary>
	/// Keeps Endeavor's InstanceContentOceanFishing memory offsets in sync with aers'
	/// FFXIVClientStructs (https://github.com/aers/FFXIVClientStructs) — the community-maintained
	/// struct-layout reference these offsets were originally confirmed against. Global-client only:
	/// RB_TC runs an older, separately-patched client build with its own unverified offsets (see
	/// Endeavor.Offsets), and main-branch FFXIVClientStructs data (which tracks the GLOBAL client's
	/// current patch) doesn't apply to it — this whole file is a no-op under RB_TC.
	///
	/// Runs at most once per bot session, off the game loop (fire-and-forget Task). Any failure —
	/// network down, GitHub unreachable, upstream file reformatted beyond what the parser expects —
	/// just leaves the last-known-good offsets in place, so this can never break fishing itself.
	/// </summary>
	public static class OceanFishingOffsetSync
	{
#if !RB_TC
		private const string SourceUrl =
			"https://raw.githubusercontent.com/aers/FFXIVClientStructs/main/FFXIVClientStructs/FFXIV/Client/Game/InstanceContent/InstanceContentOceanFishing.cs";

		private static readonly System.Text.RegularExpressions.Regex FieldOffsetPattern =
			new System.Text.RegularExpressions.Regex(
				@"\[FieldOffset\(0x([0-9A-Fa-f]+)\)\]\s*(?:public|private|internal)\s+[\w<>\[\],\.]+\s+(\w+)\s*;",
				System.Text.RegularExpressions.RegexOptions.Compiled);

		private static bool _hasRun;

		// Upstream field name -> the Endeavor.Offsets field it maps to. Only these are tracked;
		// every other field in the struct (Duration, WeatherId, etc.) is parsed but ignored since
		// Endeavor doesn't read them. Duration turned out to be a constant (always 420, not this
		// stop's actual length) — see the long comment on Endeavor.Offsets.timeOffsetOffset —
		// ContentTimeLeft replaced it but lives in a different upstream file (ContentDirector.cs)
		// this sync doesn't fetch, so it isn't self-updating like the fields below.
		private static readonly (string FieldName, Func<int> Get, Action<int> Set)[] TrackedFields =
		{
			("Status", () => Endeavor.Offsets.statusOffset, v => Endeavor.Offsets.statusOffset = v),
			("CurrentZone", () => Endeavor.Offsets.zoneOffset, v => Endeavor.Offsets.zoneOffset = v),
			("TimeOffset", () => Endeavor.Offsets.timeOffsetOffset, v => Endeavor.Offsets.timeOffsetOffset = v),
			("AllResultSize", () => Endeavor.Offsets.allResultSizeOffset, v => Endeavor.Offsets.allResultSizeOffset = v),
			("LocalIndexInAllResult", () => Endeavor.Offsets.localIndexInAllResultOffset, v => Endeavor.Offsets.localIndexInAllResultOffset = v),
			("IndividualResult", () => Endeavor.Offsets.individualResultOffset, v => Endeavor.Offsets.individualResultOffset = v),
			("LocalPlayerAllResult", () => Endeavor.Offsets.localPlayerAllResultOffset, v => Endeavor.Offsets.localPlayerAllResultOffset = v),
			("AllResults", () => Endeavor.Offsets.allResultsOffset, v => Endeavor.Offsets.allResultsOffset = v),
			("Mission1Type", () => Endeavor.Offsets.mission1TypeOffset, v => Endeavor.Offsets.mission1TypeOffset = v),
			("Mission2Type", () => Endeavor.Offsets.mission2TypeOffset, v => Endeavor.Offsets.mission2TypeOffset = v),
			("Mission3Type", () => Endeavor.Offsets.mission3TypeOffset, v => Endeavor.Offsets.mission3TypeOffset = v),
			("Mission1Progress", () => Endeavor.Offsets.mission1ProgressOffset, v => Endeavor.Offsets.mission1ProgressOffset = v),
			("Mission2Progress", () => Endeavor.Offsets.mission2ProgressOffset, v => Endeavor.Offsets.mission2ProgressOffset = v),
			("Mission3Progress", () => Endeavor.Offsets.mission3ProgressOffset, v => Endeavor.Offsets.mission3ProgressOffset = v),
		};

		private static string CachePath =>
			System.IO.Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanFishingOffsets.json");

		/// <summary>
		/// Fire-and-forget: loads any previously-synced offsets immediately (cheap, local), then
		/// checks upstream in the background. Safe to call from multiple entry points (Start(),
		/// OnButtonPress()) — only the first call in a process actually does anything.
		/// </summary>
		public static void RunAsync()
		{
			if (_hasRun)
				return;
			_hasRun = true;

			LoadCachedOverrides();

			System.Threading.Tasks.Task.Run(() =>
			{
				try
				{
					CheckUpstream();
				}
				catch (Exception ex)
				{
					Logging.Write($"[Ocean Trip] OceanFishing offset sync failed, keeping last-known offsets: {ex.Message}");
				}
			});
		}

		private static void LoadCachedOverrides()
		{
			try
			{
				if (!System.IO.File.Exists(CachePath))
					return;

				var cached = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, int>>(
					System.IO.File.ReadAllText(CachePath));
				if (cached == null)
					return;

				foreach (var (fieldName, _, set) in TrackedFields)
				{
					if (cached.TryGetValue(fieldName, out var offset))
						set(offset);
				}
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Failed to load cached OceanFishing offsets, using built-in defaults: {ex.Message}");
			}
		}

		private static void CheckUpstream()
		{
			string source;
			using (var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(10) })
				source = http.GetStringAsync(SourceUrl).GetAwaiter().GetResult();

			var parsed = ParseOffsets(source);
			if (parsed.Count == 0)
			{
				Logging.Write("[Ocean Trip] OceanFishing offset sync: couldn't parse any fields from FFXIVClientStructs — leaving offsets unchanged.");
				return;
			}

			bool changed = false;
			foreach (var (fieldName, get, set) in TrackedFields)
			{
				if (!parsed.TryGetValue(fieldName, out var newOffset))
					continue;

				int current = get();
				if (current == newOffset)
					continue;

				Logging.Write($"[Ocean Trip] OceanFishing offset changed upstream: {fieldName} 0x{current:X} -> 0x{newOffset:X}. Self-updating.");
				set(newOffset);
				changed = true;
			}

			if (changed)
				SaveCachedOverrides();
		}

		private static System.Collections.Generic.Dictionary<string, int> ParseOffsets(string source)
		{
			var result = new System.Collections.Generic.Dictionary<string, int>();

			foreach (System.Text.RegularExpressions.Match match in FieldOffsetPattern.Matches(source))
			{
				int offset = Convert.ToInt32(match.Groups[1].Value, 16);
				string fieldName = match.Groups[2].Value;

				// First occurrence wins — the fields we track only appear once, at the top level of
				// the struct, but nested result structs later in the file could coincidentally reuse
				// a name (they don't today; this just avoids a future footgun if they ever do).
				if (!result.ContainsKey(fieldName))
					result[fieldName] = offset;
			}

			return result;
		}

		private static void SaveCachedOverrides()
		{
			try
			{
				var toSave = new System.Collections.Generic.Dictionary<string, int>();
				foreach (var (fieldName, get, _) in TrackedFields)
					toSave[fieldName] = get();

				System.IO.File.WriteAllText(CachePath, Newtonsoft.Json.JsonConvert.SerializeObject(toSave, Newtonsoft.Json.Formatting.Indented));
			}
			catch (Exception ex)
			{
				Logging.Write($"[Ocean Trip] Failed to save updated OceanFishing offsets: {ex.Message}");
			}
		}
#else
		/// <summary>
		/// No-op under RB_TC — that client build's offsets are separately pinned (see
		/// Endeavor.Offsets) and aren't tracked by this file at all.
		/// </summary>
		public static void RunAsync()
		{
		}
#endif
	}
}
