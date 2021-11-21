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

        private float Limit(float y, RectangleF sizeF)
        {
            if (y > sizeF.Top)
            {
                y = sizeF.Top;
            }

            if (y < sizeF.Bottom)
            {
                y = sizeF.Bottom;
            }

            return y;
        }

        public override void DrawInner(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            float dx = 1 / translator.Resolution.X;
            float x1 = sizeF.X;
            float? y1 = null;
                        
            float dy = 0;
            float ylim = 5 / translator.Resolution.Y;

            while (x1 < sizeF.Right)
            {
                float x2 = x1 + dx;
                while (x2 == x1)
                {
                    dx = dx + 0.01f / translator.Resolution.X;
                    x2 = x1 + dx;
                }

                try
                {
                    if (!y1.HasValue)
                    {
                        y1 = this.f((float)x1);
                    }

                    float y2 = this.f((float)x2);
                    
                    if ((y1.Value < sizeF.Top && y1.Value > sizeF.Bottom) || (y2 < sizeF.Top && y2 > sizeF.Bottom))
                    {   
                        dy = MathF.Abs(y2 - y1.Value);
                        float adaptivedx = dx;
                        while (dy > ylim)
                        {
                            adaptivedx = adaptivedx / 2;

                            if (x1 != x1 + adaptivedx && this.f(x1 + adaptivedx) != y2)
                            {
                                x2 = x1 + adaptivedx;
                                y2 = this.f((float)x2);
                                dy = MathF.Abs(y2 - y1.Value);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (dy < ylim)
                        {                            
                            PointF pt1 = translator.TranslateF(new PointF(x1, Limit(y1.Value, sizeF)));
                            PointF pt2 = translator.TranslateF(new PointF(x2, Limit(y2, sizeF)));                                                        
                            //graphics.FillRectangle(this.pen.Brush, pt2.X-0.5f, pt2.Y-0.5f, 1, 1);
                            graphics.DrawLine(this.pen, pt1, pt2);
                        }
                    }

                    y1 = y2;
                }
                catch
                {

                }

                x1 = x2;
            }
        }
    }
}
