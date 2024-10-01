using Ocean_Trip.Properties;
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
    public partial class FormCurrentRoute : Form
    {
        FormSettings _parent;

        public FormCurrentRoute(FormSettings parent)
        {
            InitializeComponent();
            _parent = parent;

            exitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);

            // Databinding
            BindRouteTitle();
            BindRouteTimeOfDay();
            BindRouteIcons();
            BindRouteNames();
        }

        private void BindRouteTitle()
        {
            r1title.DataBindings.Add("Text", OceanTripPlanner.FFXIV_Databinds.Instance, "r1title", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void BindRouteTimeOfDay()
        {
            r1tod.DataBindings.Add("Image", OceanTripPlanner.FFXIV_Databinds.Instance, "r1tod", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void BindRouteIcons()
        {
            var databinds = OceanTripPlanner.FFXIV_Databinds.Instance;
            var updateMode = DataSourceUpdateMode.OnPropertyChanged;

            r1n1.DataBindings.Add("Image", databinds, "r1n1_icon", true, updateMode);
            r1n2.DataBindings.Add("Image", databinds, "r1n2_icon", true, updateMode);
            r1n3.DataBindings.Add("Image", databinds, "r1n3_icon", true, updateMode);
            r1n4.DataBindings.Add("Image", databinds, "r1n4_icon", true, updateMode);
            r1n5.DataBindings.Add("Image", databinds, "r1n5_icon", true, updateMode);
            r1n6.DataBindings.Add("Image", databinds, "r1n6_icon", true, updateMode);
            r1n7.DataBindings.Add("Image", databinds, "r1n7_icon", true, updateMode);
            r1n8.DataBindings.Add("Image", databinds, "r1n8_icon", true, updateMode);
            r1n9.DataBindings.Add("Image", databinds, "r1n9_icon", true, updateMode);
            r1n10.DataBindings.Add("Image", databinds, "r1n10_icon", true, updateMode);

            r1s1.DataBindings.Add("Image", databinds, "r1s1_icon", true, updateMode);
            r1s2.DataBindings.Add("Image", databinds, "r1s2_icon", true, updateMode);
            r1s3.DataBindings.Add("Image", databinds, "r1s3_icon", true, updateMode);
            r1s4.DataBindings.Add("Image", databinds, "r1s4_icon", true, updateMode);
            r1s5.DataBindings.Add("Image", databinds, "r1s5_icon", true, updateMode);
            r1s6.DataBindings.Add("Image", databinds, "r1s6_icon", true, updateMode);
            r1s7.DataBindings.Add("Image", databinds, "r1s7_icon", true, updateMode);
            r1s8.DataBindings.Add("Image", databinds, "r1s8_icon", true, updateMode);
            r1s9.DataBindings.Add("Image", databinds, "r1s9_icon", true, updateMode);
            r1s10.DataBindings.Add("Image", databinds, "r1s10_icon", true, updateMode);
        }

        private void BindRouteNames()
        {
            var databinds = OceanTripPlanner.FFXIV_Databinds.Instance;
            var updateMode = DataSourceUpdateMode.OnPropertyChanged;

            r1l1.DataBindings.Add("Text", databinds, "r1n1_name", true, updateMode);
            r1l2.DataBindings.Add("Text", databinds, "r1n2_name", true, updateMode);
            r1l3.DataBindings.Add("Text", databinds, "r1n3_name", true, updateMode);
            r1l4.DataBindings.Add("Text", databinds, "r1n4_name", true, updateMode);
            r1l5.DataBindings.Add("Text", databinds, "r1n5_name", true, updateMode);
            r1l6.DataBindings.Add("Text", databinds, "r1n6_name", true, updateMode);
            r1l7.DataBindings.Add("Text", databinds, "r1n7_name", true, updateMode);
            r1l8.DataBindings.Add("Text", databinds, "r1n8_name", true, updateMode);
            r1l9.DataBindings.Add("Text", databinds, "r1n9_name", true, updateMode);
            r1l10.DataBindings.Add("Text", databinds, "r1n10_name", true, updateMode);
            r1l11.DataBindings.Add("Text", databinds, "r1s1_name", true, updateMode);
            r1l12.DataBindings.Add("Text", databinds, "r1s2_name", true, updateMode);
            r1l13.DataBindings.Add("Text", databinds, "r1s3_name", true, updateMode);
            r1l14.DataBindings.Add("Text", databinds, "r1s4_name", true, updateMode);
            r1l15.DataBindings.Add("Text", databinds, "r1s5_name", true, updateMode);
            r1l16.DataBindings.Add("Text", databinds, "r1s6_name", true, updateMode);
            r1l17.DataBindings.Add("Text", databinds, "r1s7_name", true, updateMode);
            r1l18.DataBindings.Add("Text", databinds, "r1s8_name", true, updateMode);
            r1l19.DataBindings.Add("Text", databinds, "r1s9_name", true, updateMode);
            r1l20.DataBindings.Add("Text", databinds, "r1s10_name", true, updateMode);
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

        private void FormCurrentRoute_MouseDown(object sender, MouseEventArgs e)
        {
            _parent.MoveWindow(sender, e); //UIElements.MoveWindow(Handle, sender, e);
        }
    }
}
