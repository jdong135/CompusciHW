using DongUtility;

namespace PredatorPrey
{
    public abstract class Animal
    {
        public Vector2D Position { get; set; }
        public Vector2D Velocity { get; set; } = Vector2D.NullVector();

        public double Width { get; }
        public double Height { get; }

        public double MaxSpeed { get; }
        public double MaxAcceleration { get; }
        public double StepTime { get; }

        protected Arena Arena { get; }

        public Animal(double width, double height, double maxSpeed, double maxAccel, double stepTime, Vector2D position, Arena arena)
        {
            Width = width;
            Height = height;
            MaxSpeed = maxSpeed;
            MaxAcceleration = maxAccel;
            StepTime = stepTime;
            Arena = arena;
            Position = position;
        }

        private double nextDecisionTime = 0;

        public void Update(double deltaT)
        {
            nextDecisionTime -= deltaT;

            if (nextDecisionTime <= 0)
            {
                nextDecisionTime += StepTime;
                ChooseMove();
            }

            Position += Velocity * deltaT;
            AdjustPosition();
            Eat();
        }

        virtual protected void Eat()
        { }

        private void ChooseMove()
        {
            var deltaV = ChooseVelocityChange();
            // Make sure it's not too big
            if (deltaV.Magnitude > MaxAcceleration)
                deltaV = deltaV.UnitVector() * MaxAcceleration;

            Velocity += deltaV;

            if (Velocity.Magnitude > MaxSpeed)
                Velocity = Velocity.UnitVector() * MaxSpeed;
        }

        private void AdjustPosition()
        {
            if (Position.X < 0)
                Position = new Vector2D(0, Position.Y);
            if (Position.X > Arena.Width)
                Position = new Vector2D(Arena.Width, Position.Y);
            if (Position.Y < 0)
                Position = new Vector2D(Position.X, 0);
            if (Position.Y > Arena.Height)
                Position = new Vector2D(Position.X, Arena.Height);
        }

        abstract protected Vector2D ChooseVelocityChange();
    }
}
