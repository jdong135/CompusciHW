using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermodynamics;
using DongUtility;

namespace Thermodynamics
{
    class FancyParticleContainerGrid : ParticleContainerGrid
    {
        private IDictionary<Coord3D, IList<LinkedList<Particle>>> nearby = new Dictionary<Coord3D, IList<LinkedList<Particle>>>();

        public FancyParticleContainerGrid(double xsize, double ysize, double zsize, int xdiv, int ydiv, int zdiv) :
            base(xsize, ysize, zsize, xdiv, ydiv, zdiv)
        {
            foreach (var pair in Grid)
            {
                nearby.Add(pair.Key, new List<LinkedList<Particle>>());

                for (int ix = -1; ix <= 1; ++ix)
                    for (int iy = -1; iy <= 1; ++iy)
                        for (int iz = -1; iz <= 1; ++iz)
                        {
                            Coord3D coord = new Coord3D(pair.Key.X + ix, pair.Key.Y + iy, pair.Key.Z + iz);
                            if (coord != pair.Key && CheckCoord(coord))
                            {
                                nearby[pair.Key].Add(Grid[coord]);
                            }
                        }
            }
        }


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
            foreach (var point in nearby[coord])
            {
                foreach (var part in point)
                {
                    Vector diff = center.Position - part.Position;
                    if (diff.MagnitudeSquared < rad2 && !toBeRemoved.Contains(part))
                    {
                        response.Add(part);
                    }
                }
            }

            return response;
        }

        public override Particle Closest(Particle main, double rad, List<Particle> toBeRemoved)
        {
            double rad2 = rad * rad;

            Coord3D coord = ParticleMap[main];
            Particle response = null;
            double minDiff = double.MaxValue;

            foreach (var part in Grid[coord])
            {
                Vector diff = main.Position - part.Position;
                double mag2 = diff.MagnitudeSquared;
                if (mag2 < rad2 && !toBeRemoved.Contains(part) && part != main && mag2 < minDiff)
                {
                    response = part;
                    minDiff = mag2;
                }
            }
            foreach (var point in nearby[coord])
            {
                foreach (var part in point)
                {
                    Vector diff = main.Position - part.Position;
                    double mag2 = diff.MagnitudeSquared;
                    if (mag2 < rad2 && !toBeRemoved.Contains(part) && part != main && mag2 < minDiff)
                    {
                        response = part;
                        minDiff = mag2;
                    }
                }
            }

            return response;
        }
    }
}
