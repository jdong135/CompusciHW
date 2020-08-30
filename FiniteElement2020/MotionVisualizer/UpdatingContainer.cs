using GraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionVisualizerControl
{
    public class UpdatingContainer
    {
        public delegate void GetFunction(DataSource data);

        private readonly IUpdating updatingObject;
        public GetFunction Function { get; set; }

        public UpdatingContainer(IUpdating updatingObject, GetFunction function)
        {
            this.updatingObject = updatingObject;
            Function = function;
        }

        public void GetData(DataSource data)
        {
            Function(data);
        }

        public void Update(DataSource data)
        {
            updatingObject.Update(data);
        }
    }
}
