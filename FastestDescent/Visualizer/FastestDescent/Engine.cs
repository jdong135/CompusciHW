using DongUtility;
using PhysicsUtility;
using static DongUtility.UtilityFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Visualizer.FastestDescent
{
    class Engine
    {
        public double Time { get; private set; } = 0;

        public Projectile Projectile { get; }

        public Path Path { get; }

        public Vector GravitationalFieldStrength { get; set; } = new Vector(0, 0, -9.8);

        public Engine(Projectile projectile, Path path)
        {
            Projectile = projectile;
            Path = path;

            projectile.Position = Path.GetPosition(Path.InitialParameter);
        }

        public bool Tick(double newTime)
        {
            double parameter = GetParameterFromPosition(Projectile.Position);

            const double tolerance = 1e-9;

            if (parameter >= Path.FinalParameter || Math.Abs(parameter - Path.FinalParameter) < tolerance)
                return false;

            Vector tangentDirection = TangentDirection(parameter);
            ConstrainPositionAndVelocity(parameter, tangentDirection);
            Vector acceleration = CalculateAcceleration(tangentDirection);

            Projectile.AddForce(Projectile.Mass * acceleration);
            Projectile.Update(newTime - Time);

            Time = newTime;
            return true;
        }
        
        private double GetParameterFromPosition(Vector position)
        {
            const double factor = 10;
            const double tolerance = 1e-6;
            double spacing = (Path.FinalParameter - Path.InitialParameter) / factor;

            double initialValue = Path.InitialParameter;
            double finalValue = Path.FinalParameter;

            double finalParam = 0;
            while (spacing > tolerance)
            {
                finalParam = FindMinimum(position, spacing, initialValue, finalValue);
                initialValue = finalParam - spacing;
                finalValue = finalParam + spacing;
                // Keep these in bounds
                if (initialValue < Path.InitialParameter)
                    initialValue = Path.InitialParameter;
                if (finalValue > Path.FinalParameter)
                    finalValue = Path.FinalParameter;
                spacing /= factor;
            }

            return finalParam;
        }

        private double FindMinimum(Vector position, double spacing, double initialValue, double finalValue)
        {
            double minimumDistance = double.MaxValue;
            double bestParameter = double.MaxValue;

            for (double iparm = initialValue; iparm <= finalValue; iparm += spacing)
            {
                Vector positionTry = Path.GetPosition(iparm);
                double distance2 = Vector.Distance2(positionTry, position);
                if (distance2 < minimumDistance)
                {
                    minimumDistance = distance2;
                    bestParameter = iparm;
                }
            }

            return bestParameter;
        }

        private Vector TangentDirection(double param)
        {
            Vector currentPosition = Path.GetPosition(param);
            Vector nextPosition = Path.GetPosition(param + Path.MinimumStep);
            return nextPosition - currentPosition;
        }

        private void ConstrainPositionAndVelocity(double parameter, Vector tangentDirection)
        {
            Projectile.Position = Path.GetPosition(parameter);
            Projectile.Velocity = Projectile.Velocity.ProjectOnto(tangentDirection);
        }

        private Vector CalculateAcceleration(Vector tangentDirection)
        {
            return GravitationalFieldStrength.ProjectOnto(tangentDirection);
        }
    }
}
