using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorPrey
{
    public class Hare : Animal
    {
        private const double myWidth = .2;
        private const double myLength = .2;
        private const double myMaxSpeed = 6;
        private const double myStepTime = .15;
        private const double myMaxAccel = 3;
        private const double initialx = 25;
        private const double initialy = 10;

        public Lynx Other { get; set; }

        public Hare(Arena arena) :
            base(myWidth, myLength, myMaxSpeed, myMaxAccel, myStepTime, new Vector2D(initialx, initialy), arena)
        {
        }

        public Perceptron Perceptron { get; set; } = new Perceptron(4, 2);

        protected override Vector2D ChooseVelocityChange()
        {
            Perceptron.Reset();

            Perceptron.AddInput(0, Position.X);
            Perceptron.AddInput(1, Position.Y);
            Perceptron.AddInput(2, Other.Position.X);
            Perceptron.AddInput(3, Other.Position.Y);

            Perceptron.Run();

            double x = Perceptron.GetOutput(0);
            double y = Perceptron.GetOutput(1);

            return new Vector2D(x, y);
        }
    }
}
