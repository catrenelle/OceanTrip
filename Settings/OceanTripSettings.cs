using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ff14bot.Helpers;

namespace OceanTripPlanner.Settings
{
	/// <summary>
	/// Modern settings implementation using collections instead of individual boolean properties
	/// Reduces ~1,400 lines of boilerplate to ~400 lines
	/// </summary>
	public class OceanTripSettings : JsonSettings, INotifyPropertyChanged
	{
		private static OceanTripSettings _instance;

		public static OceanTripSettings Instance
		{
			get { return _instance ?? (_instance = new OceanTripSettings()); }
		}

		public OceanTripSettings()
			: base(Path.Combine(CharacterSettingsDirectory, "OceanTripSettings.json"))
		{
			// Initialize collections if null (first time load)
			EnabledMaterials ??= new List<int>();
			EnabledAethersands ??= new List<int>();
			EnabledFoods ??= new List<int>();
			EnabledPotions ??= new List<int>();
			EnabledCrystals ??= new List<int>();
			EnabledMateria ??= new List<int>();

			MigrateFromOldSettings();
			ValidateSettings();
		}

		#region Collection-Based Settings

		/// <summary>
		/// Enabled crafting materials (1-7)
		/// </summary>
		public List<int> EnabledMaterials { get; set; } = new List<int>();

		/// <summary>
		/// Enabled aethersands (1-9)
		/// </summary>
		public List<int> EnabledAethersands { get; set; } = new List<int>();

		/// <summary>
		/// Enabled crafting foods (1-4)
		/// </summary>
		public List<int> EnabledFoods { get; set; } = new List<int>();

		/// <summary>
		/// Enabled crafting potions (1-4)
		/// </summary>
		public List<int> EnabledPotions { get; set; } = new List<int>();

		/// <summary>
		/// Enabled crystals (0=fire, 1=ice, 2=wind, 3=earth, 4=lightning, 5=water)
		/// </summary>
		public List<int> EnabledCrystals { get; set; } = new List<int>();

		/// <summary>
		/// Enabled materia purchases
		/// Format: tier * 10 + type (e.g., 121 = Materia XII type 1, 45 = Materia IV type 5)
		/// </summary>
		public List<int> EnabledMateria { get; set; } = new List<int>();

		#endregion

		#region Core Fishing Settings

		private FishPriority _fishPriority;
		public FishPriority FishPriority
		{
			get => _fishPriority;
			set => SetProperty(ref _fishPriority, value);
		}

		private FishingRoute _fishingRoute;
		public FishingRoute FishingRoute
		{
			get => _fishingRoute;
			set => SetProperty(ref _fishingRoute, value);
		}

		private FullGPAction _fullGPAction;
		public FullGPAction FullGPAction
		{
			get => _fullGPAction;
			set => SetProperty(ref _fullGPAction, value);
		}

		private ShouldUsePatience _patience;
		public ShouldUsePatience Patience
		{
			get => _patience;
			set => SetProperty(ref _patience, value);
		}

		private ExchangeFish _exchangeFish;
		public ExchangeFish ExchangeFish
		{
			get => _exchangeFish;
			set => SetProperty(ref _exchangeFish, value);
		}

		private int _baitRestockThreshold;
		public int BaitRestockThreshold
		{
			get => _baitRestockThreshold;
			set => SetProperty(ref _baitRestockThreshold, value);
		}

		private int _baitRestockAmount;
		public int BaitRestockAmount
		{
			get => _baitRestockAmount;
			set => SetProperty(ref _baitRestockAmount, value);
		}

		#endregion

		#region Feature Flags

		private bool _loggingMode;
		public bool LoggingMode
		{
			get => _loggingMode;
			set => SetProperty(ref _loggingMode, value);
		}

		private bool _openWorldFishing;
		public bool OpenWorldFishing
		{
			get => _openWorldFishing;
			set => SetProperty(ref _openWorldFishing, value);
		}

		private bool _lateBoatQueue;
		public bool LateBoatQueue
		{
			get => _lateBoatQueue;
			set => SetProperty(ref _lateBoatQueue, value);
		}

		private bool _oceanFood;
		public bool OceanFood
		{
			get => _oceanFood;
			set => SetProperty(ref _oceanFood, value);
		}

		private bool _useCraftingFood;
		public bool UseCraftingFood
		{
			get => _useCraftingFood;
			set => SetProperty(ref _useCraftingFood, value);
		}

		private bool _refillScrips;
		public bool RefillScrips
		{
			get => _refillScrips;
			set => SetProperty(ref _refillScrips, value);
		}

		private bool _purchaseHiCordials;
		public bool PurchaseHiCordials
		{
			get => _purchaseHiCordials;
			set => SetProperty(ref _purchaseHiCordials, value);
		}

		private bool _customBoatOrders;
		public bool CustomBoatOrders
		{
			get => _customBoatOrders;
			set => SetProperty(ref _customBoatOrders, value);
		}

		private bool _resumeLisbeth;
		public bool ResumeLisbeth
		{
			get => _resumeLisbeth;
			set => SetProperty(ref _resumeLisbeth, value);
		}

		private int _selectedMateriaIndex;
		public int SelectedMateriaIndex
		{
			get => _selectedMateriaIndex;
			set => SetProperty(ref _selectedMateriaIndex, value);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Check if a material is enabled
		/// </summary>
		public bool IsMaterialEnabled(int index) => EnabledMaterials.Contains(index);

		/// <summary>
		/// Set material enabled state
		/// </summary>
		public void SetMaterialEnabled(int index, bool enabled)
		{
			if (enabled && !EnabledMaterials.Contains(index))
				EnabledMaterials.Add(index);
			else if (!enabled)
				EnabledMaterials.Remove(index);
			Save();
			OnPropertyChanged(nameof(EnabledMaterials));
		}

		/// <summary>
		/// Check if an aethersand is enabled
		/// </summary>
		public bool IsAethersandEnabled(int index) => EnabledAethersands.Contains(index);

		/// <summary>
		/// Set aethersand enabled state
		/// </summary>
		public void SetAethersandEnabled(int index, bool enabled)
		{
			if (enabled && !EnabledAethersands.Contains(index))
				EnabledAethersands.Add(index);
			else if (!enabled)
				EnabledAethersands.Remove(index);
			Save();
			OnPropertyChanged(nameof(EnabledAethersands));
		}

		/// <summary>
		/// Check if a food is enabled
		/// </summary>
		public bool IsFoodEnabled(int index) => EnabledFoods.Contains(index);

		/// <summary>
		/// Set food enabled state
		/// </summary>
		public void SetFoodEnabled(int index, bool enabled)
		{
			if (enabled && !EnabledFoods.Contains(index))
				EnabledFoods.Add(index);
			else if (!enabled)
				EnabledFoods.Remove(index);
			Save();
			OnPropertyChanged(nameof(EnabledFoods));
		}

		/// <summary>
		/// Check if a potion is enabled
		/// </summary>
		public bool IsPotionEnabled(int index) => EnabledPotions.Contains(index);

		/// <summary>
		/// Set potion enabled state
		/// </summary>
		public void SetPotionEnabled(int index, bool enabled)
		{
			if (enabled && !EnabledPotions.Contains(index))
				EnabledPotions.Add(index);
			else if (!enabled)
				EnabledPotions.Remove(index);
			Save();
			OnPropertyChanged(nameof(EnabledPotions));
		}

		/// <summary>
		/// Check if a crystal is enabled
		/// </summary>
		public bool IsCrystalEnabled(int index) => EnabledCrystals.Contains(index);

		/// <summary>
		/// Set crystal enabled state
		/// </summary>
		public void SetCrystalEnabled(int index, bool enabled)
		{
			if (enabled && !EnabledCrystals.Contains(index))
				EnabledCrystals.Add(index);
			else if (!enabled)
				EnabledCrystals.Remove(index);
			Save();
			OnPropertyChanged(nameof(EnabledCrystals));
		}

		/// <summary>
		/// Check if a materia is enabled (tier, type)
		/// </summary>
		public bool IsMateriaEnabled(int tier, int type)
		{
			int key = tier * 10 + type;
			return EnabledMateria.Contains(key);
		}

		/// <summary>
		/// Set materia enabled state (tier, type)
		/// </summary>
		public void SetMateriaEnabled(int tier, int type, bool enabled)
		{
			int key = tier * 10 + type;
			if (enabled && !EnabledMateria.Contains(key))
				EnabledMateria.Add(key);
			else if (!enabled)
				EnabledMateria.Remove(key);
			Save();
			OnPropertyChanged(nameof(EnabledMateria));
		}

		/// <summary>
		/// Get list of enabled material item IDs from Defaults.materials array
		/// </summary>
		public List<int> GetEnabledMaterialIds()
		{
			var result = new List<int>();
			foreach (var index in EnabledMaterials)
			{
				if (index >= 1 && index <= OceanTripPlanner.Definitions.Defaults.materials.Length)
					result.Add(OceanTripPlanner.Definitions.Defaults.materials[index - 1]);
			}
			return result;
		}

		/// <summary>
		/// Get list of enabled aethersand item IDs from Defaults.aethersands array
		/// </summary>
		public List<int> GetEnabledAethersandIds()
		{
			var result = new List<int>();
			foreach (var index in EnabledAethersands)
			{
				if (index >= 1 && index <= OceanTripPlanner.Definitions.Defaults.aethersands.Length)
					result.Add(OceanTripPlanner.Definitions.Defaults.aethersands[index - 1]);
			}
			return result;
		}

		/// <summary>
		/// Get list of enabled food item IDs from Defaults.raidfood array
		/// </summary>
		public List<int> GetEnabledFoodIds()
		{
			var result = new List<int>();
			foreach (var index in EnabledFoods)
			{
				if (index >= 1 && index <= OceanTripPlanner.Definitions.Defaults.raidfood.Length)
					result.Add(OceanTripPlanner.Definitions.Defaults.raidfood[index - 1]);
			}
			return result;
		}

		/// <summary>
		/// Get list of enabled potion item IDs from Defaults.raidpotions array
		/// </summary>
		public List<int> GetEnabledPotionIds()
		{
			var result = new List<int>();
			foreach (var index in EnabledPotions)
			{
				if (index >= 1 && index <= OceanTripPlanner.Definitions.Defaults.raidpotions.Length)
					result.Add(OceanTripPlanner.Definitions.Defaults.raidpotions[index - 1]);
			}
			return result;
		}

		/// <summary>
		/// Get list of enabled crystal types with their shard/crystal/cluster IDs
		/// Returns tuples of (shard, crystal, cluster) for each enabled type
		/// </summary>
		public List<(int cluster, int crystal, int shard)> GetEnabledCrystalIds()
		{
			var result = new List<(int, int, int)>();
			var crystalMapping = new Dictionary<int, (int, int, int)>
			{
				{ 0, (OceanTripPlanner.Definitions.Crystals.FireCluster, OceanTripPlanner.Definitions.Crystals.FireCrystal, OceanTripPlanner.Definitions.Crystals.FireShard) },
				{ 1, (OceanTripPlanner.Definitions.Crystals.IceCluster, OceanTripPlanner.Definitions.Crystals.IceCrystal, OceanTripPlanner.Definitions.Crystals.IceShard) },
				{ 2, (OceanTripPlanner.Definitions.Crystals.WindCluster, OceanTripPlanner.Definitions.Crystals.WindCrystal, OceanTripPlanner.Definitions.Crystals.WindShard) },
				{ 3, (OceanTripPlanner.Definitions.Crystals.EarthCluster, OceanTripPlanner.Definitions.Crystals.EarthCrystal, OceanTripPlanner.Definitions.Crystals.EarthShard) },
				{ 4, (OceanTripPlanner.Definitions.Crystals.LightningCluster, OceanTripPlanner.Definitions.Crystals.LightningCrystal, OceanTripPlanner.Definitions.Crystals.LightningShard) },
				{ 5, (OceanTripPlanner.Definitions.Crystals.WaterCluster, OceanTripPlanner.Definitions.Crystals.WaterCrystal, OceanTripPlanner.Definitions.Crystals.WaterShard) }
			};

			foreach (var index in EnabledCrystals)
			{
				if (crystalMapping.ContainsKey(index))
					result.Add(crystalMapping[index]);
			}
			return result;
		}

		/// <summary>
		/// Get list of enabled materia item IDs
		/// Returns tuples of (tier, type, itemId) for easier processing
		/// </summary>
		public List<int> GetEnabledMateriaIds()
		{
			var result = new List<int>();
			var materiaTierArrays = new Dictionary<int, int[]>
			{
				{ 12, OceanTripPlanner.Definitions.Defaults.materiaxii },
				{ 11, OceanTripPlanner.Definitions.Defaults.materiaxi },
				{ 10, OceanTripPlanner.Definitions.Defaults.materiax },
				{ 9, OceanTripPlanner.Definitions.Defaults.materiaix },
				{ 8, OceanTripPlanner.Definitions.Defaults.materiaviii },
				{ 7, OceanTripPlanner.Definitions.Defaults.materiavii },
				{ 6, OceanTripPlanner.Definitions.Defaults.materiavi },
				{ 5, OceanTripPlanner.Definitions.Defaults.materiav },
				{ 4, OceanTripPlanner.Definitions.Defaults.materiaiv }
			};

			foreach (var key in EnabledMateria)
			{
				int tier = key / 10;
				int type = key % 10;

				if (materiaTierArrays.ContainsKey(tier) && type >= 1 && type <= 6)
				{
					int itemId = materiaTierArrays[tier][type - 1];
					result.Add(itemId);
				}
			}
			return result;
		}

		#endregion

		#region Backward Compatibility

		/// <summary>
		/// Migrate from old OceanTripNewSettings format to new collection-based format
		/// </summary>
		private void MigrateFromOldSettings()
		{
			var oldSettingsPath = Path.Combine(CharacterSettingsDirectory, "OceanTripNewSettings.json");
			if (!File.Exists(oldSettingsPath))
				return;

			// Check if migration is needed (collections are empty but old file exists)
			if (EnabledMaterials.Count > 0 || EnabledAethersands.Count > 0 || EnabledFoods.Count > 0 ||
			    EnabledPotions.Count > 0 || EnabledCrystals.Count > 0 || EnabledMateria.Count > 0)
				return; // Already migrated

			try
			{
				// Load old settings JSON manually
				var json = File.ReadAllText(oldSettingsPath);
				var oldSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<OldSettingsFormat>(json);
				if (oldSettings == null)
					return;

				Logging.Write(Colors.Aqua, "[Ocean Trip] Migrating settings from old format to new collection-based format...");

				// Migrate materials
				for (int i = 1; i <= 7; i++)
				{
					var propertyName = $"material{i}";
					var property = typeof(OldSettingsFormat).GetProperty(propertyName);
					if (property != null && (bool)property.GetValue(oldSettings))
						EnabledMaterials.Add(i);
				}

				// Migrate aethersands
				for (int i = 1; i <= 9; i++)
				{
					var propertyName = $"aethersand{i}";
					var property = typeof(OldSettingsFormat).GetProperty(propertyName);
					if (property != null && (bool)property.GetValue(oldSettings))
						EnabledAethersands.Add(i);
				}

				// Migrate foods
				for (int i = 1; i <= 4; i++)
				{
					var propertyName = $"food{i}";
					var property = typeof(OldSettingsFormat).GetProperty(propertyName);
					if (property != null && (bool)property.GetValue(oldSettings))
						EnabledFoods.Add(i);
				}

				// Migrate potions
				for (int i = 1; i <= 4; i++)
				{
					var propertyName = $"potion{i}";
					var property = typeof(OldSettingsFormat).GetProperty(propertyName);
					if (property != null && (bool)property.GetValue(oldSettings))
						EnabledPotions.Add(i);
				}

				// Migrate crystals (specific names)
				string[] crystalNames = { "firecrystal", "icecrystal", "windcrystal", "earthcrystal", "lightningcrystal", "watercrystal" };
				for (int i = 0; i < crystalNames.Length; i++)
				{
					var property = typeof(OldSettingsFormat).GetProperty(crystalNames[i]);
					if (property != null && (bool)property.GetValue(oldSettings))
						EnabledCrystals.Add(i);
				}

				// Migrate materia (XII through IV)
				string[] materiaTiers = { "materiaxii", "materiaxi", "materiax", "materiaix", "materiaviii", "materiavii", "materiavi", "materiav", "materiaiv" };
				int[] tierNumbers = { 12, 11, 10, 9, 8, 7, 6, 5, 4 };

				for (int t = 0; t < materiaTiers.Length; t++)
				{
					for (int type = 1; type <= 6; type++)
					{
						var propertyName = $"{materiaTiers[t]}{type}";
						var property = typeof(OldSettingsFormat).GetProperty(propertyName);
						if (property != null && (bool)property.GetValue(oldSettings))
						{
							int key = tierNumbers[t] * 10 + type;
							EnabledMateria.Add(key);
						}
					}
				}

				// Migrate core settings
				FishPriority = oldSettings.FishPriority;
				FishingRoute = oldSettings.FishingRoute;
				FullGPAction = oldSettings.FullGPAction;
				Patience = oldSettings.Patience;
				ExchangeFish = oldSettings.ExchangeFish;
				BaitRestockThreshold = oldSettings.BaitRestockThreshold;
				BaitRestockAmount = oldSettings.BaitRestockAmount;
				LoggingMode = oldSettings.LoggingMode;
				OpenWorldFishing = oldSettings.OpenWorldFishing;
				LateBoatQueue = oldSettings.LateBoatQueue;
				OceanFood = oldSettings.OceanFood;
				UseCraftingFood = oldSettings.useCraftingFood;
				RefillScrips = oldSettings.refillScrips;
				PurchaseHiCordials = oldSettings.purchaseHiCordials;
				CustomBoatOrders = oldSettings.customBoatOrders;
				ResumeLisbeth = oldSettings.resumeLisbeth;
				SelectedMateriaIndex = oldSettings.selectedMateriaIndex;

				Save();

				// Rename old settings file to prevent repeated migration
				var backupPath = Path.Combine(CharacterSettingsDirectory, "OceanTripNewSettings.json.bak");
				File.Move(oldSettingsPath, backupPath);

				Logging.Write(Colors.Aqua, "[Ocean Trip] Settings migration complete! Old settings file backed up to OceanTripNewSettings.json.bak");
			}
			catch (Exception ex)
			{
				Logging.Write(Colors.Orange, $"[Ocean Trip] Warning: Could not migrate old settings: {ex.Message}");
			}
		}

		/// <summary>
		/// Temporary class for loading old settings format during migration
		/// </summary>
		private class OldSettingsFormat
		{
			// Materials
			public bool material1 { get; set; }
			public bool material2 { get; set; }
			public bool material3 { get; set; }
			public bool material4 { get; set; }
			public bool material5 { get; set; }
			public bool material6 { get; set; }
			public bool material7 { get; set; }

			// Aethersands
			public bool aethersand1 { get; set; }
			public bool aethersand2 { get; set; }
			public bool aethersand3 { get; set; }
			public bool aethersand4 { get; set; }
			public bool aethersand5 { get; set; }
			public bool aethersand6 { get; set; }
			public bool aethersand7 { get; set; }
			public bool aethersand8 { get; set; }
			public bool aethersand9 { get; set; }

			// Foods
			public bool food1 { get; set; }
			public bool food2 { get; set; }
			public bool food3 { get; set; }
			public bool food4 { get; set; }

			// Potions
			public bool potion1 { get; set; }
			public bool potion2 { get; set; }
			public bool potion3 { get; set; }
			public bool potion4 { get; set; }

			// Crystals
			public bool firecrystal { get; set; }
			public bool icecrystal { get; set; }
			public bool windcrystal { get; set; }
			public bool earthcrystal { get; set; }
			public bool lightningcrystal { get; set; }
			public bool watercrystal { get; set; }

			// Materia XII
			public bool materiaxii1 { get; set; }
			public bool materiaxii2 { get; set; }
			public bool materiaxii3 { get; set; }
			public bool materiaxii4 { get; set; }
			public bool materiaxii5 { get; set; }
			public bool materiaxii6 { get; set; }

			// Materia XI
			public bool materiaxi1 { get; set; }
			public bool materiaxi2 { get; set; }
			public bool materiaxi3 { get; set; }
			public bool materiaxi4 { get; set; }
			public bool materiaxi5 { get; set; }
			public bool materiaxi6 { get; set; }

			// Materia X
			public bool materiax1 { get; set; }
			public bool materiax2 { get; set; }
			public bool materiax3 { get; set; }
			public bool materiax4 { get; set; }
			public bool materiax5 { get; set; }
			public bool materiax6 { get; set; }

			// Materia IX
			public bool materiaix1 { get; set; }
			public bool materiaix2 { get; set; }
			public bool materiaix3 { get; set; }
			public bool materiaix4 { get; set; }
			public bool materiaix5 { get; set; }
			public bool materiaix6 { get; set; }

			// Materia VIII
			public bool materiaviii1 { get; set; }
			public bool materiaviii2 { get; set; }
			public bool materiaviii3 { get; set; }
			public bool materiaviii4 { get; set; }
			public bool materiaviii5 { get; set; }
			public bool materiaviii6 { get; set; }

			// Materia VII
			public bool materiavii1 { get; set; }
			public bool materiavii2 { get; set; }
			public bool materiavii3 { get; set; }
			public bool materiavii4 { get; set; }
			public bool materiavii5 { get; set; }
			public bool materiavii6 { get; set; }

			// Materia VI
			public bool materiavi1 { get; set; }
			public bool materiavi2 { get; set; }
			public bool materiavi3 { get; set; }
			public bool materiavi4 { get; set; }
			public bool materiavi5 { get; set; }
			public bool materiavi6 { get; set; }

			// Materia V
			public bool materiav1 { get; set; }
			public bool materiav2 { get; set; }
			public bool materiav3 { get; set; }
			public bool materiav4 { get; set; }
			public bool materiav5 { get; set; }
			public bool materiav6 { get; set; }

			// Materia IV
			public bool materiaiv1 { get; set; }
			public bool materiaiv2 { get; set; }
			public bool materiaiv3 { get; set; }
			public bool materiaiv4 { get; set; }
			public bool materiaiv5 { get; set; }
			public bool materiaiv6 { get; set; }

			// Core settings
			public FishPriority FishPriority { get; set; }
			public FishingRoute FishingRoute { get; set; }
			public FullGPAction FullGPAction { get; set; }
			public ShouldUsePatience Patience { get; set; }
			public ExchangeFish ExchangeFish { get; set; }
			public int BaitRestockThreshold { get; set; }
			public int BaitRestockAmount { get; set; }
			public bool LoggingMode { get; set; }
			public bool OpenWorldFishing { get; set; }
			public bool LateBoatQueue { get; set; }
			public bool OceanFood { get; set; }
			public bool useCraftingFood { get; set; }
			public bool refillScrips { get; set; }
			public bool purchaseHiCordials { get; set; }
			public bool customBoatOrders { get; set; }
			public bool resumeLisbeth { get; set; }
			public int selectedMateriaIndex { get; set; }
		}

		#endregion

		#region Validation

		/// <summary>
		/// Validate and correct settings to safe ranges
		/// </summary>
		public void ValidateSettings()
		{
			bool changed = false;

			// If user has set threshold to 0, they want manual control - skip validation
			if (BaitRestockThreshold == 0 && BaitRestockAmount == 0)
			{
				return; // Both 0 = manual control, valid configuration
			}

			// If only one is 0, that's likely an error - warn but don't auto-fix
			if (BaitRestockThreshold == 0 || BaitRestockAmount == 0)
			{
				Logging.Write(Colors.Orange, $"[Ocean Trip] Warning: One of BaitRestockThreshold ({BaitRestockThreshold}) or BaitRestockAmount ({BaitRestockAmount}) is 0. Set both to 0 to disable auto-restocking, or both to non-zero values to enable it.");
				return;
			}

			// Both are non-zero, validate minimum safe values
			if (BaitRestockThreshold > 0 && BaitRestockThreshold < 10)
			{
				Logging.Write(Colors.Orange, $"[Ocean Trip] BaitRestockThreshold ({BaitRestockThreshold}) is too low for safe operation, setting to minimum (10)");
				BaitRestockThreshold = 10;
				changed = true;
			}

			if (BaitRestockAmount > 0 && BaitRestockAmount < 30)
			{
				Logging.Write(Colors.Orange, $"[Ocean Trip] BaitRestockAmount ({BaitRestockAmount}) is too low for safe operation, setting to minimum (30)");
				BaitRestockAmount = 30;
				changed = true;
			}

			// Ensure bait restock amount is greater than threshold (if both enabled)
			if (BaitRestockAmount > 0 && BaitRestockThreshold > 0 && BaitRestockAmount <= BaitRestockThreshold)
			{
				Logging.Write(Colors.Orange, $"[Ocean Trip] BaitRestockAmount ({BaitRestockAmount}) must be greater than BaitRestockThreshold ({BaitRestockThreshold}), adjusting");
				BaitRestockAmount = BaitRestockThreshold + 100;
				changed = true;
			}

			if (changed)
			{
				Save();
				Logging.Write(Colors.Aqua, $"[Ocean Trip] Settings validated and corrected. Current values: BaitRestockThreshold={BaitRestockThreshold}, BaitRestockAmount={BaitRestockAmount}");
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public new event PropertyChangedEventHandler PropertyChanged;

		protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			Save();
			OnPropertyChanged(propertyName);
			return true;
		}

		#endregion
	}
}
