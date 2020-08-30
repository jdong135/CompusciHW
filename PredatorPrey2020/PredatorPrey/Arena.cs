using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredatorPrey
{
    public class Arena
    {
        public double Width { get; }
        public double Height { get; }

        public Arena(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public List<Animal> Animals { get; } = new List<Animal>();

        public void AddAnimal(Animal ani)
        {
            Animals.Add(ani);
        }

        public void RemoveAnimal(Animal ani)
        {
            toBeRemoved.Add(ani);
        }

        private List<Animal> toBeRemoved = new List<Animal>();

        public double Time { get; private set; }

        public bool Update(double newTime)
        {
            double deltaT = newTime - Time;
            Time = newTime;

            Animals.ForEach((x) => {
                x.Update(deltaT);
            });

            toBeRemoved.ForEach((x) => Animals.Remove(x));
            toBeRemoved.Clear();

            return !EndCondition();
        }

        private bool EndCondition()
        {
            return Animals.Count <= 1;
        }
    }
}
