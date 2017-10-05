using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sound_Editor
{
    public partial class Form3 : Form
    {
        private Form2 form2;
        public Form3(Form2 form2)
        {
            InitializeComponent();
            
            this.form2 = form2;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        public void dft(double[] x, double[] y)
        {
            double[] f = new double[8];
            dftd[] data = new dftd[8];
            double[] magnitude = new double[8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    data[i].real += y[j] * Math.Cos(i * 2 * Math.PI * j / 8);
                    data[i].imaginary += -1 * y[j] * Math.Sin(i * 2 * Math.PI * j / 8);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                magnitude[i] = Math.Sqrt(data[i].real * data[i].real + data[i].imaginary * data[i].imaginary);
                f[i] = i;
            }
            chart2.Series["AF"].Points.DataBindXY(f, magnitude);

        }
        public void getForm2(Form2 form)
        {
            form2 = form;
        }
    }
}
