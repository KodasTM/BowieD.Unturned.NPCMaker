﻿using System;
using System.Linq;

namespace BowieD.Unturned.NPCMaker.Mistakes.Quests
{
    /// <summary>
    /// Quest duplicate
    /// </summary>
    public class NE_4001 : Mistake
    {
        public override IMPORTANCE Importance => IMPORTANCE.CRITICAL;
        public override string MistakeNameKey => "NE_4001";
        public override bool TranslateName => false;
        public override string MistakeDescKey => "NE_4001_Desc";
        public override bool IsMistake => MainWindow.CurrentSave.quests.Any(d => MainWindow.CurrentSave.quests.Count(k => k.id == d.id) > 1);
        public override Action OnClick => () =>
        {
            MainWindow.Instance.mainTabControl.SelectedIndex = 4;
        };
    }
}