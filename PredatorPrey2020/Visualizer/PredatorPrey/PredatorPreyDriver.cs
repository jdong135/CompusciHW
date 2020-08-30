using MotionVisualizerControl;
using PredatorPrey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer.PredatorPrey
{
    static class PredatorPreyDriver
    {
        private const double width = 50;
        private const double height = 50;
        private const double timeStep = .01;
        private const double maxTime = 1000;

        static private Perceptron HarePerceptron { get; set; }
        static private MultilayerPerceptron LynxPerceptron { get; set; }

        static private Arena PrepareArenaHare(Perceptron perceptron)
        {
            var arena = new Arena(width, height);
            var hare = new Hare(arena);
            var lynx = new Lynx(arena);

            hare.Other = lynx;
            lynx.Other = hare;

            arena.AddAnimal(hare);
            arena.AddAnimal(lynx);

            hare.Perceptron = perceptron;

            return arena;
        }
        static private Arena PrepareArenaLynx(MultilayerPerceptron perceptron)
        {
            var arena = new Arena(width, height);
            var hare = new Hare(arena);
            var lynx = new Lynx(arena);

            hare.Other = lynx;
            lynx.Other = hare;

            arena.AddAnimal(hare);
            arena.AddAnimal(lynx);

            hare.Perceptron = HarePerceptron;
            lynx.Perceptron = perceptron;

            return arena;
        }

        static private Arena PrepareArenaFinal()
        {
            var arena = new Arena(width, height);
            var hare = new Hare(arena);
            var lynx = new Lynx(arena);

            hare.Other = lynx;
            lynx.Other = hare;

            arena.AddAnimal(hare);
            arena.AddAnimal(lynx);

            hare.Perceptron = HarePerceptron;
            lynx.Perceptron = LynxPerceptron;

            return arena;
        }

        static public double RunOnce(Arena arena)
        {
            bool keepRunning = true;

            while (keepRunning && arena.Time < maxTime)
            {
                keepRunning = arena.Update(arena.Time + timeStep);
            }

            return arena.Time;
        }

        static private Perceptron TrainHare()
        {
            Dictionary<Perceptron, double> perceptronList = new Dictionary<Perceptron, double>();
            for(int i = 0; i<1000; i++)
            {
                var perceptron = new Perceptron(4, 2);
                perceptron.RandomWeights(50);
                perceptronList.Add(perceptron, RunOnce(PrepareArenaHare(perceptron)));
            }

            //Get the 10 fastest perceptrons in the first generation
            Dictionary<Perceptron, double> tenFastestPerceptrons = perceptronList.OrderByDescending(pair => pair.Value)
                                                                    .Take(10)
                                                                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            double bestTime = 0;
            var bestPerceptron = new Perceptron(4,2); // Pick a random perceptron

            //Find the longest time (longest survival)
            foreach (KeyValuePair<Perceptron, double> entry in tenFastestPerceptrons)
            {
                if(entry.Value > bestTime)
                {
                    bestTime = entry.Value;
                    bestPerceptron = entry.Key;
                }
            }
            Debug.WriteLine("1\t" + bestTime);

            //Continue making new generations until the time doesn't increase
            int genNumber = 2;
            bool timeIncreased = false;
            Dictionary<Perceptron, double> newGenerationPerceptronList = new Dictionary<Perceptron, double>();
            do
            {
                timeIncreased = false;
                //Empty the dictionary
                newGenerationPerceptronList.Clear();
                //Create 100 perceptrons based on each of the 10 fastest perceptrons
                foreach (KeyValuePair<Perceptron, double> entry in tenFastestPerceptrons)
                {
                    for(int i=0; i<100; i++)
                    {
                        var perceptron = entry.Key.RandomClone(1);
                        var perceptronTime = RunOnce(PrepareArenaHare(perceptron));
                        newGenerationPerceptronList.Add(perceptron, perceptronTime);
                    }
                }
                //Find the 10 longest times in the new generation list
                tenFastestPerceptrons = newGenerationPerceptronList.OrderByDescending(pair => pair.Value)
                                                                    .Take(10)
                                                                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                //Find the best time in the ten fastest(longest survival)
                foreach (KeyValuePair<Perceptron, double> entry in tenFastestPerceptrons)
                {
                    if(entry.Value > bestTime)
                    {
                        bestTime = entry.Value;
                        bestPerceptron = entry.Key;
                        timeIncreased = true;
                    }
                }
                Debug.WriteLine(genNumber+"\t"+bestTime);
                genNumber += 1;
            }
            while (timeIncreased == true);



            //Dr. Dong's code
            //var perceptron = new Perceptron(4, 2);
            //// Random starting point
            //perceptron.RandomWeights(50);
            HarePerceptron = bestPerceptron;
            return bestPerceptron;
        }

        static private MultilayerPerceptron TrainLynx()
        {
            Dictionary<MultilayerPerceptron, double> perceptronList = new Dictionary<MultilayerPerceptron, double>();
            for (int i = 0; i < 1000; i++)
            {
                var perceptron = new MultilayerPerceptron(4, 4, 2);
                perceptron.RandomWeights(50);
                perceptronList.Add(perceptron, RunOnce(PrepareArenaLynx(perceptron)));
            }

            //Get the 10 fastest perceptrons in the first generation
            Dictionary<MultilayerPerceptron, double> tenFastestPerceptrons = perceptronList.OrderBy(pair => pair.Value)
                                                                    .Take(10)
                                                                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            double bestTime = double.MaxValue;
            var bestPerceptron = new MultilayerPerceptron(4, 4, 2); // Pick a random perceptron

            //Find the longest time (longest survival)
            foreach (KeyValuePair<MultilayerPerceptron, double> entry in tenFastestPerceptrons)
            {
                if (entry.Value < bestTime)
                {
                    bestTime = entry.Value;
                    bestPerceptron = entry.Key;
                }
            }
            Debug.WriteLine("1\t" + bestTime);

            //Continue making new generations until the time doesn't increase
            int genNumber = 2;
            bool timeDecreased = false;
            Dictionary<MultilayerPerceptron, double> newGenerationPerceptronList = new Dictionary<MultilayerPerceptron, double>();
            do
            {
                timeDecreased = false;
                //Empty the dictionary
                newGenerationPerceptronList.Clear();
                //Create 100 perceptrons based on each of the 10 fastest perceptrons
                foreach (KeyValuePair<MultilayerPerceptron, double> entry in tenFastestPerceptrons)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var perceptron = entry.Key.RandomClone(1);
                        var perceptronTime = RunOnce(PrepareArenaLynx(perceptron));
                        newGenerationPerceptronList.Add(perceptron, perceptronTime);
                    }
                }
                //Find the 10 longest times in the new generation list
                tenFastestPerceptrons = newGenerationPerceptronList.OrderBy(pair => pair.Value)
                                                                    .Take(10)
                                                                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                //Find the best time in the ten fastest(longest survival)
                foreach (KeyValuePair<MultilayerPerceptron, double> entry in tenFastestPerceptrons)
                {
                    if (entry.Value < bestTime)
                    {
                        bestTime = entry.Value;
                        bestPerceptron = entry.Key;
                        timeDecreased = true;
                    }
                }
                Debug.WriteLine(genNumber + "\t" + bestTime);
                genNumber += 1;
            }
            while (timeDecreased == true);



            //Dr. Dong's code
            //var perceptron = new Perceptron(4, 2);
            //// Random starting point
            //perceptron.RandomWeights(50);
            LynxPerceptron = bestPerceptron;
            return bestPerceptron;
        }

        static public void Run()
        {
            var harePerceptron = TrainHare();
            var lynxPerceptron = TrainLynx();

            //var arena = PrepareArena(perceptron);
            var arena = PrepareArenaFinal();

            var visualization = new PredatorPreyVisualization(arena);
            var visualizer = new MotionVisualizer(visualization);

            visualizer.Show();
        }
    }
}
