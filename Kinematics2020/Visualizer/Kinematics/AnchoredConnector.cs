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
    /// A Connector that connects a fixed point to a Projectile
    /// </summary>
    class AnchoredConnector : Connector
    {
        private readonly Vector3D anchor;
        private readonly IProjectile projectile;

        public AnchoredConnector(double radius, Color color, Vector3D anchor, IProjectile projectile) :
            base(radius, color)
        {
            this.anchor = anchor;
            this.projectile = projectile;
        }

        protected override Vector3D Point1 => anchor;

        protected override Vector3D Point2 => projectile.Position;
    }
}
