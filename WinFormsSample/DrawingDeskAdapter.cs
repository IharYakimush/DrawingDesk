using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsSample
{
    public class DrawingDeskAdapter
    {
        private readonly PictureBox pictureBox;
        private readonly DrawingDesk.DrawingDesk drawingDesk;
        private Point? mouseDown = null;
        private Point? origin = null;

        public DrawingDeskAdapter(PictureBox pictureBox, DrawingDesk.DrawingDesk drawingDesk)
        {
            if (pictureBox is null)
            {
                throw new ArgumentNullException(nameof(pictureBox));
            }

            if (drawingDesk is null)
            {
                throw new ArgumentNullException(nameof(drawingDesk));
            }

            this.pictureBox = pictureBox;
            this.drawingDesk = drawingDesk;
            this.drawingDesk.BitmapSize = pictureBox.Size;
            this.drawingDesk.Origin = new Point(this.pictureBox.Width / 2, this.pictureBox.Height / 2);

            this.pictureBox.MouseDown += this.PictureBox_MouseDown;
            this.pictureBox.MouseUp += this.PictureBox_MouseUp;
            this.pictureBox.MouseMove += this.PictureBox_MouseMove;
            this.pictureBox.MouseWheel += this.PictureBox_MouseWheel;
            this.pictureBox.Resize += this.PictureBox_Resize;
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            this.drawingDesk.BitmapSize = pictureBox.Size;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                this.drawingDesk.Resolution = new PointF(this.drawingDesk.Resolution.X * (1 + e.Delta/1200f), this.drawingDesk.Resolution.Y * (1+e.Delta / 1200f));

                // TODO: update image with delay, batch changes
                this.pictureBox.Image = this.drawingDesk.BuildBitmap();
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {            
            if (this.mouseDown != null)
            {
                this.drawingDesk.Origin = new Point(
                    e.Location.X - this.mouseDown.Value.X + this.origin.Value.X,
                    e.Location.Y - this.mouseDown.Value.Y + this.origin.Value.Y);

                // TODO: update image with delay, batch changes
                this.pictureBox.Image = this.drawingDesk.BuildBitmap();
            }

            
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = null;
            this.origin = null;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDown = e.Location;
            this.origin = this.drawingDesk.Origin;
        }
    }
}
