using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DrawingDesk;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DrawingDesk.DrawingDesk drawingDesk = new DrawingDesk.DrawingDesk();

        private void Form1_Load(object sender, EventArgs e)
        {
            this.drawingDesk.Draw(new Line(new PointF(-10, 0), new PointF(10, 0)));
            this.drawingDesk.Draw(new Line(new PointF(0, -10), new PointF(0, 10)));
            this.drawingDesk.Draw(new Line(new PointF(-1, -1), new PointF(2, 2)));
            DrawingDeskAdapter adapter = new DrawingDeskAdapter(this.pictureBox1, this.drawingDesk);
            this.pictureBox1.Image = this.drawingDesk.BuildBitmap();
        }
    }
}
