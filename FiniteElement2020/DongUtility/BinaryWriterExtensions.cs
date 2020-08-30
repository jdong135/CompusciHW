using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DongUtility
{
    static public class BinaryWriterExtensions
    {
        static public void Write(this BinaryWriter bw, Vector vec)
        {
            bw.Write(vec.X);
            bw.Write(vec.Y);
            bw.Write(vec.Z);
        }

        static public Vector ReadVector(this BinaryReader br)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();
            return new Vector(x, y, z);
        }

        static public void Write(this BinaryWriter bw, Vector2D vec)
        {
            bw.Write(vec.X);
            bw.Write(vec.Y);
        }

        static public Vector2D ReadVector2D(this BinaryReader br)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            return new Vector2D(x, y);
        }
    }
}
