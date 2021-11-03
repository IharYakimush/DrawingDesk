using System.Drawing;

namespace DrawingDesk
{
    public abstract class DrawingFigure
    {
        public Padding Padding { get; set; } = new Padding();
        public uint Tag { get; set; } = 0b_11111111_11111111_11111111_11111111;
        public void Draw(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            if (this.Padding.Pixel)
            {
                sizeF.X = sizeF.X + this.Padding.Left / translator.Resolution.X;
                sizeF.Width = sizeF.Width - (this.Padding.Left + this.Padding.Right) / translator.Resolution.X;

                sizeF.Y = sizeF.Y - this.Padding.Top / translator.Resolution.Y;
                sizeF.Height = sizeF.Height + (this.Padding.Top + this.Padding.Bottom) / translator.Resolution.Y;
            }
            else
            {
                sizeF.X = sizeF.X + this.Padding.Left;
                sizeF.Width = sizeF.Width - (this.Padding.Left + this.Padding.Right);

                sizeF.Y = sizeF.Y - this.Padding.Top;
                sizeF.Height = sizeF.Height + (this.Padding.Top + this.Padding.Bottom);
            }

            this.DrawInner(sizeF, graphics, translator);
        }

        public abstract void DrawInner(RectangleF sizeF, Graphics graphics, IPointTranslator translator);
    }

    public class Line : DrawingFigure
    {
        public Line(PointF p1, PointF p2, Pen pen = null)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.Pen = pen ?? Pens.Black;
        }

        public PointF P1 { get; }
        public PointF P2 { get; }
        public Pen Pen { get; }

        public override void DrawInner(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            //TODO: crop points out of size
            graphics.DrawLine(this.Pen, translator.Translate(this.P1), translator.Translate(this.P2));
        }
    }
}
