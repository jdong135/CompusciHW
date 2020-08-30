using DongUtility;
using FiniteElement;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Visualizer.MarbleMadness
{
    class YourParticleStructure : ParticleStructure
    {
        public YourParticleStructure()
        {
            const double radius = .01;
            Vector center = new Vector(.01, .01, .51);

            const double eachMass = .005 / 27;
            const double springConstant = .75;
            const double side = .02;
            const int nParticles = 10;

            Projectile[,,] projectiles = new Projectile[nParticles, nParticles, nParticles];
            //projectiles[0, 0, 0] = new Projectile(new Vector(-.4, 0.2, .5), Vector.NullVector(), .005 / 27);
            for (int ix = 0; ix < nParticles; ++ix)
                for (int iy = 0; iy < nParticles; ++iy)
                    for (int iz = 0; iz < nParticles; ++iz)
                    {
                        Vector potentialVector = new Vector((double)ix / nParticles * side,
                            (double)iy / nParticles * side, (double)iz / nParticles * side + .5);
                        double distance = Vector.Distance(potentialVector, center);
                        if (distance <= radius)
                        {
                            var proj = new Projectile(potentialVector, Vector.NullVector(), eachMass);
                            projectiles[ix, iy, iz] = proj;
                        }
                    }

            foreach (var proj in projectiles)
            {
                if(proj != null)
                {
                    AddProjectile(proj);

                }
            }

            for (int ix = 0; ix <= side; ++ix)
                for (int iy = 0; iy <= side; ++iy)
                    for (int iz = 0; iz <= side; ++iz)
                    {
                        if (ix < side)
                        {
                            if (projectiles[ix, iy, iz] != null && projectiles[ix + 1, iy, iz] != null)
                            {
                                AddConnector(projectiles[ix, iy, iz], projectiles[ix + 1, iy, iz], springConstant);
                            }
                        }
                        if (iy < side)
                        {
                            if (projectiles[ix, iy, iz] != null && projectiles[ix, iy + 1, iz] != null)
                            {
                                AddConnector(projectiles[ix, iy, iz], projectiles[ix, iy + 1, iz], springConstant);
                            }
                        }
                        if (iz < side)
                        {
                            if (projectiles[ix, iy, iz] != null && projectiles[ix, iy, iz + 1] != null)
                            {
                                AddConnector(projectiles[ix, iy, iz], projectiles[ix, iy, iz + 1], springConstant);
                            }
                        }
                    }
        }
    }
}
