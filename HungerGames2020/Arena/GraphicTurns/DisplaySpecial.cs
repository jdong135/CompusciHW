using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    class DisplaySpecial : GraphicTurn
    {
        private readonly int layer;
        private readonly int graphicCode;
        private readonly Vector2D coord;

        public DisplaySpecial(int layer, int graphicCode, Vector2D coord)
        {
            this.layer = layer;
            this.graphicCode = graphicCode;
            this.coord = coord;
        }

        public override void DoChange(IArenaDisplay display)
        {
            display.DisplaySpecial(layer, graphicCode, coord);
        }
    }
}
