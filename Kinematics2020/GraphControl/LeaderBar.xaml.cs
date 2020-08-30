using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphControl
{
    /// <summary>
    /// Interaction logic for LeaderBar.xaml
    /// </summary>
    public partial class LeaderBar : UserControl, IUpdating
    {
        public Color Color
        {
            get
            {
                return MyBrush.Color;
            }
            set
            {
                MyBrush.Color = value;
                Title.Foreground = new SolidColorBrush(Invert(value));
            }
        }
        public double BarLength
        {
            get
            {
                return Bar.Width / (ActualWidth * .8);
            }
            set
            {
                Bar.Width = value > 0 ? value * ActualWidth * .8 : 0;
            }
        }// From 0 to 1
        public string NameOfBar { get { return Title.Text; } set { Title.Text = value; } }
        private double number = 0;
        public double NumberOnRight { get { return number; } set { number = value; SetText(); } }
        public string AlertText;

        private Color Invert(Color color)
        {
            return new Color() { R = (byte)(255 - color.R), G = (byte)(255 - color.G), B = (byte)(255 - color.B), A = 255 };
        }

        private void SetText()
        {
            Number.Text = AlertText + " " + number.ToString();
        }

        public void Update(DataSource data)
        {
            double value = data.GetData();
            if (TextFunction != null)
                AlertText = TextFunction(value);
            NumberOnRight = value;
        }

        public delegate string AlertTextSetter(double val);
        public AlertTextSetter TextFunction { get; set; } = null;

        public LeaderBar(AlertTextSetter textFunction = null)
        {
            TextFunction = textFunction;
            InitializeComponent();
        }
    }
}
