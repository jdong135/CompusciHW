using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class GraphicTurnSet
    {
        public double Time { get; }

        public GraphicTurnSet(double time)
        {
            Time = time;
        }

        private List<GraphicTurn> turns = new List<GraphicTurn>();

        public void AddTurn(GraphicTurn turn)
        {
            lock (turns)
            {
                turns.Add(turn);
            }
        }

        public void DoTurns(IArenaDisplay display)
        {
            foreach (var turn in turns)
            {
                turn.DoChange(display);
            }
        }

        public void WriteToFile(BinaryWriter bw, FileWriter fw)
        {
            bw.Write(Time);
            bw.Write(turns.Count);
            DoTurns(fw);
        }

        public GraphicTurnSet(BinaryReader br)
        {
            Time = br.ReadDouble();
            int num = br.ReadInt32();
            var fr = new FileReader(br);
            for (int i = 0; i < num; ++i)
            {
                var nextTurn = fr.ReadOneTask(br);
                AddTurn(nextTurn);
            }
        }
    }
}
