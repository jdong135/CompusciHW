using Arena;
using HungerGamesCore.Terrain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HungerGamesCore.Terrain
{
    public class Water : Obstacle
    {
        private const string name = "Water";
        private const string filename = "water.jpg";

        public Water(double width, double length) :
            base(MakeName(width, length), width, length)
        {
        }

        public double Height { get; }

        private static HashSet<string> usedNames = new HashSet<string>();
        private static string MakeName(double width, double length)
        {
            string newName = name + width + "_" + length;
            if (!usedNames.Contains(newName))
            {
                RegisterFilename(newName, filename);
                usedNames.Add(newName);
            }
            return newName;
        }
        public override double VisionReductionPerMeter => 0;

        public override string Name => name;

        public override bool IsPassable(ArenaObject mover = null) => false;
    }
}
