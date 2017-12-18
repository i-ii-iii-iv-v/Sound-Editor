using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;

namespace Sound_Editor
{
    public partial class Form3 : Form
    {
        private int N;
        private int S;
        private double[] fBin;
        private double[] A;
        double[] dftd_imagin;
        double[] dftd_real;
        double[] idft_s;
        int[] filter;
        double selectStart;
        double selectEnd;
        int[] convolute_s;
        double[] padded_s;
        double[] idft_filter;
        //object of frequency domain graph
        private Form2 form2;

        bool convoluted;
        public Task convolveThread1;
        public Task convolveThread2;
        public Task convolveThread3;
        public Task convolveThread4;
        public Task idftThread1;
        public Task idftThread2;
        public Task idftThread3;
        public Task idftThread4;

        public Task idftSThread1;
        public Task idftSThread2;
        public Task idftSThread3;
        public Task idftSThread4;
        /// <summary>
        /// initialization
        /// </summary>
        /// <param name="form2">initialized from parent window form1</param>
        public Form3(Form2 form2, int sampleRate, double[] A, double[] fBin, double[] real, double[] imagin, double max)
        {
            
            InitializeComponent();
            
            this.form2 = form2;
            S = sampleRate;
            this.A = A;
            this.fBin = fBin;
            dftd_imagin = imagin;
            dftd_real = real;
            N = fBin.Length;
            convoluted = false;
            selectStart = -1;
            if(fBin.Length != 1)
                chart2.ChartAreas[0].CursorX.Interval = fBin[1];

            chart2.ChartAreas[0].AxisY.Maximum = max*4/3 ;
            chart2.Series["AF"].Points.DataBindXY(fBin, A);
        }

        /// <summary>
        /// Sets chart properties and size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form3_Load(object sender, EventArgs e)
        {
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = fBin[fBin.Length - 1] + 1;
            chart2.ChartAreas[0].CursorX.AutoScroll = true;
            chart2.ChartAreas[0].CursorX.SelectionColor = Color.FromArgb(10, 255, 0);

            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorX.AutoScroll = true;
            chart2.ChartAreas[0].CursorX.Interval = fBin[1];
            chart2.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart2.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart2.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = fBin[1];
        }

        /// <summary>
        /// No event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Zooms in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double mult = fBin[fBin.Length-1] / 10;
            double min = chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            double max = chart2.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

            double newMin = min + mult;
            double newMax = max - mult;

            if (newMin > newMax)
            {
                MessageBox.Show(this, "Zoom maximized", "Invalid Zoom",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(newMin, newMax);
        }
        

        /// <summary>
        /// Zooms out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double mult = fBin[fBin.Length - 1] / 10;
            double min = chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            double max = chart2.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

            double newMin = min - mult;
            double newMax = max + mult;

            if (newMin < 0 || newMax > fBin.Length)
            {
                MessageBox.Show(this, "Zoom maximized", "Invalid Zoom",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(newMin, newMax);
        }

        private void zoomResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(0, fBin[fBin.Length-1] +1);
        }

        private void zoomSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectStart != -1)
            {
                chart2.ChartAreas[0].AxisX.ScaleView.Zoom(selectStart, selectEnd);
            }
            else
            {
                MessageBox.Show(this, "Please select valid range", "Invalid Zoom",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// updates selected points on graphs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart2_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            
            selectStart = chart2.ChartAreas[0].CursorX.SelectionStart;
            selectEnd = chart2.ChartAreas[0].CursorX.SelectionEnd;

            if (selectStart > selectEnd)
            {
                double temp = selectStart;
                selectStart = selectEnd;
                selectEnd = temp;
            }

            if (selectEnd > fBin[fBin.Length-1])
            {
                selectEnd = fBin[fBin.Length-1];
            }
            if (selectStart >= fBin[fBin.Length-1])
            {
                selectStart = fBin[fBin.Length - 1];
                selectEnd = fBin[fBin.Length - 1];
            }

            chart2.ChartAreas[0].AxisX.StripLines.Clear();
        }

        /// <summary>
        /// Thread Procedure for IDFT calcltates number of 
        /// samples each thread must deal with and performs
        /// idft on it
        /// </summary>
        /// <param name="idft_r">real portion</param>
        /// <param name="idft_i">imaginary portion</param>
        /// <param name="threadNumber">thread number that is used to divide up the task</param>
        private void idft(double[] idft_r, double[] idft_i, int threadNumber)
        {
            int sN = N;
            int numSamples = sN / 4;
            int endSampleIndex = threadNumber * numSamples;
            if (threadNumber == 4)
                endSampleIndex = sN;
            int startIndex = (threadNumber - 1) * numSamples;
            double op1;
            double op2;
            for (int t = startIndex; t < endSampleIndex; t++)
            {
                for (int f = 0; f < sN; f++)
                {
                    op1 = 1.0 * idft_r[f] / N * Math.Cos(2 * Math.PI * f * t / N);
                    op2 = -1.0 * idft_i[f] / N * Math.Sin(2 * Math.PI * f * t / N);
                    idft_s[t] += op1 + op2;
                }
            }
        }

        /// <summary>
        /// Thread proc for convolution
        /// </summary>
        /// <param name="idft_r">real portion of idft</param>
        /// <param name="idft_i">imgainary portion of idft</param>
        /// <param name="threadNumber">thread number that is used to divide up the task</param>
        private void idft(int[] idft_r, double[] idft_i, int threadNumber)
        {
            int filterN = idft_filter.Length;
            int numFilterValues = filterN / 4;
            int endFilterIndex = threadNumber * numFilterValues;
            if (threadNumber == 4)
                endFilterIndex = filterN;
            int startIndex = (threadNumber - 1) * numFilterValues;

            double op1;
            double op2;
            for (int t = startIndex; t < endFilterIndex; t++)
            {
                for (int f = 0; f < filterN; f++)
                {
                    op1 = 1.0* idft_r[f] / N * Math.Cos(2 * Math.PI * f * t / N);
                    op2 = -1.0 * idft_i[f] / N * Math.Sin(2 * Math.PI * f * t / N);
                    idft_filter[t] += op1 + op2;
                }
            }
        }

        /// <summary>
        /// IDFT button selected, creates threads and performs idft
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            idft_s = new double[N];

            idftSThread1 = new Task(() => idft(dftd_real, dftd_imagin, 1));
            idftSThread2 = new Task(() => idft(dftd_real, dftd_imagin, 2));
            idftSThread3 = new Task(() => idft(dftd_real, dftd_imagin, 3));
            idftSThread4 = new Task(() => idft(dftd_real, dftd_imagin, 4));
            idftSThread1.Start();
            idftSThread2.Start();
            idftSThread3.Start();
            idftSThread4.Start();
            await Task.WhenAll(idftSThread1, idftSThread2, idftSThread3, idftSThread4);

            button1.Enabled = true;
            Form2 temp2 = new Form2(idft_s, S);
            temp2.Show();
        }

        /// <summary>
        /// Group of error messages
        /// </summary>
        /// <returns></returns>
        private int errorMsg()
        {
            if ((int)selectEnd > (int)fBin[fBin.Length / 2])
            {
                MessageBox.Show(this, "You have gone past the Nyquiste limit", "Invalid Range Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            if (selectStart == -1)
            {
                MessageBox.Show(this, "select Valid range to filter", "Invalid Range Selection",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            if (convoluted == true)
            {
                MessageBox.Show(this, "Already applied filtering", "Invalid Filter",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Convolution button selected, creates thread and convolutes the samples in time domain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {
            if (errorMsg() == 1)
                return;
            if (selectStart != 0)
            {
                chart2.ChartAreas[0].CursorX.SelectionStart = 0;
                selectStart = 0;
            }
            button2.Enabled = false;
            convoluted = true;
            
            filterSetup();
            await Task.WhenAll(idftThread1, idftThread2, idftThread3, idftThread4);
            padforConvolve(idft_filter.Length);

            convolute_s = new int[form2.getYLength()];
            setupConvolveThreads();
            await Task.WhenAll(convolveThread1, convolveThread2, convolveThread3, convolveThread4);

            button2.Enabled = true;
            form2.filter(convolute_s);
        }

        /// <summary>
        /// makes filter based on user range selection, always creates low pass filter
        /// </summary>
        private void makeFilter()
        {
            int i = 0;
            double x = 0;

            while (x < selectEnd)
            {
                i++;
                x += fBin[1];
            }

            int k = N - i;
            filter = new int[N];
            for (int j = 0; j < filter.Length; j++)
            {
                filter[j] = 0;
            }
            for (int j = 0; j <= i; j++)
            {
                filter[j] = 1;
            }

            for (int j = k; j < N; j++)
            {
                filter[j] = 1;
            }
        }

        /// <summary>
        /// Creates threads for idfting filter and starts the threads
        /// </summary>
        private void filterSetup()
        {
            makeFilter();
            idft_filter = new double[N];
            double[] zeros = new double[N];
            for (int l = 0; l < N; l++)
            {
                zeros[l] = 0;
            }
            idftThread1 = new Task(() => idft(filter, zeros, 1));
            idftThread2 = new Task(() => idft(filter, zeros, 2));
            idftThread3 = new Task(() => idft(filter, zeros, 3));
            idftThread4 = new Task(() => idft(filter, zeros, 4));

            idftThread1.Start();
            idftThread2.Start();
            idftThread3.Start();
            idftThread4.Start();
        }

        /// <summary>
        /// Creates threads for convolving and starts the threads
        /// </summary>
        private void setupConvolveThreads()
        {
            convolveThread1 = new Task(() => convolve(idft_filter, 1));
            convolveThread2 = new Task(() => convolve(idft_filter, 2));
            convolveThread3 = new Task(() => convolve(idft_filter, 3));
            convolveThread4 = new Task(() => convolve(idft_filter, 4));
            convolveThread1.Start();
            convolveThread2.Start();
            convolveThread3.Start();
            convolveThread4.Start();
        }


        /// <summary>
        /// Thread Procedure for applying filters on samples
        /// </summary>
        /// <param name="filter_idft">convolved filter</param>
        /// <param name="threadNumber">thread number</param>
        public void convolve(double[] filter_idft, int threadNumber)
        {
            int i;
            int N1 = form2.getYLength();
            int M = filter_idft.Length;
            int numSamplesToProcess = N1 / 4;
            int endSampleNum = numSamplesToProcess * threadNumber;
            if (threadNumber == 4)
                endSampleNum = N1;
            int startIndex = (threadNumber - 1) * numSamplesToProcess;
            double s;
            for (i = startIndex; i < endSampleNum; i++)
            {
                s = 0;
                for (int j = 0; j < M; j++)
                {
                    s += padded_s[i + j] * filter_idft[j];
                }
                convolute_s[i] = (int)Math.Round(s);
            }
        }
        
        /// <summary>
        /// Creates samples with padded zeros for convolution
        /// </summary>
        /// <param name="M">Size of filter</param>
        public void padforConvolve(int M)
        {
            int originalN = form2.getYLength();
            padded_s = new double[originalN + M - 1];
            int[] y = form2.getY();
            int i = 0;
            for (i = 0; i < originalN; i++)
            {
                padded_s[i] = y[i];
            }

            for (; i < padded_s.Length; i++)
            {
                padded_s[i] = 0;
            }
        }

        /// <summary>
        /// Mirrors selected frequency bins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart2_SelectionRangeChanging(object sender, CursorEventArgs e)
        {
            chart2.ChartAreas[0].AxisX.StripLines.Clear();
            StripLine stripline = new StripLine();
            double end;
            end = chart2.ChartAreas[0].CursorX.SelectionEnd;
            stripline.IntervalOffset = fBin[fBin.Length - 1] - end;
            stripline.StripWidth = fBin[1];
            stripline.BackColor = Color.Red;
            stripline.Interval = fBin[1];
            chart2.ChartAreas[0].AxisX.StripLines.Add(stripline);
        }
    }

    
}
