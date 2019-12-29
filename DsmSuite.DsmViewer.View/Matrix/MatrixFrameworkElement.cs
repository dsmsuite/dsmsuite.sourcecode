using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixFrameworkElement : FrameworkElement
    {
        private static readonly GlyphTypeface GlyphTypeface;
        private static readonly float PixelsPerDip;
        private static readonly RotateTransform TextTransform;
        private static readonly double FontSize = 12;
        private static readonly GlyphInfo[] MGlyphInfoTable;
        private static readonly List<ushort> MGlyphIndexesList = new List<ushort>();
        private static readonly List<double> MAdvanceWidthsList = new List<double>();

        private struct GlyphInfo
        {
            public readonly ushort Index;
            public readonly double Width; 

            public GlyphInfo(ushort glyphIndex, double width) : this()
            {
                Index = glyphIndex;
                Width = width;
            }
        }

        static MatrixFrameworkElement()
        {
            Typeface typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            typeface.TryGetGlyphTypeface(out GlyphTypeface);
            PixelsPerDip = 1.0f;

            MGlyphInfoTable = new GlyphInfo[char.MaxValue];
            foreach (var kvp in GlyphTypeface.CharacterToGlyphMap)
            {
                char c = (char)kvp.Key;
                var glyphIndex = kvp.Value;
                double width = GlyphTypeface.AdvanceWidths[glyphIndex] * FontSize;
                MGlyphInfoTable[c] = new GlyphInfo(glyphIndex, width);
            }

            TextTransform = new RotateTransform { Angle = 90 };
        }

        protected void DrawRotatedText(DrawingContext dc, string text, Point location, SolidColorBrush color,  double maxWidth)
        {
            Point rotatedLocation = new Point(-location.Y, -location.X);
            dc.PushTransform(TextTransform);
            DrawText(dc, text, rotatedLocation, color, maxWidth);
            dc.Pop();
        }

        protected void DrawText(DrawingContext dc, string text, Point location, SolidColorBrush color, double maxWidth)
        {
            if (text.Length > 0)
            {
                double totalWidth = 0;

                MGlyphIndexesList.Clear();
                MAdvanceWidthsList.Clear();

                foreach (char c in text)
                {
                    if (totalWidth < maxWidth)
                    {
                        var info = MGlyphInfoTable[c];
                        MGlyphIndexesList.Add(info.Index);
                        MAdvanceWidthsList.Add(info.Width);

                        totalWidth += info.Width;
                    }
                }

                GlyphRun glyphRun = new GlyphRun(GlyphTypeface, 0, false, FontSize, PixelsPerDip,
                    MGlyphIndexesList.ToArray(), location, MAdvanceWidthsList.ToArray(),
                    null, null, null, null, null, null);

                dc.DrawGlyphRun(color, glyphRun);
            }
        }
    }
}
