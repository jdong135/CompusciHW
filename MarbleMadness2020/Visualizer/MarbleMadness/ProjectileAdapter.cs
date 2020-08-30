using FiniteElement;
using PhysicsUtility;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl;
using VisualizerControl.Shapes;

namespace Visualizer.MarbleMadness
{
    /// <summary>
    /// An adapter class to get Projectile to be an IProjectile
    /// </summary>
    class ProjectileAdapter : IProjectile
    {
        private Projectile projectile;

        public ProjectileAdapter(Projectile projectile)
        {
            this.projectile = projectile;
        }

        public Vector3D Position => new Vector3D(projectile.Position.X, projectile.Position.Y, projectile.Position.Z);

        public Color Color => Colors.Red;

        public Shape3D Shape => new Sphere3D();

        static public double VisualSize { get; set; } = 1;
        public double Size => VisualSize;
    }
}
