using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
        private int sampleSize;
        private int selectStart;
        private int selectEnd;
        private int windowFunction;
        private double[] weight;
        private int zoom;
        public Form2(byte[] data, Form1 f)
        {
            InitializeComponent();
            zoom = 1;
            mainForm = f;
            y = new int[data.Length];
            x = new int[data.Length];
            for(int i = 0; i < data.Length; i++)
            {
                y[i] = data[i];
                x[i] = i;
            }
            Console.WriteLine(data.Length);
        }

        public Form2(Int16[] data, Form1 f)
        {
            InitializeComponent();
            zoom = 1;
            mainForm = f;
            y = new int[data.Length];
            x = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                y[i] = data[i];
                x[i] = i;
            }
        }
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

        private void chart1_Click(object sender, EventArgs e)
        {
            
            
        }
       
        public void getForm3(Form3 form)
        {
            form3 = form;
        }

        private void chart1_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            selectStart = (int)chart1.ChartAreas[0].CursorX.SelectionStart;
            selectEnd = (int)chart1.ChartAreas[0].CursorX.SelectionEnd;
            Console.Write(selectStart + "\n");
            Console.Write(selectEnd + "\n");

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
                selectEnd = x.Length - 1;
            }

        }


        private void button4_Click(object sender, EventArgs e)
        {
            //edit:2
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, chart1.ChartAreas[0].AxisX.Maximum/2);
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                windowFunction = 0;
                for(int i = 0; i < sampleSize; i++)
                {
                    weight[i] = 1;
                }
            }
        }

        //welch
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked)
            {
                windowFunction = 1;
                for (int i = 0; i < sampleSize; i++)
                    weight[i] = 1 - ((i - (sampleSize - 1) / 2) / ((sampleSize - 1) / 2)) * ((i - (sampleSize - 1) / 2) / ((sampleSize - 1) / 2));
            }
        }

        //perform dft on selected samples
        private void button5_Click(object sender, EventArgs e)
        {
            if(selectStart != selectEnd)
            {
                //DFT
            }
        }

        /// <summary>
        /// updataes the time domain graph that has new maximum x axis
        /// clears the current points and replaces it with new data points
        /// </summary>
        private void updateTDGraph()
        {
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            if (x.Length != 0)
                chart1.ChartAreas[0].AxisX.Maximum = x[x.Length - 1] + 1;
            else
                chart1.ChartAreas[0].AxisX.Maximum = 1;
            chart1.Series["Wave"].Points.Clear();
            chart1.Series["Wave"].Points.DataBindXY(x, y);
        }

        /// <summary>
        /// copies selected points by user and 
        /// stores it in the tempy array
        /// </summary>
        private void copySelected()
        {
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
        }


        /// <summary>
        /// deletes selected points and assigns it to x and y array
        /// </summary>
        private void deleteSelected()
        {
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

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySelected();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

            updateTDGraph();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            updateTDGraph();
        }

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


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

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

        private void zoomResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateTDGraph();
        }
    }
}
