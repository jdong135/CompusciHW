using Arena;
using HungerGames.Animals;
using HungerGames.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames
{
    class HungerGamesTest
    {
        private const double hareToLynxRatio = 10;

        private const int nLynx = 10;
        private const int nHare = (int)(nLynx * hareToLynxRatio);

        private const int arenaHeight = 30;
        private const int arenaWidth = 30;

        public static void Run()
        {
            Registry.Initialize(@"HungerGames\", @"Graphics\");

            HungerGamesArena arena = new HungerGamesArena(arenaWidth, arenaHeight);

            GameMaster master = new GameMaster(arena);

            master.AddChooser(new ChooserJayDong());
            master.AddChooser(new ChooserDefault());

            master.AddAllAnimals(nHare, nLynx);

            var sim = new HungerGamesTestWindow(new ArenaEngineAdapter(arena));
            sim.DisplayCheckBox.IsChecked = false;
            sim.Show();

            
        }
    }
}
