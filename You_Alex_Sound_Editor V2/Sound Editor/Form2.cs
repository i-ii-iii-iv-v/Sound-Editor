using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace Sound_Editor
{
    public partial class Form2 : Form
    {
        private Form1 mainForm;
        private Form3 form3;
        private int[] x;
        private int[] y;
        private int[] tempx;
        private int[] tempy;
        private int[] pastex;
        private int[] pastey;
        private int selectStart;
        private int selectEnd;
        private double[] w;
        private int windowSet;
        private int N;
        private int S;
        private double[] fBin;
        private double[] A;
        private double[] s;
        //private Task[] thread1;
        private Task thread1;
        private Task thread2;
        private Task thread3;
        private Task thread4;
        double[] dftd_imagin;
        double[] dftd_real;

        /// <summary>
        /// Constructor for samples that are 8 bits
        /// </summary>
        /// <param name="data">Samples of sound</param>
        /// <param name="f">Parent form of this form</param>
        /// <param name="SampleRate">Sampling rate of Sound</param>
        public Form2(byte[] data, Form1 f, int SampleRate)
        {
            InitializeComponent();
            mainForm = f;
            y = new int[data.Length];
            x = new int[data.Length];
            for(int i = 0; i < data.Length; i++)
            {
                y[i] = data[i];
                x[i] = i;
            }
            S = SampleRate;
            selectStart = -1;
            selectEnd = -1;
        }

        /// <summary>
        /// Constructor for samples that are 16 bit
        /// </summary>
        /// <param name="data">Samples of sound</param>
        /// <param name="f">Parent form of this form</param>
        /// <param name="SampleRate">Sampling rate of Sound</param>
        public Form2(Int16[] data, Form1 f, int SampleRate)
        {
            InitializeComponent();
            mainForm = f;
           
            y = new int[data.Length];
            x = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                y[i] = data[i];
                x[i] = i;
            }
            S = SampleRate;
            selectStart = -1;
            selectEnd = -1;
        }

        /// <summary>
        /// Constructor that is mainly used for showing result of IDFT
        /// </summary>
        /// <param name="data">Samples of sound</param>
        /// <param name="SampleRate">Sampling rate of Sound</param>
        public Form2(double[] data, int SampleRate)
        {
            InitializeComponent();
            y = new int[data.Length];
            x = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                y[i] = (int)data[i];
                x[i] = i;
            }
            S = SampleRate;
            selectStart = -1;
            selectEnd = -1;
        }

        /// <summary>
        /// Sets up the chart property and its settings
        /// </summary>
        /// <param name="sender">object invoked from</param>
        /// <param name="e">evnet</param>
        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = x[x.Length - 1] + 1;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;

            chart1.ChartAreas[0].CursorX.SelectionColor = Color.FromArgb(10, 255, 0);
            
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.Interval = 1;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = 0.5;
            
            chart1.Series["Wave"].Points.DataBindXY(x, y);
            //form3.dft(x, y);
        }

        /// <summary>
        /// no event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// stores the selected chart position in selectStart and selectEnd variables
        /// </summary>
        /// <param name="sender">object invoked from</param>
        /// <param name="e">evnet</param>
        private void chart1_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            selectStart = (int)chart1.ChartAreas[0].CursorX.SelectionStart;
            selectEnd = (int)chart1.ChartAreas[0].CursorX.SelectionEnd;

            if(selectStart > selectEnd)
            {
                int temp = selectStart;
                selectStart = selectEnd;
                selectEnd = temp;
            }

            if (selectEnd >= x.Length)
            {
                selectEnd = x.Length - 1;
            }
            if (selectStart >= x.Length)
            {
                selectStart = x.Length - 1;
                selectEnd = x.Length - 1;
            }
            N = selectEnd - selectStart + 1;

        }


        /// <summary>
        /// Standard windowing function
        /// </summary>
        /// <param name="sender">object invoked from</param>
        /// <param name="e">evnet</param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                windowFunction(0);
                windowSet = 0;
            }
        }

        /// <summary>
        /// Welch windowing function 
        /// </summary>
        /// <param name="sender">object invoked from</param>
        /// <param name="e">evnet</param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked)
            {
                windowFunction(1);
                windowSet = 1;
            }
        }

        /// <summary>
        /// calculates the weigths based on user selected window
        /// 0: standard
        /// 1: welch
        /// </summary>
        /// <param name="value">window function</param>
        private void windowFunction(int value)
        {
            if(s.Length == 0)
            {
                return;
            }
            int sN = s.Length;
            if(value == 0)
            {
                w = new double[sN];
                for (int i = 0; i < sN; i++)
                {
                    w[i] = 1;
                }
            }

            else if(value == 1)
            {
                w = new double[sN];
                for (int i = 0; i < sN; i++)
                    w[i] = 1 - ((i - (sN - 1) / 2) / ((sN - 1) / 2)) * ((i - (sN - 1) / 2) / ((sN - 1) / 2));
            }
        }

        /// <summary>
        /// Performs DFT on the samples selected using threads. 
        /// </summary>
        /// <param name="sender">object invoked from</param>
        /// <param name="e">evnet</param>
        private async void button5_ClickAsync(object sender, EventArgs e)
        {
            
            if (selectStart == -1)
            {
                MessageBox.Show(this, "Please Select points to DFT", "Invalid Range",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button5.Enabled = false;
            N = selectEnd - selectStart + 1;


            dftd_real = new double[N];
            dftd_imagin = new double[N];
            fBin = new double[N]; //frequency bin
            A = new double[N]; //Amplitude
            s = new double[N];
            for (int i = 0; i < N; i++)
            {
                s[i] = y[selectStart + i];
            }
            windowFunction(windowSet);
            for (int i = 0; i < s.Length; i++)
                s[i] = s[i] * w[i];
            if (selectStart != -1)
            {

                thread1 = new Task(() => dft(1));
                thread2 = new Task(() => dft(2));
                thread3 = new Task(() => dft(3));
                thread4 = new Task(() => dft(4));
                thread1.Start();
                thread2.Start();
                thread3.Start();
                thread4.Start();
                await Task.WhenAll(thread1, thread2, thread3, thread4);
                button5.Enabled = true; 

                if (form3 == null)
                {
                    double largest = findMax();
                    form3 = new Form3(this, S, A, fBin, dftd_real, dftd_imagin, largest);
                    form3.MdiParent = mainForm;
                    form3.Show();
                    form3.Location = new Point(10, 300);
                }
                else
                {
                    double largest = findMax();
                    form3.Close();
                    form3 = new Form3(this, S, A, fBin, dftd_real, dftd_imagin, largest);
                    form3.MdiParent = mainForm;
                    form3.Show();
                    form3.Location = new Point(10, 300);
                }

            }

            else
            {
                MessageBox.Show(this, "Please select range of points to FDFT", "Invalid Range",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        

        /// <summary>
        /// Thread Procedure
        /// calculates number of samples each thread must work with 
        /// and performs DFT on it. The Race condition does not happen in this case
        /// because no two threads are trying to 
        /// </summary>
        /// <param name="threadNumber"></param>
        public void dft(int threadNumber)
        {
            int numSamplesToProcess = N / 4;
            int endSampleNum = numSamplesToProcess * threadNumber;
            if (threadNumber == 4)
                endSampleNum = N; 
            int startIndex = (threadNumber - 1) * numSamplesToProcess;
            
            for (int f = startIndex; f < endSampleNum; f++)
            {
                for (int t = 0; t < N; t++)
                {
                    dftd_real[f] += s[t] * Math.Cos(f * 2 * Math.PI * t / N);
                    dftd_imagin[f] += -1 * s[t] * Math.Sin(f * 2 * Math.PI * t / N);
                }
                A[f] = Math.Sqrt(dftd_real[f] * dftd_real[f] + dftd_imagin[f] * dftd_imagin[f]);
                fBin[f] = 1.0 * f * S / N; //frequency for each bin
            }
        }
        /// <summary>
        /// updataes the time domain graph that has new maximum x axis
        /// clears the current points and replaces it with new data points
        /// </summary>
        private void updateTDGraph(int originalSize)
        {
            chart1.Series["Wave"].Points.SuspendUpdates();
            while (chart1.Series["Wave"].Points.Count > 0)
                chart1.Series["Wave"].Points.RemoveAt(chart1.Series["Wave"].Points.Count - 1);
            chart1.Series["Wave"].Points.ResumeUpdates();
            
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            if (x.Length != 0)
                chart1.ChartAreas[0].AxisX.Maximum = x[x.Length - 1] + 1;
            else
                chart1.ChartAreas[0].AxisX.Maximum = 1;
            chart1.Series["Wave"].Points.DataBindXY(x, y);
            chart1.ChartAreas[0].CursorX.SelectionStart = -1; 
            chart1.ChartAreas[0].CursorX.SelectionEnd = -1;
            selectStart = -1;
            selectEnd = -1;
        }

        /// <summary>
        /// copies selected points by user and 
        /// stores it in the tempy array
        /// </summary>
        private void copySelected()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            int xInitial = selectStart;
            int xEnd = selectEnd;

            int size = xEnd - xInitial + 1;
            tempx = new int[size];
            tempy = new int[size];

            for (int i = 0; i < size; i++)
            {
                tempx[i] = i;
                tempy[i] = y[xInitial + i];
            }

            sw.Stop();
            Console.WriteLine("copy: " + sw.Elapsed);
        }


        /// <summary>
        /// deletes selected points and assigns it to x and y array
        /// </summary>
        private void deleteSelected()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int size = selectEnd - selectStart + 1;
            int[] cutX = new int[x.Length - size];
            int[] cutY = new int[x.Length - size];

            //move cut data from original array to temp cut array, cutX & cutY
            //cutX,Y array will store data with cut data
            for (int i = 0, j = 0; i < x.Length; i++)
            {
                if (i >= selectStart && i <= selectEnd)
                    continue;
                cutX[j] = x[j];
                cutY[j] = y[i];
                j++;
            }

            x = cutX;
            y = cutY;
            sw.Stop();
            Console.WriteLine("delete: " + sw.Elapsed);
        }


        /// <summary>
        /// pastes selected data to the place before specified point
        /// </summary>
        private void pasteSelected()
        {
            if(tempy == null)
            {
                return;
            }
            int[] suby1;
            int[] suby2;
            pastex = new int[x.Length + tempx.Length];
            pastey = new int[y.Length + tempy.Length];

            suby1 = new int[selectStart];
            suby2 = new int[x.Length - selectStart];
            //divide x into two subarray from point selectStart
            for (int i = 0; i < selectStart; i++)
                suby1[i] = y[i];

            for (int i = selectStart, j = 0; i < x.Length; i++, j++)
                suby2[j] = y[i];

            suby1.CopyTo(pastey, 0);
            tempy.CopyTo(pastey, suby1.Length);
            suby2.CopyTo(pastey, suby1.Length + tempy.Length);

            for (int i = 0; i < pastex.Length; i++)
            {
                pastex[i] = i;
            }

            x = pastex;
            y = pastey;
        }

        /// <summary>
        /// event where user clicks on edit > copy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySelected();
        }

        /// <summary>
        /// event where user clicks on edit > paste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oSize = y.Length;
            if (selectStart == selectEnd && selectStart != -1)
            {
                pasteSelected();
            }
            else
            {

                MessageBox.Show(this, "Please select a point to paste to", "Invalid point",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                //deleteSelected();
                //pasteSelected();
            }

            updateTDGraph(oSize);
        }

        /// <summary>
        /// event where user clicks on edit > cut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oSize = y.Length;
            if(selectStart == -1)
            {
                MessageBox.Show(this, "Please select a valid range for cut", "Invalid selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            copySelected();
            deleteSelected();

            selectStart = -1;
            selectEnd = -1;
            updateTDGraph(oSize);
        }

        /// <summary>
        /// zooms on selected range of points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectStart != -1)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(selectStart, selectEnd);
            }
            else
            {
                MessageBox.Show(this, "Please select valid range", "Invalid Zoom",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Zooms in 
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            double mult = x.Length / 10;
            double min = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            double max = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

            double newMin = min + mult;
            double newMax = max - mult;

            if(newMin > newMax)
            {
                MessageBox.Show(this, "Zoom maximized", "Invalid Zoom",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(newMin, newMax);
        }
        
        /// <summary>
        /// No events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /// <summary>
        /// Zooms out 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double mult = x.Length / 10;
            double min = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            double max = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

            double newMin = min -  mult;
            double newMax = max +  mult;

            if (newMin < 0 || newMax > x.Length)
            {
                MessageBox.Show(this, "Zoom maximized", "Invalid Zoom",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(newMin, newMax);
        }

        /// <summary>
        /// Zooms out fully to view the whole chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, x.Length);
        }

        /// <summary>
        /// returns the number of samples
        /// </summary>
        /// <returns>length of y</returns>
        public int getYLength()
        {
            return y.Length;
        }

        /// <summary>
        /// updates the graph with samples that are filtered
        /// </summary>
        /// <param name="filtered_samples"></param>
        public void filter(int[] filtered_samples)
        {
            int oSize = y.Length;
            y = filtered_samples;
            updateTDGraph(oSize);
        }

        /// <summary>
        /// returns the sample points
        /// </summary>
        /// <returns>y</returns>
        public int[] getY()
        {
            return y;
        }

        /// <summary>
        /// converts the data into int16 form
        /// </summary>
        /// <returns>data in int16</returns>
        public Int16[] getDataToSave()
        {
            Int16[] sound16 = new Int16[y.Length];
            for(int i = 0; i < y.Length; i++)
            {
                if (y[i] > 65535)
                    y[i] = 65535;
                sound16[i] = (Int16)y[i];
            }
            return sound16;
        }

        /// <summary>
        /// finds the maximum amplitude value for scaling purpose
        /// </summary>
        /// <returns>maximum amplitude value in double</returns>
        public double findMax()
        {
            double largest = A[1];
            for (int i = 1; i < A.Length; i++)
            {
                if (largest < A[i])
                {
                    largest = A[i];
                }
            }
            return largest;
        }

        /// <summary>
        /// converts the data into byte form
        /// </summary>
        /// <returns></returns>
        public byte[] getDataToSaveInBytes()
        {
            byte[] sound = new byte[y.Length];
            for(int i = 0; i < y.Length; i++)
            {
                if (y[i] > 255)
                    y[i] = 255;
                sound[i] = (byte)y[i];
            }
            return sound;
            
        }
    }
}
