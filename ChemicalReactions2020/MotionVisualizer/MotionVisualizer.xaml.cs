using DongUtility;
using GraphControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VisualizerControl;

namespace MotionVisualizerControl
{
    /// <summary>
    /// Interaction logic for FullVisualizer.xaml
    /// </summary>
    public partial class MotionVisualizer : Window
    {
        public string BackgroundFile
        {
            set
            {
                Visualizer.BackgroundFile = value;
            }
        }

        public MotionVisualizer(IVisualization engine)
        {
            InitializeComponent();

            Viewport.Content = Visualizer;

            this.engine = engine;

            Visualizer = new VisualizerControl.Visualizer();
            VisualizerSpot.Content = Visualizer;

            // Do initial setup
            engine.Initialization().ProcessAll(Visualizer);
        }

        private UpdatingCollection updatingCollection = new UpdatingCollection();
        private UpdatingFunctions updatingFunctions = new UpdatingFunctions();

        public delegate double BasicFunction();
        public delegate List<double> ListFunction();
        public struct BasicFunctionPair
        {
            public BasicFunctionPair(BasicFunction xFunc, BasicFunction yFunc)
            {
                XFunc = xFunc;
                YFunc = yFunc;
            }
            public BasicFunction XFunc { get; set; }
            public BasicFunction YFunc { get; set; }
        }

        public void AddSingleGraph(string name, Color color, BasicFunction xFunc, BasicFunction yFunc,
            string xAxis, string yAxis)
        {
            var gu = new GraphUnderlying(xAxis, yAxis);
            var timeline = new Timeline(name, color);
            gu.AddTimeline(timeline);
            Graph graph = new Graph(gu);
            void function(DataSource ds)
            {
                ds.AddData(xFunc());
                ds.AddData(yFunc());
            }

            updatingCollection.AddContainer(new UpdatingContainer(gu, function));
            updatingFunctions.AddFunction(function);
            Graphs.AddGraph(graph);
        }

        public void AddHist(int nBins, Color color, ListFunction allDataFunc, string xAxis)
        {
            var hist = new Histogram(nBins, color, xAxis);
            Graph graph = new Graph(hist);
            void function(DataSource ds)
            {
                ds.AddSet(allDataFunc());
            }

            updatingCollection.AddContainer(new UpdatingContainer(hist, function));
            updatingFunctions.AddFunction(function);
            Graphs.AddGraph(graph);
        }

        public delegate string TextFunction();
        public void AddText(string title, Color color, TextFunction textFunc)
        {
            var text = new UpdatingText()
            {
                Title = title,
                Color = color
            };

            void function(DataSource ds)
            {
                ds.AddTextData(textFunc());
            }

            updatingCollection.AddContainer(new UpdatingContainer(text, function));
            updatingFunctions.AddFunction(function);
            Graphs.AddGraph(text);
        }

        public void AddGraph(GraphUnderlying graph, IEnumerable<BasicFunctionPair> functions)
        {
            Graph newGraph = new Graph(graph);

            void function(DataSource ds)
            {
                foreach (var func in functions)
                {
                    ds.AddData(func.XFunc());
                    ds.AddData(func.YFunc());
                }
            }

            updatingCollection.AddContainer(new UpdatingContainer(graph, function));
            updatingFunctions.AddFunction(function);
            Graphs.AddGraph(newGraph);
        }

        public delegate Vector3D VectorFunc();

        public void Add3DGraph(string name, BasicFunction funcX, VectorFunc funcY, string xTitle, string yTitle)
        {
            GraphUnderlying graphU = new GraphUnderlying(xTitle, yTitle);

            graphU.AddTimeline(new Timeline("x " + name, Colors.Red));
            graphU.AddTimeline(new Timeline("y " + name, Colors.Green));
            graphU.AddTimeline(new Timeline("z " + name, Colors.Blue));

            BasicFunction xVec = () => funcY().X;
            BasicFunction yVec = () => funcY().Y;
            BasicFunction zVec = () => funcY().Z;

            var list = new List<BasicFunctionPair>();
            list.Add(new BasicFunctionPair() { XFunc = funcX, YFunc = xVec });
            list.Add(new BasicFunctionPair() { XFunc = funcX, YFunc = yVec });
            list.Add(new BasicFunctionPair() { XFunc = funcX, YFunc = zVec });

            AddGraph(graphU, list);
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            bool needToRestart = false;
            if (IsRunning)
            {
                IsRunning = false;
                needToRestart = true;
            }
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Screenshot",
                DefaultExt = ".jpg"
            };

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(MakeScreenshot()));
                using (Stream fileStream = File.Create(filename))
                {
                    encoder.Save(fileStream);
                }
            }

            if (needToRestart)
            {
                IsRunning = true;
            }
        }

        private void Screenshot_Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(MakeScreenshot());
        }

        private RenderTargetBitmap MakeScreenshot()
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(this);
            return bitmap;
        }

        private VisualizerControl.Visualizer Visualizer;
        private double time = 0;
        private double timeIncrement = .01;
        public double TimeIncrement
        {
            get => timeIncrement;
            set
            {
                timeIncrement = value;
                TimeIncrementSlider.Text = timeIncrement.ToString();
            }
        }
        // in seconds
        private double timeScale = 1;
        public double TimeScale
        {
            get => timeScale;
            set
            {
                timeScale = value;
                TimeScaleSlider.Text = timeScale.ToString();
            }
        }

        private bool autoCamera = false;
        public bool AutoCamera
        {
            get => autoCamera;
            set
            {
                autoCamera = value;
                AutoCameraCheck.IsChecked = value;
            }
        }

        // To keep time
        private Stopwatch timer = new Stopwatch();

        private IVisualization engine;

        // For multithreading communications
        private BufferBlock<PackagedCommands> turnBuffer = new BufferBlock<PackagedCommands>();
        private Task engineTask;
        private Task visualizerTask;

        /// <summary>
        /// Add a new element to the top menu bar
        /// </summary>
        public void AddControl(UIElement control)
        {
            ButtonBar.Children.Add(control);
        }

        /// <summary>
        /// Whether the 3D should be updating while the engine calculates
        /// Can be turned off to speed up graph generation time
        /// </summary>
        public bool Display { get; set; } = true;

        /// <summary>
        /// Gets and sets whether the simulation is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return timer.IsRunning;
            }
            set
            {
                if (value)
                    timer.Start();
                else
                    timer.Stop();
            }
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning)
            {
                Start_Button.Content = "Resume";
                IsRunning = false;
            }
            else
            {
                Start_Button.Content = "Pause";
                IsRunning = true;

                StartAll();
            }
        }

        /// <summary>
        /// Includes all the information needed by the visualizer piece that the engine will pass it
        /// </summary>
        private class PackagedCommands
        {
            public PackagedCommands(VisualizerCommandSet commands, DataSource data, double time)
            {
                Commands = commands;
                Data = data;
                Time = time;
            }

            /// <summary>
            /// The command set
            /// </summary>
            public VisualizerCommandSet Commands { get; set; }
            /// <summary>
            /// Data, for graphing
            /// </summary>
            public DataSource Data { get; set; }
            /// <summary>
            /// The time of the commands
            /// </summary>
            public double Time { get; set; }
        }

        /// <summary>
        /// The maximum buffer size - otherwise you get memory overruns
        /// </summary>
        private const int maxBufferSize = 5;
        private const int sizeToResume = 3;

        /// <summary>
        /// Start or continue the engine running
        /// </summary>
        private void RunEngine()
        {
            while (IsRunning)
            {
                double newTime = engine.Time + TimeIncrement;
                // Create package of commands
                var package = new PackagedCommands(engine.Tick(newTime), updatingFunctions.GetData(), newTime);

                if (!engine.Continue)
                {
                    turnBuffer.Complete();
                    break;
                }

                // Stop in an empty loop until the turnBuffer has dropped down in size.
                // Otherwise you get memory overruns
                if (turnBuffer.Count > maxBufferSize)
                {
                    while (turnBuffer.Count > sizeToResume)
                    { }
                }
                // Send to the buffer
                turnBuffer.Post(package);
            }
        }

        /// <summary>
        /// This is the time I need to flush the visual memory
        /// </summary>
        private const double flushTime = 1;

        /// <summary>
        /// Updates visualization
        /// </summary>
        private async Task UpdateVisualAsync()
        {
            double timeOfLastDraw = 0;
            while (await turnBuffer.OutputAvailableAsync() && IsRunning)
            {
                var turn = turnBuffer.Receive();
                if (Display)
                {
                    // Process commands
                    turn.Commands.ProcessAll(Visualizer);
                    // Redraw screen
                    //Visualizer.InvalidateVisual();
                }
                //Update time display
                time = turn.Time;
                TimeValue.Text = Math.Round(time, 5).ToString();

                if (autoCamera)
                    Visualizer.AdjustCamera();

                // Update graphs
                Graphs.Update(turn.Data);

                //Check if delay is needed
                var timeDiff = time - timer.Elapsed.TotalSeconds * timeScale;
                int delay = 0;
                if (timeDiff > 0)
                {
                    // This delays it so the clocks will line up
                    delay = (int)(timeDiff * 1000);
                }
                await Task.Delay(delay); // Convert to milliseconds
                                         //await Task.Delay(1);
                                         //if (turnBuffer.Count > flushBufferSize)
                double timeSinceLastDraw = timer.Elapsed.TotalSeconds - timeOfLastDraw;
                if (timeSinceLastDraw > flushTime || SlowDraw)
                {
                    InvalidateVisual();
                    WaitForDrawing();
                    timeOfLastDraw = timer.Elapsed.TotalSeconds;
                }
            }
        }

        /// <summary>
        /// Waits until all drawing is done - needed to keep the visuals displaying sometimes
        /// </summary>
        private void WaitForDrawing()
        {
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
        }

        /// <summary>
        /// Starts all tasks
        /// </summary>
        private void StartAll()
        {
            visualizerTask = UpdateVisualAsync();
            engineTask = Task.Run(() => RunEngine());

        }

        private void TimeIncrementSlider_TextChanged(object sender, TextChangedEventArgs e)
        {
            SliderChanged(TimeIncrementSlider, ref timeIncrement);
        }

        private void SliderChanged(TextBox textBox, ref double result)
        {
            if (double.TryParse(textBox.Text, out double newNum))
                result = newNum;
        }

        private void AutoCameraCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoCameraCheck.IsChecked != null)
            {
                autoCamera = (bool)AutoCameraCheck.IsChecked;
                if (!timer.IsRunning && autoCamera)
                    Visualizer.AdjustCamera();
            }
        }

        private void TimeScaleSlider_TextChanged(object sender, TextChangedEventArgs e)
        {
            SliderChanged(TimeScaleSlider, ref timeScale);
        }

        private void DisplayCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (DisplayCheck.IsChecked != null)
            {
                Display = (bool)DisplayCheck.IsChecked;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Visualizer.UserControl_KeyDown(sender, e);
        }

        private bool slowDraw = false;
        public bool SlowDraw
        {
            get => slowDraw;
            set
            {
                slowDraw = value;
                SlowDrawCheck.IsChecked = value;
            }
        }
        private void SlowDrawCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (SlowDrawCheck.IsChecked != null)
            {
                SlowDraw = (bool)SlowDrawCheck.IsChecked;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
