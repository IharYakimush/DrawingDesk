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
        public override void Draw(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            float dx = 1 / translator.Resolution.X;
            float x1 = sizeF.X;

            while (x1 < sizeF.Right)
            {
                float x2 = x1 + dx;

                try
                {
                    float y1 = this.f(x1);
                    float y2 = this.f(x2);

                    graphics.DrawLine(this.pen, translator.Translate(new PointF(x1, y1)), translator.Translate(new PointF(x2, y2)));
                }
                catch
                {

                }

                x1 = x2;
            }
        }
    }
}
