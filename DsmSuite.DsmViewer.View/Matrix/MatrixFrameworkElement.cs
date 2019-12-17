using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixFrameworkElement : FrameworkElement
    {
        private static readonly GlyphTypeface GlyphTypeface;
        private static readonly SolidColorBrush TextColor;
        private static readonly float PixelsPerDip;
        private static readonly RotateTransform TextTransform;
        private static readonly double FontSize = 12;
        private static readonly GlyphInfo[] m_glyphInfoTable;
        private static List<ushort> m_glyphIndexesList = new List<ushort>();
        private static List<double> m_advanceWidthsList = new List<double>();

        private struct GlyphInfo
        {
            public readonly ushort Index;
            public readonly double Width; // Pre-computed with font size for now

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
            TextColor = new SolidColorBrush(Colors.Black);
            PixelsPerDip = 1.0f;

            m_glyphInfoTable = new GlyphInfo[char.MaxValue];
            foreach (var kvp in GlyphTypeface.CharacterToGlyphMap)
            {
                char c = (char)kvp.Key;
                var glyphIndex = kvp.Value;
                double width = GlyphTypeface.AdvanceWidths[glyphIndex] * FontSize;
                m_glyphInfoTable[c] = new GlyphInfo(glyphIndex, width);
            }

            TextTransform = new RotateTransform { Angle = 90 };
        }

        protected void DrawRotatedText(DrawingContext dc, Point location, string text, double maxWidth)
        {
            Point rotatedLocation = new Point(-location.Y, -location.X);
            dc.PushTransform(TextTransform);
            DrawText(dc, rotatedLocation, text, maxWidth);
            dc.Pop();
        }

        protected void DrawText(DrawingContext dc, Point location, string text, double maxWidth)
        {
            if (text.Length > 0)
            {
                double totalWidth = 0;

                m_glyphIndexesList.Clear();
                m_advanceWidthsList.Clear();

                foreach (char c in text)
                {
                    if (totalWidth < maxWidth)
                    {
                        var info = m_glyphInfoTable[c];
                        m_glyphIndexesList.Add(info.Index);
                        m_advanceWidthsList.Add(info.Width);

                        totalWidth += info.Width;
                    }
                }

                GlyphRun glyphRun = new GlyphRun(GlyphTypeface, 0, false, FontSize, PixelsPerDip,
                    m_glyphIndexesList.ToArray(), location, m_advanceWidthsList.ToArray(),
                    null, null, null, null, null, null);

                dc.DrawGlyphRun(TextColor, glyphRun);
            }
        }
    }
}
