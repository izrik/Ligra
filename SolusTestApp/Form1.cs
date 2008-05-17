using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetaphysicsIndustries.Solus;

namespace SolusTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SolusEngine engine = new SolusEngine();
            System.IO.Directory.SetCurrentDirectory("C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images");

            SolusMatrix tank = engine.LoadImage("tank256.bmp");
            SolusVector a = tank.GetRow(0);

            VectorFilter filter = new MovingAverageVectorFilter(5);

            SolusVector b = filter.Apply(a);
        }
    }
}