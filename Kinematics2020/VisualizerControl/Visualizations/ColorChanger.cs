using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    class ColorChanger : Object3D
    {
        private IProjectile projectile;

        public static bool Specular { get; set; } = true;

        public ColorChanger(IProjectile projectile, Shape3D shape) :
            base(shape, projectile.Color, Specular)
        {
            this.projectile = projectile;
            Position = projectile.Position;
        }

        public override void Update()
        {
            ChangeColor(projectile.Color, Specular);
        }
    }
}
