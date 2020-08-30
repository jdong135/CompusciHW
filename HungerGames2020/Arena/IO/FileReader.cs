using Arena.GraphicTurns;
using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class FileReader
    {
        private BinaryReader br;

        public FileReader(BinaryReader br)
        {
            this.br = br;
        }

        public GraphicTurn ReadOneTask(BinaryReader br)
        {
            string type = br.ReadString();

            switch (type)
            {
                case "AO":
                    {
                        int layer = br.ReadInt32();
                        int graphicCode = br.ReadInt32();
                        int objCode = br.ReadInt32();
                        var coord = br.ReadVector2D();
                        return new AddObject(layer, objCode, graphicCode, coord);
                    }

                case "DS":
                    {
                        int layer = br.ReadInt32();
                        int graphicCode = br.ReadInt32();
                        var coord = br.ReadVector2D();
                        return new DisplaySpecial(layer, graphicCode, coord);
                    }

                case "MO":
                    {
                        int layer = br.ReadInt32();
                        int objCode = br.ReadInt32();
                        var coord = br.ReadVector2D();
                        return new MoveObject(layer, objCode, coord);
                    }

                case "RO":
                    {
                        int layer = br.ReadInt32();
                        int objCode = br.ReadInt32();
                        return new RemoveObject(layer, objCode);
                    }

                case "CO":
                    {
                        int layer = br.ReadInt32();
                        int objCode = br.ReadInt32();
                        int graphicCode = br.ReadInt32();
                        return new ChangeObjectGraphic(layer, objCode, graphicCode);
                    }

                default:
                    throw new FileNotFoundException("Illegal entry in FullGameStorage.PerformTasks()!");
            }

        }

    }
}
