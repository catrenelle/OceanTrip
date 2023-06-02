using System;
using System.Windows.Forms;
using ff14bot.Managers;
using ff14bot.AClasses;
using System.Linq;
using OceanTripPlanner.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using ff14bot.Helpers;
using System.IO;

namespace OceanTripPlanner
{
	public partial class SettingsForm : Form
	{
		public bool refreshMissingFish = false;

		public SettingsForm()
		{
			InitializeComponent();

            var file = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");

			if (!File.Exists(file))
			{
				refreshMissingFishButton.Enabled = false;
				Refresh();
			}
        }

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			propertyGrid1.SelectedObject = OceanTripSettings.Instance;
		}

		public void updateMissingFish(List<string> missingFish)
		{
			//missingFishList.Items.Clear();

			//foreach (var fish in missingFish.OrderBy(x => x))
			//	missingFishList.Items.Add(fish);
		}

		public void refreshRouteInformation(object sender=null, EventArgs e=null)
		{

			if (OceanTripSettings.Instance.FishingRoute == FishingRoute.Indigo)
				routeNameValue.Text = "Indigo";
			else
				routeNameValue.Text = "Ruby";

			var schedules = Schedule.GetSchedules(18);

			scheduleGrid.Rows.Clear();
			
			foreach(var schedule in schedules)
				scheduleGrid.Rows.Add(schedule.day, schedule.time, schedule.routeName, schedule.routeTime, schedule.objectives);

			//routeArea1Label.Text = Schedule.areaName(schedule[posOnSchedule].Item1) + ", " + schedule[posOnSchedule].Item2;
			//pictureBox1.Image = getFishImage(10, 2);
            //routeArea2Label.Text = Schedule.areaName(schedule[posOnSchedule + 1].Item1) + ", " + schedule[posOnSchedule + 1].Item2;
            //pictureBox3.Image = getFishImage(10, 4);
            //routeArea3Label.Text = Schedule.areaName(schedule[posOnSchedule + 2].Item1) + ", " + schedule[posOnSchedule + 2].Item2;
            //pictureBox5.Image = getFishImage(10, 6);
        }

		private Image getFishImage(int x, int y)
		{
            Image imgsrc = Image.FromFile("BotBases/OceanTrip/Resources/Indigo.png");
			Image imgdst = new Bitmap(40, 40);
            using (Graphics gr = Graphics.FromImage(imgdst))
            {
                gr.DrawImage(imgsrc,
                    new RectangleF(0, 0, imgdst.Width, imgdst.Height),
                    new RectangleF(((imgsrc.Width / 10) * (x-1)), ((imgsrc.Height / 22) * (y-1)), (imgsrc.Width / 10), (imgsrc.Height / 22)), GraphicsUnit.Pixel);
            }

			return imgdst;

        }

		private void SettingsForm_Shown(object sender, EventArgs e)
		{
			refreshRouteInformation();
        }

		private void Lisbeth_Button(object sender, EventArgs e)
		{
            Lisbeth.OpenWindow();
        }

		private void refreshMissingFishCache_Button(object sender, EventArgs e)
		{
			var file = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");

			if (File.Exists(file))
				File.Delete(file);

			MessageBox.Show("The list of Missing Fish will update the next time you stop and start the botbase.","Refresh Cache");
			refreshMissingFishButton.Enabled = false;
			Refresh();
        }


		public void tempHideRouteInformationTab()
		{
            if (tabControl.TabPages.Contains(route1))
                tabControl.TabPages.Remove(route1);
            if (tabControl.TabPages.Contains(route2))
                tabControl.TabPages.Remove(route2);
            if (tabControl.TabPages.Contains(route3))
                tabControl.TabPages.Remove(route3);
        }
    }
}
