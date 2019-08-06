﻿using System.Windows.Media;

namespace BowieD.Unturned.NPCMaker.Coloring
{
    public abstract class Palette
    {
        public static T Convert<T>(Palette from) where T : Palette, new()
        {
            return new T().FromRGB(from.ToRGB()) as T;
        }
        public Color GetColor()
        {
            var rgb = ToRGB();
            return Color.FromRgb(rgb.R, rgb.G, rgb.B);
        }
        public Brush GetBrush()
        {
            var hex = Convert<PaletteHEX>(this).HEX;
            if (hex.Length >= 6)
            {
                BrushConverter converter = new BrushConverter();
                return converter.ConvertFromString(hex) as Brush;
            }
            else
            {
                return Brushes.Black;
            }
        }
        public abstract Palette FromRGB((byte R, byte G, byte B) rgb);
        public abstract (byte R, byte G, byte B) ToRGB();
        public abstract new string ToString();
    }
}
