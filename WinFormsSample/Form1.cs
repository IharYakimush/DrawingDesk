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
        private DrawingDeskAdapter adapter;

        private DrawingDesk.Padding graphPadding = new DrawingDesk.Padding() { Left = 100, Right = 50, Top = 30, Bottom = 20, Pixel = true };

        private void Form1_Load(object sender, EventArgs e)
        {
            this.drawingDesk.Draw(new Axis(new KeyValuePair<float, string>(MathF.PI, "π")) {Padding = graphPadding });
            this.drawingDesk.Draw(new FunctionGraph(x => MathF.Sin(x), Pens.Green) { Padding = graphPadding });
            this.drawingDesk.Draw(new FunctionGraph(x => MathF.Pow(x, 3), Pens.Blue) { Padding = graphPadding });
            this.drawingDesk.Draw(new FunctionGraph(x => 1/(x + 1f), Pens.Red) { Padding = graphPadding });

            this.adapter = new DrawingDeskAdapter(this.pictureBox1, this.drawingDesk);
            this.adapter.SetResolutionControl(this.numericUpDown1);
            this.adapter.SetCenterControls(this.numericUpDown2, this.numericUpDown3);
            this.pictureBox1.Image = this.drawingDesk.BuildBitmap();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
