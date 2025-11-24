using System;
using System.ComponentModel;
using OceanTripPlanner.Settings;

namespace OceanTripPlanner
{
	#region Enums (kept for backward compatibility)

	public enum LoggingMode : int
	{
		Normal = 0,
		Verbose = 1
	}

	public enum FishPriority : uint
	{
		FishLog,
		Points,
		Auto,
		IgnoreBoat,
		Achievements
	}

	public enum OceanFood : int
	{
		NasiGoreng = 44078,
		CrabCakes = 30481,
		None = 0
	}

	public enum FishingRoute : int
	{
		Indigo = 0,
		Ruby = 1
	}

	public enum FullGPAction : uint
	{
		Chum,
		DoubleHook,
		None
	}

	public enum ShouldUsePatience : int
	{
		OnlyForSpecificFish = 0,
		AlwaysUsePatience = 1,
		SpectralOnly = 2
	}

	public enum ExchangeFish : uint
	{
		Sell,
		Desynth,
		None
	}

	#endregion

	/// <summary>
	/// Compatibility wrapper for OceanTripNewSettings
	/// Redirects all calls to the new OceanTripSettings implementation
	/// This allows existing code to work without changes
	/// </summary>
	internal class OceanTripNewSettings : INotifyPropertyChanged
	{
		private static OceanTripNewSettings _instance;

		public static OceanTripNewSettings Instance
		{
			get { return _instance ?? (_instance = new OceanTripNewSettings()); }
		}

		// Reference to new settings implementation
		private OceanTripSettings _settings => OceanTripSettings.Instance;

		#region Backward Compatibility Properties - Materials

		public bool material1
		{
			get => _settings.IsMaterialEnabled(1);
			set => _settings.SetMaterialEnabled(1, value);
		}

		public bool material2
		{
			get => _settings.IsMaterialEnabled(2);
			set => _settings.SetMaterialEnabled(2, value);
		}

		public bool material3
		{
			get => _settings.IsMaterialEnabled(3);
			set => _settings.SetMaterialEnabled(3, value);
		}

		public bool material4
		{
			get => _settings.IsMaterialEnabled(4);
			set => _settings.SetMaterialEnabled(4, value);
		}

		public bool material5
		{
			get => _settings.IsMaterialEnabled(5);
			set => _settings.SetMaterialEnabled(5, value);
		}

		public bool material6
		{
			get => _settings.IsMaterialEnabled(6);
			set => _settings.SetMaterialEnabled(6, value);
		}

		public bool material7
		{
			get => _settings.IsMaterialEnabled(7);
			set => _settings.SetMaterialEnabled(7, value);
		}

		#endregion

		#region Backward Compatibility Properties - Aethersands

		public bool aethersand1
		{
			get => _settings.IsAethersandEnabled(1);
			set => _settings.SetAethersandEnabled(1, value);
		}

		public bool aethersand2
		{
			get => _settings.IsAethersandEnabled(2);
			set => _settings.SetAethersandEnabled(2, value);
		}

		public bool aethersand3
		{
			get => _settings.IsAethersandEnabled(3);
			set => _settings.SetAethersandEnabled(3, value);
		}

		public bool aethersand4
		{
			get => _settings.IsAethersandEnabled(4);
			set => _settings.SetAethersandEnabled(4, value);
		}

		public bool aethersand5
		{
			get => _settings.IsAethersandEnabled(5);
			set => _settings.SetAethersandEnabled(5, value);
		}

		public bool aethersand6
		{
			get => _settings.IsAethersandEnabled(6);
			set => _settings.SetAethersandEnabled(6, value);
		}

		public bool aethersand7
		{
			get => _settings.IsAethersandEnabled(7);
			set => _settings.SetAethersandEnabled(7, value);
		}

		public bool aethersand8
		{
			get => _settings.IsAethersandEnabled(8);
			set => _settings.SetAethersandEnabled(8, value);
		}

		public bool aethersand9
		{
			get => _settings.IsAethersandEnabled(9);
			set => _settings.SetAethersandEnabled(9, value);
		}

		#endregion

		#region Backward Compatibility Properties - Foods

		public bool food1
		{
			get => _settings.IsFoodEnabled(1);
			set => _settings.SetFoodEnabled(1, value);
		}

		public bool food2
		{
			get => _settings.IsFoodEnabled(2);
			set => _settings.SetFoodEnabled(2, value);
		}

		public bool food3
		{
			get => _settings.IsFoodEnabled(3);
			set => _settings.SetFoodEnabled(3, value);
		}

		public bool food4
		{
			get => _settings.IsFoodEnabled(4);
			set => _settings.SetFoodEnabled(4, value);
		}

		#endregion

		#region Backward Compatibility Properties - Potions

		public bool potion1
		{
			get => _settings.IsPotionEnabled(1);
			set => _settings.SetPotionEnabled(1, value);
		}

		public bool potion2
		{
			get => _settings.IsPotionEnabled(2);
			set => _settings.SetPotionEnabled(2, value);
		}

		public bool potion3
		{
			get => _settings.IsPotionEnabled(3);
			set => _settings.SetPotionEnabled(3, value);
		}

		public bool potion4
		{
			get => _settings.IsPotionEnabled(4);
			set => _settings.SetPotionEnabled(4, value);
		}

		#endregion

		#region Backward Compatibility Properties - Crystals

		public bool firecrystal
		{
			get => _settings.IsCrystalEnabled(0);
			set => _settings.SetCrystalEnabled(0, value);
		}

		public bool icecrystal
		{
			get => _settings.IsCrystalEnabled(1);
			set => _settings.SetCrystalEnabled(1, value);
		}

		public bool windcrystal
		{
			get => _settings.IsCrystalEnabled(2);
			set => _settings.SetCrystalEnabled(2, value);
		}

		public bool earthcrystal
		{
			get => _settings.IsCrystalEnabled(3);
			set => _settings.SetCrystalEnabled(3, value);
		}

		public bool lightningcrystal
		{
			get => _settings.IsCrystalEnabled(4);
			set => _settings.SetCrystalEnabled(4, value);
		}

		public bool watercrystal
		{
			get => _settings.IsCrystalEnabled(5);
			set => _settings.SetCrystalEnabled(5, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia XII

		public bool materiaxii1
		{
			get => _settings.IsMateriaEnabled(12, 1);
			set => _settings.SetMateriaEnabled(12, 1, value);
		}

		public bool materiaxii2
		{
			get => _settings.IsMateriaEnabled(12, 2);
			set => _settings.SetMateriaEnabled(12, 2, value);
		}

		public bool materiaxii3
		{
			get => _settings.IsMateriaEnabled(12, 3);
			set => _settings.SetMateriaEnabled(12, 3, value);
		}

		public bool materiaxii4
		{
			get => _settings.IsMateriaEnabled(12, 4);
			set => _settings.SetMateriaEnabled(12, 4, value);
		}

		public bool materiaxii5
		{
			get => _settings.IsMateriaEnabled(12, 5);
			set => _settings.SetMateriaEnabled(12, 5, value);
		}

		public bool materiaxii6
		{
			get => _settings.IsMateriaEnabled(12, 6);
			set => _settings.SetMateriaEnabled(12, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia XI

		public bool materiaxi1
		{
			get => _settings.IsMateriaEnabled(11, 1);
			set => _settings.SetMateriaEnabled(11, 1, value);
		}

		public bool materiaxi2
		{
			get => _settings.IsMateriaEnabled(11, 2);
			set => _settings.SetMateriaEnabled(11, 2, value);
		}

		public bool materiaxi3
		{
			get => _settings.IsMateriaEnabled(11, 3);
			set => _settings.SetMateriaEnabled(11, 3, value);
		}

		public bool materiaxi4
		{
			get => _settings.IsMateriaEnabled(11, 4);
			set => _settings.SetMateriaEnabled(11, 4, value);
		}

		public bool materiaxi5
		{
			get => _settings.IsMateriaEnabled(11, 5);
			set => _settings.SetMateriaEnabled(11, 5, value);
		}

		public bool materiaxi6
		{
			get => _settings.IsMateriaEnabled(11, 6);
			set => _settings.SetMateriaEnabled(11, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia X

		public bool materiax1
		{
			get => _settings.IsMateriaEnabled(10, 1);
			set => _settings.SetMateriaEnabled(10, 1, value);
		}

		public bool materiax2
		{
			get => _settings.IsMateriaEnabled(10, 2);
			set => _settings.SetMateriaEnabled(10, 2, value);
		}

		public bool materiax3
		{
			get => _settings.IsMateriaEnabled(10, 3);
			set => _settings.SetMateriaEnabled(10, 3, value);
		}

		public bool materiax4
		{
			get => _settings.IsMateriaEnabled(10, 4);
			set => _settings.SetMateriaEnabled(10, 4, value);
		}

		public bool materiax5
		{
			get => _settings.IsMateriaEnabled(10, 5);
			set => _settings.SetMateriaEnabled(10, 5, value);
		}

		public bool materiax6
		{
			get => _settings.IsMateriaEnabled(10, 6);
			set => _settings.SetMateriaEnabled(10, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia IX

		public bool materiaix1
		{
			get => _settings.IsMateriaEnabled(9, 1);
			set => _settings.SetMateriaEnabled(9, 1, value);
		}

		public bool materiaix2
		{
			get => _settings.IsMateriaEnabled(9, 2);
			set => _settings.SetMateriaEnabled(9, 2, value);
		}

		public bool materiaix3
		{
			get => _settings.IsMateriaEnabled(9, 3);
			set => _settings.SetMateriaEnabled(9, 3, value);
		}

		public bool materiaix4
		{
			get => _settings.IsMateriaEnabled(9, 4);
			set => _settings.SetMateriaEnabled(9, 4, value);
		}

		public bool materiaix5
		{
			get => _settings.IsMateriaEnabled(9, 5);
			set => _settings.SetMateriaEnabled(9, 5, value);
		}

		public bool materiaix6
		{
			get => _settings.IsMateriaEnabled(9, 6);
			set => _settings.SetMateriaEnabled(9, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia VIII

		public bool materiaviii1
		{
			get => _settings.IsMateriaEnabled(8, 1);
			set => _settings.SetMateriaEnabled(8, 1, value);
		}

		public bool materiaviii2
		{
			get => _settings.IsMateriaEnabled(8, 2);
			set => _settings.SetMateriaEnabled(8, 2, value);
		}

		public bool materiaviii3
		{
			get => _settings.IsMateriaEnabled(8, 3);
			set => _settings.SetMateriaEnabled(8, 3, value);
		}

		public bool materiaviii4
		{
			get => _settings.IsMateriaEnabled(8, 4);
			set => _settings.SetMateriaEnabled(8, 4, value);
		}

		public bool materiaviii5
		{
			get => _settings.IsMateriaEnabled(8, 5);
			set => _settings.SetMateriaEnabled(8, 5, value);
		}

		public bool materiaviii6
		{
			get => _settings.IsMateriaEnabled(8, 6);
			set => _settings.SetMateriaEnabled(8, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia VII

		public bool materiavii1
		{
			get => _settings.IsMateriaEnabled(7, 1);
			set => _settings.SetMateriaEnabled(7, 1, value);
		}

		public bool materiavii2
		{
			get => _settings.IsMateriaEnabled(7, 2);
			set => _settings.SetMateriaEnabled(7, 2, value);
		}

		public bool materiavii3
		{
			get => _settings.IsMateriaEnabled(7, 3);
			set => _settings.SetMateriaEnabled(7, 3, value);
		}

		public bool materiavii4
		{
			get => _settings.IsMateriaEnabled(7, 4);
			set => _settings.SetMateriaEnabled(7, 4, value);
		}

		public bool materiavii5
		{
			get => _settings.IsMateriaEnabled(7, 5);
			set => _settings.SetMateriaEnabled(7, 5, value);
		}

		public bool materiavii6
		{
			get => _settings.IsMateriaEnabled(7, 6);
			set => _settings.SetMateriaEnabled(7, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia VI

		public bool materiavi1
		{
			get => _settings.IsMateriaEnabled(6, 1);
			set => _settings.SetMateriaEnabled(6, 1, value);
		}

		public bool materiavi2
		{
			get => _settings.IsMateriaEnabled(6, 2);
			set => _settings.SetMateriaEnabled(6, 2, value);
		}

		public bool materiavi3
		{
			get => _settings.IsMateriaEnabled(6, 3);
			set => _settings.SetMateriaEnabled(6, 3, value);
		}

		public bool materiavi4
		{
			get => _settings.IsMateriaEnabled(6, 4);
			set => _settings.SetMateriaEnabled(6, 4, value);
		}

		public bool materiavi5
		{
			get => _settings.IsMateriaEnabled(6, 5);
			set => _settings.SetMateriaEnabled(6, 5, value);
		}

		public bool materiavi6
		{
			get => _settings.IsMateriaEnabled(6, 6);
			set => _settings.SetMateriaEnabled(6, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia V

		public bool materiav1
		{
			get => _settings.IsMateriaEnabled(5, 1);
			set => _settings.SetMateriaEnabled(5, 1, value);
		}

		public bool materiav2
		{
			get => _settings.IsMateriaEnabled(5, 2);
			set => _settings.SetMateriaEnabled(5, 2, value);
		}

		public bool materiav3
		{
			get => _settings.IsMateriaEnabled(5, 3);
			set => _settings.SetMateriaEnabled(5, 3, value);
		}

		public bool materiav4
		{
			get => _settings.IsMateriaEnabled(5, 4);
			set => _settings.SetMateriaEnabled(5, 4, value);
		}

		public bool materiav5
		{
			get => _settings.IsMateriaEnabled(5, 5);
			set => _settings.SetMateriaEnabled(5, 5, value);
		}

		public bool materiav6
		{
			get => _settings.IsMateriaEnabled(5, 6);
			set => _settings.SetMateriaEnabled(5, 6, value);
		}

		#endregion

		#region Backward Compatibility Properties - Materia IV

		public bool materiaiv1
		{
			get => _settings.IsMateriaEnabled(4, 1);
			set => _settings.SetMateriaEnabled(4, 1, value);
		}

		public bool materiaiv2
		{
			get => _settings.IsMateriaEnabled(4, 2);
			set => _settings.SetMateriaEnabled(4, 2, value);
		}

		public bool materiaiv3
		{
			get => _settings.IsMateriaEnabled(4, 3);
			set => _settings.SetMateriaEnabled(4, 3, value);
		}

		public bool materiaiv4
		{
			get => _settings.IsMateriaEnabled(4, 4);
			set => _settings.SetMateriaEnabled(4, 4, value);
		}

		public bool materiaiv5
		{
			get => _settings.IsMateriaEnabled(4, 5);
			set => _settings.SetMateriaEnabled(4, 5, value);
		}

		public bool materiaiv6
		{
			get => _settings.IsMateriaEnabled(4, 6);
			set => _settings.SetMateriaEnabled(4, 6, value);
		}

		#endregion

		#region Core Settings Properties

		public int selectedMateriaIndex
		{
			get => _settings.SelectedMateriaIndex;
			set => _settings.SelectedMateriaIndex = value;
		}

		public bool useCraftingFood
		{
			get => _settings.UseCraftingFood;
			set => _settings.UseCraftingFood = value;
		}

		public bool refillScrips
		{
			get => _settings.RefillScrips;
			set => _settings.RefillScrips = value;
		}

		public bool purchaseHiCordials
		{
			get => _settings.PurchaseHiCordials;
			set => _settings.PurchaseHiCordials = value;
		}

		public bool customBoatOrders
		{
			get => _settings.CustomBoatOrders;
			set => _settings.CustomBoatOrders = value;
		}

		public bool resumeLisbeth
		{
			get => _settings.ResumeLisbeth;
			set => _settings.ResumeLisbeth = value;
		}

		public bool LoggingMode
		{
			get => _settings.LoggingMode;
			set => _settings.LoggingMode = value;
		}

		public int BaitRestockThreshold
		{
			get => _settings.BaitRestockThreshold;
			set => _settings.BaitRestockThreshold = value;
		}

		public int BaitRestockAmount
		{
			get => _settings.BaitRestockAmount;
			set => _settings.BaitRestockAmount = value;
		}

		public bool OpenWorldFishing
		{
			get => _settings.OpenWorldFishing;
			set => _settings.OpenWorldFishing = value;
		}

		public FishPriority FishPriority
		{
			get => _settings.FishPriority;
			set => _settings.FishPriority = value;
		}

		public bool LateBoatQueue
		{
			get => _settings.LateBoatQueue;
			set => _settings.LateBoatQueue = value;
		}

		public bool OceanFood
		{
			get => _settings.OceanFood;
			set => _settings.OceanFood = value;
		}

		public FishingRoute FishingRoute
		{
			get => _settings.FishingRoute;
			set => _settings.FishingRoute = value;
		}

		public FullGPAction FullGPAction
		{
			get => _settings.FullGPAction;
			set => _settings.FullGPAction = value;
		}

		public ShouldUsePatience Patience
		{
			get => _settings.Patience;
			set => _settings.Patience = value;
		}

		public ExchangeFish ExchangeFish
		{
			get => _settings.ExchangeFish;
			set => _settings.ExchangeFish = value;
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged
		{
			add => _settings.PropertyChanged += value;
			remove => _settings.PropertyChanged -= value;
		}

		#endregion
	}
}
