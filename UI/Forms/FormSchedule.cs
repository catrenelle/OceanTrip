using Ocean_Trip.Properties;
using OceanTripPlanner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ocean_Trip
{
    public partial class FormSchedule : Form
    {
        FormSettings _parent;

        public FormSchedule(FormSettings parent)
        {
            InitializeComponent();
            _parent = parent;

            exitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);

            if (OceanTripNewSettings.Instance.FishingRoute == FishingRoute.Indigo)
                routeNameValue.Text = "Indigo";
            else
                routeNameValue.Text = "Ruby";

            refreshScheduleGrid();
        }

        private void exitIcon_Click(object sender, EventArgs e)
        {
            _parent.Close();
        }

        private void exitIcon_MouseEnter(object sender, EventArgs e)
        {
            exitIcon.Image = Resources.exit;
        }

        private void exitIcon_MouseLeave(object sender, EventArgs e)
        {
            exitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);
        }

        private void FormSchedule_MouseDown(object sender, MouseEventArgs e)
        {
            _parent.MoveWindow(sender, e); //UIElements.MoveWindow(Handle, sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshScheduleGrid();
        }

        private void refreshScheduleGrid()
        {
            var schedules = Schedule.GetSchedules(18, routeNameValue.Text);

            scheduleGrid.Rows.Clear();

            foreach (var schedule in schedules)
            {
                // Time of Day
                Image ToD;
                switch (schedule.routeTime)
                {
                    case "Day":
                        ToD = Resources.day;
                        break;
                    case "Sunset":
                        ToD = Resources.sunset;
                        break;
                    case "Night":
                        ToD = Resources.night;
                        break;
                    default:
                        ToD = Resources.day;
                        break;
                }

                // Objectives
                Image objective1 = Resources.blank;
                Image objective2 = Resources.blank;
                var objectives = schedule.objectives.Split(',');
                if (objectives.Length > 0)
                    objective1 = objectiveImages(objectives[0].Trim());
                if (objectives.Length > 1)
                    objective2 = objectiveImages(objectives[1].Trim());

                scheduleGrid.Rows.Add(schedule.day, schedule.time, schedule.routeName, ToD, objective1, objective2);
            }
        }

        private Image objectiveImages(string objective)
        {
            Image obj = null;

            switch (objective)
            {
                // Indigo
                case "Mantas":
                    obj = UIElements.getIconImage(9, 32);
                    break;
                case "Octopods":
                    obj = UIElements.getIconImage(3, 32);
                    break;
                case "Sharks":
                    obj = UIElements.getIconImage(4, 32);
                    break;
                case "Jellyfish":
                    obj = UIElements.getIconImage(5, 32);
                    break;
                case "Seadragons":
                    obj = UIElements.getIconImage(6, 32);
                    break;
                case "Balloons":
                    obj = UIElements.getIconImage(7, 32);
                    break;
                case "Crabs":
                    obj = UIElements.getIconImage(8, 32);
                    break;
                case "Coral Manta":
                    obj = UIElements.getIconImage(10, 4);
                    break;
                case "Sothis":
                    obj = UIElements.getIconImage(10, 2);
                    break;
                case "Elasmosaurus":
                    obj = UIElements.getIconImage(10, 6);
                    break;
                case "Stonescale":
                    obj = UIElements.getIconImage(10, 8);
                    break;
                case "Hafgufa":
                    obj = UIElements.getIconImage(10, 10);
                    break;
                case "Seafaring Toad":
                    obj = UIElements.getIconImage(10, 12);
                    break;
                case "Placodus":
                    obj = UIElements.getIconImage(10, 14);
                    break;


                // Ruby
                case "Shellfish":
                    obj = UIElements.getIconImage(2, 34);
                    break;
                case "Squid":
                    obj = UIElements.getIconImage(3, 34);
                    break;
                case "Shrimp":
                    obj = UIElements.getIconImage(4, 34);
                    break;
                case "Taniwha":
                    obj = UIElements.getIconImage(10, 16);
                    break;
                case "Glass Dragon":
                    obj = UIElements.getIconImage(10, 18);
                    break;
                case "Hells' Claw":
                    obj = UIElements.getIconImage(10, 20);
                    break;
                case "Jewel of Plum Spring":
                    obj = UIElements.getIconImage(10, 22);
                    break;
                default:
                    obj = Resources.blank;
                    break;
            }

            return obj;
        }
    }
}
