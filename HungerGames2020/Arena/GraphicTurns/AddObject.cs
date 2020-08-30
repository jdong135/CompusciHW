using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena.GraphicTurns
{
    class AddObject : GraphicTurn
    {
        private readonly int layer;
        private readonly int objCode;
        private readonly int graphicCode;
        private Vector2D coord;

        public AddObject(ArenaObject obj)
        {
            layer = obj.Layer;
            objCode = obj.Code;
            graphicCode = obj.GraphicCode;
            coord = obj.Position;
        }

        public AddObject(int layer, int objCode, int graphicCode, Vector2D coord)
        {
            this.layer = layer;
            this.objCode = objCode;
            this.graphicCode = graphicCode;
            this.coord = coord;
        }

        public override void DoChange(IArenaDisplay display)
        {
            display.AddObject(layer, graphicCode, objCode, coord);
        }
    }
}
