using System;
using System.Collections.Generic;
using System.Text;

namespace GraphData
{
    public interface IGraphPrototype
    {
        public enum GraphType { Graph = 0, Histogram = 1, Text = 2 }

        public GraphType GetGraphType();
    }
}
