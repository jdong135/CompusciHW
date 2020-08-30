using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Visualizations
{
    public class BridgeVisualization : BasicVisualization
    {
        private MeshGeometry3D mesh = new MeshGeometry3D();
        private IEngineWithConnectors engine;
        private Color color;

        private class Graph
        {
            private class Node
            {
                public Node(int index, Vector3D position)
                {
                    ThisIndex = index;
                    Position = position;
                }

                public int ThisIndex { get; set; }
                public List<int> ConnectedNodes { get; set; } = new List<int>();
                public Vector3D Position { get; set; }
            }

            private List<Node> nodes = new List<Node>();

            public Graph(IList<IProjectile> projectiles, IList<Tuple<int, int>> connectors)
            {
                for (int i = 0; i < projectiles.Count; ++i)
                {
                    nodes.Add(new Node(i, projectiles[i].Position));
                }

                foreach (var connector in connectors)
                {
                    nodes[connector.Item1].ConnectedNodes.Add(connector.Item2);
                    nodes[connector.Item2].ConnectedNodes.Add(connector.Item1);
                }
            }

            public Point3DCollection GetPoints()
            {
                var list = new Point3DCollection();
                foreach (var node in nodes)
                {
                    list.Add(new Point3D(node.Position.X, node.Position.Y, node.Position.Z));
                }

                return list;
            }

            public Int32Collection GetTriangles()
            {
                var list = new Int32Collection();

                foreach (var node in nodes)
                {
                    foreach (var con1 in node.ConnectedNodes)
                    {
                        foreach (var con2 in nodes[con1].ConnectedNodes)
                        {
                            if (con2 != node.ThisIndex && con1 < con2)
                            {
                                list.Add(node.ThisIndex);

                                var vec1 = nodes[con1].Position - node.Position;
                                var vec2 = nodes[con2].Position - nodes[con1].Position;
                                var cross = Vector3D.CrossProduct(vec1, vec2);
                                bool wind = cross.Z > 0;
                                if (cross.Z == 0)
                                {
                                    wind = cross.Y > 0;
                                    if (cross.Y == 0)
                                    {
                                        wind = cross.X > 0;
                                    }
                                }
                                if (wind)
                                {
                                    list.Add(con1);
                                    list.Add(con2);
                                }
                                else
                                {
                                    list.Add(con2);
                                    list.Add(con1);
                                }
                            }
                        }
                    }
                }

                return list;
            }
        }

        public BridgeVisualization(IEngineWithConnectors engine, Color color)
        {
            this.engine = engine;
            this.color = color;

            foreach (var proj in engine.GetProjectiles())
            {
                AddObject(new SingleParticle(proj));
            }
        }

        public override void Initialize()
        {
            createObject();
            var material = new DiffuseMaterial(new SolidColorBrush(color));
            //var material = new SpecularMaterial(new SolidColorBrush(color), 1);
            //var material = new MaterialGroup();
            //material.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            //material.Children.Add(new SpecularMaterial(new SolidColorBrush(color), 1));
            //material.Children.Add(new EmissiveMaterial(new SolidColorBrush(color)));
            var geo = new GeometryModel3D(mesh, material);
            geo.BackMaterial = material;
            // collection.Add(geo);
        }

        private void createObject()
        {
            var graph = new Graph(engine.GetProjectiles(), engine.GetConnectors());
            mesh.Positions = graph.GetPoints();
            mesh.TriangleIndices = graph.GetTriangles();
        }

        private bool fixNextTime = false;

        public override bool Tick(double newTime)
        {
            engine.Tick(newTime);

            if (fixNextTime)
            {
                createObject();
                fixNextTime = false;
            }

            if (engine.ConnectorsToRemove().Count > 0)
            {
                fixNextTime = true;
            }

            var list = engine.GetProjectiles();
            for (int i = 0; i < list.Count; ++i)
            {
                var pos = list[i].Position;
                mesh.Positions[i] = new Point3D(pos.X, pos.Y, pos.Z);
            }

            return true;
        }
    }
}
