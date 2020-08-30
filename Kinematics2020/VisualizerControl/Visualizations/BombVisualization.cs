using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    public class BombVisualization : BasicVisualization
    {
        private IEngine engine;
        private List<IProjectile> particles = new List<IProjectile>();

        public BombVisualization(IEngine engine)
        {
            this.engine = engine;

            foreach (var proj in engine.GetProjectiles())
            {
                var part = new ColorChanger(proj, new Cube3D());
                part.ScaleEvenly(proj.Size);
                AddObject(part);
            }
        }

        public override void Initialize()
        {
        }

        public override bool Tick(double newTime)
        {
            engine.Tick(newTime);
            return base.Tick(newTime);
        }
    }
}
