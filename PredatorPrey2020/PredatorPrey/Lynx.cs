using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorPrey
{
    public class Lynx : Animal
    {
        private const double myWidth = .3;
        private const double myLength = 1;
        private const double myMaxSpeed = 10;
        private const double myStepTime = .3;
        private const double myMaxAccel = 4;
        private const double initialx = 25;
        private const double initialy = 40;

        public Hare Other { get; set; }
        public MultilayerPerceptron Perceptron { get; set; } = new MultilayerPerceptron(4, 4, 2);
        public Lynx(Arena arena) :
            base(myWidth, myLength, myMaxSpeed, myMaxAccel, myStepTime, new Vector2D(initialx, initialy), arena)
        { }

        protected override Vector2D ChooseVelocityChange()
        {
            //Vector2D diff = Other.Position - Position;
            Perceptron.AddInput(0, Position.X);
            Perceptron.AddInput(1, Position.Y);
            Perceptron.AddInput(2, Other.Position.X);
            Perceptron.AddInput(3, Other.Position.Y);

            Perceptron.Run();

            double x = Perceptron.GetOutput(0);
            double y = Perceptron.GetOutput(1);

            return new Vector2D(x, y);

            //return diff.UnitVector() * myMaxAccel;
        }

        protected override void Eat()
        {
            double xdiff = Math.Abs(Other.Position.X - Position.X);
            double ydiff = Math.Abs(Other.Position.Y - Position.Y);
            if (xdiff < myWidth / 2 && ydiff < myLength / 2)
            {
                Arena.RemoveAnimal(Other);
            }
        }
    }
}
