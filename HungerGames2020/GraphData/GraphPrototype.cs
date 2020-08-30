using System;
using System.Collections.Generic;
using System.Text;

namespace GraphData
{
    public class GraphPrototype : IGraphPrototype
    {
        public string XAxisTitle { get; }
        public string YAxisTitle { get; }
        public List<TimelinePrototype> Timelines { get; } = new List<TimelinePrototype>();

        public GraphPrototype(string xAxisTitle, string yAxisTitle)
        {
            XAxisTitle = xAxisTitle;
            YAxisTitle = yAxisTitle;
        }

        public void AddTimeline(TimelinePrototype timeline)
        {
            Timelines.Add(timeline);
        }

        IGraphPrototype.GraphType IGraphPrototype.GetGraphType()
        {
            return IGraphPrototype.GraphType.Graph;
        }
    }
}
