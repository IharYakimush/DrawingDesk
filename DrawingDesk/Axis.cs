using System;
using System.Collections.Generic;
using System.Drawing;

namespace DrawingDesk
{
    public class Axis : DrawingFigure
    {
        private readonly KeyValuePair<float, string> xunit;
        private readonly Pen pen;
        private readonly Font font;
        private readonly Brush brush;

        public Axis(KeyValuePair<float,string>? xunit = null, Pen pen = null, Font font = null, Brush brush = null)
        {
            this.xunit = xunit ?? new KeyValuePair<float, string>(1, string.Empty);
            this.pen = pen ?? Pens.Black;
            this.font = font ?? SystemFonts.DefaultFont;
            this.brush = brush ?? Brushes.Black;
        }
        public override void Draw(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            graphics.DrawLine(this.pen, translator.Translate(new PointF(sizeF.Left, 0)), translator.Translate(new PointF(sizeF.Right, 0)));
            graphics.DrawLine(this.pen, translator.Translate(new PointF(0, sizeF.Top)), translator.Translate(new PointF(0, sizeF.Bottom)));

            float dxu = GetDu(20, translator.Resolution.X);
            float dyu = GetDu(20, translator.Resolution.Y);

            this.DrawXUnit(sizeF, graphics, translator, dxu);
            this.DrawXUnit(sizeF, graphics, translator, -dxu);
            graphics.DrawString("0", this.font, this.brush, translator.Translate(new PointF(0, 0)));
        }

        private void DrawXUnit(RectangleF sizeF, Graphics graphics, IPointTranslator translator, float dxu)
        {
            float x0 = 0;
            float y1 = 2 / translator.Resolution.Y;
            float y2 = -2 / translator.Resolution.Y;

            while (x0 < sizeF.Right && x0 > sizeF.Left)
            {
                x0 = x0 + dxu;

                if (0 <= sizeF.Bottom && 0 >= sizeF.Top)
                {
                    Point pt1 = translator.Translate(new PointF(x0, y1));
                    Point pt2 = translator.Translate(new PointF(x0, y2));

                    graphics.DrawLine(this.pen, pt1, pt2);

                    string ks = ((int)MathF.Round(x0 / this.xunit.Key)).ToString("D");
                    string text = string.IsNullOrWhiteSpace(this.xunit.Value) ? ks : ks == "1" || ks == "-1" ? this.xunit.Value : ks + this.xunit.Value;

                    graphics.DrawString(text, this.font, this.brush, pt2);
                }
            }
        }

        private float GetDu(int pixelLimit,float resolution)
        {
            float du = this.xunit.Key;

            for (int k = 0; k < 4; k++)
            {
                du = MathF.Pow(10, k) * this.xunit.Key;

                if (du * resolution > pixelLimit)
                {
                    break;
                }
            }

            return du;
        }
    }
}
