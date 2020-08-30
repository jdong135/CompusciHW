using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermodynamics;
using DongUtility;

namespace Thermodynamics
{
    /// <summary>
    /// A grid setup that does not look outside its cell for nearby particles
    /// </summary>
    public class SimpleParticleContainerGrid : ParticleContainerGrid
    {
        public SimpleParticleContainerGrid(double xsize, double ysize, double zsize, int xdiv, int ydiv, int zdiv) :
                        base(xsize, ysize, zsize, xdiv, ydiv, zdiv)
        { }

        override public List<Particle> Nearby(Particle center, double rad, List<Particle> toBeRemoved)
        {
            double rad2 = rad * rad;

            Coord3D coord = ParticleMap[center];
            List<Particle> response = new List<Particle>();

            foreach (var part in Grid[coord])
            {
                Vector diff = center.Position - part.Position;
                if (diff.MagnitudeSquared < rad2 && !toBeRemoved.Contains(part) && part != center)
                {
                    response.Add(part);
                }
            }

            return response;
        }

        public override Particle Closest(Particle main, double rad, List<Particle> toBeRemoved)
        {
            double rad2 = rad * rad;

            Coord3D coord = ParticleMap[main];
            double minDist = double.MaxValue;
            Particle response = null;

            foreach (var part in Grid[coord])
            {
                Vector diff = main.Position - part.Position;
                double mag2 = diff.MagnitudeSquared;
                if (mag2 < rad2 && !toBeRemoved.Contains(part) && part != main && mag2 < minDist)
                {
                    response = part;
                    minDist = mag2; 
                }
            }

            return response;
        }
    }
}
