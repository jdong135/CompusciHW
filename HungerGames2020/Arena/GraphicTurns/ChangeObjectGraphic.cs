using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    class ChangeObjectGraphic : GraphicTurn
    {
        private readonly int layer;
        private readonly int graphicCode;
        private readonly int objCode;

        public ChangeObjectGraphic(int layer, int objCode, int graphicCode)
        {
            this.layer = layer;
            this.graphicCode = graphicCode;
            this.objCode = objCode;
        }

        public override void DoChange(IArenaDisplay display)
        {
            display.ChangeObjectGraphic(layer, objCode, graphicCode);
        }
    }
}
