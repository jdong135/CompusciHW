using System;
using System.Collections.Generic;

namespace PhysicsUtility
{
    /// <summary>
    /// The class responsible for running the motion of all projectiles
    /// </summary>
    public class KinematicsEngine
    {
        /// <summary>
        /// The current time of the simulation
        /// </summary>
        public double Time { get; set; } = 0;

        private double oldTime = 0;
        public double DeltaTime => Time - oldTime;
        /// <summary>
        /// All the projectiles that are in motion
        /// </summary>
        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        /// <summary>
        /// All the forces that act on the particles
        /// </summary>
        private List<Force> forces = new List<Force>();

        /// <summary>
        /// Add a projectile to the simulation
        /// </summary>
        public void AddProjectile(Projectile projectile)
        {
            if (Projectiles.Contains(projectile))
            {
                throw new InvalidOperationException("Attempted to add Projectile that already exists!");
            }
            else
            {
                Projectiles.Add(projectile);
            }
        }

        /// <summary>
        /// Add a Force object to apply to the simulation
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Force force)
        {
            forces.Add(force);
        }

        /// <summary>
        /// Runs one tick of the clock
        /// </summary>
        public bool Increment(double timeIncrement)
        {
            // Increment time
            oldTime = Time;
            Time += timeIncrement;

            // Add all forces
            foreach (Force force in forces)
            {
                force.AddForce();
            }

            // Update all projectiles
            foreach (Projectile projectile in Projectiles)
            {
                projectile.Update(timeIncrement);
            }

            return true;
        }



    }
}
