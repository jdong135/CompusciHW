using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphData
{
    public interface IGraphDataManager
    {
        public GraphDataPacket GetData();
        public IEnumerable<IGraphPrototype> Graphs { get; }
    }
}
