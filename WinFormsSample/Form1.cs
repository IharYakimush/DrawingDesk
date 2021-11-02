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
            this.drawingDesk.Resolution = new PointF(100, 100);

            this.drawingDesk.Draw(new Axis(new KeyValuePair<float, string>(MathF.PI, "π")));
            this.drawingDesk.Draw(new FunctionGraph(x => MathF.Sin(x), Pens.Green));

            DrawingDeskAdapter adapter = new DrawingDeskAdapter(this.pictureBox1, this.drawingDesk);
            this.pictureBox1.Image = this.drawingDesk.BuildBitmap();
        }
    }
}
