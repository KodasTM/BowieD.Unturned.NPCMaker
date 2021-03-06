﻿using BowieD.Unturned.NPCMaker.NPC;
using System.Windows;
using System.Windows.Controls;
using Condition = BowieD.Unturned.NPCMaker.NPC.Conditions.Condition;
using Reward = BowieD.Unturned.NPCMaker.NPC.Rewards.Reward;

namespace BowieD.Unturned.NPCMaker.Controls
{
    /// <summary>
    /// Логика взаимодействия для Condition_ItemList.xaml
    /// </summary>
    public partial class Universal_ItemList : UserControl
    {
        public Universal_ItemList(object input, ReturnType type, bool localizable, bool showMoveButtons = false)
        {
            InitializeComponent();
            Value = input;
            mainLabel.Content = Value is IHasUIText ? (Value as IHasUIText).UIText : Value.ToString();
            mainLabel.ToolTip = mainLabel.Content;
            Localizable = localizable;
            Type = type;
            moveUpButton.Visibility = showMoveButtons ? Visibility.Visible : Visibility.Collapsed;
            moveDownButton.Visibility = showMoveButtons ? Visibility.Visible : Visibility.Collapsed;
        }

        public object Value { get; private set; }
        public bool Localizable { get; private set; }
        public ReturnType Type { get; private set; }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Type == ReturnType.Condition)
            {
                Condition Condition = Value as Condition;
                Forms.Universal_ConditionEditor uce = new Forms.Universal_ConditionEditor(Condition, Localizable);
                uce.ShowDialog();
                if (uce.DialogResult == true)
                {
                    Value = uce.Result;
                    mainLabel.Content = Value is IHasUIText ? (Value as IHasUIText).UIText : Value.ToString();
                    mainLabel.ToolTip = mainLabel.Content;
                }
            }
            else if (Type == ReturnType.Dialogue)
            {
                return;
            }
            else if (Type == ReturnType.Quest)
            {
                return;
            }
            else if (Type == ReturnType.Reward)
            {
                Reward reward = Value as Reward;
                Forms.Universal_RewardEditor ure = new Forms.Universal_RewardEditor(reward, Localizable);
                ure.ShowDialog();
                if (ure.DialogResult == true)
                {
                    Value = ure.Result;
                    mainLabel.Content = Value is IHasUIText ? (Value as IHasUIText).UIText : Value.ToString();
                    mainLabel.ToolTip = mainLabel.Content;
                }
            }
            else if (Type == ReturnType.Vendor)
            {
                return;
            }
            else if (Type == ReturnType.VendorItem)
            {
                NPC.VendorItem Item = Value as NPC.VendorItem;
                bool old = Item.isBuy;
                Forms.Universal_VendorItemEditor uvie = new Forms.Universal_VendorItemEditor(Item);
                if (uvie.ShowDialog() == true)
                {
                    NPC.VendorItem NewItem = uvie.Result as NPC.VendorItem;
                    if (old != NewItem.isBuy)
                    {
                        if (NewItem.isBuy)
                        {
                            MainWindow.Instance.MainWindowViewModel.VendorTabViewModel.RemoveItemSell(Util.FindParent<Universal_ItemList>(sender as Button));
                            MainWindow.Instance.MainWindowViewModel.VendorTabViewModel.AddItemBuy(NewItem);
                        }
                        else
                        {
                            MainWindow.Instance.MainWindowViewModel.VendorTabViewModel.RemoveItemBuy(Util.FindParent<Universal_ItemList>(sender as Button));
                            MainWindow.Instance.MainWindowViewModel.VendorTabViewModel.AddItemSell(NewItem);
                        }
                    }
                    Value = NewItem;
                }
                mainLabel.Content = Value is IHasUIText ? (Value as IHasUIText).UIText : Value.ToString();
                mainLabel.ToolTip = mainLabel.Content;
            }
            else if (Type == ReturnType.Character)
            {
                return;
            }
            else if (Type == ReturnType.Object)
            {
                return;
            }
        }

        public enum ReturnType
        {
            Reward, Condition, Dialogue, Vendor, Quest, VendorItem, Object, Character
        }
    }
}
