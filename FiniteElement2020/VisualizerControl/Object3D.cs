﻿using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl
{
    /// <summary>
    /// A Class containing geometry, material, and transform - everything you need to draw in 3D
    /// </summary>
    public class Object3D
    {
        private readonly Shape3D shape;

        /// <summary>
        /// The geometry of the object
        /// </summary>
        public GeometryModel3D GeoModel { get; private set; }

        private TranslateTransform3D translation = new TranslateTransform3D();
        private ScaleTransform3D scale = new ScaleTransform3D();
        private AxisAngleRotation3D thetaRotation = new AxisAngleRotation3D();
        private AxisAngleRotation3D phiRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);

        /// <param name="shape">The shape (geometry) of the object</param>
        /// <param name="material">The material of the object</param>
        public Object3D(Shape3D shape, Material material)
        {
            this.shape = shape;

            // Create transformation
            var transGroup = new Transform3DGroup();
            transGroup.Children.Add(scale);
            transGroup.Children.Add(new RotateTransform3D(phiRotation));
            transGroup.Children.Add(new RotateTransform3D(thetaRotation));
            transGroup.Children.Add(translation);

            // Create geometry model
            GeoModel = new GeometryModel3D(shape.Mesh, material)
            {
                Transform = transGroup
            };

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Object3D(Object3D other) :
            this(other.shape, other.GeoModel.Material)
        {
            Position = other.Position;
            Scale = other.Scale;
            AzimuthalAngle = other.AzimuthalAngle;
            PolarAngle = other.PolarAngle;
        }

        public Object3D(ObjectPrototype proto) : 
            this(proto.Shape, proto.MaterialPrototype.Material)
        {
            Position = proto.Position;
            Scale = proto.Scale;
            AzimuthalAngle = proto.AzimuthalAngle;
            PolarAngle = proto.PolarAngle;
        }

        /// <summary>
        /// The object's current position
        /// </summary>
        public Vector3D Position
        {
            get
            {
                return new Vector3D(translation.OffsetX, translation.OffsetY, translation.OffsetZ);
            }
            set
            {
                translation.OffsetX = value.X;
                translation.OffsetY = value.Y;
                translation.OffsetZ = value.Z;
            }
        }

        /// <summary>
        /// The object's current scale, as a vector
        /// Each component represents the scale (relative to 1) of that axis.
        /// Most often all components are the same, but it is not a requirement.
        /// </summary>
        public Vector3D Scale
        {
            get
            {
                return new Vector3D(scale.ScaleX, scale.ScaleY, scale.ScaleZ);
            }
            set
            {
                scale.ScaleX = value.X;
                scale.ScaleY = value.Y;
                scale.ScaleZ = value.Z;
            }
        }

        /// <summary>
        /// Sets the scale to a given factor, scaled evenly in all directions
        /// </summary>
        public void ScaleEvenly(double scale)
        {
            Scale = new Vector3D(scale, scale, scale);
        }

        /// <summary>
        /// The rotation of the object, right-handed around the z axis, in radians
        /// (phi in physics, theta in math)
        /// </summary>
        public double AzimuthalAngle
        {
            get
            {
                return phiRotation.Angle;
            }
            set
            {
                phiRotation.Angle = UtilityFunctions.RadiansToDegrees(value);
                thetaRotation.Axis = new Vector3D(-Math.Sin(value), Math.Cos(value), 0);
            }
        }

        /// <summary>
        /// The angle of declination from the positive z axis, in radians
        /// (theta in physics, phi in math)
        /// </summary>
        public double PolarAngle
        {
            get
            {
                return thetaRotation.Angle;
            }
            set
            {
                thetaRotation.Angle = UtilityFunctions.RadiansToDegrees(value);
            }
        }




    }
}
