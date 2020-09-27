using System;
using System.Windows.Forms;
using ff14bot.Managers;
using ff14bot.AClasses;
using System.Linq;
using OceanTripPlanner.Helpers;

namespace OceanTripPlanner
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			propertyGrid1.SelectedObject = OceanTripSettings.Instance;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Lisbeth.OpenWindow();
		}
	}
}
