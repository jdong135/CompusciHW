using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;

namespace FiniteElement
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
        /// <summary>
        /// All the projectiles that are in motion
        /// </summary>
        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        private StreamWriter ps;
        private List<Force> forces = new List<Force>();

        public KinematicsEngine(StreamWriter ps = null)
        {
            this.ps = ps;
        }

        /// <summary>
        /// Runs the simulation for a set number of increments
        /// </summary>
        /// <param name="print">Whether to print the results to the StreamWriter</param>
        public void Run(double timeIncrement, int nIncrements, bool print = false)
        {
            // Set print to false if StreamWriter is not defined
            if (ps == null)
                print = false;

            // Write a header
            if (print)
            {
                ps.WriteLine(Header());
            }

            // Run the loop
            for (int i = 0; i < nIncrements; ++i)
            {
                Increment(timeIncrement);
                if (print)
                {
                    ps.WriteLine(ToString());
                }

            }
        }

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

        override public string ToString()
        {
            string response = "";
            foreach (Projectile projectile in Projectiles)
            {
                response += "\t" + Time + "\t" + projectile;
            }
            return response;
        }

        /// <summary>
        /// Create a header string for output in a text file
        /// </summary>
        public string Header()
        {
            string response = "";
            foreach (Projectile projectile in Projectiles)
            {
                response += projectile.Name;
                response += "\ttime\tx\ty\tz\tMag\tvx\tvy\tvz\tSpeed\tax\tay\taz\t|a|";
            }
            return response;
        }


        /// <summary>
        /// Runs one tick of the clock
        /// </summary>
        public bool Increment(double timeIncrement)
        {
            // Increment time
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
