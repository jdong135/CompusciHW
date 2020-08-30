using GraphControl;
using MotionVisualizerControl;
using System.Windows.Media;
using Thermodynamics;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.ChemicalReactions
{
    class ChemicalDriver
    {
        static internal void Run()
        {
            const double containerSize = 25;

            const double deltaTime = .001;
            const double temperature = 293.17;
            const double reactionRadius = 2;

            var container = new MyReactingParticleContainer(containerSize, reactionRadius);
            container.AddRxn("Susceptible+Infected->Infected+Infected", 1, 0);
            container.AddRxn("Infected->Recovered",1,0);
            container.AddRxn("Infected->Dead", .2, 0);

            const double massPerson = 5e-25;

            Color colorSusceptible = Colors.Red;
            Color colorInfected = Colors.Orange;
            Color colorRecovered = Colors.Green;
            Color colorDead = Colors.Blue;

            container.Dictionary.AddParticle(new ParticleInfo("Susceptible", massPerson, ConvertColor(colorSusceptible)));
            container.Dictionary.AddParticle(new ParticleInfo("Infected", massPerson, ConvertColor(colorInfected)));
            container.Dictionary.AddParticle(new ParticleInfo("Recovered", massPerson, ConvertColor(colorRecovered)));
            container.Dictionary.AddParticle(new ParticleInfo("Dead", massPerson, ConvertColor(colorDead)));

            var SusceptibleGenerator = new BoltzmannGenerator(container, temperature, container.Dictionary.Map["Susceptible"]);
            var InfectedGenerator = new BoltzmannGenerator(container, temperature, container.Dictionary.Map["Infected"]);

            const int nSusceptible = 500;
            const int nInfected = 1;

            container.AddRandomParticles(SusceptibleGenerator, nSusceptible, "Susceptible");
            container.AddRandomParticles(InfectedGenerator, nInfected, "Infected");

            var visualization = new ChemicalVisualization(container)
            {
                BoxColor = Colors.IndianRed
            };

            var viz = new MotionVisualizer(visualization)
            {
                TimeIncrement = deltaTime,
                TimeScale = 1,
                SlowDraw = true
            };

            Timeline.MaximumPoints = 3000;

            var chemicalGraph = new ChemicalGraph(container, visualization);
            viz.AddGraph(chemicalGraph, chemicalGraph.Functions);

            viz.AutoCamera = true;
            viz.AutoCamera = false;
            viz.Show();
        }
        
    }
}
