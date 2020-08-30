using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HungerGames
{
    public class HareIntelligenceJayDong : HareIntelligence
    {
        public override Color Color { get { return Color.MidnightBlue; } }
        //Herbivore name
        public override string Name { get { return "Jay's Herbivore"; } }
        public override string BitmapFilename { get { return "JayHare.jpg"; } }

        public override Turn ChooseTurn()
        {
            var animals = GetOtherAnimals<Animal>(); // All animals
            var animalsList = animals.ToList();
            //var lynxes = GetOtherAnimals<Lynx>();
            var trees = GetObstacles<Tree>();

            //You can iterate through GetOtherAnimals
            foreach (var animal in animals)
            {

                //You can test if it's battle royale and different game modes
                if (IsBattleRoyale)
                {

                }
            }

            //TEST CODE

            var lynxes = GetOtherAnimals<Lynx>().OrderBy(lynx => (lynx.Position - Position).Magnitude);
            var rabbits = GetOtherAnimals<Hare>().OrderBy(shrub => (shrub.Position - Position).Magnitude);
            var shrubs = GetObstacles<Shrub>().ToList();
            var lynxesnear = new List<VisibleAnimal>();
            var lynxescoming = new List<VisibleAnimal>();
            Vector2D shrubLocation = Position - shrubs[0].Position;


            foreach (var lynx in lynxes)
            {
                bool lynxNear = (lynx.Position - Position).MagnitudeSquared < 25;
                bool lynxComing = (Math.Sign(lynx.Position.X - Position.X) != Math.Sign(lynx.Velocity.X)) && (Math.Sign(lynx.Position.Y - Position.Y) != Math.Sign(lynx.Velocity.Y));

                if (lynxNear && lynxComing) { lynxescoming.Add(lynx); }
                if (lynxNear && !lynxComing) { lynxesnear.Add(lynx); }

                //This is basically saying if the lynx is near, run!
                if((lynxNear && lynxComing) || (lynxNear && !lynxComing))
                {
                    double lynxX = lynxescoming.Sum(lynx => lynx.Position.X - Position.X);
                    double lynxY = lynxescoming.Sum(lynx => lynx.Position.Y - Position.Y);

                    //Run away from the lynx in the opposite direction
                    var velocity = new Vector2D(lynxY, lynxX).UnitVector() * 100;

                    return ChangeVelocity(velocity - Velocity);
                }

                else
                {
                    Vocalize(100, 64);
                }
            }

            //Default to go if no lynxes around
            return ChangeVelocity(shrubLocation.UnitVector() * 5);
            //END TEST CODE
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }

    }
}
