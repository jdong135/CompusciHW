using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Thermodynamics
{
    public class MyReactingParticleContainer : ReactingParticleContainer
    {
        List<JayDongReactionClass> ReactionList = new List<JayDongReactionClass>();
        public MyReactingParticleContainer(double side, double collisionRadius) :
            base(side, collisionRadius)
        { }
        public void AddRxn(string rxn, double forwardProbability, double backwardProbability)
        {
            ReactionList.Add(new JayDongReactionClass(rxn, forwardProbability, backwardProbability));
        }
        //Code to do reaction logic
        protected override void React(Particle particle, List<Particle> nearby)
        {
            Boolean catalystPresent = false;
            double activationEnergy = 0;
            int numInfected = GetNParticles("Infected");
            //Remove the particles in nearby list that are farther than collision radius from particle
            foreach (Particle p in nearby)
            {
                if ((p.Position - particle.Position).Magnitude > CollisionRadius)
                {
                    nearby.Remove(p);
                }
                if(p.Info.Name == "Promoter") {  catalystPresent = true; }
            }
            if (catalystPresent) { activationEnergy = 0; }
            else { activationEnergy = 1e-20; }
            
            //Add particle to nearby because you need to include the particle in your checks
            nearby.Add(particle);
            //Create a list of names for nearby
            List<string> nearbyNames = new List<string>();
            foreach (Particle p in nearby)
                nearbyNames.Add(p.Info.Name);
            //For each loop through reaction list
            foreach (JayDongReactionClass rxn in ReactionList)
            {
                //Reactant momentums & positions
                Vector momentums = new Vector();
                Vector positions = new Vector();
                //Check if nearbyNames contains rxn reactants
                if(rxn.reactants.All(elem => nearbyNames.Contains(elem))){
                    double kineticEnergy = 0;
                    Random rand = new Random();
                    double randomNum = rand.NextDouble();
                    if(numInfected >= 50)
                    {
                        rxn.forwardProbablity += .3;
                    }
                    if(randomNum <= rxn.forwardProbablity)
                    {
                        List<int> indicesToRemove = new List<int>();
                        List<Particle> particlesToRemove = new List<Particle>();
                        //Remove the reactants
                        foreach (String s in rxn.reactants)
                        {
                            int indexToRemove = nearbyNames.IndexOf(s);
                            momentums += (nearby[indexToRemove].Momentum);
                            positions += (nearby[indexToRemove].Position);
                            kineticEnergy += nearby[indexToRemove].KineticEnergy;
                            Particle p = nearby[indexToRemove];
                            particlesToRemove.Add(p);
                            indicesToRemove.Add(indexToRemove);
                        }
                        if(kineticEnergy >= activationEnergy)
                        {
                            foreach(Particle p in particlesToRemove)
                            {
                                RemoveParticle(p);
                                nearby.Remove(p);
                                nearbyNames.Remove(p.Info.Name);
                            }
                            //Create the products
                            foreach (String s in rxn.products)
                            {
                                var particleInfo = Dictionary.Map[s];
                                double mass = particleInfo.Mass;
                                double randX = RandomUtility.NextDouble(rand, -2, 2);
                                double randY = RandomUtility.NextDouble(rand, -2, 2);
                                double randZ = RandomUtility.NextDouble(rand, -2, 2);
                                //Add particle
                                AddParticle(s, positions / (rxn.reactants.Count) + new Vector(randX, randY, randZ), momentums / mass);
                                //Rectants momentum = products momentum --> mv + mv = mv
                            }
                        }
                    }
                }
                //If nearyNames contains products, it must be a reversible reaction
                else if(rxn.products.All(elem => nearbyNames.Contains(elem)) && rxn.reversible == true){
                    double kineticEnergy = 0;
                    Random rand = new Random();
                    double randomNum = rand.NextDouble();
                    if(randomNum <= rxn.backwardProbability)
                    {
                        List<int> indicesToRemove = new List<int>();
                        List<Particle> particlesToRemove = new List<Particle>();
                        //Remove the products
                        foreach (String s in rxn.products)
                        {
                            int indexToRemove = nearbyNames.IndexOf(s);
                            momentums += (nearby[indexToRemove].Momentum);
                            positions += (nearby[indexToRemove].Position);
                            Particle p = nearby[indexToRemove];
                            particlesToRemove.Add(p);
                            indicesToRemove.Add(indexToRemove);

                            //RemoveParticle(p);
                            //nearby.RemoveAt(indexToRemove);
                            //nearbyNames.RemoveAt(indexToRemove);
                        }
                        if(kineticEnergy >= activationEnergy)
                        {
                            foreach(Particle p in particlesToRemove)
                            {
                                RemoveParticle(p);
                                nearby.Remove(p);
                                nearbyNames.Remove(p.Info.Name);
                            }
                            //Add the reactants
                            foreach (String s in rxn.reactants)
                            {
                                var particleInfo = Dictionary.Map[s];
                                double mass = particleInfo.Mass;
                                //Add particle
                                AddParticle(s, positions / (rxn.products.Count), momentums / mass);
                            }
                        }
                    }
                }
            }
        }
    }
}
