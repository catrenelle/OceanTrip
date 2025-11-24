using System;
using OceanTripPlanner.Definitions;
using Ocean_Trip.Helpers;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Detects spectral weather changes and logs transitions
	/// Centralizes spectral current tracking logic
	/// </summary>
	public class SpectralDetector
	{
		private readonly GameStateCache _gameCache;
		private readonly Action<string, OceanLogLevel> _logAction;

		public SpectralDetector(GameStateCache gameCache, Action<string, OceanLogLevel> logAction)
		{
			_gameCache = gameCache ?? throw new ArgumentNullException(nameof(gameCache));
			_logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
		}

		/// <summary>
		/// Check for spectral weather changes and log transitions
		/// </summary>
		/// <param name="spectraled">Reference to the spectral state flag (will be updated)</param>
		/// <returns>True if spectral state changed, false otherwise</returns>
		public bool DetectSpectralChange(ref bool spectraled)
		{
			bool stateChanged = false;

			// Check for spectral weather changes
			if (_gameCache.CurrentWeatherId != Weather.Spectral)
			{
				if (spectraled == true)
				{
					_logAction("Spectral over.", OceanLogLevel.Info);
					spectraled = false;
					stateChanged = true;
				}
			}
			else
			{
				if (spectraled == false)
				{
					_logAction("Spectral popped!", OceanLogLevel.Info);
					spectraled = true;
					stateChanged = true;
				}
			}

			return stateChanged;
		}

		/// <summary>
		/// Check if spectral current is active
		/// </summary>
		public bool IsSpectralActive()
		{
			return _gameCache.CurrentWeatherId == Weather.Spectral;
		}
	}
}
