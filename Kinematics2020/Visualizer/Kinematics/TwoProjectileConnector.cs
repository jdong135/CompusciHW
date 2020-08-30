using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Visualizer.Kinematics
{
    /// <summary>
    /// A Connector that connects two projectiles
    /// </summary>
    class TwoProjectileConnector : Connector
    {
        private readonly IProjectile projectile1;
        private readonly IProjectile projectile2;

        public TwoProjectileConnector(double radius, Color color, IProjectile proj1, IProjectile proj2) :
            base(radius, color)
        {
            projectile1 = proj1;
            projectile2 = proj2;
        }

        protected override Vector3D Point1 => projectile1.Position;

        protected override Vector3D Point2 => projectile2.Position;
    }
}
