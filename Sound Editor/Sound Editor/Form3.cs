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

        public struct dftd
        {
            public double real;
            public double imaginary;
        };

        private void Form3_Load(object sender, EventArgs e)
        {
            dft(null, null);
        }
        public void dft(double[] x, double[] y)
        {
            int S = 40;
            int N = 10;
            x = new double[N];
            y = new double[N];
            double[] f = new double[N]; //frequency bin
            dftd[] data = new dftd[N]; //real part and imaginary part
            double[] A = new double[N]; //Amplitude

            for (int i = 0; i < N; i++)//sample data in y: s(t)
            {
                x[i] = i / (double)N;
                y[i] = Math.Cos(2 * Math.PI * 3 * x[i])*2 + Math.Sin(2 * Math.PI * 14 * x[i])*3;
            }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    data[i].real += y[j] * Math.Cos(i * 2 * Math.PI * j / N);
                    data[i].imaginary += -1 * y[j] * Math.Sin(i * 2 * Math.PI * j / N);
                }
                Console.WriteLine("real " + i + ": " + data[i].real);
                Console.WriteLine("imag " + i + ": " + data[i].imaginary);
                A[i] = Math.Sqrt(data[i].real * data[i].real + data[i].imaginary * data[i].imaginary);
                f[i] = i*S/N;
            }
            chart2.Series["AF"].Points.DataBindXY(f, A);
        }
        public void getForm2(Form2 form)
        {
            form2 = form;
        }
    }
}
