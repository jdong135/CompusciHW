﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using HungerGames.Interface;
using HungerGamesCore.Interface;

namespace HungerGames.Animals
{
    abstract public class Lynx : Animal
    {
        static private readonly AnimalStats lynxStats = new AnimalStats()
        {
            BaseWalkingVolume = 5e-8,
            HearingThreshold = 1e-10,
            MaxAcceleration = 4,
            MaxSpeed = 22,
            MaxStamina = 100,
            MaxVocalizationVolume = 1e-2,
            MaxWalkingVolume = 1e-5,
            StaminaPerSecondAtTopSpeed = 10,
            StaminaRestoredPerSecond = 1,
            StepTime = .3,
            WalkingSpeed = 1,
            VisionBase = 100
        };

        private const double lynxWidth = .5;
        private const double lynxLength = .5;

        public Lynx(AnimalIntelligence intel) :
            base(intel, lynxStats, lynxLength, lynxWidth)
        { }

        public override bool IsPassable(ArenaObject mover)
        {
            if (mover == null)
                return false;
            else
                return !(mover is Lynx);
        }
    }
}
