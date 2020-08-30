using MotionVisualizerControl;
using System;
using System.Windows.Media;
using Thermodynamics;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.Thermodynamics
{
    class ThermodynamicsDriver
    {
        static internal void Run()
        {
            const double containerSize = 50;
            const double minSpeed = 1;
            const double maxSpeed = 100;
            const double mass = 4.65e-23;
            Color color = Colors.Lavender;
            const int nParticles = 1000;
            const double deltaTime = .01;
            const string name = "Molecule";
            double temperature = 293.15;

            var cont = new ParticleContainer(containerSize, containerSize, containerSize);
            var info = new ParticleInfo(name, mass, ConvertColor(color));
            //var generator = new FlatGenerator(cont, minSpeed, maxSpeed);
            var generator = new BoltzmannDistribution(cont, minSpeed, maxSpeed, temperature);

            cont.Dictionary.AddParticle(info);
            cont.AddRandomParticles(generator, name, nParticles);

            cont.Pressure = cont.Particles.Count * DongUtility.Constants.BoltzmannConstant * cont.GetTemperature() / (50*50*50);


            var visualization = new ThermodynamicsVisualization(cont)
            {
                BoxColor = Colors.IndianRed
            };

            var viz = new MotionVisualizer(visualization)
            {
                TimeIncrement = deltaTime,
                TimeScale = 1
            };

            const int histogramBins = 50;

            viz.AddSingleGraph("Pressure", Colors.CornflowerBlue, () => visualization.Time, () => cont.Pressure, "Time (s)", "Pressure (N/m^2)");
            //viz.AddText("Pressure", Colors.CadetBlue, () => cont.GetTemperature() * nParticles * DongUtility.Constants.BoltzmannConstant / (Math.Pow(containerSize,3)) );

            //viz.AddSingleGraph("Pressure vs. Temperature", Colors.CornflowerBlue, () => cont.GetTemperature(), () => cont.Pressure, "Temperature (K)", "Pressure (N/m^2)");
            viz.AddSingleGraph("Temperature vs. Time", Colors.CornflowerBlue, () => visualization.Time, () => cont.GetTemperature(), "Time (s)", "Temperature (K)");

            //viz.AddHist(histogramBins, Colors.BlueViolet, () => cont.GetParticlePropertyList((Particle part) => part.Velocity.Magnitude), "Speed (m/s)");

            //Volume vs. Temperature when n and P are constant
            viz.AddSingleGraph("Volume vs. Temperature", Colors.CornflowerBlue, () => cont.GetTemperature(), () => nParticles * DongUtility.Constants.BoltzmannConstant * cont.GetTemperature() / cont.Pressure, "Temperature (K)", "Volume (m^3)");
            //viz.AddText("Volume", Colors.CadetBlue, () => cont.GetTemperature() * nParticles * DongUtility.Constants.BoltzmannConstant / (cont.Pressure) );


            viz.AutoCamera = true;
            viz.AutoCamera = false;
            viz.Show();
        }
    }
}
