using Arena;
using HungerGames.Animals;
using HungerGamesCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGamesCore.Terrain
{
    abstract public class Obstacle : StationaryObject
    {
        static private Dictionary<string, int> codeDictionary = new Dictionary<string, int>();
        static private Dictionary<string, string> filenameDictionary = new Dictionary<string, string>();
        static protected void RegisterFilename(string name, string filename)
        {
            filenameDictionary.Add(name, filename);
        }

        private const int obstacleLayer = 1;

        public Obstacle(string name, double width, double height) :
            base(GetCode(name, width, height), obstacleLayer, width, height)
        {
            VisibleObstacle = new VisibleObstacle(this);
        }

        static private int GetCode(string name, double width, double height)
        {
            if (codeDictionary.ContainsKey(name))
            {
                return codeDictionary[name];
            }
            else
            {
                int code = Registry.AddEntry(new GraphicInfo(filenameDictionary[name], width, height));
                codeDictionary.Add(name, code);
                return code;
            }
        }

        abstract public double VisionReductionPerMeter { get; }
        
        public VisibleObstacle VisibleObstacle { get; }
    }
}
