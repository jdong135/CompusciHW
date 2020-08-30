﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arena;
using HungerGames.Animals;

namespace HungerGames.Interface
{
    abstract public class LocationChooserTemplateIntermediate<Intel1, Intel2> : LocationChooser where Intel1 : HareIntelligence where Intel2 : LynxIntelligence
    {
        public override Animal MakeOrganism(bool hare)
        {
            if (hare)
                return MakeOrganism<Intel1>(hare);
            else
                return MakeOrganism<Intel2>(hare);
        }
    }
}
