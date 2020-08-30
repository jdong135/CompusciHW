﻿using Arena;
using DongUtility;
using HungerGames.Interface;
using HungerGamesCore.Animals;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Collections.Generic;
using static DongUtility.UtilityFunctions;

namespace HungerGames.Animals
{
    public abstract class Animal : IntelligentOrganism
    {
        public Vector2D Velocity { get; set; } = Vector2D.NullVector();

        public AnimalStats Stats { get; }

        internal double Stamina { get; private set; }

        public bool Dead { get; set; } = false;

        public List<Sound> Sounds { get; } = new List<Sound>();

        public VisibleAnimal VisibleAnimal { get; }

        public Animal(Intelligence intel, AnimalStats stats, double width, double height) :
            base(intel, width, height)
        {
            Stats = stats;
            Stamina = Stats.MaxStamina;
            VisibleAnimal = new VisibleAnimal(this);
        }

        protected override void UserDefinedBeginningOfTurn()
        {
            if (Stamina <= 0 && Velocity.Magnitude > Stats.WalkingSpeed)
            {
                Stamina = 0;
                Velocity *= Stats.WalkingSpeed / Velocity.Magnitude;
            }
        }

        private double nextDecisionTime = 0;
        private double lastTime = 0;

        public void AddDecisionTime(double time)
        {
            nextDecisionTime += time;
        }

        protected override Turn UserDefinedChooseAction()
        {
            double deltaT = Arena.Time - lastTime;
            nextDecisionTime -= deltaT;

            if (nextDecisionTime <= 0)
            {
                nextDecisionTime = Stats.StepTime;
                var result = HungerGamesChooseAction();
                Footstep();
                return result;
            }
            return null;
        }

        public void AddSound(Sound sound)
        {
            lock (Sounds)
            {
                Sounds.Add(sound);
            }
        }

        private void ProjectSound(double volume, byte soundCode)
        {
            foreach (var animal in Arena.GetObjectsOfType<Animal>())
            {
                if (animal == this)
                    return;

                var distance2 = Vector2D.Distance2(Position, animal.Position);
                var magnitude = volume / distance2;
                if (magnitude > animal.Stats.HearingThreshold)
                {
                    var direction = animal.Position - Position;
                    animal.AddSound(new Sound(Arena.Time, direction.UnitVector(), magnitude, soundCode));
                }
            }
        }

        public void Vocalize(double volume, byte soundCode)
        {
            volume = Math.Min(volume, Stats.MaxVocalizationVolume);

            // Code zero not allowed!
            // It is reserved for footsteps
            if (soundCode == 0)
                return;

            ProjectSound(volume, soundCode);
        }

        private void Footstep()
        {
            double speed = Velocity.Magnitude;
            double percent = speed / Stats.MaxSpeed;
            double volume = percent * (Stats.MaxWalkingVolume - Stats.BaseWalkingVolume) + Stats.BaseWalkingVolume;
            ProjectSound(volume, 0);
        }

        protected override bool DoTurn(Turn turn)
        {
            double deltaT = Arena.Time - lastTime;
            if (Velocity != Vector2D.NullVector())
            {
                bool result = Arena.MoveObject(this, Position + Velocity * deltaT);
                if (!result)
                {
                    //If you hit something, your velocity goes to zero
                    Velocity = Vector2D.NullVector();
                }
            }

            double staminaLost = StaminaRate(Velocity.Magnitude) * deltaT;
            if (staminaLost > 0)
            {
                Stamina -= staminaLost;
                Stamina = Math.Max(0, Stamina);
            }
            else
            {
                Stamina += Stats.StaminaRestoredPerSecond * deltaT;
                Stamina = Math.Min(Stamina, Stats.MaxStamina);
            }

            if (turn == null)
            {
                return false;
            }

            Sounds.Clear();
            return turn.DoTurn();
        }

        private double StaminaRate(double speed)
        {
            double excess = Math.Max(speed - Stats.WalkingSpeed, 0);
            double placement = excess / (Stats.MaxSpeed - Stats.WalkingSpeed);
            return placement * Stats.StaminaPerSecondAtTopSpeed;
        }

        protected override void UserDefinedEndOfTurn()
        {
            lastTime = Arena.Time;
        }

        public IEnumerable<T> GetVisibleObjects<T>() where T : ArenaObject
        {
            foreach (var obj in Arena.GetNearbyObjects<T>(Position, Stats.VisionBase))
            {
                if (obj != this && CanSee(obj, Stats.VisionBase))
                    yield return obj;
            }
        }

        private bool CanSee(ArenaObject other, double vision)
        {
            // First check if it's too far, to avoid calculations if possible
            double distance2 = Vector2D.Distance2(other.Position, Position);
            if (vision * vision < distance2)
                return false;

            foreach (var plant in Arena.GetNearbyObjects<Obstacle>(Position, Math.Sqrt(distance2)))
            {
                vision -= VisionReduction(other, plant);
                if (vision < 0)
                    return false;
            }
            return vision * vision > distance2;
        }

        private double VisionReduction(ArenaObject other, Obstacle plant)
        {
            double distanceInPlant = AmountTraversed(Position, other.Position, plant.Size);
            return distanceInPlant * plant.VisionReductionPerMeter;
        }

        private double AmountTraversed(Vector2D point1, Vector2D point2, Rectangle area)
        {
            // If they are the same point, of course it's zero
            if (point1 == point2)
                return 0;

            // If both x points or both y points lie outside the area's, it's also zero
            if ((point1.X < area.MinX && point2.X < area.MinX) || (point1.X > area.MaxX && point2.X > area.MaxX)
                || (point1.Y < area.MinY && point2.Y < area.MinY) || (point1.Y > area.MaxY && point2.Y > area.MaxY))
                return 0;

            // If the area contains both points, it's trivial
            if (area.Contains(point1) && area.Contains(point2))
                return Vector2D.Distance(point1, point2);

            // Special case of vertical lines
            if (point1.X == point2.X)
            {
                Vector2D higher = point1.Y > point2.Y ? point1 : point2;
                Vector2D lower = point1.Y < point2.Y ? point1 : point2;

                if (area.MinX > point1.X || area.MaxX < point1.X)
                    return 0;
                if (area.MinY > higher.Y || area.MaxY < lower.Y)
                    return 0;
                return Math.Min(area.MaxY, higher.Y) - Math.Max(area.MinY, lower.Y);
            }

            double slope = (point1.Y - point2.Y) / (point1.X - point2.X);
            double intercept = point1.Y - slope * point1.X;

            Vector2D? intersect1 = null, intersect2 = null;

            double y1 = area.MinX * slope + intercept;
            double y2 = area.MaxX * slope + intercept;
            double x1 = (area.MinY - intercept) / slope;
            double x2 = (area.MaxY - intercept) / slope;

            if (Between(y1, area.MinY, area.MaxY) && Between(area.MinX, Math.Min(point1.X, point2.X), Math.Max(point1.X, point2.X)))
                intersect1 = new Vector2D(area.MinX, y1);

            if (Between(y2, area.MinY, area.MaxY) && Between(area.MaxX, Math.Min(point1.X, point2.X), Math.Max(point1.X, point2.X)))
            {
                if (intersect1 == null)
                    intersect1 = new Vector2D(area.MaxX, y2);
                else
                    intersect2 = new Vector2D(area.MaxX, y2);
            }

            if (intersect2 == null && Between(x1, area.MinX, area.MaxX) && Between(area.MinY, Math.Min(point1.Y, point2.Y), Math.Max(point1.Y, point2.Y)))
            {
                if (intersect1 == null)
                    intersect1 = new Vector2D(x1, area.MinY);
                else
                    intersect2 = new Vector2D(x1, area.MinY);
            }

            if (intersect2 == null && Between(x2, area.MinX, area.MaxX) && Between(area.MaxY, Math.Min(point1.Y, point2.Y), Math.Max(point1.Y, point2.Y)))
            {
                if (intersect1 == null)
                    intersect1 = new Vector2D(x1, area.MaxY);
                else
                    intersect2 = new Vector2D(x2, area.MaxY);
            }

            if (intersect1 == null || intersect2 == null)
                return 0;

            Vector2D i1 = (Vector2D)intersect1;
            Vector2D i2 = (Vector2D)intersect2;

            if (intersect2 == null)
            {
                if (area.Contains(point1))
                    return Vector2D.Distance(point1, i1);
                else
                    return Vector2D.Distance(point2, i1);                    
            }
           
             return Vector2D.Distance(i1, i2);
        }

    }
}
