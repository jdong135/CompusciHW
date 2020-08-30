using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class CompleteTurnWriter : IArenaOutput
    {
        private BinaryWriter bw;
        FileWriter fw;

        private const string header = "Hunger Games Storage format 2020";
        public CompleteTurnWriter(string filename, string registryFile, ArenaEngine arena)
        {
            bw = new BinaryWriter(File.Create(filename));

            bw.Write(header);

            bw.Write(arena.Width);
            bw.Write(arena.Height);

            fw = new FileWriter(bw, registryFile, arena.Width, arena.Height);
        }

        public void Process(CompleteTurn turn)
        {
            turn.Graphics.WriteToFile(bw, fw);
            turn.Statistics.WriteData(bw);
        }

        public void Close()
        {
            fw.Close();
        }
    }
}
