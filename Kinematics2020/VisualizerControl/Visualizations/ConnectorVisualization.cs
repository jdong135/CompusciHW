using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    public class ConnectorVisualization : SingleParticleVisualization
    {
        private IEngineWithConnectors MyEngine { get { return (IEngineWithConnectors)Engine; } }

        private List<Connector> connectors = new List<Connector>();

        public ConnectorVisualization(IEngineWithConnectors engine) :
            base(engine)
        { }

        public override void Initialize()
        {
            base.Initialize();

            Cylinder3D cylinder = new Cylinder3D();

            foreach (var connector in MyEngine.GetConnectors())
            {
                var newCon = new Connector(Objects[connector.Item1], Objects[connector.Item2]);
                connectors.Add(newCon);
                AddObject(newCon);
            }
        }

        public override bool Tick(double newTime)
        {
            bool result = base.Tick(newTime);

            foreach (var connector in MyEngine.ConnectorsToRemove())
            {
                // Remove connectors - not yet implemented
            }

            return result;
        }

    }
}
