using DongUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GraphData
{
    public class RealTimeGraphDataManager : IGraphDataManager
    {
        private readonly UpdatingFunctions updatingFunctions = new UpdatingFunctions();

        public GraphDataPacket GetData() => updatingFunctions.GetData();

        private List<IGraphPrototype> graphs = new List<IGraphPrototype>();
        public IEnumerable<IGraphPrototype> Graphs => graphs;

        public delegate double BasicFunction();
        public delegate List<double> ListFunction();
        public delegate Vector VectorFunc();
        public struct BasicFunctionPair
        {
            public BasicFunctionPair(BasicFunction xFunc, BasicFunction yFunc)
            {
                XFunc = xFunc;
                YFunc = yFunc;
            }
            public BasicFunction XFunc { get; set; }
            public BasicFunction YFunc { get; set; }
        }

        public void AddSingleGraph(string name, Color color, BasicFunction xFunc, BasicFunction yFunc,
            string xAxis, string yAxis)
        {
            var info = new TimelineInfo {
                Timeline = new TimelinePrototype(name, color),
                Functions = new BasicFunctionPair(xFunc, yFunc)
            };

            AddGraph(new List<TimelineInfo>() { info }, xAxis, yAxis);
        }

        public void AddHist(int nBins, Color color, ListFunction allDataFunc, string xAxis)
        {
            graphs.Add(new HistogramPrototype(nBins, color, xAxis));

            void function(GraphDataPacket ds)
            {
                ds.AddSet(allDataFunc());
            }

            updatingFunctions.AddFunction(function);
        }

        public delegate string TextFunction();
        public void AddText(string title, Color color, TextFunction textFunc)
        {
            graphs.Add(new TextPrototype(title, color));

            void function(GraphDataPacket ds)
            {
                ds.AddTextData(textFunc());
            }

            updatingFunctions.AddFunction(function);
        }

        public class TimelineInfo
        {
            public TimelinePrototype Timeline { get; set; }
            public BasicFunctionPair Functions { get; set; }
        }

        public void AddGraph(IEnumerable<TimelineInfo> timelines, string xAxis, string yAxis)
        {
            var graph = new GraphPrototype(xAxis, yAxis);
            foreach (var timeline in timelines)
            {
                graph.AddTimeline(timeline.Timeline);
            }
            graphs.Add(graph);

            void function(GraphDataPacket ds)
            {
                foreach (var timeline in timelines)
                {
                    ds.AddData(timeline.Functions.XFunc());
                    ds.AddData(timeline.Functions.YFunc());
                }
            }
            updatingFunctions.AddFunction(function);
        }

        public void Add3DGraph(string name, BasicFunction funcX, VectorFunc funcY, string xAxis, string yAxis)
        {
            var xVec = new TimelineInfo {
                Timeline = new TimelinePrototype("x " + name, Color.Red),
                Functions = new BasicFunctionPair(funcX, () => funcY().X)
            };

            var yVec = new TimelineInfo
            {
                Timeline = new TimelinePrototype("y " + name, Color.Green),
                Functions = new BasicFunctionPair(funcX, () => funcY().Y)
            };

            var zVec = new TimelineInfo
            {
                Timeline = new TimelinePrototype("z " + name, Color.Blue),
                Functions = new BasicFunctionPair(funcX, () => funcY().Z)
            };

            AddGraph(new List<TimelineInfo> { xVec, yVec, zVec }, xAxis, yAxis);

        }
    }
}
