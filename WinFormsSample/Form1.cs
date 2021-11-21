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
        private static DrawingDesk.Padding graphPadding = new DrawingDesk.Padding() { Left = 20, Right = 20, Top = 20, Bottom = 20, Pixel = true };
        private readonly List<FunctionGraph> graphs = new List<FunctionGraph> 
        { 
            new FunctionGraph(x => MathF.Sin(x), Pens.Green) { Padding = graphPadding } ,
            new FunctionGraph(x => MathF.Pow(x, 3), Pens.Blue) { Padding = graphPadding },
            new FunctionGraph(x => 1/(x + 1f), Pens.Red) { Padding = graphPadding }
        };

        private KeyValuePair<float, string>? axisx = null;
        private KeyValuePair<float, string>? axisy = null;
        private bool linex = true;
        private bool liney = true;

        private void Form1_Load(object sender, EventArgs e)
        {            
            this.adapter = new DrawingDeskAdapter(this.pictureBox1, this.drawingDesk);
            this.adapter.SetResolutionControl(this.numericUpDown1);
            this.adapter.SetCenterControls(this.numericUpDown2, this.numericUpDown3);

            this.Draw();
        }

        private void Draw()
        {
            this.drawingDesk.Clear();
            this.drawingDesk.Draw(new Axis(this.axisx, this.axisy) { Padding = graphPadding, LineX = this.linex, LineY = this.liney });

            foreach (var item in this.graphs)
            {
                this.drawingDesk.Draw(item);
            }

            this.pictureBox1.Image = this.drawingDesk.BuildBitmap();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox1.Text)
            {
                case "None": { this.linex = false; break; }
                case "1": { this.linex = true; this.axisx = null; break; }
                case "π": { this.linex = true; this.axisx = new KeyValuePair<float, string>(MathF.PI, "π"); break; }
                default:
                    throw new NotSupportedException();
            }

            this.Draw();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox2.Text)
            {
                case "None": { this.liney = false; break; }
                case "1": { this.liney = true; this.axisy = null; break; }
                case "π": { this.liney = true; this.axisy = new KeyValuePair<float, string>(MathF.PI, "π"); break; }
                default:
                    throw new NotSupportedException();
            }

            this.Draw();
        }
    }
}
