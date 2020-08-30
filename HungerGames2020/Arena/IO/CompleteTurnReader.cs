using DongUtility;
using GraphData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class CompleteTurnReader : IVisualizerDataSource
    {
        private BinaryReader br;

        public bool IsPaused { get; set; }

        public CompleteTurn CurrentTurn { get; private set; }

        public double Width { get; }
        public double Height { get; }

        public CompleteTurnReader(string filename, string registryFilename)
        {
            br = new BinaryReader(File.OpenRead(filename));

            using (var br2 = new BinaryReader(File.OpenRead(registryFilename)))
            {
                Registry.Read(br2);
            }

            string header = br.ReadString();
            if (header != headerString) 
            {
                throw new FileNotFoundException("Invalid file format loaded into FileReader!");
            }

            Width = br.ReadDouble();
            Height = br.ReadDouble();
        }

        private const string headerString = "Hunger Games Storage format 2020";

        public CompleteTurn GetInitialTurnset()
        {
            GetNextTurn();
            return CurrentTurn;
        }

        public bool Update(double newTime)
        {
           return GetNextTurn();
        }

        private bool GetNextTurn()
        {
            if (br.BaseStream.Position >= br.BaseStream.Length)
                return false;
            var newTurn = new GraphicTurnSet(br);
            var stats = new GraphDataPacket(br);
            CurrentTurn = new CompleteTurn(newTurn, stats);
            return true;
        }

        public IEnumerable<IGraphPrototype> GetGraphSetup()
        {
            throw new NotImplementedException();
        }
    }
}
