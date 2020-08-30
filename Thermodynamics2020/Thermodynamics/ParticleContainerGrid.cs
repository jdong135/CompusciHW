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
    /// A particle container that implements a grid strategy to deal with collisions
    /// </summary>
    abstract public class ParticleContainerGrid : ParticleContainer
    {
        /// <summary>
        /// A simple three-dimensional integer coordinate
        /// </summary>
        protected struct Coord3D
        {
            public Coord3D(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public int X;
            public int Y;
            public int Z;

            static public bool operator ==(Coord3D one, Coord3D two)
            {
                return one.X == two.X && one.Y == two.Y && one.Z == two.Z;
            }

            static public bool operator !=(Coord3D one, Coord3D two)
            {
                return !(one == two);
            }

            public override bool Equals(object obj)
            {
                return (Coord3D)obj == this;
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
            }
        }

        /// <summary>
        /// The distance between two cells, given as a vector
        /// The x component is the distance in x between two cells, and so forth
        /// </summary>
        private Vector intervals;

        /// <summary>
        /// The maximum number of cells in each direction
        /// The x component is the number of x divisions, and so forth
        /// </summary>
        private Coord3D limits;

        /// <summary>
        /// All the partices and the grid coordinate that they map to
        /// </summary>
        protected Dictionary<Particle, Coord3D> ParticleMap { get; set; } = new Dictionary<Particle, Coord3D>();

        /// <summary>
        /// All coordinates and the list of particles in each grid division
        /// </summary>
        protected Dictionary<Coord3D, LinkedList<Particle>> Grid { get; set; } = new Dictionary<Coord3D, LinkedList<Particle>>();

        public ParticleContainerGrid(double xsize, double ysize, double zsize, int xdiv, int ydiv, int zdiv) :
            base(xsize, ysize, zsize)
        {
            limits = new Coord3D(xdiv, ydiv, zdiv);
            intervals = new Vector(xsize / xdiv, ysize / ydiv, zsize / zdiv);

            for (int ix = 0; ix < xdiv; ++ix)
                for (int iy = 0; iy < ydiv; ++iy)
                    for (int iz = 0; iz < zdiv; ++iz)
                    {
                        Grid.Add(new Coord3D(ix, iy, iz), new LinkedList<Particle>());
                    }
        }

        /// <summary>
        /// Make sure the position is valid
        /// </summary>
        protected bool CheckCoord(Coord3D coord)
        {
            return (coord.X >= 0 && coord.Y >= 0 && coord.Z >= 0 && coord.X < limits.X && coord.Y < limits.Y && coord.Z < limits.Z);
        }

        protected override void AddParticleDirectly(Particle part)
        {
            base.AddParticleDirectly(part);
            Coord3D coord = TransformToCoord(part.Position);
            ParticleMap.Add(part, coord);
            Grid[coord].AddLast(part);
        }

        protected override void RemoveParticleDirectly(Particle part)
        {
            base.RemoveParticleDirectly(part);

            if (ParticleMap.ContainsKey(part))
            {
                Coord3D coord = ParticleMap[part];
                Grid[coord].Remove(part);
                ParticleMap.Remove(part);
            }
        }

        /// <summary>
        /// Convert from a position to a coordinate in a single dimension
        /// </summary>
        /// <param name="loc">The current position</param>
        /// <param name="interval">The distance between two cells</param>
        /// <param name="max">The maximum number of cells</param>
        static private int TransformToInt(double loc, double interval, int max)
        {
            int returnVal = (int)Math.Floor(loc / interval);
            if (returnVal < 0)
            {
                return 0;
            }
            else if (returnVal >= max)
            {
                return max - 1;
            }
            else
            {
                return returnVal;
            }
        }

        /// <summary>
        /// Transforms from a position to a 3D integer coordinate
        /// </summary>
        private Coord3D TransformToCoord(Vector vec)
        {
            return new Coord3D(TransformToInt(vec.X, intervals.X, limits.X),
                TransformToInt(vec.Y, intervals.Y, limits.Y),
                TransformToInt(vec.Z, intervals.Z, limits.Z));
        }

        /// <summary>
        /// Advance all particles
        /// </summary>
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (var part in ParticleMap.Keys.ToList())
            {
                Coord3D oldCoord = ParticleMap[part];
                Coord3D newCoord = TransformToCoord(part.Position);
                ParticleMap[part] = newCoord;
                Grid[oldCoord].Remove(part);
                Grid[newCoord].AddLast(part);
            }
        }

        /// <summary>
        /// Find all particles near a given particle
        /// </summary>
        /// <param name="center">The position of the current particle</param>
        /// <param name="rad">The radius to look within</param>
        /// <param name="toBeRemoved">A list of particles that have already been removed from simulation</param>
        /// <returns>All particles within the radius rad from the given particle</returns>
        abstract public List<Particle> Nearby(Particle center, double rad, List<Particle> toBeRemoved);

        /// <summary>
        /// Find the closest particle to a given particle
        /// </summary>
        /// <param name="main">The given particle</param>
        /// <param name="rad">The radius to look within</param>
        /// <param name="toBeRemoved">A list of particles that have already been removed from simulation</param>
        abstract public Particle Closest(Particle main, double rad, List<Particle> toBeRemoved);
    }
}
