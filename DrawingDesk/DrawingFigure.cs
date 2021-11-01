using System.Drawing;

namespace DrawingDesk
{
    public abstract class DrawingFigure    
    {
        public uint Tag { get; set; } = 0b_11111111_11111111_11111111_11111111;
        public abstract void Draw(RectangleF sizeF, Graphics graphics, IPointTranslator translator);
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

        public override void Draw(RectangleF sizeF, Graphics graphics, IPointTranslator translator)
        {
            //TODO: crop points out of size
            graphics.DrawLine(this.Pen, translator.Translate(this.P1), translator.Translate(this.P2));
        }
    }
}
