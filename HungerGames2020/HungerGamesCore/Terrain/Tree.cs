using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;

namespace HungerGamesCore.Terrain
{
    public class Tree : Obstacle
    {
        static Tree()
        {
            RegisterFilename(name, "tree.png");
        }

        private const string name = "Tree";

        private const double maxWidth = 1;
        private const double maxHeight = 1;
        private const double minWidth = .5;
        private const double minHeight = .5;

        public Tree() :
            base(name, ArenaEngine.Random.NextDouble(minWidth, maxWidth), ArenaEngine.Random.NextDouble(minHeight, maxHeight))
        { }

        public override double VisionReductionPerMeter => 1e7;

        public override string Name => name;

        public override bool IsPassable(ArenaObject mover) => false;
    }
}
