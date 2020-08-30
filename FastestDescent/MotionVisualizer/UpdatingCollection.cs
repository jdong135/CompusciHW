using GraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionVisualizerControl
{
    public class UpdatingCollection
    {
        private List<UpdatingContainer> containers = new List<UpdatingContainer>();

        private Queue<DataSource> dataPoints = new Queue<DataSource>();

        public void AddContainer(UpdatingContainer cont)
        {
            containers.Add(cont);
        }

        public IEnumerable<UpdatingContainer.GetFunction> Functions
        {
            get
            {
                foreach (var cont in containers)
                {
                    yield return cont.Function;
                }
            }
        }

        public void AddData(DataSource point)
        {
            dataPoints.Enqueue(point);
        }

        public void Update()
        {
            var data = dataPoints.Dequeue();
            foreach (var cont in containers)
            {
                cont.Update(data);
            }
        }
    }
}
