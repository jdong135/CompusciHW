using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;

namespace HungerGamesCore.Terrain
{
    public class Shrub : Obstacle
    {
        static Shrub()
        {
            RegisterFilename(name, "shrub.png");
        }

        private const string name = "Shrub";

        private const double maxWidth = 1.5;
        private const double maxHeight = 1.5;
        private const double minWidth = .25;
        private const double minHeight = .25;

        public Shrub() :
            base(name, ArenaEngine.Random.NextDouble(minWidth, maxWidth), ArenaEngine.Random.NextDouble(minHeight, maxHeight))
        {
        }

        public override double VisionReductionPerMeter => .5;

        public override string Name => name;

        public override bool IsPassable(ArenaObject mover) => true;
    }
}
