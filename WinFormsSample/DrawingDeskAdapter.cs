using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
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
        private bool pending = false;
        private object SyncObj = new object();

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

        private async Task Update()
        {
            if (this.pending)
            {
                return;
            }

            this.pending = true;
            
            await Task.Delay(20);

            pending = false;
            lock (this.SyncObj)
            {
                this.pictureBox.Image = this.drawingDesk.BuildBitmap();
            }
        }

        private async void PictureBox_Resize(object sender, EventArgs e)
        {
            lock (this.SyncObj)
            {
                this.drawingDesk.BitmapSize = pictureBox.Size;
            }

            await this.Update();
        }

        private async void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                lock (this.SyncObj)
                {
                    this.drawingDesk.Resolution = new PointF(this.drawingDesk.Resolution.X * (1 + e.Delta / 1200f), this.drawingDesk.Resolution.Y * (1 + e.Delta / 1200f));
                }

                await this.Update();
            }
        }

        private async void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {            
            if (this.mouseDown != null)
            {
                lock (this.SyncObj)
                {
                    this.drawingDesk.Origin = new Point(
                    e.Location.X - this.mouseDown.Value.X + this.origin.Value.X,
                    e.Location.Y - this.mouseDown.Value.Y + this.origin.Value.Y);
                }

                await this.Update();
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
