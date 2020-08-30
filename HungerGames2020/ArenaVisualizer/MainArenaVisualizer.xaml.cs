using Arena;
using GraphControl;
using GraphData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArenaVisualizer
{
    /// <summary>
    /// Interaction logic for ArenaVisualizer.xaml
    /// </summary>
    public partial class MainArenaVisualizer : UserControl, IArenaOutput
    {
        private IArenaOutput output;
        private ArenaVisualizerStandalone arena;
        private GraphManager manager;

        readonly BufferBlock<CompleteTurn> turnBuffer = new BufferBlock<CompleteTurn>();

        public bool IsPaused
        {
            get
            {
                return arena.TheArena.IsPaused;
            }
        }

        public bool Display { get; set; } = false;
        public bool SlowDraw { get; set; } = false;

        public double TimeInterval { get; set; } // in seconds

        public MainArenaVisualizer(IVisualizerDataSource engine)
        {
            InitializeComponent();

            arena = new ArenaVisualizerStandalone(engine);
            arena.TheArena.IsPaused = true;
            ArenaViewport.Content = arena;
            output = this;

            var tempEngine = (ArenaEngineAdapter)engine;
            var dataManager = tempEngine.Manager;
            manager = new GraphManager(dataManager, Graphs);
            manager.Initialize();
        }

        public const int ForceRedrawTurns = 10;

        private bool pauseDisplay = false;

        public void Start()
        {
            arena.TheArena.IsPaused = false;
            pauseDisplay = false;
            if (arenaTask == null && visualizerTask == null)
            {
                StartAll();
            }
        }

        public void Pause()
        {
            arena.TheArena.IsPaused = true;
            pauseDisplay = true;
        }

        private double currentTime = 0;

        private void RunArena()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                if (!arena.TheArena.IsPaused)
                {
                    currentTime += TimeInterval;

                    keepGoing = arena.TheArena.Update(currentTime);

                    turnBuffer.Post(arena.TheArena.CurrentTurn);
                }
            }

            turnBuffer.Complete();
        }

        private async Task UpdateVisualAsync()
        {
            while (await turnBuffer.OutputAvailableAsync())
            {
                var turn = turnBuffer.Receive();

                output.Process(turn);
                await Task.Delay((int)(TimeInterval * 1000));
            }
        }

        private Task arenaTask;
        private Task visualizerTask;

        private void StartAll()
        {
            visualizerTask = UpdateVisualAsync();

            arenaTask = Task.Factory.StartNew(() => RunArena());
        }

        public void Process(CompleteTurn turn)
        {
            try
            {
                turn.Graphics.DoTurns(arena.Display);
                Graphs.Update(turn.Statistics);

                InvalidateVisual();
                arena.InvalidateVisual();
                Graphs.InvalidateVisual();

                if (SlowDraw)
                {
                    // This forces it to wait until the drawing is complete
                    Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
                }
            }
            catch (Exception)
            {
                // Do nothing
            }
        }

        //private double zoomFactor = 1;
        //private void MyViewport_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    const double factor = 1.1;
        //    double scale = e.Delta < 0 ? factor : 1 / factor;
        //    Display.Zoom(scale, scale, 0, 0);
        //    InvalidateVisual();
        //}
    }
}
