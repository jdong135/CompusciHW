using System.Collections.Generic;
using GraphControl;
using Thermodynamics;
using static MotionVisualizerControl.MotionVisualizer;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.ChemicalReactions
{
    class ChemicalGraph : GraphUnderlying
    {
        public ChemicalGraph(ParticleContainer container, ChemicalVisualization viz) :
            base("Time (s)", "Number of particles")
        {
            foreach (var info in container.Dictionary.Map.Values)
            {
                AddTimeline(container, info, viz);
            }
        }

        public ChemicalGraph(ParticleContainer container, List<string> particleNames, ChemicalVisualization viz) :
            base("Time (s)", "Number of particles")
        {
            foreach (var part in particleNames)
            {
                AddTimeline(container, container.Dictionary.Map[part], viz);
            }
        }

        public List<BasicFunctionPair> Functions { get; } = new List<BasicFunctionPair>(); 
        private void AddTimeline(ParticleContainer container, ParticleInfo info, ChemicalVisualization viz)
        {
            Timeline tl = new Timeline(info.Name, ConvertColor(info.Color));
            Functions.Add(new BasicFunctionPair(() => viz.Time, () => container.GetNParticles(info.Name)));
            AddTimeline(tl);
        }
    }
}
