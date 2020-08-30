using GraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionVisualizerControl
{
    public class UpdatingFunctions
    {
        private List<UpdatingContainer.GetFunction> functions = new List<UpdatingContainer.GetFunction>();

        public void AddFunction(UpdatingContainer.GetFunction function)
        {
            functions.Add(function);
        }

        public DataSource GetData()
        {
            var data = new DataSource();
            foreach (var func in functions)
            {
                func(data);
            }
            return data;
        }
    }
}
