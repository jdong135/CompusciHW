using Arena;
using ArenaVisualizer;
using GraphControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HungerGames
{
    /// <summary>
    /// Interaction logic for HungerGamesTestWindow.xaml
    /// </summary>
    public partial class HungerGamesTestWindow : Window
    {
        private IVisualizerDataSource engine;
        private MainArenaVisualizer arena;

        public HungerGamesTestWindow(IVisualizerDataSource engine)
        {
            this.engine = engine;
            arena = new MainArenaVisualizer(engine);
            InitializeComponent();

            ArenaSpot.Content = arena;        

            TimeIncrementSlider.Text = arena.TimeInterval.ToString();

            arena.Display = true;
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (arena.IsPaused)
            {
                arena.Start();
                Start_Button.Content = "Pause";
            }
            else
            {
                arena.Pause();
                Start_Button.Content = "Resume";
            }
        }

        private void TimeIncrementSlider_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(TimeIncrementSlider.Text, out double result))
            {
                arena.TimeInterval = result;
            }
        }
      

        private void DisplayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            arena.Display = DisplayCheckBox.IsChecked == true;
        }

        private void SlowDrawCheckBox_Click (object sender, RoutedEventArgs e)
        {
            arena.SlowDraw = SlowDrawCheckBox.IsChecked == true;
        }
    }
}
