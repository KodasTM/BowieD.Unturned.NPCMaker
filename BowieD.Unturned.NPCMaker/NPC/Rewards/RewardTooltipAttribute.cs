﻿using BowieD.Unturned.NPCMaker.Localization;
using System;

namespace BowieD.Unturned.NPCMaker.NPC.Rewards
{
    public class RewardTooltipAttribute : Attribute
    {
        public readonly string Text;
        public RewardTooltipAttribute(string key)
        {
            Text = LocUtil.LocalizeReward(key);
        }
    }
}
