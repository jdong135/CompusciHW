using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    abstract public class GraphicTurn
    {
        public GraphicTurn()
        { }

        abstract public void DoChange(IArenaDisplay display);
    }
}
