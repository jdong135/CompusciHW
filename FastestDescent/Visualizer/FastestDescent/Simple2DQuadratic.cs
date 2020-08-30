using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;
using static DongUtility.UtilityFunctions;

namespace Visualizer.FastestDescent
{
    public class Simple2DQuadratic : Path
    {
        private double[] parameters;

        public Simple2DQuadratic(params double[] parameters)
        {
            this.parameters = parameters;
        }
        public override double InitialParameter => -10;

        public override double FinalParameter => 0;

        protected override Vector Function(double parameter)
        {
            double z = parameters[0] + parameters[1] * parameter + parameters[2] * Square(parameter);
            return new Vector(parameter, 0, z);
        }
    }
}
