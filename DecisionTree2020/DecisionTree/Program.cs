using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class Program
    {
        private static readonly string path = @"C:\Users\Jay Dong\Desktop\School 2019-20\Computational Science\CompuSci Spring 2020\";
        static void Level1(DataSet signal, DataSet background, DataSet data)
        {
            double min = double.MaxValue;
            double max = 0;

            int bestVariable = 0;
            double bestPurityDif = 0;
            double bestPurityAbove = 0;
            double bestPurityUnder = 0;
            double bestCutoff = 0;

            for (int i = 0; i < signal.Names.Length; i++)
            {
                foreach (DataPoint dp in data.Points)
                {
                    if (dp.Variables[i] < min)
                    {
                        min = dp.Variables[i];
                    }
                    if (dp.Variables[i] > max)
                    {
                        max = dp.Variables[i];
                    }
                }
                for (double cutoff = min; cutoff < max; cutoff++)
                {
                    int signalUnder = 0;
                    int signalAbove = 0;
                    int backgroundUnder = 0;
                    int backgroundAbove = 0;
                    foreach (DataPoint dp in signal.Points)
                    {
                        if (dp.Variables[i] < cutoff)
                        {
                            signalUnder += 1;
                        }
                        else
                        {
                            signalAbove += 1;
                        }
                    }
                    foreach (DataPoint dp in background.Points)
                    {
                        if (dp.Variables[i] < cutoff)
                        {
                            backgroundUnder += 1;
                        }
                        else
                        {
                            backgroundAbove += 1;
                        }
                    }

                    double PurityAbove = (double)signalAbove / (signalAbove + backgroundAbove);
                    double PurityUnder = (double)signalUnder / (signalUnder + backgroundUnder);
                    //I didn't want the purity to be 0 or 1 because that is overfitting
                    if (PurityAbove < .95 && PurityAbove > .05 && PurityUnder < .95 && PurityUnder > .05)
                    {
                        if (Math.Abs(PurityAbove - PurityUnder) > bestPurityDif)
                        {
                            bestCutoff = cutoff;
                            bestVariable = i;
                            bestPurityDif = Math.Abs(PurityAbove - PurityUnder);
                            bestPurityAbove = PurityAbove;
                            bestPurityUnder = PurityUnder;
                        }

                    }
                }
            }
            using var file = File.CreateText(path + "decisionTreeResults.txt");
            file.WriteLine("Event\tPurity");
            for (int i = 0; i < data.Points.Count; i++)
            {
                if (data.Points[i].Variables[bestVariable] < bestCutoff)
                {
                    file.WriteLine((i + 1).ToString() + "\t" + 1);
                }
                else
                {
                    file.WriteLine((i + 1).ToString() + "\t" + 0);
                }
            }
            //Debug.WriteLine("Best cutoff: " + bestCutoff);
            //Debug.WriteLine("Best var index: " + bestVariable);
            //Debug.WriteLine("Best purity above: " + bestPurityAbove);
            //Debug.WriteLine("Best purity under: " + bestPurityUnder);
        }
        static void Main()
        {
            // Load training samples
            var background = DataSet.ReadDataSet(path + "backgroundOverallTrainingSample.dat");
            var signal = DataSet.ReadDataSet(path + "signalOverallTrainingSample.dat");

            // Load data sample
            //var data = DataSet.ReadDataSet(path + "project3Data.dat");
            var data = DataSet.ReadDataSet(path + "project3Data.dat");

            int numTrees = 30;
            Forest forest = new Forest(numTrees);
            forest.Train(signal, background);
            List<double> outputs = new List<double>();
            for (int i = 0; i<data.Points.Count; i++)
            {
                double output = forest.RunDataPoint(data.Points[i]);
                outputs.Add(output);
                //Need to normalize datapoint on new scale
            }
            //double normalizationFactor = outputs.Max();
            double normalizationFactor = 1;
            //It seems like numAbove is background and numBelow is signal
            int numAbove = 0;
            int numBelow = 0;
            using var file = File.CreateText(path + "JayAyanSignals.txt");
            int count = 0;
            foreach (double d in outputs)
            {
                Console.WriteLine(d / normalizationFactor);
                if (d / normalizationFactor >= numTrees)
                {
                    numAbove += 1;
                    file.WriteLine(count);
                }
                else
                {
                    numBelow += 1;
                }
                count += 1;
            }
            Console.WriteLine("num above: " + numAbove);
            Console.WriteLine("num below: " + numBelow);
            Console.WriteLine(count);
            //Level1(signal, background, data);

            //var tree = new Tree();

            //// Train the tree
            //tree.Train(signal, background);

            //// Calculate output value for each event and write to file
            //using var file = File.CreateText(path + "Level 2 Results.txt");
            //file.WriteLine("Event\tPurity");

            //for (int i = 0; i < data.Points.Count; ++i)
            //{
            //    double output = tree.RunDataPoint(data.Points[i]);
            //    file.WriteLine(i + "\t" + output);
            //}
        }
    }
}
