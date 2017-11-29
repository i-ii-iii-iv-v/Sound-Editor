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
        //Sampling rate
        int S;
        //Number of Samples
        int N;
        //x: represents sample number 0~N
        int[] x;
        //y: represents data value
        int[] y;


        //weights to use when doing forward fourier transform, always same size as Number of samples
        double[] w;
        //frequency bins: maxFrequency*N/S
        double[] f;
        //amplitude of each frequency in frequency domain graph
        double[] A;


        //object of frequency domain graph
        private Form2 form2;

        /// <summary>
        /// initialization
        /// </summary>
        /// <param name="form2">initialized from parent window form1</param>
        public Form3(Form2 form2)
        {
            InitializeComponent();
            
            this.form2 = form2;

        }

        /// <summary>
        /// real and imaginary parts for forward fouriers
        /// </summary>
        public struct dftd
        {
            public double real;
            public double imaginary;
        };

        private void Form3_Load(object sender, EventArgs e)
        {
            //???
            //dft(null, null);
        }

        /// <summary>
        /// forword fourier
        /// </summary>
        /// <param name="x">don't actually need</param>
        /// <param name="y">values to fourier</param>
        /// 
        public void dft()
        {
            S = 8;
            N = 8;
            x = new int[N];
            y = new int[N];
            w = new double[N];
            f = new double[N]; //frequency bin
            dftd[] data = new dftd[N]; //real part and imaginary part
            A = new double[N]; //Amplitude
            
            /*
            for (int i = 0; i < N; i++)//sample data in y: s(t)
            {
                x[i] = i / (double)N;
                y[i] = Math.Sin(2 * Math.PI * x[i]);
                if (radioButton2.Checked == true)
                    y[i] *= w[i];
            }*/
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

        /// <summary>
        /// calculuates the weights using welch window function
        /// </summary>
        public void welchWindow()
        {

            for (int i = 0; i < N; i++)
            {
                w[i] = 1 - ((i - (N - 1) / 2) / ((N - 1) / 2)) * ((i - (N - 1) / 2) / ((N - 1) / 2));
            }
        }

        /// <summary>
        /// sets window weights to 0
        /// </summary>
        public void noWindow()
        {
            for (int i = 0; i < N; i++)
            {
                w[i] = 0;
            }
        }
        public void getForm2(Form2 form)
        {
            form2 = form;
        }

        public void setDataX()
        {
            
        }

        public void setDataY()
        {

        }

        public void openFile()
        {

        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                //dft(null, null);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked==true)
            {
                //dft(null, null);
            }
        }
    }
}
