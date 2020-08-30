using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GraphData
{
    public class TimelinePrototype
    {
        public string Name { get; }
        public Color Color { get; }

        public TimelinePrototype(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
