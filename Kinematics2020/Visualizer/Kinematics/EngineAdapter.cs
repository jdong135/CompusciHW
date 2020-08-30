using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework_2;

namespace Visualizer.Kinematics
{
    class EngineAdapter : IEngine
    {
        private World world;
        private List<IProjectile> projectileList = new List<IProjectile>();
        public EngineAdapter(World world)
        {
            this.world = world;
            foreach(var projectile in this.world.ProjectileList)
            {
                //ProjectileAdapter implements IProjectile (is of type IProjectile)
                projectileList.Add(new ProjectileAdapter(projectile));
            }
        }
        public List<IProjectile> Projectiles => projectileList;

        public double Time => world.Time;

        public bool Tick(double newTime)
        {
            world.IncrementAll(newTime-world.Time); 
            world.AdvanceTime(newTime-world.Time);
            if(world.Time <= 30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
