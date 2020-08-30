using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphData;

namespace Arena
{
    public class CompleteTurn
    {
        public GraphicTurnSet Graphics { get; }
        public GraphDataPacket Statistics { get; }

        public CompleteTurn(GraphicTurnSet graphics, GraphDataPacket statistics)
        {
            Graphics = graphics;
            Statistics = statistics;
        }
    }
}
