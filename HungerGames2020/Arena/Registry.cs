using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    static public class Registry
    {
        static private List<GraphicInfo> typeMap = new List<GraphicInfo>();
        static private Dictionary<GraphicInfo, int> codeMap = new Dictionary<GraphicInfo, int>();

        static public string ImageDirectory { get; private set; }

        static public void Initialize(string directory, string suffix = "")
        {
            if (directory == "")
            {
                ImageDirectory = Directory.GetCurrentDirectory() + "\\" + suffix;
            }
            else
            {
                string dir = Directory.GetCurrentDirectory();
                string comparator = directory;
                var index = dir.LastIndexOf(comparator);
                var length = comparator.Length;
                ImageDirectory = dir.Substring(0, index + length) + suffix;
            }
        }

        static public GraphicInfo GetInfo(int graphicCode)
        {
            return typeMap[graphicCode];
        }

        static public int GetGraphicCode(GraphicInfo obj)
        {
            return codeMap[obj];
        }

        static public List<GraphicInfo> GetAllGraphicInfo()
        {
            return typeMap;
        }

        static public int AddEntry(GraphicInfo obj)
        {
            typeMap.Add(obj);
            codeMap.Add(obj, typeMap.Count - 1);
            return typeMap.Count - 1;
        }

        static public void WriteToFile(BinaryWriter bw)
        {
            bw.Write("MAIN");
            bw.Write(typeMap.Count);

            for (int i = 0; i < typeMap.Count; ++i)
            {
                var gi = typeMap[i];
                bw.Write(gi.Filename);
                bw.Write(gi.XSize);
                bw.Write(gi.YSize);
            }
        }

        static private void Clear()
        {
            typeMap.Clear();
            codeMap.Clear();
        }

        static public void Read(BinaryReader br)
        {
            string header = br.ReadString();
            if (header != "MAIN")
            {
                throw new FileNotFoundException("Invalid file passed to MainRegistry.FillFromFile()!");
            }

            int length = br.ReadInt32();

            Clear();

            for (int i = 0; i < length; ++i)
            {
                string filename = br.ReadString();
                double xSize = br.ReadDouble();
                double ySize = br.ReadDouble();

                var gi = new GraphicInfo(filename, xSize, ySize);
                AddEntry(gi);
            }
        }
    }
}
