using FiniteElement;
using System.Collections.Generic;
using VisualizerControl;

namespace Visualizer.FiniteElement
{
    /// <summary>
    /// An adapter class to get KinematicsEngine to fit IEngine
    /// </summary>
    class EngineAdapter : IEngine
    {
        private KinematicsEngine engine;

        public double Time => engine.Time;

        public EngineAdapter(KinematicsEngine engine)
        {
            this.engine = engine;
        }

        public List<IProjectile> Projectiles
        {
            get
            {
                var list = new List<IProjectile>();

                foreach (var proj in engine.Projectiles)
                {
                    list.Add(new ProjectileAdapter(proj));
                }

                return list;
            }
        }

        public bool Tick(double newTime)
        {
            double increment = newTime - engine.Time;
            return engine.Increment(increment);
        }
    }
}
