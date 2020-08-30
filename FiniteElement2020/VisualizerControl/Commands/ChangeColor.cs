using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VisualizerControl.Commands
{
    public class ChangeColor : ChangeMaterial
    {
        public ChangeColor(int index, Color color, bool isSpecular = false) :
            base(index, new BasicMaterial(color, isSpecular))
        {
        }
    }
}
