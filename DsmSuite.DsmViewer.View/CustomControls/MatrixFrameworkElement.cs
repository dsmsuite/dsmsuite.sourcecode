using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.CustomControls
{
    public class MatrixFrameworkElement : FrameworkElement
    {
        private static readonly GlyphTypeface GlyphTypeface;
        private static readonly SolidColorBrush TextColor;
        private static readonly float PixelsPerDip;
        private static readonly RotateTransform TextTransform;
        private static readonly double FontSize = 12;

        static MatrixFrameworkElement()
        {
            Typeface typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            typeface.TryGetGlyphTypeface(out GlyphTypeface);
            TextColor = new SolidColorBrush(Colors.Black);
            PixelsPerDip = 1.0f;

            TextTransform = new RotateTransform {Angle = 90};
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
            double totalWidth = 0;

            List<ushort> glyphIndexesList = new List<ushort>();
            List<double> advanceWidthsList = new List<double>();
            foreach (char t in text)
            {
                if (totalWidth < maxWidth)
                {
                    ushort glyphIndex = GlyphTypeface.CharacterToGlyphMap[t];
                    glyphIndexesList.Add(glyphIndex);

                    double width = GlyphTypeface.AdvanceWidths[glyphIndex] * FontSize;
                    advanceWidthsList.Add(width);

                    totalWidth += width;
                }
            }

            if (glyphIndexesList.Count > 0)
            {
                GlyphRun glyphRun = new GlyphRun(GlyphTypeface, 0, false, FontSize, PixelsPerDip,
                    glyphIndexesList.ToArray(), location, advanceWidthsList.ToArray(),
                    null, null, null, null, null, null);

                dc.DrawGlyphRun(TextColor, glyphRun);
            }
        }
    }
}
