﻿using BowieD.Unturned.NPCMaker.Localization;
using System;
using System.Text;

namespace BowieD.Unturned.NPCMaker.NPC.Rewards
{
    public sealed class RewardFlagShort : Reward
    {
        public override RewardType Type => RewardType.Flag_Short;
        public override string DisplayName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{LocUtil.LocalizeReward("Reward_Type_RewardFlagShort")} [{ID}] ");
                switch (Modification)
                {
                    case Modification_Type.Assign:
                        sb.Append("= ");
                        break;
                    case Modification_Type.Increment:
                        sb.Append("+ ");
                        break;
                    case Modification_Type.Decrement:
                        sb.Append("- ");
                        break;
                }
                sb.Append(Value);
                return sb.ToString();
            }
        }
        public UInt16 ID;
        public Int16 Value;
        public Modification_Type Modification;
    }
}
