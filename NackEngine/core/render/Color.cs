using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.render
{
    public struct Color
    {
        // ------- Default colors -------
        // --- BASIC COLORS ---
        public static readonly Color WHITE = new Color("#FFFFFF");
        public static readonly Color BLACK = new Color("#000000");
        public static readonly Color GREY_LIGHT = new Color("#D3D3D3");
        public static readonly Color GREY_DARK = new Color("#A9A9A9");

        // --- MODERN PALETTE ---
        public static readonly Color RED_MODERN = new Color("#C1121F");
        public static readonly Color BLUE_WATER = new Color("#669BBC");
        public static readonly Color BLUE_NAVY = new Color("#003049");
        public static readonly Color YELLOW_EGG = new Color("#FFB703");
        public static readonly Color ORANGE_SUNSET = new Color("#FB8500");

        // --- ESSENTIALS ---
        public static readonly Color GREEN_FOREST = new Color("#2A9D8F");
        public static readonly Color GREEN_LIME = new Color("#9EF01A");
        public static readonly Color PURPLE_ROYAL = new Color("#7209B7");
        public static readonly Color PURPLE_LAVENDER = new Color("#B5179E");
        public static readonly Color PINK_HOT = new Color("#F72585");
        public static readonly Color CYAN_NEON = new Color("#4CC9F0");

        // --- EARTH TONES ---
        public static readonly Color BROWN_WOOD = new Color("#6F4E37");
        public static readonly Color BEIGE_SAND = new Color("#F4A261");
        public static readonly Color GOLD = new Color("#FFD700");
        public static readonly Color SILVER = new Color("#C0C0C0");
        public static readonly Color BRONZE = new Color("#CD7F32");


        private NVector vector;

        public Color(double r, double g, double b) {
            this.vector = new NVector(r, g, b);
        }

        public Color(string hex)
        {
            if (hex.StartsWith("#")) { hex = hex.Substring(1); }
            if (hex.Length != 6)
            {
                throw new ArgumentException("The color must be in the format RRGGBB (6 characters)");
            }
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            this.vector = new NVector(r / 255.0, g / 255.0, b / 255.0);
        }

        public Color(NVector vector) {
            this.vector = vector;
        }

        public NVector Vector() { 
            return this.vector;
        }

        private double Linear2Gamma(double linear) {
            return (linear > 0) ? Math.Sqrt(linear) : 0;
        }

        public override string ToString() {
            var r = this.vector.X();
            var g = this.vector.Y();
            var b = this.vector.Z();

            r = Linear2Gamma(r);
            g = Linear2Gamma(g);
            b = Linear2Gamma(b);

            Range intensity = new Range(0.000,0.999);
            int ir = (int)(256 * intensity.Clamp(r));
            int ig = (int)(256 * intensity.Clamp(g));
            int ib = (int)(256 * intensity.Clamp(b));

            return $"{ir} {ig} {ib}";
        }

        /*
         * Arithmetic operators between colors
         */

        public static Color operator *(Color a, Color b)
        {
            return new Color(a.vector * b.vector);
        }

        public static Color operator *(Color a, double b)
        {
            return new Color(a.vector * b);
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.vector + b.vector);
        }

        public bool IsNaN()
        {
            return vector.IsNaN();
        }

        public Color Clamp(double min, double max)
        {
            return new Color(
                Math.Clamp(this.vector.X(), min, max),
                Math.Clamp(this.vector.Y(), min, max),
                Math.Clamp(this.vector.Z(), min, max)
            );
        }
    }
}
