using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;

namespace HungerGames
{
    public class ChooserJayDong : LocationChooserTemplateIntermediate<HareIntelligenceJayDong, LynxIntelligenceJayDong>
    {
        protected override Vector2D UserDefinedChooseLocation(VisibleArena arena, bool hare, int organismNumber)
        {
            // Fill in your decision algorithm here.
            // The organismNumber variable counts from 0 to the total number of organisms.  This can be useful for
            // spreading your organisms out a bit

            var water = arena.GetObstacles<Water>();
            List<Vector2D> ponds = new List<Vector2D>();
            var trees = arena.GetObstacles<Tree>();
            List<Vector2D> treeLocations = new List<Vector2D>();
            var shrubs = arena.GetObstacles<Shrub>();
            List<Vector2D> shrubLocations = new List<Vector2D>();
            var grass = arena.GetObstacles<Grass>();
            List<Vector2D> grassLocations = new List<Vector2D>();
            foreach (var pond in water)
                ponds.Add(pond.Position);
            foreach (var tree in trees)
                treeLocations.Add(tree.Position);
            foreach (var shrub in shrubs)
                shrubLocations.Add(shrub.Position);
            foreach (var lawn in grass)
                grassLocations.Add(lawn.Position);

            //Spawn mechanism for hares
            if (hare)
            {
                //We want to spawn in trees, shrubs, or grass
                int obstacleChoice = Random.Next(0, 3);
                //Tree
                if (obstacleChoice == 0)
                {
                    //Pick a random tree to spawn in
                    int treeIndex = Random.Next(0, treeLocations.Count);
                    Vector2D position = treeLocations[treeIndex];
                    //We don't want to overlap so we alter the position in case of overlap
                    double jitterX = .5 * Random.NextDouble();
                    double jitterY = .5 * Random.NextDouble();
                    return position + new Vector2D(jitterX, jitterY);
                }
                //Shrub
                else if (obstacleChoice == 1)
                {
                    int shrubIndex = Random.Next(0, shrubLocations.Count);
                    Vector2D position = shrubLocations[shrubIndex];
                    double jitterX = .5 * Random.NextDouble();
                    double jitterY = .5 * Random.NextDouble();
                    return position + new Vector2D(jitterX, jitterY);
                }
                //Grass
                else
                {
                    int grassIndex = Random.Next(0, grassLocations.Count);
                    Vector2D position = grassLocations[grassIndex];
                    double jitterX = .5 * Random.NextDouble();
                    double jitterY = .5 * Random.NextDouble();
                    return position + new Vector2D(jitterX, jitterY);
                }
            }
            //Spawn mechanism for lynx
            //I think the best strat is to just randomly spawn them since you don't know where the opponents are
            else
            {
                return new Vector2D(30 * Random.NextDouble(), 30 * Random.NextDouble());
            }

            return RandomLocation(arena, hare);
        }
    }
}
