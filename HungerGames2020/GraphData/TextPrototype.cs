using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GraphData
{
    public class TextPrototype : IGraphPrototype
    {
        public string Title { get; }
        public Color Color { get; }

        public TextPrototype(string title, Color color)
        {
            Title = title;
            Color = color;
        }

        IGraphPrototype.GraphType IGraphPrototype.GetGraphType()
        {
            return IGraphPrototype.GraphType.Text;
        }
    }
}
