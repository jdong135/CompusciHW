using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl.Commands
{
    public class ChangeMaterial : IVisualizerCommand
    {
        private readonly int index;
        private readonly MaterialPrototype material;

        public ChangeMaterial(int index, MaterialPrototype materialPrototype)
        {
            this.index = index;
            material = materialPrototype;
        }
        public void Do(Visualizer viz)
        {
            var obj = viz.RetrieveObject(index);
            obj.GeoModel.Material = material.Material;
        }
    }
}
