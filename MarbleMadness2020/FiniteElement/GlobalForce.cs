using System.Collections.Generic;
using DongUtility;
using PhysicsUtility;

namespace FiniteElement
{
    /// <summary>
    /// A force that affects all projectiles in the simulation
    /// </summary>
    abstract public class GlobalForce : Force
    {
        private KinematicsEngine engine;

        public GlobalForce(KinematicsEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// The list of all projectiles, which some global forces will need, e.g. gravity
        /// </summary>
        protected IList<Projectile> Projectiles => engine.Projectiles;

        /// <summary>
        /// Returns the actual force on a given projectile
        /// </summary>
        abstract protected Vector GetForce(Projectile proj);

        public override void AddForce()
        {
            foreach (Projectile proj in Projectiles)
            {
                proj.AddForce(GetForce(proj));
            }
        }
    }
}
