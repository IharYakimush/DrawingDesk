using System;

using System.Drawing;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsSample
{
    public class DrawingDeskAdapter
    {
        private const int ResolutionFactor = 100;
        private NumericUpDown resolutionControl = null;
        private NumericUpDown centerXControl = null;
        private NumericUpDown centerYControl = null;

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
            this.drawingDesk.Resolution = new PointF(this.drawingDesk.BitmapSize.Width / 10f, this.drawingDesk.BitmapSize.Width / 10f);
            this.drawingDesk.Origin = new Point(this.pictureBox.Width / 2, this.pictureBox.Height / 2);

            this.pictureBox.MouseDown += this.PictureBox_MouseDown;
            this.pictureBox.MouseUp += this.PictureBox_MouseUp;
            this.pictureBox.MouseMove += this.PictureBox_MouseMove;
            this.pictureBox.MouseWheel += this.PictureBox_MouseWheel;
            this.pictureBox.Resize += this.PictureBox_Resize;            
        }

        public void SetResolutionControl(NumericUpDown value)
        {
            this.resolutionControl = value ?? throw new ArgumentNullException();
            this.resolutionControl.Value = (decimal)(this.drawingDesk.BitmapSize.Width / this.drawingDesk.Resolution.X);
            this.resolutionControl.ValueChanged += this.ResolutionControl_ValueChanged;
        }

        public void SetCenterControls(NumericUpDown x, NumericUpDown y)
        {
            this.centerXControl = x ?? throw new ArgumentNullException();
            this.centerYControl = y ?? throw new ArgumentNullException();

            this.InitCenterValues();

            this.centerXControl.ValueChanged += this.CenterXControl_ValueChanged;
            this.centerYControl.ValueChanged += this.CenterYControl_ValueChanged;
        }

        private void InitCenterValues()
        {
            if (this.centerXControl != null)
            {
                bool limit = false;
                decimal v = (decimal)((this.drawingDesk.BitmapSize.Width / 2 - this.drawingDesk.Origin.X) / this.drawingDesk.Resolution.X);

                if (v > this.centerXControl.Maximum)
                {
                    v = this.centerXControl.Maximum;
                    limit = true;
                }

                if (v < this.centerXControl.Minimum)
                {
                    v = this.centerXControl.Minimum;
                    limit = true;
                }

                this.centerXControl.Value = v;

                if (limit)
                {
                    this.CenterXControl_ValueChanged(null, null);
                }
            }

            if (this.centerYControl != null)
            {
                bool limit = false;
                decimal v = (decimal)((-this.drawingDesk.BitmapSize.Height / 2 + this.drawingDesk.Origin.Y) / this.drawingDesk.Resolution.Y);

                if (v > this.centerYControl.Maximum)
                {
                    v = this.centerYControl.Maximum;
                    limit = true;
                }

                if (v < this.centerYControl.Minimum)
                {
                    v = this.centerYControl.Minimum;
                    limit = true;
                }

                this.centerYControl.Value = v;

                if (limit)
                {
                    this.CenterYControl_ValueChanged(null, null);
                }
            }
        }

        private async void CenterYControl_ValueChanged(object sender, EventArgs e)
        {
            this.drawingDesk.Origin = new Point(
                    this.drawingDesk.Origin.X,
                    (int)Math.Floor(this.drawingDesk.BitmapSize.Height / 2 + (float)this.centerYControl.Value * this.drawingDesk.Resolution.Y));

            await this.Update();
        }

        private async void CenterXControl_ValueChanged(object sender, EventArgs e)
        {
            this.drawingDesk.Origin = new Point(
                    (int)Math.Floor(this.drawingDesk.BitmapSize.Width / 2 - (float)this.centerXControl.Value * this.drawingDesk.Resolution.X),
                    this.drawingDesk.Origin.Y);

            await this.Update();
        }

        private async void ResolutionControl_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)(this.drawingDesk.BitmapSize.Width / this.resolutionControl.Value);

            this.drawingDesk.Resolution = new PointF(value, value);
            this.InitCenterValues();
            await this.Update();
        }

        public async Task Update()
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
            bool update = pictureBox.Size.Width > 0 && pictureBox.Size.Height > 0;
            lock (this.SyncObj)
            {
                if (update)
                {
                    this.drawingDesk.BitmapSize = pictureBox.Size;
                    if (this.resolutionControl != null)
                    {
                        this.ResolutionControl_ValueChanged(null, null);
                    }
                }
            }

            if (centerXControl != null && update)
            {
                this.CenterXControl_ValueChanged(null, null);
            }

            if (centerYControl != null && update)
            {
                this.CenterYControl_ValueChanged(null, null);
            }

            if (update)
            {
                await this.Update();
            }
        }

        private async void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                lock (this.SyncObj)
                {
                    this.drawingDesk.Resolution = new PointF(this.drawingDesk.Resolution.X * (1 + e.Delta / 1200f), this.drawingDesk.Resolution.Y * (1 + e.Delta / 1200f));
                    if (this.resolutionControl != null)
                    {
                        bool limit = false;
                        decimal v = new decimal(this.drawingDesk.BitmapSize.Width / this.drawingDesk.Resolution.X);

                        if (this.resolutionControl != null)
                        {
                            if (v < this.resolutionControl.Minimum)
                            {
                                v = this.resolutionControl.Minimum;
                                limit = true;
                            }

                            if (v > this.resolutionControl.Maximum)
                            {
                                v = this.resolutionControl.Maximum;
                                limit = true;
                            }

                            this.resolutionControl.Value = v;

                            if (limit)
                            {
                                this.drawingDesk.Resolution = new PointF((float)(this.drawingDesk.BitmapSize.Width / v), (float)(this.drawingDesk.BitmapSize.Width / v));
                            }
                        }
                    }
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

                    this.InitCenterValues();
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
