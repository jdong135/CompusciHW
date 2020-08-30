using DongUtility;
using FiniteElement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Visualizer.FiniteElement
{
    class YourParticleStructure : ParticleStructure
    {
        public double Mass;
        public double SideLength;
        public double Height;
        public double NumParticles;
        public double Spacing;
        public List<Projectile> ProjectileList = new List<Projectile>();

        public YourParticleStructure()
        {
            Mass = 5;
            SideLength = 1;
            Height = 3;
            NumParticles = 27;
            var particlesPerSide = NumParticles / 3; // 9
            Spacing = SideLength / (NumParticles / particlesPerSide - 1); // 1/2

            //Initialize the particles
            for (double i = 0; i < NumParticles / 9; i++)
            {
                for (double j = 0; j < NumParticles / 9; j++)
                {
                    for (double k = 0; k < NumParticles / 9; k++)
                    {
                        Projectile projectile = new Projectile(new Vector(i * Spacing, j * Spacing, Height + k * Spacing), Vector.NullVector(), Mass);
                        AddProjectile(projectile);
                        ProjectileList.Add(projectile);
                    }
                }
            }

            Vector com = CalcCOM();
            foreach (Projectile projectile in ProjectileList)
            {
                projectile.Position = Vector.RotateAboutX(projectile.Position, Math.PI / 8, com);
            }

            for (int i = 0; i < NumParticles; i++)
            {
                for (int j = i + 1; j < NumParticles; j++)
                {
                    AddConnector(ProjectileList[i], ProjectileList[j], 1000);
                }
            }


        }

        public Vector CalcCOM()
        {
            //PositionMass = Position*Mass for each projectile
            Vector positionMass = new Vector();
            double totalMass = 0;
            foreach(Projectile projectile in ProjectileList)
            {
                positionMass += projectile.Position * projectile.Mass;
                totalMass += projectile.Mass;
            }
            return positionMass/totalMass;
        }
    }
}
