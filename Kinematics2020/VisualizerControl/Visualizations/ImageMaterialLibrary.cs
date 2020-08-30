using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Visualizations
{
    static class ImageMaterialLibrary
    {
        private static Dictionary<string, DiffuseMaterial> materials = new Dictionary<string, DiffuseMaterial>();

        static public Material GetMaterial(string photoFilename)
        {
            if (materials.ContainsKey(photoFilename))
            {
                return materials[photoFilename];
            }
            else
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(@"..\..\" + photoFilename, UriKind.Relative));
                var material = new DiffuseMaterial(brush);
                materials.Add(photoFilename, material);
                return material;
            }
        }
    }
}
