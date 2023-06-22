using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Ocean_Trip.Properties;

namespace Ocean_Trip
{
    internal class UIElements
    {
        // Click to Drag
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);



        // Rounded corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        // Click to Drag Window
        public static void MoveWindow(IntPtr Handle, object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        // Parse Tile Sheet, 10x32 Tiles (320), 40x40 pixels each (400x1280)
        public static Image getIconImage(int x, int y)
        {
            Image imgsrc = Resources.icons;
            Image imgdst = new Bitmap(40, 40);
            using (Graphics gr = Graphics.FromImage(imgdst))
            {
                gr.DrawImage(imgsrc,
                    new RectangleF(0, 0, imgdst.Width, imgdst.Height),
                    new RectangleF(((imgsrc.Width / 10) * (x - 1)), ((imgsrc.Height / 34) * (y - 1)), (imgsrc.Width / 10), (imgsrc.Height / 34)), GraphicsUnit.Pixel);
            }

            return imgdst;
        }
    }
}
