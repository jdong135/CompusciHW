using DongUtility;
using FiniteElement;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using static DongUtility.UtilityFunctions;

namespace Visualizer.MarbleMadness
{
    /// <summary>
    /// A force simulating a set of surfaces that does not let particles go through.
    /// This must be the last of the set of forces to work properly.
    /// This is hardly ideal, but is the best I have right now.
    /// </summary>
    class SurfaceForce : GlobalForce
    {
        /// <summary>
        /// All the surfaces in use
        /// </summary>
        private readonly List<Surface> surfaces = new List<Surface>();
        /// <summary>
        /// The kinematics engine, mainly for the DeltaTime feature
        /// </summary>
        private readonly KinematicsEngine engine;

        public SurfaceForce(KinematicsEngine engine) :
            base(engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Finds the normal force on a particle that is striking a triangle,
        /// given an elasticity.
        /// The perpendicular component of velocity should already have been calculated.
        /// </summary>
        private Vector CalcNormalForce(Projectile projectile, Vector perpendicularVelocity,
            Triangle triangle, double elasticity)
        {
            Vector newPerpendicularVelocity = -perpendicularVelocity * elasticity;
            Vector deltaVPerp = newPerpendicularVelocity - perpendicularVelocity;

            Vector accelerationPerp = deltaVPerp / engine.DeltaTime;

            return projectile.Mass * accelerationPerp + NormalForceToCancel(projectile, triangle);
        }

        /// <summary>
        /// Finds the force of friction on an object, given a friction coefficient and an already calcualted normal force.
        /// The parallel component of velocity is also assumed to have been calculated already
        /// </summary>
        private Vector CalcFrictionForce(Projectile projectile, Vector parallelVelocity,
            Triangle triangle, double frictionCoefficient, Vector normalForce)
        {
            Vector frictionForce = -normalForce.Magnitude * frictionCoefficient * parallelVelocity.UnitVector();
            Vector predictedVelocityParallel = PredictVelocity(projectile, frictionForce).ProjectOnto(frictionForce);

            // Don't use the full friction force if the force will stop the object
            if (!Vector.SameDirection(predictedVelocityParallel, parallelVelocity))
            {
                // Instead, just apply enough force to stop it
                return ForceToStop(predictedVelocityParallel, projectile.Mass);
            }
            else
            {
                return frictionForce;
            }
        }

        /// <summary>
        /// The amount of force needed to stop a projectile moving at a given velocity
        /// </summary>
        private Vector ForceToStop(Vector velocity, double mass)
        {
            Vector deltaV = -velocity;
            Vector acceleration = deltaV / engine.DeltaTime;
            return mass * acceleration;
        }


        /// <summary>
        /// Calculates the normal force needed to cancel the existing perpendicular forces on an object
        /// </summary>
        private Vector NormalForceToCancel(Projectile projectile, Triangle triangle)
        {
            Vector currentForcePerp = ProjectInSameDirection(projectile.NetForce, triangle.Normal);
            return -currentForcePerp;
        }

        /// <summary>
        /// Returns the projection of vec1 onto vec2 if they point in the same direction, zero otherwise;
        /// </summary>

        static private Vector ProjectInSameDirection(Vector vec1, Vector vec2)
        {
            if (Vector.SameDirection(vec1, vec2))
            {
                return vec1.ProjectOnto(vec2);
            }
            else
            {
                return Vector.NullVector();
            }
        }

        /// <summary>
        /// Predicts the velocity of the projectile in the next time step
        /// </summary>
        /// <param name="extraForce">The proposed external force acting on the particle (from the surface, in addition to the other forces already acting on it)</param>
        private Vector PredictVelocity(Projectile projectile, Vector extraForce)
        {
            Vector newAcceleration = (projectile.NetForce + extraForce) / projectile.Mass;
            return projectile.Velocity + newAcceleration * engine.DeltaTime;
        }

        /// <summary>
        /// Predicts the position of the particle in the next time step
        /// </summary>
        /// <param name="extraForce">The proposed external force acting on the particle (from the surface, in addition to the other forces already acting on it)</param>
        private Vector PredictPosition(Projectile projectile, Vector extraForce)
        {
            return projectile.Position + PredictVelocity(projectile, extraForce) * engine.DeltaTime;
        }

        /// <summary>
        /// Returns the force on the particle, corrected to ensure that it does not penetrate the triangle.
        /// It does this by predicting the final position, and correcting it using kinematics if it is the wrong one.
        /// This varies the proposed deltaV slightly but keeps the integrity of the surface.
        /// </summary>
        private bool CheckForce(Projectile projectile, Vector proposedForce, Triangle triangle)
        {
            Vector predictedVelocity = PredictVelocity(projectile, proposedForce);
            Vector intersectionPoint = triangle.Intersection(projectile.Position, predictedVelocity);
            double distanceFromTriangle = Vector.Distance(intersectionPoint, projectile.Position);
            Vector predictedPosition = PredictPosition(projectile, proposedForce);
            double predictedDistance = Vector.Distance(predictedPosition, projectile.Position);

            Vector currentToPredictedPosition = predictedPosition - projectile.Position;
            Vector forceDirection = (intersectionPoint - projectile.Position).ProjectOnto(triangle.Normal);

            return !Vector.SameDirection(currentToPredictedPosition, forceDirection)
                || predictedDistance <= distanceFromTriangle;
            //    return newForce;
            //else
            //{
            //    return AdjustForceForPosition(projectile, proposedForce, intersectionPoint);
            //}
        }

        private bool IntersectsATriangle(Vector initial, Vector final, Triangle notThisOne = null)
        {
            foreach (var surface in surfaces)
                foreach (var triangle in surface.Triangles)
                {
                    if (triangle == notThisOne)
                        continue;

                    if (triangle.PassedThrough(initial, final))
                    {
                        return true;
                    }

                }
            return false;
        }

        private Vector GetInteraction(Vector initial, Vector final, Triangle triangle)
        {
            const double scale = .99;

            Vector interactionPoint = triangle.Intersection(initial, final);
            while (triangle.PassedThrough(initial, interactionPoint))
            {
                Vector direction = interactionPoint - initial;
                direction *= scale;
                interactionPoint = direction + initial;
            }
            return interactionPoint;
        }

        private Vector ChooseNewFinalPosition(Projectile projectile, Vector finalPosition)
        {
            Vector finalPoint = Vector.NullVector();
            bool worked = false;
            if (finalPosition.Y < 0)
            {
                int q = 0;
            }
            foreach (var surface in surfaces)
                foreach (var triangle in surface.Triangles)
                {
                    if (triangle.PassedThrough(projectile.Position, finalPosition))
                    {
                        Vector interactionPoint = GetInteraction(projectile.Position, finalPosition, triangle);

                        if (!IntersectsATriangle(projectile.Position, interactionPoint))
                        {
                            if (!worked)
                            {
                                finalPoint = interactionPoint;
                                worked = true;
                            }
                            else
                            {
                                double currentDistance = Vector.Distance(finalPoint, projectile.Position);
                                double newDistance = Vector.Distance(interactionPoint, projectile.Position);
                                if (currentDistance > newDistance)
                                    finalPoint = interactionPoint;
                            }
                        }
                    }

                }
            if (!worked)
                throw new InvalidOperationException("Somehow no point works?");

            return finalPoint;
        }

        /// <summary>
        /// Adjusts the proposed force to make sure the projectile's final position is at the intersectionPoint given.
        /// </summary>
        /// <param name="projectile">The projectile in question</param>
        /// <param name="proposedForce">The suggested (but wrong) force</param>
        /// <param name="target">The place the projectile needs to end up</param>
        /// <returns>A new force that puts the particle at the right place in the next time step</returns>
        private Vector AdjustForceForPosition(Projectile projectile, Vector proposedForce, Vector target)
        {
            // Kinematic equation: deltaX = v0 * t + 1/2 * a * t^2
            // So a = (deltaX - v0 * t) * 2 / t^2

            Vector predictedVelocity = PredictVelocity(projectile, proposedForce);
            double distanceFromTarget = Vector.Distance(target, projectile.Position);
            Vector deltaX = target - projectile.Position;
            //Vector velocityToward = predictedVelocity.ProjectOnto(target - projectile.Position);
            //Vector velocityToward = predictedVelocity;
            //double initialSpeed = velocityToward.Magnitude;
            //double accelerationMag = (distanceFromTarget - initialSpeed * engine.DeltaTime) * 2 / (UtilityFunctions.Square(engine.DeltaTime));
            //Vector acceleration = -accelerationMag * velocityToward.UnitVector();
            Vector acceleration = 2 / Square(engine.DeltaTime) * (deltaX - projectile.Velocity * engine.DeltaTime);
            Vector newForce = acceleration * projectile.Mass;
            //Vector forceToward = proposedForce.ProjectOnto(target - projectile.Position);
            //Vector finalForce = proposedForce - forceToward + newForce;
            Vector finalForce = newForce - projectile.NetForce;


            // Check it
            const double scaleUp = 1.01;
            const int maxTries = 100;
            int tries = 0;

            // This can also hang.  It that better?
            //while (IntersectsATriangle(projectile.Position, PredictPosition(projectile, finalForce)))
            //{
            //    Vector position = PredictPosition(projectile, finalForce);
            //    if (tries++ >= maxTries)
            //    {
            //        finalForce /= scaleUp;
            //    }
            //    else
            //    {
            //        finalForce *= scaleUp;
            //    }
            //}
            return finalForce;
        }

        //        private class TriangleForces
        //        {
        //    private readonly Dictionary<Triangle, Vector> forces = new Dictionary<Triangle, Vector>();

        //    public void AddForce(Triangle triangle, Vector force)
        //    {
        //        if (forces.ContainsKey(triangle))
        //        {
        //            forces[triangle] = force;
        //        }
        //        else
        //        {
        //            forces.Add(triangle, force);
        //        }
        //    }

        //    public Vector TotalForce
        //    {
        //        get
        //        {
        //            Vector totalForce = Vector.NullVector();

        //            foreach (var force in forces.Values)
        //            {
        //                totalForce += force;
        //            }

        //            return totalForce;
        //        }
        //    }
        //}

        override protected Vector GetForce(Projectile projectile)
        {
            //TriangleForces forces = new TriangleForces();

            //const int maxTries = 100;

            //// Note the danger of an infinite loop, a real if hopefully remote possibility!
            //for (int i = 0; i < maxTries; ++i)
            //{
            //    bool keepGoing = CheckAndGetForce(projectile);

            //    // If nothing was found, then we're done
            //    if (!keepGoing)
            //    {
            //        return forces.TotalForce;
            //    }
            //}
            //throw new InvalidOperationException("Got stuck in an infinite loop in surface detection!");
            Vector force = CheckAndGetForce(projectile);
            Vector finalPosition = PredictPosition(projectile, force);
            if (IntersectsATriangle(projectile.Position, finalPosition))
            {
                Vector newPosition = ChooseNewFinalPosition(projectile, finalPosition);
                force = AdjustForceForPosition(projectile, force, newPosition);
                Vector tryAgain = PredictPosition(projectile, force);
                //if (IntersectsATriangle(projectile.Position, tryAgain))
                //    throw new ExecutionEngineException("PLA!");
            }
            return force;
        }

        /// <summary>
        /// Checks all triangles in all surfaces for a contact, and returns the force from the first contact found
        /// </summary>
        /// <param name="projectile">The projectile in question</param>
        /// <param name="currentForce">The current total surface force acting on the object</param>
        /// <returns>The force from the first triangle contact found</returns>
        private Vector CheckAndGetForce(Projectile projectile)
        {
            Vector totalForce = Vector.NullVector();
            Vector nextLocation = PredictPosition(projectile, totalForce);
            foreach (var surface in surfaces)
                foreach (var triangle in surface.Triangles)
                {
                    if (triangle.PassedThrough(projectile.Position, nextLocation))
                    {
                        if (totalForce.MagnitudeSquared > 0)
                        {
                            int j = 0;
                        }
                        totalForce += ForceFromTriangle(projectile, surface, triangle, totalForce);
                    }
                }
            return totalForce;
        }

        private Vector ForceFromTriangle(Projectile projectile, Surface surface, Triangle triangle, Vector currentForce)
        {
            Vector velocity = PredictVelocity(projectile, currentForce);
            Vector perpendicularComponent = velocity.ProjectOnto(triangle.Normal);
            Vector parallelComponent = velocity - perpendicularComponent;

            Vector normalForce = CalcNormalForce(projectile, perpendicularComponent, triangle, surface.Elasticity);
            Vector frictionForce = CalcFrictionForce(projectile, parallelComponent, triangle, surface.FrictionCoefficient, normalForce);
            Vector force = normalForce + frictionForce;
            //force = CheckForce(projectile, force, currentForce, triangle);

            return force;
        }

        public void AddSurface(Surface surface)
        {
            surfaces.Add(surface);
        }
    }
}
