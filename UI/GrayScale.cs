using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class ImageExtensions
{
	static ColorMatrix grayMatrix = new ColorMatrix(new float[][]
	{
		new float[] { .2126f, .2126f, .2126f, 0, 0 },
		new float[] { .7152f, .7152f, .7152f, 0, 0 },
		new float[] { .0722f, .0722f, .0722f, 0, 0 },
		new float[] { 0, 0, 0, 1, 0 },
		new float[] { 0, 0, 0, 0, 1 }
	});

	public static Bitmap ToGrayScale(this Image source)
	{
		var grayImage = new Bitmap(source.Width, source.Height, source.PixelFormat);
		grayImage.SetResolution(source.HorizontalResolution, source.VerticalResolution);

		using (var g = Graphics.FromImage(grayImage))
		using (var attributes = new ImageAttributes())
		{
			attributes.SetColorMatrix(grayMatrix);
			g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
						0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
			return grayImage;
		}
	}
}
