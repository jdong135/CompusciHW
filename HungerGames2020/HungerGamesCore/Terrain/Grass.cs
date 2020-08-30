using Arena;
using HungerGames.Animals;
using System.Collections.Generic;

namespace HungerGamesCore.Terrain
{
    public class Grass : Obstacle
    {
        private const string name = "Grass";
        private const string filename = "grass.jpg";

        public Grass(double width, double length, double height) :
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

        public override double VisionReductionPerMeter => .5 * Height;

        public override string Name => name;

        public override bool IsPassable(ArenaObject mover) => true;
    }
}
