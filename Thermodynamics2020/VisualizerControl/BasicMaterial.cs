using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VisualizerControl
{
    public class BasicMaterial : MaterialPrototype
    {
        private readonly Color color;
        private readonly bool specular;

        public BasicMaterial(Color color, bool specular = false)
        {
            this.color = color;
            this.specular = specular;
        }

        private const double specularCoefficient = 1;
        static private Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();
        static private Dictionary<Color, DiffuseMaterial> diffMaterials = new Dictionary<Color, DiffuseMaterial>();
        static private Dictionary<Color, SpecularMaterial> specMaterials = new Dictionary<Color, SpecularMaterial>();

        /// <summary>
        /// Creates a new material if one does not exist, or otherwise returns it from a dictionary
        /// </summary>
        public override Material Material
        {
            get
            {
                if (!brushes.ContainsKey(color))
                {
                    var newBrush = new SolidColorBrush(color);
                    newBrush.Freeze();
                    brushes[color] = newBrush;
                }
                var brush = brushes[color];

                if (specular)
                {
                    if (!specMaterials.ContainsKey(color))
                    {
                        var newMaterial = new SpecularMaterial(brush, specularCoefficient);
                        newMaterial.Freeze();
                        specMaterials[color] = newMaterial;
                    }
                    return specMaterials[color];
                }
                else
                {
                    if (!diffMaterials.ContainsKey(color))
                    {
                        var newMaterial = new DiffuseMaterial(brush);
                        newMaterial.Freeze();
                        diffMaterials[color] = newMaterial;
                    }
                    return diffMaterials[color];
                }
            }
        }
    }
}
