using GraphData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public interface IVisualizerDataSource
    {
        bool IsPaused { get; set; }
        IEnumerable<IGraphPrototype> GetGraphSetup();
        CompleteTurn GetInitialTurnset();
        CompleteTurn CurrentTurn { get; }
        bool Update(double newTime);
        double Width { get; }
        double Height { get; }
    }
}
