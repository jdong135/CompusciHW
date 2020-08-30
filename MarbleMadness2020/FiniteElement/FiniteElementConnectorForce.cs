using PhysicsUtility;

namespace FiniteElement
{
    public class FiniteElementConnectorForce : ProjectileBoundSpringForce
    {
        public FiniteElementConnectorForce(Projectile projectile1, Projectile projectile2, double springConstant, double unstretchedLength) :
            base(projectile1, projectile2, springConstant, unstretchedLength)
        { }
    }
}
