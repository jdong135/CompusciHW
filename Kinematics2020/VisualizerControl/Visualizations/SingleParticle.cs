using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Visualizations
{
    public class SingleParticle : Object3D
    {
        private IProjectile proj;

        public SingleParticle(IProjectile proj) : 
           base(proj.Shape, proj.Color)
        {
            this.proj = proj;
            Position = proj.Position;
            ScaleEvenly(proj.Size);
        }

        public override void Update()
        {
            Position = proj.Position;
        }
    }
}
