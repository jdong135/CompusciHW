﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    /// <summary>
    /// A leaf or branch of a tree
    /// Does most of the work of decision trees
    /// </summary>
    internal class Leaf
    {
        /// <summary>
        /// A pointer to the next leaves, if this is a branch
        /// </summary>
        private Leaf output1 = null;
        /// <summary>
        /// A pointer to the next leaves, if this is a branch
        /// </summary>
        private Leaf output2 = null;
        /// <summary>
        /// The value of the cut that is applied at this branch (unneeded if it is a leaf)
        /// </summary>
        private double split;
        /// <summary>
        /// The index of the variable which is used to make the cut (unneeded if this is a leaf)
        /// </summary>
        private int variable;

        /// <summary>
        /// The number of background training events in this leaf
        /// </summary>
        private int nBackground = 0;
        /// <summary>
        /// The number of signal training events in this leaf
        /// </summary>
        private int nSignal = 0;

        /// <summary>
        /// A default constructor is needed for some applications, but it is generally not sensible
        /// </summary>
        internal Leaf() :
            this(-1, 0) // Default values will generate an error if used
        { }

        /// <param name="variable">The index of the variable used to make the cut</param>
        /// <param name="split">The value of the cut used for the branch</param>
        public Leaf(int variable, double split)
        {
            this.variable = variable;
            this.split = split;
        }

        /// <summary>
        /// Write the leaf to a binary file
        /// </summary>
        internal void Write(BinaryWriter bw)
        {
            bw.Write(variable);
            bw.Write(split);
            bw.Write(nSignal);
            bw.Write(nBackground);

            bw.Write(IsFinal);
            if (!IsFinal)
            {
                output1.Write(bw);
                output2.Write(bw);
            }
        }

        /// <summary>
        /// Construct a leaf from a binary file
        /// </summary>
        internal Leaf(BinaryReader br)
        {
            variable = br.ReadInt32();
            split = br.ReadDouble();
            nSignal = br.ReadInt32();
            nBackground = br.ReadInt32();

            bool fin = br.ReadBoolean();
            if (!fin)
            {
                output1 = new Leaf(br);
                output2 = new Leaf(br);
            }
        }

        /// <summary>
        /// Determines if it is a leaf or a branch (true for leaves)
        /// </summary>
        public bool IsFinal => output1 == null || output2 == null;

        /// <summary>
        /// The purity of the leaf
        /// </summary>
        public double Purity => (double)nSignal / (nSignal + nBackground);

        /// <summary>
        /// Calculates the return value for a single data point, forwarding it to other leaves as needed
        /// </summary>
        public double RunDataPoint(DataPoint dataPoint)
        {
            if (IsFinal)
            {
                return Purity;
            }

            if (DoSplit(dataPoint))
            {
                return output1.RunDataPoint(dataPoint);
            }
            else
            {
                return output2.RunDataPoint(dataPoint);
            }
        }

        /// <summary>
        /// Checks to see whether the DataPoint fails or passes the cut
        /// </summary>
        private bool DoSplit(DataPoint dataPoint)
        {
            return dataPoint.Variables[variable] <= split;
        }

        /// <summary>
        /// Trains this leaf based on input DataSets for signal and background
        /// </summary>
        public void Train(DataSet signal, DataSet background)
        {
            nSignal = signal.Points.Count;
            nBackground = background.Points.Count;

            // Determines whether this is a final leaf or if it branches
            bool branch = ChooseVariable(signal, background);

            if (branch)
            {
                // Creates a branch
                output1 = new Leaf();
                output2 = new Leaf();

                DataSet signalLeft = new DataSet(signal.Names);
                DataSet signalRight = new DataSet(signal.Names);
                DataSet backgroundLeft = new DataSet(background.Names);
                DataSet backgroundRight = new DataSet(background.Names);

                foreach (var dataPoint in signal.Points)
                {
                    if (DoSplit(dataPoint))
                    {
                        signalLeft.AddDataPoint(dataPoint);
                    }
                    else
                    {
                        signalRight.AddDataPoint(dataPoint);
                    }
                }

                foreach (var dataPoint in background.Points)
                {
                    if (DoSplit(dataPoint))
                    {
                        backgroundLeft.AddDataPoint(dataPoint);
                    }
                    else
                    {
                        backgroundRight.AddDataPoint(dataPoint);
                    }
                }

                //Console.WriteLine("Splitting sLeft: " + signalLeft.Points.Count + " sRight: " + signalRight.Points.Count
                //    + " bLeft: " + backgroundLeft.Points.Count + " bRight: " + backgroundRight.Points.Count);
                // Trains each of the resulting leaves
                output1.Train(signalLeft, backgroundLeft);
                output2.Train(signalRight, backgroundRight);
            }
            // Do nothing more if it is not a branch
        }

        /// <summary>
        /// Chooses which variable and cut value to use
        /// </summary>
        /// <returns>True if a branch was created, false if this is a final leaf</returns>
        private bool ChooseVariable(DataSet signal, DataSet background)
        {
            // TODO set the values of variable and split here		
            // Return true if you were able to find a useful variable, 
            // and false if you were not and want to make a final leaf here

            // If you are going to branch, you should end with, for example:

            // variable = 3; // The index number of the variable you want
            // split = 2.55; // The value of the cut
            // return true;

            // Or if you cannot split usefully, you should
            // return false;
            // Make sure to do this or your code will run forever!
            bool createBranch = false;
            double min = double.MaxValue;
            double max = 0;
            int bestVariable = 0;
            double bestPurityDif = 0;
            double bestCutoff = 0;
            
            if (signal.Points.Count >= 1 && background.Points.Count >= 1)
            {
                for (int i = 0; i < signal.Names.Length; i++)
                {
                    foreach (DataPoint dp in signal.Points)
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
                    foreach (DataPoint dp in background.Points)
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
                    for (double divideValue = min; divideValue < max; divideValue++)
                    {
                        double signalBelowSplit = 0;
                        double signalAboveSplit = 0;
                        double backgroundBelowSplit = 0;
                        double backgroundAboveSplit = 0;
                        foreach (DataPoint dp in signal.Points)
                        {
                            if (dp.Variables[i] <= divideValue)
                            {
                                signalBelowSplit += dp.weight;
                            }
                            else
                            {
                                signalAboveSplit += dp.weight;
                            }
                        }
                        foreach (DataPoint dp in background.Points)
                        {
                            if (dp.Variables[i] <= divideValue)
                            {
                                backgroundBelowSplit += dp.weight;
                            }
                            else
                            {
                                backgroundAboveSplit += dp.weight;
                            }
                        }
                        double PurityAboveSplit = signalAboveSplit / (signalAboveSplit + backgroundAboveSplit);
                        double PurityBelowSplit = signalBelowSplit / (signalBelowSplit + backgroundBelowSplit);
                        //I didn't want the purity to be 0 or 1 because that is overfitting
                        if (PurityAboveSplit < .95 && PurityAboveSplit > .05 && PurityBelowSplit < .95
                            && PurityBelowSplit > .05 && signalAboveSplit > 0 && signalBelowSplit > 0 &&
                            backgroundAboveSplit > 0 && backgroundBelowSplit > 0)
                        {
                            if (Math.Abs(PurityAboveSplit - PurityBelowSplit) > bestPurityDif)
                            {
                                bestCutoff = divideValue;
                                bestVariable = i;
                                bestPurityDif = Math.Abs(PurityAboveSplit - PurityBelowSplit);
                                createBranch = true;
                            }
                        }
                    }
                }
            }

            //bestLeftPurity is 1 by default, bestRightPurity is 0 by default
            if(createBranch == true)
            {
                variable = bestVariable;
                split = bestCutoff;
            }
            //Console.WriteLine("split: " + split + " var: " + variable + "purity above: " + bestPurityAboveSplit + " purity below " + bestPurityBelowSplit);
            return createBranch;
        }

    }
}
