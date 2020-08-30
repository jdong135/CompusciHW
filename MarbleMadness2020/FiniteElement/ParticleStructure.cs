using DongUtility;
using PhysicsUtility;
using System;
using System.Collections.Generic;

namespace FiniteElement
{
    public class ParticleStructure
    {
        public double DampingCoefficient { get; set; } = 1;

        public List<Projectile> Projectiles { get; } = new List<Projectile>();
        public List<Connector> Connectors { get; } = new List<Connector>();

        //public double MinimumDistance
        //{
        //    get { return Connector.MinimumDistance; }
        //    set { Connector.MinimumDistance = value; }
        //}

        public int GetIndex(Projectile projectile)
        {
            return Projectiles.IndexOf(projectile);
        }
        
        public Tuple<int, int> GetIndexOfProjectiles(Connector connector)
        {
            return new Tuple<int, int>(GetIndex(connector.Projectile1), GetIndex(connector.Projectile2));
        }

        public class Connector
        {
            public Connector(Projectile proj1, Projectile proj2, double springConstant)
            {
                Projectile1 = proj1;
                Projectile2 = proj2;

                UnstretchedLength = (proj1.Position - proj2.Position).Magnitude;
                this.SpringConstant = springConstant;
            }

            public Projectile Projectile1 { get; set; }
            public Projectile Projectile2 { get; set; }

            public double UnstretchedLength { get; }
            public double SpringConstant { get; }

            //public void AddForces(double damping, double dt)
            //{
            //    var displacement = Projectile1.Position - Projectile2.Position;
            //    double length = displacement.Magnitude;

            //    double forceMag = (length - unstretchedLength) * springConstant;

            //    var unitD = displacement.UnitVector();
            //    var force1on2 = forceMag * unitD;
            //    var force2on1 = -forceMag * unitD;

            //    Projectile1.AddForce(force2on1);
            //    Projectile2.AddForce(force1on2);

            //    Projectile1.Velocity *= Math.Pow(damping, dt);
            //    Projectile2.Velocity *= Math.Pow(damping, dt);

            //}
        }

        public void AddProjectile(Projectile proj)
        {
            Projectiles.Add(proj);
        }

        public void AddProjectile(Vector position, double mass)
        {
            var proj = new Projectile(position, Vector.NullVector(), mass);
            AddProjectile(proj);
        }

        public void AddConnector(Projectile proj1, Projectile proj2, double springConstant)
        {
            if (proj1 == proj2)
            {
                return;
            }

            foreach (var con in Connectors)
            {
                if ((con.Projectile1 == proj1 && con.Projectile2 == proj2) || (con.Projectile1 == proj2 && con.Projectile2 == proj1))
                {
                    return; // This returns without error because I suspect people would like that behavior
                }
            }
            Connectors.Add(new Connector(proj1, proj2, springConstant));
        }

    }
}
