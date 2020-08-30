using DongUtility;
using PhysicsUtility;

namespace FiniteElement
{
    /// <summary>
    /// A spring force for which the other end of the spring is connected to another projectile
    /// Note that this needs to be set on both projectiles for Newton's third law to work properly
    /// </summary>
    public class ProjectileBoundSpringForce : SpringForce
    {
        private readonly Projectile other;

        public ProjectileBoundSpringForce(Projectile projectile1, Projectile projectile2, double springConstant,
            double unstretchedLength = 0) :
            base (projectile1, springConstant, unstretchedLength)
        {
            other = projectile2;
        }

        protected override Vector SpringPosition()
        {
            return other.Position;
        }
    }
}
