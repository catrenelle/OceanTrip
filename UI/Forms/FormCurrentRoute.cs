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
    }
}
