using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DrawingDesk
{
    public interface IPointTranslator
    {
        Point Translate(PointF point);
        PointF Resolution { get; }
    }
    public class DrawingDesk : IPointTranslator
    {
        private readonly List<DrawingFigure> figures = new List<DrawingFigure>();
        public Size BitmapSize { get; set; } = new Size(100, 100);
        public Color BackgroundColor { get; set; } = Color.White;
        public Point Origin { get; set; }
        public PointF Resolution { get; set; } = new PointF(1, 1);
        public Bitmap BuildBitmap(uint tags = 0b_11111111_11111111_11111111_11111111)
        {            
            Bitmap result = new Bitmap(this.BitmapSize.Width, this.BitmapSize.Height, PixelFormat.Format24bppRgb);

            Graphics graphics = Graphics.FromImage(result);

            graphics.Clear(this.BackgroundColor);

            RectangleF size = new RectangleF(
                -this.Origin.X / this.Resolution.X,
                this.Origin.Y / this.Resolution.Y,
                this.BitmapSize.Width  / this.Resolution.X,
                -this.BitmapSize.Height / this.Resolution.Y);

            foreach (var item in this.figures)
            {
                if ((tags & item.Tag) == item.Tag)
                {
                    item.Draw(size, graphics, this);
                }
            }

            return result;
        }

        public void Draw(DrawingFigure figure)
        {
            if (figure is null)
            {
                throw new ArgumentNullException(nameof(figure));
            }

            this.figures.Add(figure);
        }

        public void Clear()
        {
            this.figures.Clear();
        }

        public Point Translate(PointF point)
        {
            int x = this.Origin.X + (int)Math.Floor(point.X * this.Resolution.X);
            int y = this.Origin.Y - (int)Math.Floor(point.Y * this.Resolution.Y);

            return new Point(x, y);
        }
    }
}
