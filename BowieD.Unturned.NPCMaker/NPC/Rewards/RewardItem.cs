﻿using BowieD.Unturned.NPCMaker.Localization;
using System;

namespace BowieD.Unturned.NPCMaker.NPC.Rewards
{
    public sealed class RewardItem : Reward
    {
        public override RewardType Type => RewardType.Item;
        public override string DisplayName
        {
            get
            {
                return $"{LocUtil.LocalizeReward("Reward_Type_RewardItem")} {ID} x{Amount}";
            }
        }
        public UInt16 ID;
        public byte Amount;
        [RewardOptional(0, 0)]
        public UInt16 Sight;
        [RewardOptional(0, 0)]
        public UInt16 Tactical;
        [RewardOptional(0, 0)]
        public UInt16 Grip;
        [RewardOptional(0, 0)]
        public UInt16 Barrel;
        [RewardOptional(0, 0)]
        public UInt16 Magazine;
        [RewardOptional(0, 0)]
        public byte Ammo;
        public bool Auto_Equip;
    }
}