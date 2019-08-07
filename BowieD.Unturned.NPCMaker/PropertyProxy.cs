﻿using BowieD.Unturned.NPCMaker.Coloring;
using BowieD.Unturned.NPCMaker.Configuration;
using BowieD.Unturned.NPCMaker.Data;
using BowieD.Unturned.NPCMaker.Forms;
using BowieD.Unturned.NPCMaker.Localization;
using BowieD.Unturned.NPCMaker.Logging;
using BowieD.Unturned.NPCMaker.NPC;
using DiscordRPC;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BowieD.Unturned.NPCMaker
{
    public class PropertyProxy
    {
        public PropertyProxy(MainWindow main)
        {
            inst = main;
        }
        public void RegisterEvents()
        {
            inst.newButton.Click += NewButtonClick;
            inst.lstMistakes.SelectionChanged += MistakeList_Selected;
            inst.regenerateGuidsButton.Click += RegenerateGuids_Click;
            inst.optionsMenuItem.Click += Options_Click;
            inst.randomColorButton.Click += RandomColor_Click;
            inst.exitButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                MainWindow.PerformExit();
            });
            inst.exportButton.Click += ExportClick;
            inst.saveButton.Click += SaveClick;
            inst.saveAsButton.Click += SaveAsClick;
            inst.loadButton.Click += LoadClick;
            inst.mainTabControl.SelectionChanged += TabControl_SelectionChanged;
            inst.colorSliderR.ValueChanged += ColorSliderChange;
            inst.colorSliderG.ValueChanged += ColorSliderChange;
            inst.colorSliderB.ValueChanged += ColorSliderChange;
            inst.aboutMenuItem.Click += AboutMenu_Click;
            inst.vkComm.Click += FeedbackItemClick;
            inst.discordComm.Click += FeedbackItemClick;
            inst.steamComm.Click += FeedbackItemClick;
            inst.userColorSaveButton.Click += UserColorList_AddColor;
            inst.switchToAnotherScheme.Click += ColorScheme_Switch;
            inst.colorHexOut.PreviewTextInput += ColorHex_Input;
            inst.colorBoxR.ValueChanged += ColorBox_ValueChanged;
            inst.colorBoxG.ValueChanged += ColorBox_ValueChanged;
            inst.colorBoxB.ValueChanged += ColorBox_ValueChanged;
            DataObject.AddPastingHandler(inst.colorHexOut, ColorHex_Pasted);
            RoutedCommand saveHotkey = new RoutedCommand();
            saveHotkey.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            inst.CommandBindings.Add(new CommandBinding(saveHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    MainWindow.Save();
                })));
            RoutedCommand loadHotkey = new RoutedCommand();
            loadHotkey.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            inst.CommandBindings.Add(new CommandBinding(loadHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    LoadClick(sender, null);
                })));
            RoutedCommand exportHotkey = new RoutedCommand();
            exportHotkey.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            inst.CommandBindings.Add(new CommandBinding(exportHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    ExportClick(sender, null);
                })));
            RoutedCommand newFileHotkey = new RoutedCommand();
            newFileHotkey.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            inst.CommandBindings.Add(new CommandBinding(newFileHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    NewButtonClick(sender, null);
                })));
            RoutedCommand logWindow = new RoutedCommand();
            logWindow.InputGestures.Add(new KeyGesture(Key.F1, ModifierKeys.Control));
            inst.CommandBindings.Add(new CommandBinding(logWindow,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
            {
                if (!LogWindow.IsOpened)
                {
                    Logger.Log("Opening Log Window...");
                    MainWindow.LogWindow = new LogWindow();
                    MainWindow.LogWindow.Show();
                }
            })));
        }


        private MainWindow inst;

        #region EVENTS
        private void ColorBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            inst.colorSliderR.Value = inst.colorBoxR.Value ?? 0;
            inst.colorSliderG.Value = inst.colorBoxG.Value ?? 0;
            inst.colorSliderB.Value = inst.colorBoxB.Value ?? 0;
        }
        internal void ColorHex_Input(object sender, TextCompositionEventArgs e)
        {
            string text = inst.colorHexOut.Text;
            int cursorPos = inst.colorHexOut.SelectionStart;
            text = text.Insert(inst.colorHexOut.SelectionStart, e.Text);
            if (text.StartsWith("#"))
                text = text.Substring(1);
            if (text.Length < 6)
                return;
            try
            {
                PaletteHEX paletteHEX = new PaletteHEX() { HEX = text };

                inst.colorHexOut.Text = paletteHEX.HEX;

                if (MainWindow.IsRGB)
                {
                    var rgb = paletteHEX.ToRGB();
                    inst.colorSliderR.Value = rgb.R;
                    inst.colorBoxR.Value = rgb.R;
                    inst.colorSliderG.Value = rgb.G;
                    inst.colorBoxG.Value = rgb.G;
                    inst.colorSliderB.Value = rgb.B;
                    inst.colorBoxB.Value = rgb.B;
                    inst.colorBoxR.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                    inst.colorBoxG.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                    inst.colorBoxB.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                }
                else
                {
                    var hsv = Palette.Convert<PaletteHSV>(paletteHEX).HSV;
                    inst.colorSliderR.Value = hsv.H;
                    inst.colorBoxR.Value = hsv.H;
                    inst.colorSliderG.Value = hsv.S;
                    inst.colorBoxG.Value = hsv.S;
                    inst.colorSliderB.Value = hsv.V;
                    inst.colorBoxB.Value = hsv.V;
                    inst.colorBoxR.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                    inst.colorBoxG.ParsingNumberStyle = System.Globalization.NumberStyles.Float;
                    inst.colorBoxB.ParsingNumberStyle = System.Globalization.NumberStyles.Float;
                }
                inst.colorHexOut.SelectionStart = cursorPos + 1;
                inst.userColorSaveButton.IsEnabled = true;
            }
            catch
            {
                e.Handled = true;
                inst.userColorSaveButton.IsEnabled = false;
            }
        }
        internal void ColorHex_Pasted(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
            {
                try
                {
                    var input = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
                    PaletteHEX paletteHEX = new PaletteHEX() { HEX = input };

                    inst.colorHexOut.Text = paletteHEX.HEX;
                    if (MainWindow.IsRGB)
                    {
                        var rgb = paletteHEX.ToRGB();
                        inst.colorSliderR.Value = rgb.R;
                        inst.colorBoxR.Value = rgb.R;
                        inst.colorSliderG.Value = rgb.G;
                        inst.colorBoxG.Value = rgb.G;
                        inst.colorSliderB.Value = rgb.B;
                        inst.colorBoxB.Value = rgb.B;
                        inst.colorBoxR.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                        inst.colorBoxG.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                        inst.colorBoxB.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                    }
                    else
                    {
                        var hsv = Palette.Convert<PaletteHSV>(paletteHEX).HSV;
                        inst.colorSliderR.Value = hsv.H;
                        inst.colorBoxR.Value = hsv.H;
                        inst.colorSliderG.Value = hsv.S;
                        inst.colorBoxG.Value = hsv.S;
                        inst.colorSliderB.Value = hsv.V;
                        inst.colorBoxB.Value = hsv.V;
                        inst.colorBoxR.ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
                        inst.colorBoxG.ParsingNumberStyle = System.Globalization.NumberStyles.Float;
                        inst.colorBoxB.ParsingNumberStyle = System.Globalization.NumberStyles.Float;
                    }

                    inst.userColorSaveButton.IsEnabled = true;
                }
                catch
                {
                    e.Handled = true;
                    inst.userColorSaveButton.IsEnabled = false;
                }
            }
            else
            {
                e.Handled = true;
                inst.userColorSaveButton.IsEnabled = false;
            }
        }
        internal void MistakeList_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (inst.lstMistakes.SelectedItem != null && inst.lstMistakes.SelectedItem is Mistake mist)
            {
                mist.OnClick?.Invoke();
            }
        }
        internal void RegenerateGuids_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentProject.characters != null)
            {
                foreach (NPCCharacter c in MainWindow.CurrentProject.characters)
                {
                    if (c != null)
                        c.guid = Guid.NewGuid().ToString("N");
                }
            }
            if (MainWindow.CurrentProject.dialogues != null)
            {
                foreach (NPCDialogue d in MainWindow.CurrentProject.dialogues)
                {
                    if (d != null)
                        d.guid = Guid.NewGuid().ToString("N");
                }
            }
            if (MainWindow.CurrentProject.vendors != null)
            {
                foreach (NPCVendor v in MainWindow.CurrentProject.vendors)
                {
                    if (v != null)
                        v.guid = Guid.NewGuid().ToString("N");
                }
            }
            if (MainWindow.CurrentProject.quests != null)
            {
                foreach (NPCQuest q in MainWindow.CurrentProject.quests)
                {
                    if (q != null)
                        q.guid = Guid.NewGuid().ToString("N");
                }
            }
            MainWindow.NotificationManager.Notify(LocUtil.LocalizeInterface("general_Regenerated"));
            MainWindow.isSaved = false;
        }
        internal void Options_Click(object sender, RoutedEventArgs e)
        {
            Configuration.ConfigWindow cw = new Configuration.ConfigWindow();
            cw.ShowDialog();
        }
        internal void RandomColor_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.IsRGB)
            {
                byte[] colors = new byte[3];
                System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(colors);
                inst.colorSliderR.Value = colors[0];
                inst.colorSliderG.Value = colors[1];
                inst.colorSliderB.Value = colors[2];
            }
            else
            {
                inst.colorSliderR.Value = new Random().Next(360);
                inst.colorSliderG.Value = new Random().NextDouble();
                inst.colorSliderB.Value = new Random().NextDouble();
            }
        }
        internal void UserColorListChanged()
        {
            inst.userColorSampleList.Children.Clear();
            List<string> unturnedColors = new List<string>()
            {
                "F4E6D2",
                "D9CAB4",
                "BEA582",
                "9D886B",
                "94764B",
                "706049",
                "534736",
                "4B3D31",
                "332C25",
                "231F1C",
                "D7D7D7",
                "C1C1C1",
                "CDC08C",
                "AC6A39",
                "665037",
                "57452F",
                "352C22",
                "373737",
                "191919"
            };
            BrushConverter brushConverter = new BrushConverter();
            foreach (string uColor in unturnedColors)
            {
                Grid g = new Grid();
                Border b = new Border
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 16,
                    Height = 16,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Background = brushConverter.ConvertFromString($"#{uColor}") as Brush
                };
                Label l = new Label
                {
                    Content = $"#{uColor}",
                    Margin = new Thickness(16, 0, 0, 0)
                };
                Button copyButton = new Button
                {
                    Content = new MahApps.Metro.IconPacks.PackIconMaterial()
                    {
                        Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ContentCopy
                    },
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 10, 0),
                    Tag = $"#{uColor}"
                };
                (copyButton.Content as MahApps.Metro.IconPacks.PackIconMaterial).SetResourceReference(MahApps.Metro.IconPacks.PackIconMaterial.ForegroundProperty, "AccentColor");
                copyButton.Click += new RoutedEventHandler((sender, e) =>
                {
                    try
                    {
                        Button b1 = (sender as Button);
                        string toCopy = (string)b1.Tag;
                        Clipboard.SetText(toCopy);
                    }
                    catch { }
                });
                g.Children.Add(b);
                g.Children.Add(l);
                g.Children.Add(copyButton);
                inst.userColorSampleList.Children.Add(g);
            }
            UserColorsList userColors = new UserColorsList();
            userColors.Load();
            foreach (string uColor in userColors.data)
            {
                if (uColor.StartsWith("#"))
                {
                    if (!brushConverter.IsValid(uColor))
                        continue;
                }
                else
                {
                    if (!brushConverter.IsValid($"#{uColor}"))
                        continue;
                }
                Grid g = new Grid();
                Border b = new Border
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 16,
                    Height = 16,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Background = brushConverter.ConvertFromString($"#{uColor}") as Brush
                };
                Label l = new Label
                {
                    Content = $"#{uColor}",
                    Margin = new Thickness(16, 0, 0, 0)
                };
                Button button = new Button()
                {
                    Content = new MahApps.Metro.IconPacks.PackIconMaterial()
                    {
                        Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.TrashCan
                    },
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 10, 0),
                    ToolTip = LocUtil.LocalizeInterface("apparel_User_Color_Remove")
                };
                (button.Content as MahApps.Metro.IconPacks.PackIconMaterial).SetResourceReference(MahApps.Metro.IconPacks.PackIconMaterial.ForegroundProperty, "AccentColor");
                Button copyButton = new Button
                {
                    Content = new MahApps.Metro.IconPacks.PackIconMaterial()
                    {
                        Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ContentCopy
                    },
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 42, 0),
                    Tag = $"#{uColor}"
                };
                (copyButton.Content as MahApps.Metro.IconPacks.PackIconMaterial).SetResourceReference(MahApps.Metro.IconPacks.PackIconMaterial.ForegroundProperty, "AccentColor");
                copyButton.Click += new RoutedEventHandler((sender, e) =>
                {
                    try
                    {
                        Button b1 = (sender as Button);
                        string toCopy = (string)b1.Tag;
                        Clipboard.SetText(toCopy);
                    }
                    catch { }
                });
                button.Click += UserColorList_RemoveColor;
                g.Children.Add(b);
                g.Children.Add(l);
                g.Children.Add(button);
                g.Children.Add(copyButton);
                inst.userColorSampleList.Children.Add(g);
            }
        }
        internal void UserColorList_RemoveColor(object sender, RoutedEventArgs e)
        {
            Grid g = Util.FindParent<Grid>(sender as Button);
            Label l = Util.FindChildren<Label>(g);
            string color = l.Content.ToString();
            UserColorsList userColors = new UserColorsList();
            userColors.data = userColors.data.Where(d => d != color.Trim('#')).ToArray();
            userColors.Save();
            UserColorListChanged();
        }
        internal void UserColorList_AddColor(object sender, RoutedEventArgs e)
        {
            UserColorsList userColors = new UserColorsList();
            userColors.Load();
            if (userColors.data.Contains(inst.colorHexOut.Text.Trim('#')))
                return;
            var lst = userColors.data.ToList();
            lst.Add(inst.colorHexOut.Text.Trim('#'));
            userColors.data = lst.ToArray();
            userColors.Save();
            UserColorListChanged();
        }
        internal void ExportClick(object sender, RoutedEventArgs e)
        {
            Mistakes.MistakesManager.FindMistakes();
            if (Mistakes.MistakesManager.Criticals_Count > 0)
            {
                SystemSounds.Hand.Play();
                inst.mainTabControl.SelectedIndex = inst.mainTabControl.Items.Count - 1;
                return;
            }
            if (Mistakes.MistakesManager.Warnings_Count > 0)
            {
                var res = MessageBox.Show(LocUtil.LocalizeInterface("export_Warnings_Desc"), LocUtil.LocalizeInterface("export_Warnings_Title"), MessageBoxButton.YesNo);
                if (!(res == MessageBoxResult.OK || res == MessageBoxResult.Yes))
                    return;
            }
            MainWindow.Save();
            Export.Exporter.ExportNPC(MainWindow.CurrentProject);
        }
        internal void NewButtonClick(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.SavePrompt())
                return;
            MainWindow.ResetEditors();
            MainWindow.isSaved = true;
            MainWindow.Started = DateTime.UtcNow;
        }
        internal void SaveClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Save();
        }
        internal void SaveAsClick(object sender, RoutedEventArgs e)
        {
            MainWindow.oldFile = MainWindow.saveFile;
            MainWindow.saveFile = "";
            MainWindow.Save();
            MainWindow.oldFile = "";
        }
        internal void LoadClick(object sender, RoutedEventArgs e)
        {
            string path = "";
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = $"{LocUtil.LocalizeInterface("save_Filter")} (*.npc,*.npcproj)|*.npc;*.npcproj",
                Multiselect = false
            };
            var res = ofd.ShowDialog();
            if (res == true)
                path = ofd.FileName;
            else
                return;
            if (inst.Load(path))
            {
                inst.notificationsStackPanel.Children.Clear();
                MainWindow.NotificationManager.Notify(LocUtil.LocalizeInterface("notify_Loaded"));
            }
        }
        internal void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count == 0 || sender == null)
                return;
            int selectedIndex = (sender as TabControl).SelectedIndex;
            TabItem tab = e?.AddedItems[0] as TabItem;
            if (AppConfig.Instance.animateControls && tab?.Content is Grid g)
            {
                DoubleAnimation anim = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                g.BeginAnimation(MainWindow.OpacityProperty, anim);
            }
            if (selectedIndex == (sender as TabControl).Items.Count - 1)
            {
                Mistakes.MistakesManager.FindMistakes();
            }
            if (MainWindow.DiscordManager != null)
            {
                switch (selectedIndex)
                {
                    case 0:
                        MainWindow.CharacterEditor.SendPresence();
                        break;
                    case 1:
                        MainWindow.DialogueEditor.SendPresence();
                        break;
                    case 2:
                        MainWindow.VendorEditor.SendPresence();
                        break;
                    case 3:
                        MainWindow.QuestEditor.SendPresence();
                        break;
                    case 4:
                        {
                            RichPresence presence = new RichPresence
                            {
                                Timestamps = new Timestamps
                                {
                                    StartUnixMilliseconds = (ulong)(MainWindow.Started.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                                },
                                Assets = new Assets()
                            };
                            presence.Assets.SmallImageKey = "icon_warning_outlined";
                            presence.Assets.SmallImageText = $"Mistakes: {MainWindow.Instance.lstMistakes.Items.Count}";
                            presence.Details = $"Critical errors: {Mistakes.MistakesManager.Criticals_Count}";
                            presence.State = $"Warnings: {Mistakes.MistakesManager.Warnings_Count}";
                            MainWindow.DiscordManager.SendPresence(presence);
                            break;
                        }
                    default:
                        {

                            RichPresence presence = new RichPresence
                            {
                                Timestamps = new Timestamps
                                {
                                    StartUnixMilliseconds = (ulong)(MainWindow.Started.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                                },
                                Assets = new Assets()
                            };
                            presence.Assets.SmallImageKey = "icon_question_outlined";
                            presence.Assets.SmallImageText = "Something?..";
                            presence.Details = $"If this is shown";
                            presence.State = $"Then something gone wrong.";
                            MainWindow.DiscordManager.SendPresence(presence);
                            break;
                        }
                }
            }
        }
        internal void ColorSliderChange(object sender, RoutedPropertyChangedEventArgs<double> value)
        {
            NPCColor c;
            if (MainWindow.IsRGB)
                c = new NPCColor((byte)inst.colorSliderR.Value, (byte)inst.colorSliderG.Value, (byte)inst.colorSliderB.Value);
            else
                c = (NPCColor)new PaletteHSV() { HSV = ((int)inst.colorSliderR.Value, inst.colorSliderG.Value, inst.colorSliderB.Value) };
            string res = Palette.Convert<PaletteHEX>((PaletteRGB)c).HEX;
            inst.colorRectangle.Fill = new BrushConverter().ConvertFromString(res) as Brush;
            inst.colorHexOut.Text = res;
            inst.userColorSaveButton.IsEnabled = true;
            inst.colorBoxR.Value = Math.Round(inst.colorSliderR.Value, 2);
            inst.colorBoxG.Value = Math.Round(inst.colorSliderG.Value, 2);
            inst.colorBoxB.Value = Math.Round(inst.colorSliderB.Value, 2);
            if (AppConfig.Instance.experimentalFeatures)
            {
                if (!MainWindow.IsRGB)
                {
                    var HSV = Palette.Convert<PaletteHSV>((PaletteRGB)c).HSV;
                    // build first bar (Hue)
                    Slider senderSlider = sender as Slider ?? new Slider();
                    if (senderSlider.Name != inst.colorSliderR.Name)
                    {
                        List<GradientStop> stopsHue = new List<GradientStop>();
                        for (int k = 0; k <= 360; k++)
                        {
                            stopsHue.Add(new GradientStop(new PaletteHSV() { HSV = (k, HSV.Item2, HSV.Item3) }.GetColor(), k / 360d));
                        }
                        inst.colorSliderR.Background = new LinearGradientBrush(new GradientStopCollection(stopsHue), 0);
                    }
                    // build second bar (Saturation)
                    if (senderSlider.Name != inst.colorSliderG.Name)
                    {
                        List<GradientStop> stopsSatur = new List<GradientStop>();
                        for (double k = 0; k <= 1; k += 0.01)
                        {
                            stopsSatur.Add(new GradientStop(new PaletteHSV() { HSV = (HSV.Item1, k, HSV.Item3) }.GetColor(), k));
                        }
                        inst.colorSliderG.Background = new LinearGradientBrush(new GradientStopCollection(stopsSatur), 0);
                    }
                    // build third bar (Value)
                    if (senderSlider.Name != inst.colorSliderB.Name)
                    {
                        List<GradientStop> stopsValue = new List<GradientStop>();
                        for (double k = 0; k <= 1; k += 0.01)
                        {
                            stopsValue.Add(new GradientStop(new PaletteHSV() { HSV = (HSV.Item1, HSV.Item2, k) }.GetColor(), k));
                        }
                        inst.colorSliderB.Background = new LinearGradientBrush(new GradientStopCollection(stopsValue), 0);
                    }
                }
                else
                {
                    Slider senderSlider = sender as Slider ?? new Slider();
                    if (senderSlider.Name != inst.colorSliderR.Name)
                    {
                        List<GradientStop> stopsRed = new List<GradientStop>();
                        for (byte red = 0; red < 255; red++)
                        {
                            var clr = new NPCColor(red, c.G, c.B);
                            stopsRed.Add(new GradientStop(Color.FromRgb(clr.R, clr.G, clr.B), red / 255d));
                        }
                        inst.colorSliderR.Background = new LinearGradientBrush(new GradientStopCollection(stopsRed), 0);
                    }
                    if (senderSlider.Name != inst.colorSliderG.Name)
                    {
                        List<GradientStop> stopsGreen = new List<GradientStop>();
                        for (byte green = 0; green < 255; green++)
                        {
                            var clr = new NPCColor(c.R, green, c.B);
                            stopsGreen.Add(new GradientStop(Color.FromRgb(clr.R, clr.G, clr.B), green / 255d));
                        }
                        inst.colorSliderG.Background = new LinearGradientBrush(new GradientStopCollection(stopsGreen), 0);
                    }
                    if (senderSlider.Name != inst.colorSliderB.Name)
                    {
                        List<GradientStop> stopsBlue = new List<GradientStop>();
                        for (byte blue = 0; blue < 255; blue++)
                        {
                            var clr = new NPCColor(c.R, c.G, blue);
                            stopsBlue.Add(new GradientStop(Color.FromRgb(clr.R, clr.G, clr.B), blue / 255d));
                        }
                        inst.colorSliderB.Background = new LinearGradientBrush(new GradientStopCollection(stopsBlue), 0);
                    }
                }
            }
        }
        internal void ColorScheme_Switch(object sender, RoutedEventArgs e)
        {
            MainWindow.IsRGB = !MainWindow.IsRGB;
            NPCColor c;
            if (MainWindow.IsRGB)
            {
                c = (NPCColor)new PaletteHSV() { HSV = ((int)inst.colorSliderR.Value, inst.colorSliderG.Value, inst.colorSliderB.Value) };
                inst.colorSliderR.Value = 0;
                inst.colorSliderG.Value = 0;
                inst.colorSliderB.Value = 0;
                inst.colorSliderR.Minimum = 0;
                inst.colorSliderG.Minimum = 0;
                inst.colorSliderB.Minimum = 0;
                inst.colorSliderR.Maximum = 255;
                inst.colorSliderG.Maximum = 255;
                inst.colorSliderB.Maximum = 255;
                inst.colorSliderG.SmallChange = 1;
                inst.colorSliderG.LargeChange = 5;
                inst.colorSliderB.SmallChange = 1;
                inst.colorSliderB.LargeChange = 5;
                inst.colorSliderR.AutoToolTipPrecision = 0;
                inst.colorSliderG.AutoToolTipPrecision = 0;
                inst.colorSliderB.AutoToolTipPrecision = 0;
                inst.colorRLabel.Content = LocUtil.LocalizeInterface("tool_Color_Red");
                inst.colorGLabel.Content = LocUtil.LocalizeInterface("tool_Color_Green");
                inst.colorBLabel.Content = LocUtil.LocalizeInterface("tool_Color_Blue");
                inst.colorRLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Red_Tip");
                inst.colorGLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Green_Tip");
                inst.colorBLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Blue_Tip");
                inst.switchToAnotherScheme.Content = LocUtil.LocalizeInterface("tool_Color_SwitchTo_HSV");
                inst.colorSliderR.Value = c.R;
                inst.colorSliderG.Value = c.G;
                inst.colorSliderB.Value = c.B;
            }
            else
            {
                c = new NPCColor((byte)inst.colorSliderR.Value, (byte)inst.colorSliderG.Value, (byte)inst.colorSliderB.Value);
                inst.colorSliderR.Value = 0;
                inst.colorSliderG.Value = 0;
                inst.colorSliderB.Value = 0;
                inst.colorSliderR.Minimum = 0;
                inst.colorSliderG.Minimum = 0;
                inst.colorSliderB.Minimum = 0;
                inst.colorSliderR.Maximum = 359.99;
                inst.colorSliderG.Maximum = 1;
                inst.colorSliderB.Maximum = 1;
                inst.colorSliderG.AutoToolTipPrecision = 2;
                inst.colorSliderB.AutoToolTipPrecision = 2;
                inst.colorSliderG.SmallChange = 0.01;
                inst.colorSliderG.LargeChange = 0.1;
                inst.colorSliderB.SmallChange = 0.01;
                inst.colorSliderB.LargeChange = 0.1;
                inst.colorRLabel.Content = LocUtil.LocalizeInterface("tool_Color_Hue");
                inst.colorGLabel.Content = LocUtil.LocalizeInterface("tool_Color_Saturation");
                inst.colorBLabel.Content = LocUtil.LocalizeInterface("tool_Color_Value");
                inst.colorRLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Hue_Tip");
                inst.colorGLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Saturation_Tip");
                inst.colorBLabel.ToolTip = LocUtil.LocalizeInterface("tool_Color_Value_Tip");
                inst.switchToAnotherScheme.Content = LocUtil.LocalizeInterface("tool_Color_SwitchTo_RGB");
                var cHSV = Palette.Convert<PaletteHSV>((PaletteRGB)c).HSV;
                inst.colorSliderR.Value = cHSV.H;
                inst.colorSliderG.Value = cHSV.S;
                inst.colorSliderB.Value = cHSV.V;
            }
            ColorSliderChange(null, null);
        }
        internal void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            Forms.Form_About a = new Forms.Form_About();
            a.ShowDialog();
        }
        internal void FeedbackItemClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((sender as MenuItem).Tag.ToString());
        }
        internal void WhatsNew_Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    new Whats_New().ShowDialog();
                }
            }
            catch (Exception ex) { Logging.Logger.Log(ex); }
        }
        #endregion
    }
}