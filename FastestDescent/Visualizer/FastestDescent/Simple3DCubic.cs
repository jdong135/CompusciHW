using DongUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static DongUtility.UtilityFunctions;

namespace Visualizer.FastestDescent
{
    class Simple3DCubic : Path
    {
        private double[] parameters;

        public Simple3DCubic(params double[] parameters)
        {
            this.parameters = parameters;
        }
        public override double InitialParameter => -16.326901661786017;

        public override double FinalParameter => 0;
        protected override Vector Function(double parameter)
        {
            Rotation rotate = new Rotation();
            Vector axisRotation = Vector.Cross(new Vector(-3, 1, -2), new Vector(0, 1, 0)); // Cross of normal of plane and xz plane
            double angleBetweenPlanes = Vector.AngleBetween(new Vector(-3, 1, -2), new Vector(0, 1, 0));
            rotate.RotateArbitraryAxis(axisRotation, -angleBetweenPlanes);
            double z = parameters[0] + parameters[1] * parameter + parameters[2] * Square(parameter) + 
                parameters[3] * Square(parameter) * parameter;
            Vector rotateThisVector = new Vector(parameter, 0, z);
            Vector returnVector = rotate.ApplyRotation(rotateThisVector);
            return returnVector;
        }
    }
}
