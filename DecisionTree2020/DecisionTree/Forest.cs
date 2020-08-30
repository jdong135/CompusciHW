using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTree
{
    class Forest
    {
        public int nTrees;
        public List<Tree> trees = new List<Tree>();
        public List<double> sums = new List<double>();
        public Forest(int nTrees)
        { 
            this.nTrees = nTrees;
        }

        //Need a method RunDataPoint(dp), weighted sum
        public double RunDataPoint(DataPoint dp)
        {
            double weightedSum = 0;
            for(int i=0; i<nTrees; i++)
            {
                weightedSum += sums[i] * trees[i].RunDataPoint(dp);
            }
            return weightedSum;
        }

        public void Train(DataSet signal, DataSet background)
        {
            for(int i=0; i<nTrees; i++)
            {
                Tree tree = new Tree();
                tree.Train(signal, background);
                trees.Add(tree);
                sums.Add(findBoostValue(tree, signal, background));
                //reassign signal and background weights
                double multiplyValue = findBoostValue(tree, signal, background);
                alterWeights(signal, background, multiplyValue);
            }

        }

        public double findBoostValue(Tree tree, DataSet signal, DataSet background)
        {
            double misclassifiedWeight = 0;
            double totalDataWeight = signal.calcWeightedSum() + background.calcWeightedSum();
            foreach(DataPoint dp in signal.Points)
            {
                if(tree.RunDataPoint(dp) < .5)
                {
                    misclassifiedWeight += dp.weight;
                }
            }
            foreach(DataPoint dp in background.Points)
            {
                if(tree.RunDataPoint(dp) > .5)
                {
                    misclassifiedWeight += dp.weight;
                }
            }
            double rValue = misclassifiedWeight / totalDataWeight;
            return Math.Log((1-rValue)/rValue);
        }

        public void alterWeights(DataSet signal, DataSet background, double multiplyValue)
        {
            foreach(DataPoint dp in signal.Points)
            {
                dp.weight *= multiplyValue;
            }
            foreach(DataPoint dp in background.Points)
            {
                dp.weight *= multiplyValue;
            }
        }
    }
}
