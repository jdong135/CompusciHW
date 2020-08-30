using DongUtility;
using System;
using System.Collections.Generic;

namespace Thermodynamics
{
    /// <summary>
    /// A container class for holding gas particles in a cubical container
    /// </summary>
    public class ParticleContainer
    {
        public double GetTemperature()
        {
            double boltzmannConstant = 1.38 * Math.Pow(10, -23);
            double kineticEnergy = 0;
            foreach (Particle particle in Particles)
            {
                kineticEnergy += particle.KineticEnergy;
            }
            double avgKE = kineticEnergy / Particles.Count;
            double temperature = avgKE * 2 / (3 * boltzmannConstant);
            
            return temperature;
        }

        public double Pressure { get; set; }
        public double SurfaceArea { get; }
        /// <summary>
        /// The size of the container
        /// Each component of the vector is the size in that dimension
        /// </summary>
        public Vector Size { get; set; }
        /// <summary>
        /// All the particles in the container
        /// </summary>
        public List<Particle> Particles { get; } = new List<Particle>();

        /// <summary>
        /// A dictionary to keep track of all the particle types
        /// </summary>
        public ParticleDictionary Dictionary { get; } = new ParticleDictionary();

        /// <summary>
        /// Access to the random generator
        /// </summary>
        static protected Random Random { get { return RandomGenerator.RandomGen; } }

        public ParticleContainer(double xSize, double ySize, double zSize)
        {
            Size = new Vector(xSize, ySize, zSize);
            SurfaceArea = 2*(xSize * ySize + xSize * zSize + ySize * zSize);
        }
        public List<Particle> ParticlesToAdd { get; } = new List<Particle>();
        public List<Particle> ParticlesToRemove { get; } = new List<Particle>();

        public void AddParticle(Particle part)
        {
            ParticlesToAdd.Add(part);
        }

        public void RemoveParticle(Particle part)
        {
            ParticlesToRemove.Add(part);
        }

        /// <summary>
        /// Adds a particle directly
        /// Not for general use; hence, it is protected
        /// </summary>
        protected virtual void AddParticleDirectly(Particle part)
        {
            Particles.Add(part);
        }

        /// <summary>
        /// Removes a particle directly
        /// Not for general use; hence, it is protected
        /// </summary>
        /// <param name="part"></param>
        protected virtual void RemoveParticleDirectly(Particle part)
        {
            Particles.Remove(part);
        }

        /// <summary>
        /// Gets the total number of particles of a given name
        /// </summary>
        public int GetNParticles(string name)
        {
            int total = 0;

            foreach (var part in Particles)
            {
                if (part.Info.Name == name)
                {
                    ++total;
                }
            }

            return total;
        }

        /// <summary>
        /// Adds a number of particles at random
        /// </summary>
        /// <param name="generator">The random generator to use</param>
        /// <param name="name">The name of the particle type</param>
        /// <param name="number">The number of particles to add</param>
        public void AddRandomParticles(RandomGenerator generator, string name, int number)
        {
            for (int i = 0; i < number; ++i)
            {
                AddParticleDirectly(generator.GetRandomParticle(name));
            }
        }

        /// <summary>
        /// Updates all particles for a given time increment
        /// </summary>
        public virtual void Update(double deltaTime)
        {
            ParticlesToAdd.Clear();
            ParticlesToRemove.Clear();

            Setup();

            //Only uncomment when you want to change pressure
            //Pressure = 0;
            foreach (Particle part in Particles)
            {
                part.Update(deltaTime);
                ParticleUpdate(part);
            }

            ParticlesToAdd.ForEach((x) => AddParticleDirectly(x));
            ParticlesToRemove.ForEach((x) => RemoveParticleDirectly(x));
        }

        protected virtual void ParticleUpdate(Particle part)
        {
            CheckParticle(part);
        }

        protected virtual void Setup()
        { }

        /// <summary>
        /// A function that extracts a specific property of a particle
        /// </summary>
        public delegate double ParticleFunction(Particle part);

        /// <summary>
        /// Get a list of values for a given property of all particles
        /// Useful for histograms
        /// </summary>
        public List<double> GetParticlePropertyList(ParticleFunction func)
        {
            var response = new List<double>();

            foreach (var part in Particles)
            {
                response.Add(func(part));
            }

            return response;
        }

        /// <summary>
        /// Make sure the particle lies within the bounds of the box
        /// Reflect it back if it is not
        /// </summary>
        double changeKE = 1e-21; // Change the temperature gradually
        
        protected virtual void CheckParticle(Particle particle)
        {
            Vector initialVelocity = particle.Velocity;
            Vector newVec = particle.Position;
            if (particle.Position.X < 0 || particle.Position.X > Size.X)
            {
                particle.Velocity = new Vector(-particle.Velocity.X, particle.Velocity.Y, particle.Velocity.Z);
                if (particle.Position.X < 0)
                {
                    newVec.X = 0;
                }
                else if (particle.Position.X > Size.X)
                {
                    newVec.X = Size.X;
                }
            }
            if (particle.Position.Y < 0 || particle.Position.Y > Size.Y)
            {
                particle.Velocity = new Vector(particle.Velocity.X, -particle.Velocity.Y, particle.Velocity.Z);
                if (particle.Position.Y < 0)
                {
                    newVec.Y = 0;
                }
                else if (particle.Position.Y > Size.Y)
                {
                    newVec.Y = Size.Y;
                }
            }
            if (particle.Position.Z < 0 || particle.Position.Z > Size.Z)
            {
                particle.Velocity = new Vector(particle.Velocity.X, particle.Velocity.Y, -particle.Velocity.Z);
                if (particle.Position.Z < 0)
                {
                    newVec.Z = 0;
                }
                else if (particle.Position.Z > Size.Z)
                {
                    newVec.Z = Size.Z;
                }
            }
            // Update pressure = F/A = (changePressure/(changeTime*SurfaceArea))
            // particle.Velocity *= increaseFactor; // Change temperature by changing velocity (bouncing off wall)
            // 1/2 mv^2 + changeKE = 1/2 mv_f^2
            // mv^2 + 2changeKE = mv_f^2
            // v^2 + 2changeKE/m = v_f^2
            // v_f = sqrt(v^2 + 2changeKE/m)

            //Beginning of code to change temperature
            double newVelocityMag = Math.Sqrt(particle.Velocity.Magnitude * particle.Velocity.Magnitude + 2 * changeKE / particle.Info.Mass);
            particle.Velocity = particle.Velocity.UnitVector() * newVelocityMag;
            //End of code to change temperature

            //Beginning of code to change pressure
            //double changePressure = particle.Info.Mass * (particle.Velocity - initialVelocity).Magnitude;
            //Pressure += changePressure / (0.01 * SurfaceArea);
            //End of code to change pressure

            particle.Position = newVec;
        }
    }
}
