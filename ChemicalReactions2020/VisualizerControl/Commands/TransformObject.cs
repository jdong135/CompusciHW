using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Commands
{
    /// <summary>
    /// Performs a generic rotation, scale, and translation at once
    /// </summary>
    public class TransformObject : IVisualizerCommand
    {
        private readonly int objectIndex;
        private readonly Vector3D position;
        private readonly Vector3D scale;
        private readonly double azimuthalRotation;
        private readonly double polarRotation;

        /// <param name="objectIndex">The index of the object to transform</param>
        /// <param name="position">The position of the center of the object, as a Vector3D</param>
        /// <param name="scale">A Vector3D in which each component represents a scale factor in that direction</param>
        /// <param name="azimuthalRotation">Rotation counterclockwise about the z axis, in radians</param>
        /// <param name="polarRotation">Rotation from the z axis, in radians</param>
        public TransformObject(int objectIndex, Vector3D position, Vector3D scale, double azimuthalRotation, double polarRotation)
        {
            this.objectIndex = objectIndex;
            this.position = position;
            this.scale = scale;
            this.azimuthalRotation = azimuthalRotation;
            this.polarRotation = polarRotation;
        }

        public void Do(Visualizer viz)
        {
            var obj = viz.RetrieveObject(objectIndex);
            obj.Position = position;
            obj.Scale = scale;
            obj.AzimuthalAngle = azimuthalRotation;
            obj.PolarAngle = polarRotation;
        }
    }
}
