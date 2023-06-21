using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ocean_Trip.UI.Controls
{

    // Thanks RJ!
    public class ToggleButton : CheckBox
    {
        private Color onBackColor = Colors.buttonActiveBackgroundColor;
        private Color onToggleColor = Color.WhiteSmoke;
        private Color offBackColor = Color.Gray;
        private Color offToggleColor = Color.Gainsboro;

        public Color OnBackColor { get => onBackColor; set { onBackColor = value; Invalidate(); } }
        public Color OnToggleColor { get => onToggleColor; set { onToggleColor = value; Invalidate(); } }
        public Color OffBackColor { get => offBackColor; set { offBackColor = value; Invalidate(); } }
        public Color OffToggleColor { get => offToggleColor; set { offToggleColor = value; Invalidate(); } }

        public override string Text { get => ""; }

        public ToggleButton()
        {
            this.MinimumSize = new Size(22, 10);
        }

        private GraphicsPath GetFigurePath()
        {
            int arcSize = this.Height - 1;
            Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
            Rectangle rightArc = new Rectangle(this.Width - arcSize - 2, 0, arcSize, arcSize);

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            int toggleSize = this.Height - 5;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(this.Parent.BackColor);

            if (Checked)
            {
                pevent.Graphics.FillPath(new SolidBrush(onBackColor), GetFigurePath());
                pevent.Graphics.FillEllipse(new SolidBrush(onToggleColor), new Rectangle(this.Width - this.Height + 1, 2, toggleSize, toggleSize));
            }
            else
            {
                pevent.Graphics.FillPath(new SolidBrush(offBackColor), GetFigurePath());
                pevent.Graphics.FillEllipse(new SolidBrush(offToggleColor), new Rectangle(2, 2, toggleSize, toggleSize));
            }
        }
    }
}
