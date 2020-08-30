using DongUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public class FileWriter : IDisposable, IArenaDisplay
    {
        private BinaryWriter bw;
        private string registryFile = "";

        public double Width { get; set; }
        public double Height { get; set; }

        public FileWriter(BinaryWriter bw, string registryFile, double width, double height) 
        {
            Width = width;
            Height = height;
            this.registryFile = registryFile;
            this.bw = bw;
        }

        public void Close()
        {
            bw.Close();

            if (registryFile.Length > 0)
            {
                using var bw2 = new BinaryWriter(File.Create(registryFile));
                Registry.WriteToFile(bw2);
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void DisplaySpecial(int layer, int graphicCode, Vector2D coord)
        {
            bw.Write("DS");
            bw.Write(layer);
            bw.Write(graphicCode);
            bw.Write(coord);
        }

        public void AddObject(int layer, int graphicCode, int objCode, Vector2D coord)
        {
            bw.Write("AO");
            bw.Write(layer);
            bw.Write(graphicCode);
            bw.Write(objCode);
            bw.Write(coord);
        }

        public void MoveObject(int layer, int objCode, Vector2D newCoord)
        {
            bw.Write("MO");
            bw.Write(layer);
            bw.Write(objCode);
            bw.Write(newCoord);
        }

        public void RemoveObject(int layer, int objCode)
        {
            bw.Write("RO");
            bw.Write(layer);
            bw.Write(objCode);
        }

        public void ChangeObjectGraphic(int layer, int objCode, int graphicCode)
        {
            bw.Write("CO");
            bw.Write(layer);
            bw.Write(objCode);
            bw.Write(graphicCode);
        }
    }
}
