using DongUtility;
using FiniteElement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Visualizer.FiniteElement
{
    class GroundForce : GlobalForce
    {
        private Vector finalPosition = Vector.NullVector();
        private KinematicsEngine engine;

        private Vector forceLastTime = Vector.NullVector();
        private double formerTime = 0;

        private Vector frictionForce = new Vector();
        private double frictionConstant = 1;

        public GroundForce(KinematicsEngine engine) :
            base(engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// The condition that must be met for the force to "turn on"
        /// Otherwise, nothing happens
        /// </summary>
        protected bool ConditionMet(Projectile projectile)
        {
            return projectile.Position.Z <= 0;
        }

        override protected Vector GetForce(Projectile projectile)
        {
            Vector response = Vector.NullVector();

            if (engine.Time > 0 && ConditionMet(projectile))
            {
                if (projectile.Velocity.Z < 0)
                {
                    projectile.Velocity = new Vector(projectile.Velocity.X, projectile.Velocity.Y, -projectile.Velocity.Z);
                    // normal is perpendicular to velocity
                    double normalForce = projectile.Mass * 9.8;
                    Vector frictionDirection = new Vector(-projectile.Velocity.X, -projectile.Velocity.Y, 0);
                    frictionForce = frictionConstant * normalForce * frictionDirection.UnitVector();
                }
            }

            formerTime = engine.Time;
            forceLastTime = response;
            return frictionForce;
        }
    }
}
