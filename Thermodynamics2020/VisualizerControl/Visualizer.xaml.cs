using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace VisualizerControl
{
    /// <summary>
    /// A 3D visualizer with mouse control
    /// </summary>
    public partial class Visualizer : UserControl
    {
        /// <summary>
        /// This connects indices to stored 3D objects
        /// </summary>
        private Dictionary<int, Object3D> objects = new Dictionary<int, Object3D>();

        /// <summary>
        /// This connects indices to indices of 3D objects in Group.Children
        /// </summary>
        private Dictionary<int, int> graphicsIndices = new Dictionary<int, int>();

        public string BackgroundFile
        {
            set
            {
                Background = new ImageBrush(new BitmapImage(new Uri(@"..\..\" + value, UriKind.Relative)));
            }
        }

        public Visualizer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a particle with a user-defined index for later manipulation
        /// </summary>
        public void AddParticle(Object3D part, int index)
        {
            if (graphicsIndices.ContainsKey(index))
                throw new ArgumentException("Attempted to add index which already exists!");

            objects.Add(index, part);
            Group.Children.Add(part.GeoModel);
            graphicsIndices.Add(index, Group.Children.Count - 1);

        }

        /// <summary>
        /// Removes a particle with a given index
        /// </summary>
        public void RemoveParticle(int index)
        {
            objects.Remove(index);
            int localIndex = graphicsIndices[index];
            Group.Children.RemoveAt(localIndex);
            graphicsIndices.Remove(index);
            foreach (var key in graphicsIndices.Keys.ToList())
            {
                if (graphicsIndices[key] > localIndex)
                {
                    --graphicsIndices[key];
                }
            }
        }

        /// <summary>
        /// Retrieves an object for modification
        /// </summary>
        public Object3D RetrieveObject(int index)
        {
            return objects[index];
        }

        /// <summary>
        /// Clears all objects from the visualizer
        /// </summary>
        public void Clear()
        {
            objects.Clear();
            // Keep the first two objects, since those are the lights
            var store1 = Group.Children[0];
            var store2 = Group.Children[1];
            Group.Children.Clear();
            Group.Children.Add(store1);
            Group.Children.Add(store2);

            graphicsIndices.Clear();
        }

        /// <summary>
        /// Performs an automatic camera adjustment to keep all particles in the field of view and centered
        /// </summary>
        public void AdjustCamera()
        {
            if (objects.Count == 0)
            {
                return;
            }
            else if (objects.Count == 1)
            {
                double offset = 10; // Because why not
                Vector3D direction = new Vector3D(1, 1, 1);
                direction.Normalize();
                Vector3D pos = objects[0].Position + offset * direction;
                Camera.Position = new Point3D(pos.X, pos.Y, pos.Z);
                Camera.LookDirection = objects[0].Position - pos;
            }
            else
            {
                Vector3D centerOfParticles = Center();
                Camera.Position = new Point3D(centerOfParticles.X, centerOfParticles.Y, centerOfParticles.Z);
                Camera.LookDirection = (-centerOfParticles) / centerOfParticles.Length;

                double maxDistance = 0;

                foreach (var proj in objects.Values)
                {
                    double bestDistance = CamDistance(proj.Position);
                    if (bestDistance > maxDistance)
                        maxDistance = bestDistance;
                }

                Vector3D newPos = maxDistance / centerOfParticles.Length * centerOfParticles;
                Camera.Position = new Point3D(newPos.X, newPos.Y, newPos.Z);
            }
        }

        /// <summary>
        /// Finds the center of many particles
        /// </summary>
        private Vector3D Center()
        {
            Vector3D response = new Vector3D(0, 0, 0);

            foreach (var proj in objects.Values)
            {
                response += proj.Position;
            }
            return response /= objects.Count;
        }

        /// <summary>
        /// Checks whether a given point is within the camera's range
        /// </summary>
        private bool IsInCamera(Vector3D vec)
        {
            Vector3D fromCamera = vec - new Vector3D(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
            fromCamera.Normalize();
            Vector3D lookDirection = Camera.LookDirection;
            lookDirection.Normalize();
            double dot = Vector3D.DotProduct(fromCamera, lookDirection);
            double angleBetween = Math.Acos(dot);
            return angleBetween < Camera.FieldOfView / 2;
        }

        /// <summary>
        /// Findes the distance bewteen a given point and the camera
        /// </summary>
        private double CamDistance(Vector3D vec)
        {
            double angle = Camera.FieldOfView / 2;

            Vector3D camPosition = new Vector3D(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
            Vector3D camDirection = Camera.LookDirection - camPosition;
            double distance = DistanceLineToPoint(camPosition, camDirection, vec);

            double distanceAlongAxis = distance / Math.Tan(angle);
            double pointDistance = Vector3D.DotProduct(vec, camPosition) / camPosition.Length;
            return distanceAlongAxis + pointDistance;
        }

        /// <summary>
        /// Calculates the distance from a line to a point?
        /// I'm honestly not sure.
        /// From Wolfram Mathworld,
        /// http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
        /// </summary>
        /// <param name="lineVec1"></param>
        /// <param name="lineVec2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private double DistanceLineToPoint(Vector3D lineVec1, Vector3D lineVec2, Vector3D point)
        {
            Vector3D x0x1 = point - lineVec1;
            Vector3D x0x2 = point - lineVec2;
            Vector3D x2x1 = lineVec2 - lineVec1;

            Vector3D numVec = Vector3D.CrossProduct(x0x1, x0x2);
            Vector3D denVec = x2x1;
            return numVec.Length / denVec.Length;
        }

        private Point previousMouse;
        private Transform3D currentTransform = MatrixTransform3D.Identity;

        private void MyViewport_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            previousMouse = e.GetPosition(null);
        }

        private void MyViewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentTransform = Camera.Transform;
        }

        /// <summary>
        /// Allows rotation of the viewscreen
        /// </summary>
        private void MyViewport_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var diff = e.GetPosition(null) - previousMouse;

                double xSize = myViewport.ActualWidth;
                double ySize = myViewport.ActualHeight;

                const double totalScreen = Math.PI / 2;

                double xRot = diff.X / xSize * totalScreen;
                double yRot = diff.Y / ySize * totalScreen;

                MouseRotate(xRot, yRot);
            }
        }

        /// <summary>
        /// Rotates the view by two angles based on mouse movement
        /// </summary>
        private void MouseRotate(double leftRightAngle, double upDownAngle) // in radians
        {
            var middle = Center();
            var midpoint = new Point3D(middle.X, middle.Y, middle.Z);
            var axisLR = -Camera.UpDirection;
            var axisUD = Vector3D.CrossProduct(Camera.LookDirection, axisLR);

            var rotationLR = new RotateTransform3D(new AxisAngleRotation3D(axisLR, UtilityFunctions.RadiansToDegrees(leftRightAngle)), midpoint);
            var rotationUD = new RotateTransform3D(new AxisAngleRotation3D(axisUD, UtilityFunctions.RadiansToDegrees(upDownAngle)), midpoint);
            var rotation = new Transform3DGroup() { Children = new Transform3DCollection() { currentTransform, rotationLR, rotationUD } };

            Camera.Transform = rotation;
            Camera.LookDirection = middle - new Vector3D(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
        }

        /// <summary>
        /// Zooms based on mouse wheel
        /// </summary>
        private void MyViewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double factor = 1.1;
            double scale = e.Delta < 0 ? factor : 1 / factor;
            Camera.Position = new Point3D(Camera.Position.X * scale, Camera.Position.Y * scale, Camera.Position.Z * scale);
        }

        private double rotationSpeed = 1;

        public void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    MoveCamera(Vector3D.CrossProduct(Camera.UpDirection, Camera.LookDirection));
                    break;

                case Key.D:
                    MoveCamera(Vector3D.CrossProduct(Camera.LookDirection, Camera.UpDirection));
                    break;

                case Key.S:
                    MoveCamera(-Camera.LookDirection);
                    break;

                case Key.W:
                    MoveCamera(Camera.LookDirection);
                    break;

                case Key.LeftShift:
                    MoveCamera(-Camera.UpDirection);
                    break;

                case Key.Space:
                    MoveCamera(Camera.UpDirection);
                    break;

                case Key.Up:
                    var axis = Vector3D.CrossProduct(Camera.LookDirection, Camera.UpDirection);
                    RotateLookDirection(axis);
                    RotateUpDirection(axis);
                    break;

                case Key.Down:
                    var axis2 = Vector3D.CrossProduct(Camera.UpDirection, Camera.LookDirection);
                    RotateLookDirection(axis2);
                    RotateUpDirection(axis2);
                    break;

                case Key.Left:
                    RotateLookDirection(Camera.UpDirection);
                    break;

                case Key.Right:
                    RotateLookDirection(-Camera.UpDirection);
                    break;

                case Key.Q:
                    RotateUpDirection(Camera.LookDirection);
                    break;

                case Key.E:
                    RotateUpDirection(-Camera.LookDirection);
                    break;

                default:
                    break;
            }
        }

        private void MoveCamera(Vector3D direction)
        {
            var velocity = Camera.FieldOfView / 1000;
            var final = direction *= velocity / direction.Length;
            var position = Camera.Position;
            position += final;
            Camera.Position = position;
        }

        private void RotateLookDirection(Vector3D axis)
        {
            var rotation = new AxisAngleRotation3D(axis, rotationSpeed);
            var transform = new RotateTransform3D(rotation);
            Camera.LookDirection = transform.Transform(Camera.LookDirection);
        }

        private void RotateUpDirection(Vector3D axis)
        {
            var rotation = new AxisAngleRotation3D(axis, rotationSpeed);
            var transform = new RotateTransform3D(rotation);
            Camera.UpDirection = transform.Transform(Camera.UpDirection);
        }
    }

}
