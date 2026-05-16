using ff14bot.Managers;
using Ocean_Trip.Properties;
using Ocean_Trip.Definitions;
using OceanTripPlanner;
using OceanTripPlanner.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace Ocean_Trip
{
	public partial class FormOceanSettings : BaseForm
	{
		public FormOceanSettings(FormSettings parent)
		{
			InitializeComponent();
			InitializeBaseForm(parent, exitIcon);

			// Data Binding
			lateQueueToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "lateBoatQueue", false, DataSourceUpdateMode.OnPropertyChanged);
			fishFoodToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "OceanFood", false, DataSourceUpdateMode.OnPropertyChanged);
			numericRestockAmount.DataBindings.Add("Value", OceanTripPlanner.OceanTripNewSettings.Instance, "baitRestockAmount", false, DataSourceUpdateMode.OnPropertyChanged);
			numericRestockThreshold.DataBindings.Add("Value", OceanTripPlanner.OceanTripNewSettings.Instance, "baitRestockThreshold", false, DataSourceUpdateMode.OnPropertyChanged);
			assistedFishingToggle.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "openWorldFishing", false, DataSourceUpdateMode.OnPropertyChanged);

			// Parent
			//toggleButton1.DataBindings.Add("Checked", OceanTripPlanner.OceanTripNewSettings.Instance, "openWorldFishing", false, DataSourceUpdateMode.OnPropertyChanged);


			//// Radio Buttons
			// Fishing Priority
			switch (OceanTripNewSettings.Instance.FishPriority)
			{
				case OceanTripPlanner.FishPriority.Auto:
					automatic.Select();
					break;
				case OceanTripPlanner.FishPriority.Points:
					points.Select();
					break;
				case OceanTripPlanner.FishPriority.FishLog:
					fishingLog.Select();
					break;
				case OceanTripPlanner.FishPriority.Achievements:
					achievements.Select();
					break;
				case OceanTripPlanner.FishPriority.IgnoreBoat:
					ignoreBoat.Select();
					break;
			}

			// Fishing Route
			switch (OceanTripNewSettings.Instance.FishingRoute)
			{
				case OceanTripPlanner.FishingRoute.Indigo:
					indigo.Select();
					break;
				case OceanTripPlanner.FishingRoute.Ruby:
					ruby.Select();
					break;
			}

			// Full GP Action
			switch(OceanTripNewSettings.Instance.FullGPAction)
			{
				case OceanTripPlanner.FullGPAction.None:
					GPActionNothing.Select();
					break;
				case OceanTripPlanner.FullGPAction.DoubleHook:
					GPActionDoubleHook.Select();
					break;
				case OceanTripPlanner.FullGPAction.Chum:
					GPActionChum.Select();
					break;
			}

			// Use Patience
			switch (OceanTripNewSettings.Instance.Patience)
			{
				case ShouldUsePatience.OnlyForSpecificFish:
					patienceDefaultLogic.Select();
					break;
				case ShouldUsePatience.SpectralOnly:
					patienceSpectralOnly.Select();
					break;
				case ShouldUsePatience.AlwaysUsePatience:
					patienceAlways.Select();
					break;
			}

			// Exchange Fish
			switch (OceanTripNewSettings.Instance.ExchangeFish)
			{
				case ExchangeFish.None:
					fishExchangeNone.Select();
					break;
				case ExchangeFish.Desynth:
					fishExchangeDesynthesize.Select();
					break;
				case ExchangeFish.Sell:
					fishExchangeSell.Select();
					break;
			}

			// Lure
			lureStacksNumeric.DataBindings.Add("Value", OceanTripPlanner.OceanTripNewSettings.Instance, "LureMaxStacks", false, DataSourceUpdateMode.OnPropertyChanged);
			switch (OceanTripNewSettings.Instance.LurePreference)
			{
				case LurePreference.None:
					lureNone.Select();
					break;
				case LurePreference.Modest:
					lureModest.Select();
					break;
				case LurePreference.Ambitious:
					lureAmbitious.Select();
					break;
				case LurePreference.Auto:
					lureAuto.Select();
					break;
			}

			// Target Fish
			if (OceanTripNewSettings.Instance.TargetFishId == (uint)OceanFish.JuniorJinbei)
				targetFishJinbei.Select();
			else
				targetFishNone.Select();


			// PictureBox Tooltips
			new ToolTip().SetToolTip(pictureBoxRagworm, $"{DataManager.ItemCache[FishBait.Ragworm].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxKrill, $"{DataManager.ItemCache[FishBait.Krill].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxPlumpWorm, $"{DataManager.ItemCache[FishBait.PlumpWorm].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxRatTail, $"{DataManager.ItemCache[FishBait.RatTail].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxGlowWorm, $"{DataManager.ItemCache[FishBait.GlowWorm].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxHeavySteelJig, $"{DataManager.ItemCache[FishBait.HeavySteelJig].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxShrimpCageFeeder, $"{DataManager.ItemCache[FishBait.ShrimpCageFeeder].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxPillBug, $"{DataManager.ItemCache[FishBait.PillBug].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxSquidStrip, $"{DataManager.ItemCache[FishBait.SquidStrip].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxMackerelStrip, $"{DataManager.ItemCache[FishBait.MackerelStrip].CurrentLocaleName}");
			new ToolTip().SetToolTip(pictureBoxStoneflyNymph, $"{DataManager.ItemCache[FishBait.StoneflyNymph].CurrentLocaleName}");


			// PictureBox Images
			pictureBoxRagworm.Image = UIElements.getIconImage(8, 23);
			pictureBoxKrill.Image = UIElements.getIconImage(9, 23);
			pictureBoxPlumpWorm.Image = UIElements.getIconImage(10, 23);
			pictureBoxRatTail.Image = UIElements.getIconImage(2, 23);
			pictureBoxGlowWorm.Image = UIElements.getIconImage(3, 23);
			pictureBoxHeavySteelJig.Image = UIElements.getIconImage(5, 23);
			pictureBoxShrimpCageFeeder.Image = UIElements.getIconImage(4, 23);
			pictureBoxPillBug.Image = UIElements.getIconImage(1, 23);
			pictureBoxSquidStrip.Image = UIElements.getIconImage(7, 23);
			pictureBoxMackerelStrip.Image = UIElements.getIconImage(2, 24);
			pictureBoxStoneflyNymph.Image = UIElements.getIconImage(6, 23);



			ragwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "ragwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
			krillLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "krillCount", false, DataSourceUpdateMode.OnPropertyChanged);
			plumpwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "plumpwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
			rattailLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "rattailCount", false, DataSourceUpdateMode.OnPropertyChanged);
			glowwormLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "glowwormCount", false, DataSourceUpdateMode.OnPropertyChanged);
			heavysteeljigLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "heavysteeljigCount", false, DataSourceUpdateMode.OnPropertyChanged);
			shrimpcagefeederLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "shrimpcagefeederCount", false, DataSourceUpdateMode.OnPropertyChanged);
			pillbugLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "pillbugCount", false, DataSourceUpdateMode.OnPropertyChanged);
			squidstripLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "squidstripCount", false, DataSourceUpdateMode.OnPropertyChanged);
			mackerelstripLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "mackerelstripCount", false, DataSourceUpdateMode.OnPropertyChanged);
			stoneflynymphLabel.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "stoneflynymphCount", false, DataSourceUpdateMode.OnPropertyChanged);


			// Achievements
			mantasPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 32));
			octopodsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(3, 32));
			sharksPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 32));
			jellyfishPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(5, 32));
			seadragonPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(6, 32));
			balloonsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(7, 32));
			crabsPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(8, 32));
			indigo5kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
			indigo10kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
			indigo16kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));
			indigo20kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 27));

			shrimpPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(4, 34));
			shellfishPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(2, 34));
			squidPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(3, 34));
			ruby5kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));
			ruby10kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));
			ruby16kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(9, 27));

#if !RB_TC
			mantisPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(5, 34));
			prehistoricPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(6, 34));
#endif

			overall100kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
			overall500kPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
			overall1mPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));
			overall3mPicture.Image = ImageExtensions.ToGrayScale(UIElements.getIconImage(1, 27));

			// Wire up achievement checkbox handlers BEFORE data bindings so the
			// CheckedChanged handler is attached when bindings read initial values.
			// Without this, reopening the form after dispose loses color icons because
			// the singleton's properties are already true, the binding fires CheckedChanged
			// immediately, but no handler is attached yet to set the color image. Then
			// RefreshAchievements() no-ops (value unchanged) so the icon stays greyscale.
			SetupAchievementCheckboxHandlers();

			// Achievements Databind
			mantasPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementMantas", false, DataSourceUpdateMode.OnPropertyChanged);
			octopodsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementOctopods", false, DataSourceUpdateMode.OnPropertyChanged);
			sharksPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSharks", false, DataSourceUpdateMode.OnPropertyChanged);
			jellyfishPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementJellyfish", false, DataSourceUpdateMode.OnPropertyChanged);
			seadragonPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSeadragons", false, DataSourceUpdateMode.OnPropertyChanged);
			balloonsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementBalloons", false, DataSourceUpdateMode.OnPropertyChanged);
			crabsPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementCrabs", false, DataSourceUpdateMode.OnPropertyChanged);
			indigo5kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement5kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
			indigo10kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement10kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
			indigo16kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement16kindigo", false, DataSourceUpdateMode.OnPropertyChanged);
			indigo20kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement20kindigo", false, DataSourceUpdateMode.OnPropertyChanged);

			shrimpPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementShrimp", false, DataSourceUpdateMode.OnPropertyChanged);
			shellfishPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementShellfish", false, DataSourceUpdateMode.OnPropertyChanged);
			squidPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementSquid", false, DataSourceUpdateMode.OnPropertyChanged);
#if !RB_TC
			mantisPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementMantisShrimp", false, DataSourceUpdateMode.OnPropertyChanged);
			prehistoricPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievementPrehistoric", false, DataSourceUpdateMode.OnPropertyChanged);
#endif
			ruby5kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement5kruby", false, DataSourceUpdateMode.OnPropertyChanged);
			ruby10kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement10kruby", false, DataSourceUpdateMode.OnPropertyChanged);
			ruby16kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement16kruby", false, DataSourceUpdateMode.OnPropertyChanged);

			overall100kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement100koverall", false, DataSourceUpdateMode.OnPropertyChanged);
			overall500kPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement500koverall", false, DataSourceUpdateMode.OnPropertyChanged);
			overall1mPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement1moverall", false, DataSourceUpdateMode.OnPropertyChanged);
			overall3mPicture.DataBindings.Add("Checked", OceanTripPlanner.FFXIV_Databinds.Instance, "achievement3moverall", false, DataSourceUpdateMode.OnPropertyChanged);

			// Wire up unified radio button handlers
			SetupRadioButtonHandlers();
		}

		/// <summary>
		/// Mapping of achievement checkboxes to their icon coordinates
		/// </summary>
		private readonly Dictionary<CheckBox, (int x, int y)> _achievementIconMap = new Dictionary<CheckBox, (int, int)>();

		/// <summary>
		/// Mapping of radio buttons to their corresponding enum values
		/// </summary>
		private readonly Dictionary<RadioButton, object> _radioButtonEnumMap = new Dictionary<RadioButton, object>();

		/// <summary>
		/// Mapping of Achievement Focus radio buttons to AchievementType int values
		/// </summary>
		private readonly Dictionary<RadioButton, int> _achievementFocusMap = new Dictionary<RadioButton, int>();

		/// <summary>
		/// Setup unified event handlers for all achievement checkboxes
		/// </summary>
		private void SetupAchievementCheckboxHandlers()
		{
			// Build icon coordinate mapping from constructor initialization
			_achievementIconMap[mantasPicture] = (9, 32);
			_achievementIconMap[octopodsPicture] = (3, 32);
			_achievementIconMap[sharksPicture] = (4, 32);
			_achievementIconMap[jellyfishPicture] = (5, 32);
			_achievementIconMap[seadragonPicture] = (6, 32);
			_achievementIconMap[balloonsPicture] = (7, 32);
			_achievementIconMap[crabsPicture] = (8, 32);
			_achievementIconMap[indigo5kPicture] = (4, 27);
			_achievementIconMap[indigo10kPicture] = (4, 27);
			_achievementIconMap[indigo16kPicture] = (4, 27);
			_achievementIconMap[indigo20kPicture] = (4, 27);
			_achievementIconMap[shrimpPicture] = (4, 34);
			_achievementIconMap[shellfishPicture] = (2, 34);
			_achievementIconMap[squidPicture] = (3, 34);
			_achievementIconMap[ruby5kPicture] = (9, 27);
			_achievementIconMap[ruby10kPicture] = (9, 27);
			_achievementIconMap[ruby16kPicture] = (9, 27);
#if !RB_TC
			_achievementIconMap[mantisPicture] = (5, 34);
			_achievementIconMap[prehistoricPicture] = (6, 34);
#endif
			_achievementIconMap[overall100kPicture] = (1, 27);
			_achievementIconMap[overall500kPicture] = (1, 27);
			_achievementIconMap[overall1mPicture] = (1, 27);
			_achievementIconMap[overall3mPicture] = (1, 27);

			// Wire up single event handler for all achievement checkboxes
			foreach (var checkbox in _achievementIconMap.Keys)
			{
				checkbox.CheckedChanged += AchievementCheckbox_CheckedChanged;
			}
		}

		/// <summary>
		/// Unified event handler for all achievement checkboxes
		/// Updates the checkbox image to color when checked
		/// </summary>
		private void AchievementCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (sender is CheckBox checkbox && checkbox.Checked)
			{
				if (_achievementIconMap.TryGetValue(checkbox, out var coords))
				{
					checkbox.Image = UIElements.getIconImage(coords.x, coords.y);
				}
			}
		}

		/// <summary>
		/// Setup unified event handlers for all radio button groups
		/// </summary>
		private void SetupRadioButtonHandlers()
		{
			// Fishing Priority radio buttons
			_radioButtonEnumMap[automatic] = FishPriority.Auto;
			_radioButtonEnumMap[points] = FishPriority.Points;
			_radioButtonEnumMap[fishingLog] = FishPriority.FishLog;
			_radioButtonEnumMap[achievements] = FishPriority.Achievements;
			_radioButtonEnumMap[ignoreBoat] = FishPriority.IgnoreBoat;

			// Fishing Route radio buttons
			_radioButtonEnumMap[indigo] = FishingRoute.Indigo;
			_radioButtonEnumMap[ruby] = FishingRoute.Ruby;

			// Full GP Action radio buttons
			_radioButtonEnumMap[GPActionNothing] = FullGPAction.None;
			_radioButtonEnumMap[GPActionDoubleHook] = FullGPAction.DoubleHook;
			_radioButtonEnumMap[GPActionChum] = FullGPAction.Chum;

			// Patience radio buttons
			_radioButtonEnumMap[patienceDefaultLogic] = ShouldUsePatience.OnlyForSpecificFish;
			_radioButtonEnumMap[patienceSpectralOnly] = ShouldUsePatience.SpectralOnly;
			_radioButtonEnumMap[patienceAlways] = ShouldUsePatience.AlwaysUsePatience;

			// Fish Exchange radio buttons
			_radioButtonEnumMap[fishExchangeNone] = ExchangeFish.None;
			_radioButtonEnumMap[fishExchangeDesynthesize] = ExchangeFish.Desynth;
			_radioButtonEnumMap[fishExchangeSell] = ExchangeFish.Sell;

			// Lure radio buttons
			_radioButtonEnumMap[lureNone] = LurePreference.None;
			_radioButtonEnumMap[lureModest] = LurePreference.Modest;
			_radioButtonEnumMap[lureAmbitious] = LurePreference.Ambitious;
			_radioButtonEnumMap[lureAuto] = LurePreference.Auto;

			// Target Fish radio buttons — handled separately (maps to uint, not enum)
			targetFishNone.CheckedChanged += TargetFish_CheckedChanged;
			targetFishJinbei.CheckedChanged += TargetFish_CheckedChanged;

			// Achievement Focus radio buttons — maps to AchievementType int
			_achievementFocusMap[indigoMantas] = (int)AchievementType.Mantas;
			_achievementFocusMap[radioButtonFlat1] = (int)AchievementType.Octopods;
			_achievementFocusMap[radioButtonFlat2] = (int)AchievementType.Sharks;
			_achievementFocusMap[radioButtonFlat3] = (int)AchievementType.Jellyfish;
			_achievementFocusMap[radioButtonFlat4] = (int)AchievementType.Seadragons;
			_achievementFocusMap[radioButtonFlat5] = (int)AchievementType.Balloons;
			_achievementFocusMap[radioButtonFlat6] = (int)AchievementType.Crabs;
			_achievementFocusMap[radioButtonFlat7] = (int)AchievementType.Shrimp;
			_achievementFocusMap[radioButtonFlat8] = (int)AchievementType.Shellfish;
			_achievementFocusMap[radioButtonFlat9] = (int)AchievementType.Squid;
#if !RB_TC
			_achievementFocusMap[thavnairMantis] = (int)AchievementType.MantisShrimp;
			_achievementFocusMap[thavnairPrehistoric] = (int)AchievementType.Prehistoric;
#endif

			foreach (var rb in _achievementFocusMap.Keys)
				rb.CheckedChanged += AchievementFocus_CheckedChanged;

			// Wire up single event handler for all radio buttons
			foreach (var radioButton in _radioButtonEnumMap.Keys)
			{
				radioButton.CheckedChanged += RadioButton_CheckedChanged;
			}

			// Restore saved achievement focus for both routes
			int savedIndigo = OceanTripNewSettings.Instance.IndigoAchievementFocus;
			int savedRuby = OceanTripNewSettings.Instance.RubyAchievementFocus;
			foreach (var kvp in _achievementFocusMap)
			{
				if (kvp.Key.Parent == groupBox1 && kvp.Value == savedIndigo)
					kvp.Key.Checked = true;
				else if (kvp.Key.Parent == groupBox2 && kvp.Value == savedRuby)
					kvp.Key.Checked = true;
			}
		}

		/// <summary>
		/// Unified event handler for all radio buttons
		/// Updates the corresponding settings property when a radio button is selected
		/// </summary>
		private void RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (sender is RadioButton radioButton && radioButton.Checked)
			{
				if (_radioButtonEnumMap.TryGetValue(radioButton, out var enumValue))
				{
					// Determine which property to update based on the enum type
					switch (enumValue)
					{
						case FishPriority priority:
							OceanTripNewSettings.Instance.FishPriority = priority;
							break;
						case FishingRoute route:
							OceanTripNewSettings.Instance.FishingRoute = route;
							break;
						case FullGPAction gpAction:
							OceanTripNewSettings.Instance.FullGPAction = gpAction;
							break;
						case ShouldUsePatience patience:
							OceanTripNewSettings.Instance.Patience = patience;
							break;
						case ExchangeFish exchangeFish:
							OceanTripNewSettings.Instance.ExchangeFish = exchangeFish;
							break;
						case LurePreference lure:
							OceanTripNewSettings.Instance.LurePreference = lure;
							break;
					}
				}
			}
		}

		private void AchievementFocus_CheckedChanged(object sender, EventArgs e)
		{
			if (sender is RadioButton rb && rb.Checked)
			{
				if (_achievementFocusMap.TryGetValue(rb, out var achievementType))
				{
					if (rb.Parent == groupBox1)
						OceanTripNewSettings.Instance.IndigoAchievementFocus = achievementType;
					else
						OceanTripNewSettings.Instance.RubyAchievementFocus = achievementType;
				}
			}
		}

		private void TargetFish_CheckedChanged(object sender, EventArgs e)
		{
			if (sender is RadioButton rb && rb.Checked)
			{
				if (rb == targetFishJinbei)
					OceanTripNewSettings.Instance.TargetFishId = (uint)OceanFish.JuniorJinbei;
				else
					OceanTripNewSettings.Instance.TargetFishId = 0;
			}
		}


		private void FormOceanSettings_MouseDown(object sender, MouseEventArgs e)
		{
			HandleMouseDown(sender, e);
		}
	}
}
