using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace VisualizerControl
{
    public class TextureMaterial : MaterialPrototype
    {
        private string filename;
        public TextureMaterial(string filename)
        {
            this.filename = filename;
        }
        public override Material Material
        {
            get
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(@"..\..\..\" + filename, UriKind.Relative));
                return new DiffuseMaterial(brush);
            }
        }
    }
}
