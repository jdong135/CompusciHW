using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Visualizer.MarbleMadness
{
    public abstract class MarbleMachine
    {
        public List<Surface> Surfaces { get; } = new List<Surface>();

        public void AddSurface(Surface surface)
        {
            Surfaces.Add(surface);
        }

        abstract protected Vector Beginning { get; }

        abstract protected Vector Ending { get; }
    }
}
