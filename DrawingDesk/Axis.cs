﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace DrawingDesk
{
    public class Axis : DrawingFigure
    {
        private const int UnitPixels = 20;

        private readonly KeyValuePair<float, string> xunit;
        private readonly KeyValuePair<float, string> yunit;
        private readonly Pen pen;
        private readonly Font font;
        private readonly Brush brush;

        public bool LineX { get; set; } = true;

        public bool LineY { get; set; } = true;

        public Axis(KeyValuePair<float,string>? xunit = null, KeyValuePair<float, string>? yunit = null, Pen pen = null, Font font = null, Brush brush = null)
        {
            this.xunit = xunit ?? new KeyValuePair<float, string>(1, string.Empty);
            this.yunit = yunit ?? new KeyValuePair<float, string>(1, string.Empty);
            this.pen = pen ?? Pens.Black;
            this.font = font ?? SystemFonts.DefaultFont;
            this.brush = brush ?? Brushes.Black;
        }
        public override void DrawInner(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {            
            if (sizeF.Top >= 0 && sizeF.Bottom <= 0 && this.LineX)
            {
                // x line
                graphics.DrawLine(this.pen, translator.Translate(new PointF(sizeF.Left, 0)), translator.Translate(new PointF(sizeF.Right, 0)));
                if (sizeF.Right >= 0)
                {
                    // arrow
                    graphics.DrawLine(this.pen, translator.Translate(new PointF(sizeF.Right, 0)), translator.Translate(new PointF(sizeF.Right - 6 / translator.Resolution.X, 3 / translator.Resolution.Y)));
                    graphics.DrawLine(this.pen, translator.Translate(new PointF(sizeF.Right, 0)), translator.Translate(new PointF(sizeF.Right - 6 / translator.Resolution.X, -3 / translator.Resolution.Y)));
                }
            }

            if (sizeF.Right >= 0 && sizeF.Left <= 0 && this.LineY)
            {
                //y line
                graphics.DrawLine(this.pen, translator.Translate(new PointF(0, sizeF.Top)), translator.Translate(new PointF(0, sizeF.Bottom)));
                if (sizeF.Top >= 0)
                {
                    ///arrow
                    graphics.DrawLine(this.pen, translator.Translate(new PointF(0, sizeF.Top)), translator.Translate(new PointF(3 / translator.Resolution.X, sizeF.Top - 6 / translator.Resolution.Y)));
                    graphics.DrawLine(this.pen, translator.Translate(new PointF(0, sizeF.Top)), translator.Translate(new PointF(-3 / translator.Resolution.X, sizeF.Top - 6 / translator.Resolution.Y)));
                }
            }

            if (this.LineX)
            {
                this.DrawXUnit(sizeF, graphics, translator);
            }

            if (this.LineY)
            {
                this.DrawYUnit(sizeF, graphics, translator);
            }

            if (this.LineX || this.LineY)
            {
                var zero = new PointF(0, 0);

                if (sizeF.Top >= 0 && sizeF.Right >= 0 && sizeF.Left <= 0 && sizeF.Bottom <= 0)
                {
                    graphics.DrawString("0", this.font, this.brush, translator.Translate(zero));
                }
            }            
        }

        private void DrawXUnit(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            float dxu = GetDu(20, translator.Resolution.X, this.xunit);

            float x0 = MathF.Floor(sizeF.Left / dxu) * dxu + dxu;
            float y1 = 2 / translator.Resolution.Y;
            float y2 = -2 / translator.Resolution.Y;

            while (x0 < sizeF.Right && x0 > sizeF.Left)
            {
                if (y1 <= sizeF.Top && y1 >= sizeF.Bottom)
                {
                    Point pt1 = translator.Translate(new PointF(x0, y1));
                    Point pt2 = translator.Translate(new PointF(x0, y2));

                    graphics.DrawLine(this.pen, pt1, pt2);

                    this.DrawUnitText(graphics, x0, pt2, this.xunit);
                }

                x0 = x0 + dxu;
            }
        }

        private void DrawYUnit(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            float dyu = GetDu(UnitPixels, translator.Resolution.Y, this.yunit);

            float min = MathF.Min(sizeF.Top, sizeF.Bottom);
            float max = MathF.Max(sizeF.Top, sizeF.Bottom);

            float y0 = MathF.Floor(min / dyu) * dyu + dyu;
            float x1 = 2 / translator.Resolution.X;
            float x2 = -2 / translator.Resolution.X;

            while (y0 < max && y0 > min)
            {
                if (x1 <= sizeF.Right && x1 >= sizeF.Left)
                {
                    Point pt1 = translator.Translate(new PointF(x1, y0));
                    Point pt2 = translator.Translate(new PointF(x2, y0));

                    graphics.DrawLine(this.pen, pt1, pt2);

                    this.DrawUnitText(graphics, y0, pt1, this.yunit);
                }

                y0 = y0 + dyu;
            }
        }

        private void DrawUnitText(Graphics graphics, float value, Point pt, KeyValuePair<float, string> unit)
        {
            string ks = ((int)MathF.Round(value / unit.Key)).ToString("D");
            if (ks != "0")
            {
                string text = string.IsNullOrWhiteSpace(unit.Value)
                    ? ks
                    : ks == "1"
                        ? unit.Value
                        : ks == "-1"
                            ? "-" + unit.Value
                            : ks + unit.Value;

                graphics.DrawString(text, this.font, this.brush, pt);
            }
        }

        private float GetDu(int pixelLimit,float resolution, KeyValuePair<float, string> unit)
        {
            float du = unit.Key;

            for (int k = 0; k < 4; k++)
            {
                du = MathF.Pow(10, k) * unit.Key;

                if (du * resolution > pixelLimit)
                {
                    break;
                }
            }

            return du;
        }
    }
}
