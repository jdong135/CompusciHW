using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Thermodynamics
{
    public class JayDongReactionClass
    {
        public List<string> reactants { get; set; }
        public List<string> products { get; set; }
        public double forwardProbablity { get; set; }
        public double backwardProbability { get; set; }
        public bool reversible { get; set; } = false;
        //Input a reaction like "NH3+HCl->NH4Cl, 1, 0" (100% of time it goes forward)
        public JayDongReactionClass(String reaction, double forward, double backward)
        {
            forwardProbablity = forward;
            backwardProbability = backward;
            if (reaction.Contains("<")) { reversible = true; }
            if (!reversible)
            {
                string reactantString = reaction.Substring(0, reaction.IndexOf("-"));
                reactants = reactantString.Split('+').OfType<string>().ToList();
                string productString = reaction.Substring(reaction.IndexOf(">")+1);
                products = productString.Split('+').OfType<string>().ToList();
            }
            else
            {
                string reactantString = reaction.Substring(0, reaction.IndexOf("<"));
                reactants = reactantString.Split('+').OfType<string>().ToList();
                string productString = reaction.Substring(reaction.IndexOf(">")+1);
                products = productString.Split('+').OfType<string>().ToList();
            }
        }
        
    }
}
