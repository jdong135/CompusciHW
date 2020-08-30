using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Shapes
{
    /// <summary>
    /// An abstract base class for shapes
    /// </summary>
    abstract public class Shape3D
    {
        /// <summary>
        /// The mesh for the class
        /// </summary>
        internal MeshGeometry3D Mesh
        {
            get
            {
                // If it is already in the dictionary, don't generate a new one
                if (shapeName != "" && meshes.ContainsKey(shapeName))
                {
                    return meshes[shapeName];
                }
                else
                {
                    var mesh = MakeMesh();
                    if (freezeMesh)
                    {
                        mesh.Freeze();
                    }
                    meshes[shapeName] = mesh;

                    return mesh;
                }
            }
        }

        /// <summary>
        /// A static dictionary holding many meshes.
        /// This way, only one mesh is created per unique shape.
        /// </summary>
        private static ConcurrentDictionary<string, MeshGeometry3D> meshes = new ConcurrentDictionary<string, MeshGeometry3D>();

        /// <summary>
        /// Protected constructor to create unique shapes
        /// </summary>
        /// <param name="shapeName">Name of the shape</param>
        /// <param name="freezeMesh">Whether the mesh can safely be frozen for performance reasons</param>
        protected Shape3D(string shapeName, bool freezeMesh = true)
        {
            this.shapeName = shapeName;
            this.freezeMesh = freezeMesh;
        }

        public static Shape3D CreateShape(DongUtility.Shapes.Shapes3D shape, bool freezeMesh = true)
        {
            switch (shape)
            {
                case DongUtility.Shapes.Shapes3D.Cube:
                    return new Cube3D();

                case DongUtility.Shapes.Shapes3D.Sphere:
                    return new Sphere3D();

                case DongUtility.Shapes.Shapes3D.Cylinder:
                    return new Cylinder3D();

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The name of the shape, used to avoid duplicate meshes
        /// </summary>
        private readonly string shapeName;
        /// <summary>
        /// Whether or not to freeze the mesh
        /// </summary>
        private readonly bool freezeMesh;

        /// <summary>
        /// A class to hold vertex information
        /// </summary>
        public class Vertex
        {
            internal Vertex(Point3D position, Vector3D normal, Point textureCoordinate)
            {
                Position = position;
                Normal = normal;
                TextureCoordinate = textureCoordinate;
            }

            /// <summary>
            /// The position of the vertex
            /// </summary>
            public Point3D Position { get; set; }
            /// <summary>
            /// The normal vector at the vertex
            /// </summary>
            public Vector3D Normal { get; set; }
            /// <summary>
            /// The (u,v) coordinate of this point
            /// </summary>
            public Point TextureCoordinate { get; set; }
        }

        /// <summary>
        /// Create the actual mesh
        /// </summary>
        private MeshGeometry3D MakeMesh()
        {
            var mesh = new MeshGeometry3D();

            var vertices = MakeVertices();

            mesh.Positions = new Point3DCollection(vertices.Count);
            mesh.Normals = new Vector3DCollection(vertices.Count);
            mesh.TextureCoordinates = new PointCollection(vertices.Count);
            mesh.TriangleIndices = MakeTriangles();

            foreach (var vertex in vertices)
            {
                mesh.Positions.Add(vertex.Position);
                mesh.Normals.Add(vertex.Normal);
                mesh.TextureCoordinates.Add(vertex.TextureCoordinate);
            }

            return mesh;
        }

        /// <summary>
        /// Create the vertices
        /// </summary>
        /// <returns>A list of vertices</returns>
        abstract protected List<Vertex> MakeVertices();
        /// <summary>
        /// Create triangle mapping
        /// </summary>
        /// <returns>A list of indices that come in sets of three, matching the results of MakeVertices()</returns>
        abstract protected Int32Collection MakeTriangles();
    }
}
