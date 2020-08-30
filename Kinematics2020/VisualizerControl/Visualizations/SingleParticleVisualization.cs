using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    public class SingleParticleVisualization : BasicVisualization
    {
        protected IEngine Engine { get; private set; }
        protected List<SingleParticle> Objects { get; } = new List<SingleParticle>();

        private Dictionary<Color, Material> materialDict = new Dictionary<Color, Material>();

        public SingleParticleVisualization(IEngine engine)
        {
            Engine = engine;

            foreach (var part in Engine.GetProjectiles())
            {
                AddProjectile(part);
            }
        }

        virtual protected void AddProjectile(IProjectile proj)
        {
            var newPart = new SingleParticle(proj);
            Objects.Add(newPart);
            AddObject(newPart);
        }

        override public void Initialize()
        {
        }

        public override bool Tick(double newTime)
        {
            base.Tick(newTime);
            return Engine.Tick(newTime);
        }
    }
}
