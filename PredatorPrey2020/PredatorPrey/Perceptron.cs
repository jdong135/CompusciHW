using DongUtility;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class Perceptron
    {
        static public Random Random { get; } = new Random();

        public class Connector
        {
            public double Weight { get; set; }
            public Node Node { get; set; }
        }

        public class Node
        {
            public List<Connector> Connectors { get; set; } = new List<Connector>();

            private double total = 0;

            public void Reset()
            {
                total = 0;
            }

            public void AddData(double value, double weight = 1)
            {
                total += value * weight;
            }

            public void FeedForward()
            {
                Connectors.ForEach((x) => x.Node.AddData(total, x.Weight));
            }

            public double GetValue()
            {
                return total;
            }
        }

        public List<Node> InputNodes { get; } = new List<Node>();
        public List<Node> OutputNodes { get; } = new List<Node>();

        public void AddInput(int index, double value)
        {
            InputNodes[index].AddData(value);
        }

        public void Run()
        {
            InputNodes.ForEach((x) => x.FeedForward());
        }

        public void Reset()
        {
            InputNodes.ForEach((x) => x.Reset());
            OutputNodes.ForEach((x) => x.Reset());
        }

        public Perceptron Clone()
        {
            var clone = new Perceptron(InputNodes.Count, OutputNodes.Count);

            for (int inode = 0; inode < InputNodes.Count; ++inode)
            {
                for (int iconnect = 0; iconnect < InputNodes[inode].Connectors.Count; ++iconnect)
                {
                    clone.InputNodes[inode].Connectors[iconnect].Weight =
                        InputNodes[inode].Connectors[iconnect].Weight;
                }
            }

            return clone;
        }

        public Perceptron RandomClone(double standardDeviation)
        {
            var clone = Clone();

            clone.RandomWeights(standardDeviation);

            return clone;
        }

        public double GetOutput(int index)
        {
            return OutputNodes[index].GetValue();
        }

        public Perceptron(int nInputs, int nOutputs)
        {
            for (int i = 0; i < nOutputs; ++i)
            {
                OutputNodes.Add(new Node());
            }

            for (int i = 0; i < nInputs; ++i)
            {
                var node = new Node();
                foreach (var output in OutputNodes)
                {
                    node.Connectors.Add(new Connector() { Weight = 1, Node = output });
                }
                InputNodes.Add(node);
            }
        }

        public void RandomWeights(double standardDeviation)
        {
            foreach (var node in InputNodes)
            {
                foreach (var connector in node.Connectors)
                {
                    connector.Weight *= Random.NextGaussian(1, standardDeviation);
                }
            }
        }
    }
}
