using System;

using DongUtility;

namespace FiniteElement
{
    /// <summary>
    /// A basic projectile which can have forces applied to it
    /// </summary>
    public class Projectile
    {
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public Vector Acceleration { get; private set; } = Vector.NullVector();
        private Vector netForce = Vector.NullVector();

        private double mass;
        public double Mass
        {
            get
            {
                return mass;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("mass", "Mass cannot be zero or negative!");
                }
                else
                {
                    mass = value;
                }
            }
        }

        /// <summary>
        /// This allows you to name your projectiles for future reference
        /// </summary>
        public string Name { get; }

        public Projectile(Vector position, Vector velocity, double mass, String name = "")
        {
            Position = position;
            Velocity = velocity;
            this.mass = mass;
            Name = name;
        }

        override public string ToString()
        {
            return Position + "\t" + Velocity + "\t" + Acceleration;
        }

        private void UpdateAcceleration()
        {
            Acceleration = netForce / mass;
        }

        private void UpdateVelocity(double timeIncrement)
        {
            Velocity += Acceleration * timeIncrement;
        }

        private void UpdatePosition(double timeIncrement)
        {
            Position += Velocity * timeIncrement;
        }

        /// <summary>
        /// Adds a force to the particle.
        /// This lasts only until the next time Update() is called
        /// </summary>
        public void AddForce(Vector force)
        {
            netForce += force;
        }

        /// <summary>
        /// Updates the position, velocity, and acceleration of the particle
        /// </summary>
        public void Update(double timeIncrement)
        {
            UpdateAcceleration();
            netForce = Vector.NullVector(); // Reset forces
            UpdateVelocity(timeIncrement);
            UpdatePosition(timeIncrement);
        }

        /// <summary>
        /// The kinetic energy of the particle
        /// </summary>
        public double KineticEnergy()
        {
            double speed = Velocity.Magnitude;
            return .5 * mass * speed * speed;
        }
    }
}
