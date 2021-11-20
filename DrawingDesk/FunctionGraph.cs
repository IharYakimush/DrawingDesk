using System;
using System.Drawing;

namespace DrawingDesk
{
    public class FunctionGraph : DrawingFigure
    {
        private readonly Func<float, float> f;
        private readonly Pen pen;

        public FunctionGraph(Func<float,float> f,Pen pen)
        {
            this.f = f ?? throw new ArgumentNullException(nameof(f));
            this.pen = pen ?? throw new ArgumentNullException(nameof(pen));
        }
        public override void DrawInner(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            float dx = 1 / translator.Resolution.X;
            float x1 = sizeF.X;

            while (x1 < sizeF.Right)
            {
                float x2 = x1 + dx;
                while (x2 == x1)
                {
                    dx = dx + 1 / translator.Resolution.X;
                    x2 = x1 + dx;
                }

                try
                {
                    float y1 = this.f((float)x1);
                    y1 = MathF.Min(y1, sizeF.Top + 1/translator.Resolution.Y);
                    y1 = MathF.Max(y1, sizeF.Bottom - 1/translator.Resolution.Y);

                    float y2 = this.f((float)x2);
                    y2 = MathF.Min(y2, sizeF.Top + 1/translator.Resolution.Y);
                    y2 = MathF.Max(y2, sizeF.Bottom - 1/translator.Resolution.Y);
                    
                    if (y2 < sizeF.Top && y2 > sizeF.Bottom)
                    {
                        graphics.DrawLine(this.pen, translator.Translate(new PointF(x1, y1)), translator.Translate(new PointF(x2, y2)));
                    }
                }
                catch
                {

                }

                x1 = x2;
            }
        }
    }
}
