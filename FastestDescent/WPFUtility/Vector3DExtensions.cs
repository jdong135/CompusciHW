﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace WPFUtility
{
    static public class Vector3DExtensions
    {
        /// <summary>
        /// The azimuthal angle is the angle from the x axis in the x-y plane (phi in physics, theta in math)
        /// </summary>
        static public double Azimuthal(this Vector3D vector)
        {
            double answer = Math.Atan2(vector.Y, vector.X);
            if (answer < 0)
                answer += 2 * Math.PI;
            return answer;
        }

        static public double Polar(this Vector3D vector)
        {
            return Math.Acos(vector.Z / vector.Length);
        }
    }
}
