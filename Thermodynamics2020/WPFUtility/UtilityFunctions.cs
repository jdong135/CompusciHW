using System;
using System.Reflection;
using System.Windows;

namespace WPFUtility
{
    static public class UtilityFunctions
    {
        /// <summary>
        /// Gets the x and y dpi of the current device as a pair.
        /// </summary>
        static public Tuple<double, double> GetDPI()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);
            return new Tuple<double, double>(dpiX, dpiY);
        }

        /// <summary>
        /// Converts from a .NET Standard-compatible System.Drawing.Color to
        /// a WPF-compatible System.Windows.Media.Color
        /// </summary>
        static public System.Windows.Media.Color ConvertColor(System.Drawing.Color color)
        {
            return new System.Windows.Media.Color()
            {
                R = color.R,
                G = color.G,
                B = color.B,
                A = color.A
            };
        }

        static public System.Drawing.Color ConvertColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        static public System.Windows.Media.Media3D.Vector3D ConvertVector (DongUtility.Vector vec)
        {
            return new System.Windows.Media.Media3D.Vector3D(vec.X, vec.Y, vec.Z);
        }
    }
}
