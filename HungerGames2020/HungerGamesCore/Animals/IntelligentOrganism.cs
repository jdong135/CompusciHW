using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HungerGames.Turns;
using Arena;
using System.IO;
using HungerGames.Interface;
using DongUtility;
using System.Drawing;

namespace HungerGames.Animals
{
    abstract public class IntelligentOrganism : MovingObject
    {
        static private Dictionary<Type, Codes> speciesCodes = new Dictionary<Type, Codes>();
        static private object locker = new object();

        private struct Codes
        {
            public int GraphicsCode { get; set; }
            public int SpeciesCode { get; set; }
        }

        static public void AddCodes(Type type, int gCode, int sCode)
        {
            speciesCodes.Add(type, new Codes() { GraphicsCode = gCode, SpeciesCode = sCode });
        }

        private Intelligence intelligence;

        public override string Name { get { return intelligence.Name; } }
        public Color Color { get { return intelligence.Color; } }

        private const int orgLayer = 2;

        public IntelligentOrganism(Intelligence intel, double width, double height) :
            base(GetCode(intel, width, height), orgLayer, width, height)
        {
            intelligence = intel;
            intelligence.Organism = this;
            SpeciesCode = speciesCodes[intel.GetType()].SpeciesCode;
        }

        private static int speciesCode = 0;
        private static int GetCode(Intelligence intel, double width, double height)
        {
            lock (locker)
            {
                var type = intel.GetType();
                if (!speciesCodes.ContainsKey(type))
                {
                    int code = Registry.AddEntry(new GraphicInfo(intel.BitmapFilename, width, height));
                    AddCodes(type, code, speciesCode++);
                }
                return speciesCodes[type].GraphicsCode;
            }
        }

        public int SpeciesCode { get; }

        protected Turn HungerGamesChooseAction()
        {
            try
            {
                return intelligence.ChooseTurn();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
