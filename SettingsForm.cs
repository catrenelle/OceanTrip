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
			missingFishList.Items.Clear();

			foreach (var fish in missingFish.OrderBy(x => x))
				missingFishList.Items.Add(fish);
		}

		public void refreshRouteInformation(object sender=null, EventArgs e=null)
		{
			var nextBoat = OceanTrip.TimeUntilNextBoat();
			DateTime time = DateTime.Now.AddMinutes(nextBoat.TotalMinutes);

			// Sometimes a mismatch can happen between when the timespan was captured and when the datetime is generated
			if (time.Minute == 59)
				time = time.AddMinutes(1);

            var schedule = OceanTrip.GetSchedule(time);
            int posOnSchedule = 0;

            routeTimeValueLabel.Text = time.ToString("hh:mm tt");
			
			
			/*
			routeArea1Label.Text = areaName(schedule[posOnSchedule].Item1) + ", " + schedule[posOnSchedule].Item2;
			pictureBox1.Image = getFishImage(10, 2);
            routeArea2Label.Text = areaName(schedule[posOnSchedule + 1].Item1) + ", " + schedule[posOnSchedule + 1].Item2;
            pictureBox3.Image = getFishImage(10, 4);
            routeArea3Label.Text = areaName(schedule[posOnSchedule + 2].Item1) + ", " + schedule[posOnSchedule + 2].Item2;
            pictureBox5.Image = getFishImage(10, 6);
			*/
        }


        private string areaName(string shortname)
		{
			string name;

			switch (shortname)
			{
				case "south":
					name = "Southern Strait of Merlthor";
					break;
				case "galadion":
					name = "Galadion Bay";
					break;
				case "north":
					name = "Northern Strait of Merlthor";
					break;
				case "rhotano":
					name = "Rhotano Sea";
					break;
				case "ciel":
					name = "Cieldalaes";
					break;
				case "blood":
					name = "Bloodbrine Sea";
					break;
				case "sound":
					name = "Rothlyt Sound";
					break;
                default:
					name = "???";
					break;
			}

			return name;
		}

		private Image getFishImage(int x, int y)
		{
            Image imgsrc = Image.FromFile("BotBases/OceanTrip/Resources/Fish.png");
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
			if (tabControl.TabPages.Contains(routeInformationTab))
				tabControl.TabPages.Remove(routeInformationTab);
		}
	}
}
