using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using HungerGames.Animals;
using HungerGames.Interface;
using DongUtility;
using HungerGamesCore.Terrain;

namespace HungerGames
{
    public class HungerGamesArena : ArenaEngine
    {
        private const string backgroundFilename = "dirt.jpg";
        private readonly double maxTime;

        public bool IsBattleRoyale { get; set; } = false;
        public HungerGamesArena(double width, double height, double maxTime = double.MaxValue, int xDivs = 10, int yDivs = 10) :
            base(width, height, backgroundFilename, xDivs, yDivs)
        {
            this.maxTime = maxTime;
        }

        static private object locker = new object();

        protected override void UserDefinedBeginningOfTurn()
        {
            base.UserDefinedBeginningOfTurn();

            if (colors == null)
            {
                colors = new Dictionary<string, System.Drawing.Color>();
                foreach (var animal in GetObjectsOfType<Animal>())
                {
                    if (!colors.ContainsKey(animal.Name))
                    {
                        colors.Add(animal.Name, animal.Color);
                    }
                }
            }
        }

        private Dictionary<string, System.Drawing.Color> colors = null;

        public void AddAnimalsPossible<T>(int num) where T : Animals.Animal, new()
        {
            var animalForChecking = new T();
            for (int i = 0; i < num;)
            {
                double xCoord = Random.NextDouble(0, Width);
                double yCoord = Random.NextDouble(0, Height);
                var location = new Vector2D(xCoord, yCoord);
                if (IsOccupied(location, animalForChecking))
                {
                    var ani = new T();
                    AddObject(ani, location);
                    ++i;
                }
            }
        }

        public IEnumerable<Animal> GetOtherAnimals(Animal ani)
        {
            foreach (var animal in GetObjectsOfType<Animal>())
            {
                if (ani != animal)
                    yield return animal;
            }
        }

        private const double treesPerSquareMeter = .05;
        private const double shrubsPerSquareMeter = .025;
        private const double areaOfMaxHeight = 10;
        private const double maxGrassHeight = .5;
        private const int nGrassTries = 5;
        private const int nWaterTries = 3;
        private const double maxWaterWidth = 3;
        private const double maxWaterLength = 5;

        public override void Initialize()
        {
            for (int i = 0; i < nWaterTries; ++i)
            {
                PlaceWater();
            }
            PlacePlants<Tree>(treesPerSquareMeter);
            PlacePlants<Shrub>(shrubsPerSquareMeter);
            for (int i = 0; i < nGrassTries; ++i)
            {
                PlaceGrass(new Vector2D(Random.NextDouble(0, Width), Random.NextDouble(0, Height)));
            }
        }

        private void PlaceWater()
        {
            Rectangle rectangle;
            do
            {
                var position = new Vector2D(Random.NextDouble(0, Width), Random.NextDouble(0, Height));
                rectangle = new Rectangle(position, Random.NextDouble(0, maxWaterWidth),
                    Random.NextDouble(0, maxWaterLength));
            } while (!IsValidLocation(rectangle));
            AddObject(new Water(rectangle.Width, rectangle.Height),
                rectangle.Center);
        }
        private void PlacePlants<T>(double concentration) where T : Obstacle, new()
        {
            double area = Width * Height;
            double nTrees = area * concentration;

            for (int i = 0; i < nTrees; ++i)
            {
                bool isPlaced = false;
                var plant = new T();
                while (!isPlaced)
                {
                    var location = new Vector2D(Random.NextDouble(0, Width), Random.NextDouble(0, Height));
                    if (IsValidLocation(new Rectangle(location, plant.Size.Width, plant.Size.Height)))
                    {
                        AddObject(plant, location);
                        isPlaced = true;
                    }
                }
            }
        }

        private void PlaceGrass(Vector2D center)
        {
            var plants = GetObjectsOfType<Obstacle>();

            if (Occupied(new Rectangle(center, 1, 1), plants))
                return;

            double width = 1;
            double length = 1;

            bool keepGoing = true;
            while (keepGoing)
            {
                keepGoing = false;
                var rect1 = new Rectangle(center, width + 1, length);
                if (!Occupied(rect1, plants) && TestRectangle(rect1))
                {
                    width += 1;
                    keepGoing = true;
                }
                var rect2 = new Rectangle(center, width, length + 1);
                if (!Occupied(rect2, plants) && TestRectangle(rect2))
                {
                    length += 1;
                    keepGoing = true;
                }
            }

            double area = length * width;
            double height = area / areaOfMaxHeight * maxGrassHeight;
            AddObject(new Grass(width, length, height), center);
        }

        private bool Occupied(Rectangle rect, IEnumerable<Obstacle> plants)
        {
            foreach (var plant in plants)
            {
                if (plant.Size.Overlaps(rect))
                    return true;
            }
            return false;
        }

        public IEnumerable<string> GetNames<T>() where T : Animal
        {
            HashSet<string> response = new HashSet<string>();

            foreach (var ani in GetObjectsOfType<T>())
            {
                response.Add(ani.Name);
            }

            return new List<string>(response);
        }

        protected override void UserDefinedEndOfTurn()
        {
            foreach (var lynx in GetObjectsOfType<Lynx>())
            {
                double radius = Math.Max(lynx.Size.Width, lynx.Size.Height);
                foreach (var hare in GetNearbyObjects<Hare>(lynx.Position, radius))
                {
                    if (!hare.Dead && hare.Overlaps(lynx))
                    {
                        hare.Dead = true;
                        RemoveObjectDelay(hare);
                    }
                }
            }
        }

        protected override bool Done()
        {
            return Time >= maxTime || GetObjectsOfType<Hare>().Count() <= 0;
        }
    }
}
