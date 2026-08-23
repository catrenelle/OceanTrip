using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Best-effort detection of interfering Dalamud plugins (notably AutoHook) that fight OceanTrip for
	/// rod control. RebornBuddy runs as its OWN process (RebornBuddy64.exe) and reads FFXIV externally —
	/// it is NOT injected into the game — whereas Dalamud and its plugins live inside ffxiv_dx11.exe. So
	/// we must enumerate the GAME process's modules (via Core.Memory.Process), not RB's own. Dalamud.dll
	/// reliably shows up there when Dalamud is injected; AutoHook's plugin assembly may or may not appear
	/// as a module depending on how Dalamud loads it. The on-disk XIVLauncher folders are the backstop
	/// for "installed" (though installed != enabled this session). LOG-ONLY: it never changes behaviour.
	/// Rely on the behavioural watchdog (external-recaster detection) for the definitive "actively
	/// interfering" signal while fishing.
	/// </summary>
	public static class PluginDetector
	{
		// Plugins known to drive the fishing rod and conflict with OceanTrip. Case-insensitive substrings.
		private static readonly string[] InterferingPlugins = { "autohook" };
		private static readonly string[] DalamudMarkers = { "dalamud" };

		public static void ScanAndLog()
		{
			var hits = new List<string>();
			bool autohook = false;
			bool dalamud = false;
			bool installed = false;

			// 1) Loaded native modules of the FFXIV process (where Dalamud + its plugins actually live).
			// Core.Memory.Process is the game process RB attached to — NOT RebornBuddy64.exe.
			try
			{
				var gameProc = Core.Memory?.Process;
				if (gameProc == null)
				{
					Info("game process handle unavailable — skipping module scan.");
				}
				else
				{
					foreach (ProcessModule m in gameProc.Modules)
					{
						string name = m?.ModuleName ?? "";
						string low = name.ToLowerInvariant();
						if (DalamudMarkers.Any(low.Contains)) { dalamud = true; hits.Add($"module:{name}"); }
						if (InterferingPlugins.Any(low.Contains)) { autohook = true; hits.Add($"module:{name}"); }
					}
				}
			}
			catch (Exception ex) { Info($"module scan skipped: {ex.Message}"); }

			// 3) On-disk XIVLauncher plugin folders — detects "installed" (not necessarily enabled).
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string xl = Path.Combine(appData, "XIVLauncher");
				foreach (var sub in new[] { "installedPlugins", "devPlugins" })
				{
					string dir = Path.Combine(xl, sub);
					if (!Directory.Exists(dir)) continue;
					foreach (var pluginDir in Directory.GetDirectories(dir))
					{
						string leaf = Path.GetFileName(pluginDir).ToLowerInvariant();
						if (InterferingPlugins.Any(leaf.Contains)) { installed = true; hits.Add($"{sub}:{Path.GetFileName(pluginDir)}"); }
					}
				}
				// AutoHook keeps a per-plugin config here once it has run at least once.
				string cfgDir = Path.Combine(xl, "pluginConfigs");
				if (Directory.Exists(cfgDir))
				{
					foreach (var cfg in Directory.GetFiles(cfgDir).Concat(Directory.GetDirectories(cfgDir)))
					{
						string leaf = Path.GetFileName(cfg).ToLowerInvariant();
						if (InterferingPlugins.Any(leaf.Contains)) { installed = true; hits.Add($"config:{Path.GetFileName(cfg)}"); }
					}
				}
			}
			catch (Exception ex) { Info($"filesystem scan skipped: {ex.Message}"); }

			// Report.
			if (autohook)
				Warn("AutoHook appears LOADED in the game process — DISABLE it before fishing. It auto-recasts " +
					"independently and fights OceanTrip's rod control (corrupts bite timers, mimics cast/bait bugs).");
			else if (installed)
				Warn("AutoHook is INSTALLED (couldn't confirm it's loaded/enabled from here) — make sure it's " +
					"disabled in Dalamud before fishing, or it will fight OceanTrip for rod control.");
			else
				Info("No AutoHook signature found in loaded modules/assemblies or on disk.");

			Info($"Dalamud present: {dalamud}. Detection hits: {(hits.Any() ? string.Join(", ", hits.Distinct()) : "none")} " +
				"(note: Dalamud loads plugins in a separate context, so in-process scans can miss an active plugin).");
		}

		private static void Warn(string text) => Logging.Write(Colors.OrangeRed, "[Ocean Trip] ⚠ " + text);
		private static void Info(string text) => Logging.Write(Colors.Aqua, "[Ocean Trip] PluginScan: " + text);
	}
}
