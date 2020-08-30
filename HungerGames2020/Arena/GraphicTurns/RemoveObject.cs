using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    class RemoveObject : GraphicTurn
    {
        private readonly int layer;
        private readonly int objCode;

        public RemoveObject(ArenaObject obj)
        {
            layer = obj.Layer;
            objCode = obj.Code;
        }

        public RemoveObject(int layer, int objCode)
        {
            this.layer = layer;
            this.objCode = objCode;
        }

        public override void DoChange(IArenaDisplay display)
        {
            display.RemoveObject(layer, objCode);
        }
    }
}
