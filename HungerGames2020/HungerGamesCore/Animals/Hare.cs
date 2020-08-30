﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using DongUtility;
using HungerGames.Interface;
using HungerGamesCore.Interface;
using HungerGamesCore.Terrain;

namespace HungerGames.Animals
{
    abstract public class Hare : Animal
    {
        static private readonly AnimalStats hareStats = new AnimalStats()
        {
            BaseWalkingVolume = 1e-8,
            HearingThreshold = 1e-9,
            MaxAcceleration = 3,
            MaxSpeed = 12,
            MaxStamina = 75,
            MaxVocalizationVolume = 1e-4,
            MaxWalkingVolume = 1e-6,
            StaminaPerSecondAtTopSpeed = 7,
            StaminaRestoredPerSecond = 2,
            StepTime = .25,
            WalkingSpeed = .5,
            VisionBase = 40
        };

        private const double hareWidth = .2;
        private const double hareLength = .2;

        public Hare(HareIntelligence intel) :
            base(intel, hareStats, hareLength, hareWidth) // So the hare is horizontal
        { }

        public override bool IsPassable(ArenaObject mover)
        {
            if (mover == null)
                return false;
            else
                return !(mover is Hare);
        }
    }
}
