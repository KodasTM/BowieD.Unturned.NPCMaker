﻿using BowieD.Unturned.NPCMaker.Localization;

namespace BowieD.Unturned.NPCMaker.NPC.Conditions
{
    public sealed class ConditionQuest : Condition
    {
        public override Condition_Type Type => Condition_Type.Quest;
        public ushort ID;
        public Quest_Status Status;
        public Logic_Type Logic;
        [ConditionName("Condition_Reset_Quest_Title")]
        [ConditionTooltip("Condition_Reset_Quest_Tooltip")]
        [ConditionNoValue]
        public bool Reset;
        public override string DisplayName
        {
            get
            {
                string outp = LocUtil.LocalizeCondition("Condition_Type_ConditionQuest") + $" [{ID}] ";
                switch (Logic)
                {
                    case Logic_Type.Equal:
                        outp += "= ";
                        break;
                    case Logic_Type.Not_Equal:
                        outp += "!= ";
                        break;
                    case Logic_Type.Greater_Than:
                        outp += "> ";
                        break;
                    case Logic_Type.Greater_Than_Or_Equal_To:
                        outp += ">= ";
                        break;
                    case Logic_Type.Less_Than:
                        outp += "< ";
                        break;
                    case Logic_Type.Less_Than_Or_Equal_To:
                        outp += "<= ";
                        break;
                }
                outp += LocUtil.LocalizeCondition($"Condition_Status_Enum_{Status.ToString()}");
                return outp;
            }
        }
    }
}