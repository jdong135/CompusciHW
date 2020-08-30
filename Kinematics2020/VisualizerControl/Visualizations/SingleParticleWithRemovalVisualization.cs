using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    public class SingleParticleWithRemovalVisualization : SingleParticleVisualization
    {
        private IEngineWithRemoval MyEngine { get { return (IEngineWithRemoval)Engine; } }
        private Dictionary<IProjectile, Object3D> objects = new Dictionary<IProjectile, Object3D>();

        public SingleParticleWithRemovalVisualization(IEngineWithRemoval engine) :
            base(engine)
        { }

        protected override void AddProjectile(IProjectile proj)
        {
            var newPart = new SingleParticle(proj);
            Objects.Add(newPart);
            objects.Add(proj, newPart);
            AddObject(newPart);
        }

        public override bool Tick(double newTime)
        {
            foreach (var projectile in MyEngine.ParticlesToRemove())
            {
                Viz.RemoveParticle(objects[projectile]);
            }

            foreach (var projectile in MyEngine.ParticlesToAdd())
            {
                AddProjectile(projectile);
                Viz.AddParticle(Objects.Last());
            }

            return base.Tick(newTime);
        }


    }
}
