using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    abstract public class BasicVisualization : IVisualization
    {
        protected Visualizer Viz { get; private set; }

        public void SetVisualizer(Visualizer viz)
        {
            Viz = viz;
        }

        public void AddGround(double side, Vector3D center, string filename)
        {
            var obj = new StaticObject(new Square3D(), ImageMaterialLibrary.GetMaterial(filename))
            {
                Scale = new Vector3D(side / 2, side / 2, 1),
                Position = center
            };

            AddObject(obj);
        }

        public void AddBox(double side, Vector3D center, Color color, bool isSpecular)
        {
            const double specularCoefficient = 1;

            var brush = new SolidColorBrush(color);
            Material material;
            if (isSpecular)
                material = new SpecularMaterial(brush, specularCoefficient);
            else
                material = new DiffuseMaterial(brush);

            var obj = new StaticObject(new Cube3D(), material)
            {
                Scale = new Vector3D(side / 2, side / 2, side / 2),
                Position = center
            };
            AddObject(obj);
        }

        static private Point3D ConvertToPoint3D(Vector3D vec)
        {
            return new Point3D(vec.X, vec.Y, vec.Z);
        }

        abstract public void Initialize();

        private List<Object3D> list = new List<Object3D>();
        public List<Object3D> GetObjects()
        {
            return list;
        }

        protected void AddObject(Object3D obj)
        {
            list.Add(obj);
        }

        public virtual bool Tick(double newTime)
        {
            foreach (var obj in list)
            {
                obj.Update();
            }

            return true;
        }
    }
}
