using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public struct dftd
{
    public double real;
    public double imaginary;
};

namespace Sound_Editor
{
    

 
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //reset_chart();
        }


        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            double[] x = new double[8];
            double[] y = new double[8];
            for(int i = 0; i < 8; i++)
            {
                x[i] = i / 8.0;
                y[i] = Math.Sin(2*Math.PI*x[i]);
            }
            chart1.Series["Wave"].Points.DataBindXY(x, y);
            dft(x, y);

        }
        private void dft(double[] x, double[] y)
        {
            double[] f = new double[8];
            dftd[] data = new dftd[8];
            double[] magnitude = new double[8];
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    data[i].real += y[j] * Math.Cos(i*2*Math.PI*j/8);
                    data[i].imaginary += -1 * y[j] * Math.Sin(i * 2 * Math.PI * j / 8);
                }
            }
            
            for(int i = 0; i < 8; i++)
            {
                magnitude[i]= Math.Sqrt(data[i].real * data[i].real+ data[i].imaginary* data[i].imaginary);
                f[i] = i;
            }
            chart2.Series["AF"].Points.DataBindXY(f, magnitude);

        }
        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
