using Arena;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArenaVisualizer
{
    /// <summary>
    /// Interaction logic for ArenaVisualizerStandalone.xaml
    /// </summary>
    public partial class ArenaVisualizerStandalone : UserControl
    {
        public IVisualizerDataSource TheArena { get; set; }
        public ArenaVisualizerStandalone(IVisualizerDataSource arena)
        {
            InitializeComponent();
            TheArena = arena;
            Loaded += WhenLoaded;
        }

        private Application app;
        private IntPtr hwndListBox;
        private Window myWindow;
        internal ArenaCoreInterface Display { get; private set; }

        private void OnUIReady(object sender, EventArgs e)
        {
            var initial = TheArena.GetInitialTurnset();

            app = Application.Current;
            myWindow = app.MainWindow;
            //myWindow.SizeToContent = SizeToContent.WidthAndHeight;
            Display = new ArenaCoreInterface(ArenaCoreInterfaceHolder.ActualWidth,
                ArenaCoreInterfaceHolder.ActualHeight, TheArena.Width, TheArena.Height);
            ArenaCoreInterfaceHolder.Child = Display;
            hwndListBox = Display.HwndListBox;

            Display.AfterStartup(initial.Graphics);
        }

        private void WhenLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            InvalidateVisual();
        }

        public bool ShowVisual { get; set; } = true;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Display != null && ShowVisual)
            {
                Display.Redraw();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Display != null)
                Display.ScaleDisplay((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height);
        }
    }
}
