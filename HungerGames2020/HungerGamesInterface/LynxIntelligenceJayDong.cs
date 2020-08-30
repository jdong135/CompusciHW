using Arena;
using DongUtility;
using HungerGames.Animals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace HungerGames.Interface
{
    public class LynxIntelligenceJayDong : LynxIntelligence
    {
        public override Color Color { get { return Color.MidnightBlue; } }
        //Predator name
        public override string Name { get { return "Jay's Predator"; } }
        public override string BitmapFilename {  get { return "JayLynx.jpg"; } }

        public override Turn ChooseTurn()
        {
            var hares = GetOtherAnimals<Hare>().OrderBy(hare => (hare.Position - Position).Magnitude);
            int numharesnearby = 0;
            foreach (var hare in hares)
            {
                numharesnearby += 1;
            }
            if(numharesnearby == 0)
            {
                if (Position.X < 1)
                {
                    return ChangeVelocity(new Vector2D(15, 0).UnitVector() * 50);
                }
                else if (Position.Y < 1)
                {
                    var velocity = new Vector2D(0, 15) * 50;
                    return ChangeVelocity(velocity - Velocity);
                }
                else if (Position.X > 29)
                {
                    var velocity = new Vector2D(-15, 0) * 50;
                    return ChangeVelocity(velocity - Velocity);
                }
                else if (Position.Y > 29)
                {
                    var velocity = new Vector2D(0, -15) * 50;
                    return ChangeVelocity(velocity - Velocity);
                }
                else
                {
                    return ChangeVelocity(new Vector2D(15*Random.NextDouble(), 15*Random.NextDouble()).UnitVector() * 50);
                }
            }
            else
            {
                var listen = Listen();
                int numSounds = 0;
                foreach (var sound in listen)
                {
                    numSounds += 1;
                    //Run in opposite direction if sound code is from your own hare
                    if (sound.SoundCode == 64)
                    {
                        var hareX = sound.Direction.X;
                        var hareY = sound.Direction.Y;
                        var velocity = new Vector2D(-hareX, hareY).UnitVector() * 50;
                        return ChangeVelocity(velocity - Velocity);
                    }
                    //Otherwise chase
                    else
                    {
                        var hareX = sound.Direction.X;
                        var hareY = sound.Direction.Y;
                        var velocity = new Vector2D(hareX, hareY).UnitVector() * 50;
                        return ChangeVelocity(velocity - Velocity);
                    }
                }
                if(numSounds == 0)
                {
                    
                        var targetHare = hares.ElementAt(0);
                        var velocity = new Vector2D(targetHare.Position.X, targetHare.Position.Y).UnitVector() * 50;
                        return ChangeVelocity(velocity - Velocity);
                }
            }
            
            //If there is no sound being produced nearby
            //if(numSounds == 0)
            //{
            //    var velocity = new Vector2D(3 * Random.NextDouble(), 3 * Random.NextDouble()).UnitVector() * 50;
            //    return ChangeVelocity(velocity - Velocity);
            //}
            
            //Default return
            return ChangeVelocity(Vector2D.PolarVector(1, Random.NextDouble(0, 2 * Math.PI)));
        }
    }
}
