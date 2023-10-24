using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WarnetClient
{
		internal class ViewportHelper
		{
				public double Width { get; set; }
				public double Height { get; set; }

				double z = 0;

				public ViewportHelper(double width, double height)
				{
						Width = width;
						Height = height;
				}

				public Point3D CoordinateToViewport(double x, double y)
				{
						//z -= 0.001;
						return CoordinateToViewport(Width, Height, x, y);	
				}

				static public Point3D CoordinateToViewport(double viewportWidth, double viewportHeight, double x, double y)
				{
						double vx = x / viewportWidth - 0.5;
						double vy = y / viewportWidth;

						double halfViewportY = viewportHeight / 2 / viewportWidth;

						vy = halfViewportY - vy;

						//MessageBox.Show("x " + x + " y " + y + " vx " + vx + " vy " + vy + " half " + halfViewportY);

						return new Point3D(Math.Round(vx, 4), Math.Round(vy, 4), 0);
				}
		}
}
