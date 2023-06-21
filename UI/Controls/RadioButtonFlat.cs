using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ocean_Trip.UI.Controls
{
    // Thanks RJ!
    public partial class RadioButtonFlat : RadioButton
    {
        private Color checkedColor = Colors.buttonActiveBackgroundColor;
        private Color unCheckedColor = Color.Gray;
        private Color disabledColor = Color.SlateGray;

        public Color CheckedColor { get => checkedColor; set { checkedColor = value; this.Invalidate(); } }
        public Color UnCheckedColor { get => unCheckedColor; set { unCheckedColor = value; this.Invalidate(); } }
        public Color DisabledColor { get => disabledColor; set { disabledColor = value; this.Invalidate(); } }

        public RadioButtonFlat()
        {
            this.MinimumSize = new Size(0, 15);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float rbBorderSize = 14F;
            float rbCheckSize = 8F;

            RectangleF rectRbBorder = new RectangleF()
            {
                X = 0.5F,
                Y = (Height - rbBorderSize) / 2, //Center
                Width = rbBorderSize,
                Height = rbBorderSize
            };

            RectangleF rectRbCheck = new RectangleF()
            {
                X = rectRbBorder.X + ((rectRbBorder.Width - rbCheckSize) / 2), // Center
                Y = (Height - rbCheckSize) / 2, //Center
                Width = rbCheckSize,
                Height = rbCheckSize
            };

            using (Pen penBorder = new Pen(checkedColor, 1.6F))
            using (SolidBrush brushRbCheck = new SolidBrush(checkedColor))
            using (SolidBrush brushText = new SolidBrush(this.ForeColor))
            {
                g.Clear(BackColor);
                if (Checked)
                {
                    g.DrawEllipse(penBorder, rectRbBorder);
                    g.FillEllipse(brushRbCheck, rectRbCheck);
                }
                else if (!Enabled)
                {
                    penBorder.Color = disabledColor;
                    ForeColor = disabledColor;
                    g.DrawEllipse(penBorder, rectRbBorder);
                }
                else 
                {
                    penBorder.Color = unCheckedColor;
                    g.DrawEllipse(penBorder, rectRbBorder);
                }

                g.DrawString(Text, Font, brushText, rbBorderSize + 8, (Height - TextRenderer.MeasureText(Text, Font).Height) / 2); // Y=Center
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = TextRenderer.MeasureText(Text, Font).Width + 30;
        }
    }
}
