using GraphData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static GraphData.RealTimeGraphDataManager;

namespace Arena
{
    public class ArenaEngineAdapter : IVisualizerDataSource
    {
        public ArenaEngine Arena { get; }

        public RealTimeGraphDataManager Manager { get; } = new RealTimeGraphDataManager();

        private readonly Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Purple };

        public ArenaEngineAdapter(ArenaEngine arena)
        {
            Arena = arena;

            var timelines = new List<TimelineInfo>();
            int counter = 0;
            foreach (var obj in arena.UpdatingObjects)
            {
                bool notFound = true;
                foreach (var tl in timelines)
                {
                    if (tl.Timeline.Name == obj.Name)
                    {
                        notFound = false;
                        break;
                    }
                }
                if (notFound)
                {
                    var tp = new TimelinePrototype(obj.Name, colors[counter++]);
                    var ti = new TimelineInfo
                    {
                        Timeline = tp,
                        Functions = new BasicFunctionPair(() => Arena.Time, 
                        () => Arena.GetObjects(obj.Name).Count())
                    };
                    timelines.Add(ti);
                }
            }

            Manager.AddGraph(timelines, "Time (s)", "Population");
        }
        public bool IsPaused { get => Arena.IsPaused; set => Arena.IsPaused = value; }
        public CompleteTurn CurrentTurn
        {
            get
            {
                var turn = Arena.CurrentTurn;
                var graphData = Manager.GetData();
                return new CompleteTurn(turn, graphData);
            }
        }

        public double Width => Arena.Width;

        public double Height => Arena.Height;

        public CompleteTurn GetInitialTurnset()
        {
            var turn = Arena.CurrentTurn;
            return new CompleteTurn(turn, new GraphDataPacket());
        }

        public bool Update(double newTime)
        {
            return Arena.Update(newTime);
        }

        public IEnumerable<IGraphPrototype> GetGraphSetup()
        {
            return Manager.Graphs;
        }
    }
}
