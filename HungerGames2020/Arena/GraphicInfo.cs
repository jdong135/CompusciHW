using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Arena
{
    public class GraphicInfo
    {
        public string Filename { get; }
        public double XSize { get; }
        public double YSize { get; }

        public GraphicInfo(string pictureFile, double xsize, double ysize)
        {
            XSize = xsize;
            YSize = ysize;
            Filename = pictureFile;
        }



    }
}
