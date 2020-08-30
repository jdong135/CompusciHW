using DongUtility;
using PhysicsUtility;

namespace FiniteElement
{
    /// <summary>
    /// A constant gravitational force applied to all objects
    /// </summary>
    public class ConstantGravitationForce : GlobalForce
    {
        private Vector fieldStrength;

        public ConstantGravitationForce(KinematicsEngine engine, Vector fieldStrength) :
            base(engine)
        {
            this.fieldStrength = fieldStrength;
        }

        protected override Vector GetForce(Projectile proj)
        {
            return proj.Mass * fieldStrength;
        }
    }
}
