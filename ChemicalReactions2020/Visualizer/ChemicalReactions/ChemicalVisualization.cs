using DongUtility;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Thermodynamics;
using VisualizerControl;
using VisualizerControl.Commands;
using VisualizerControl.Shapes;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.ChemicalReactions
{
    /// <summary>
    /// Adapted to use visualizer for thermodynamics project
    /// </summary>
    class ChemicalVisualization : IVisualization
    {
        /// <summary>
        /// The underlying particle container
        /// </summary>
        private readonly ParticleContainer container;

        /// <summary>
        /// A map of particles to indices
        /// </summary>
        private readonly Dictionary<Particle, int> particleMap = new Dictionary<Particle, int>();

        public ChemicalVisualization(ParticleContainer container)
        {
            this.container = container;
        }

        public bool Continue => true;
        public double Time { get; private set; }

        public double ParticleSize { get; set; } = 1;
        private int counter = 0;

        public double BoxScale { get; set; } = 1.1;

        public VisualizerCommandSet Initialization()
        {
            var set = new VisualizerCommandSet();

            var box = new ObjectPrototype(new Cube3D(), new BasicMaterial(BoxColor, true),
                ConvertToVector3D(container.Size / 2), ConvertToVector3D(container.Size / 2 * BoxScale));

            set.AddCommand(new AddObject(box, counter));
            ++counter;

            // Add all the particles
            container.Particles.ForEach((particle) => AddParticle(particle, set));

            return set;
        }

        private void AddParticle(Particle particle, VisualizerCommandSet set)
        {
            // Start it off in the right place
            var obj = new ObjectPrototype(new Sphere3D(), new BasicMaterial(ConvertColor(particle.Info.Color)),
                ConvertToVector3D(particle.Position), new Vector3D(ParticleSize, ParticleSize, ParticleSize));

            set.AddCommand(new AddObject(obj, counter));
            particleMap.Add(particle, counter);
            ++counter;
        }

        private void RemoveParticle(Particle particle, VisualizerCommandSet set)
        {
            int index = particleMap[particle];
            set.AddCommand(new RemoveObject(index));
        }

        public double SpecularCoefficient { get; set; } = 1;
        public Color BoxColor { get; set; } = Colors.SlateBlue;

        public VisualizerCommandSet Tick(double newTime)
        {
            var set = new VisualizerCommandSet();

            container.Update(newTime - Time);

            // Box scale
            Vector3D position = ConvertToVector3D(container.Size / 2);
            Vector3D scale = ConvertToVector3D(container.Size / 2 * BoxScale);
            set.AddCommand(new TransformObject(0, position, scale, 0, 0));

            container.ParticlesToAdd.ForEach((particle) => AddParticle(particle, set));
            container.ParticlesToRemove.ForEach((particle) => RemoveParticle(particle, set));

            foreach (var particle in container.Particles)
            {
                int index = particleMap[particle];
                set.AddCommand(new MoveObject(index, ConvertToVector3D(particle.Position)));
            }


            Time = newTime;

            return set;
        }

        static private Vector3D ConvertToVector3D(Vector vec)
        {
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }
    }
}
