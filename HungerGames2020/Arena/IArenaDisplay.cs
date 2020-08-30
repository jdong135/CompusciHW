using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public interface IArenaDisplay
    {
        void DisplaySpecial(int layer, int graphicCode, Vector2D coord);
        void AddObject(int layer, int graphicCode, int objCode, Vector2D coord);
        void MoveObject(int layer, int objCode, Vector2D newCoord);
        void RemoveObject(int layer, int objCode);
        void ChangeObjectGraphic(int layer, int objCode, int graphicCode);
    }
}
