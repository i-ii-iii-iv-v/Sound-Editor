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
    
        private Form3 form3;
        private double[] x;
        private double[] y;
        private double[] tempx;
        private double[] tempy;
        private double[] pastex;
        private double[] pastey;
        private int sampleSize;
        private double selectStart;
        private double selectEnd;
        public Form2(Form3 form3)
        {
            InitializeComponent();
            this.form3 = form3;
            selectStart = 0;
            selectEnd = 0;
            sampleSize = 162;
            x = new double[sampleSize];
            y = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                x[i] = i / (double)8;// this 8 is not sample size
                y[i] = Math.Sin(2 * Math.PI * x[i]);
            }


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = x[sampleSize - 1] + 5;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;

            chart1.ChartAreas[0].CursorX.SelectionColor = Color.FromArgb(10, 255, 0);
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, x[x.Length - 1]);
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.Interval = 1 / 8.0;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = 0.5;

            chart1.Series["Wave"].Points.DataBindXY(x, y);
            form3.dft(x, y);
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
            selectStart = chart1.ChartAreas[0].CursorX.SelectionStart;
            selectEnd = chart1.ChartAreas[0].CursorX.SelectionEnd;
            Console.Write(selectStart + "\n");
            Console.Write(selectEnd + "\n");
            
        }

        //copying to clipboard
        private void button1_Click(object sender, EventArgs e)
        {
            double xInitial = selectStart;
            double xEnd = selectEnd;

            int initialIndex = (int)(xInitial / 0.125);
            int endIndex = (int)(xEnd / 0.125);
            int initialCount = initialIndex;

            if (endIndex > (x.Length - 1))
                return;//message box saying "plz select valid data"
            int size = endIndex - initialIndex + 1;
            tempx = new double[size];
            tempy = new double[size];
            
            for (int i = 0; i < size; i++)
            {
                tempx[i] = x[x.Length - 1] + 0.125*(i+1);//change this so that it works for other location pasting
                tempy[i] = y[initialCount++];
                Console.WriteLine("temp points: " + tempx[i] + ", " + tempy[i]);
            }
            /*
            var newx = new double[tempx.Length + x.Length];
            var newy = new double[tempy.Length + y.Length];

            x.CopyTo(newx, 0);
            tempx.CopyTo(newx, x.Length);

            y.CopyTo(newy, 0);
            tempy.CopyTo(newy, y.Length);

            x = newx;
            y = newy;

            chart1.Series["Wave"].Points.Clear();
            chart1.Series["Wave"].Points.DataBindXY(x, y);*/

        }

        //cut
        private void button2_Click(object sender, EventArgs e)
        {
            double xInitial = selectStart;
            double xEnd = selectEnd;
            
            int initialIndex = (int)(xInitial / 0.125);
            int endIndex = (int)(xEnd / 0.125);
            int initialCount = initialIndex;
            if (endIndex > (x.Length - 1))
                return;//message box saying "plz select valid data"
            int size = endIndex - initialIndex + 1;
            tempx = new double[size];
            tempy = new double[size];

            double[] cutX = new double[x.Length - size];
            double[] cutY = new double[x.Length - size];

            //stored cut array
            for (int i = 0; i < size; i++)
            {
                tempx[i] = x[x.Length - 1] + 0.125 * (i + 1);
                tempy[i] = y[initialCount++];
                Console.WriteLine("temp points: " + tempx[i] + ", " + tempy[i]);
            }

            //erase the cut array
            for (int i = 0, j = 0; i < x.Length; i++)
            {
                if (i >= initialIndex && i <= endIndex)
                    continue;
                cutX[j] = 0.125*(j);
                cutY[j] = y[i];
                j++;
            }

            x = cutX;
            y = cutY;

            chart1.Series["Wave"].Points.Clear();
            chart1.Series["Wave"].Points.DataBindXY(x, y);
        }

        //paste
        private void button3_Click(object sender, EventArgs e)
        {
            

            double[] subx1;
            double[] suby1;
            double[] subx2;
            double[] suby2;

            if(selectStart == selectEnd)
            {
                int pastePos = (int)(selectStart / 0.125);
                pastex = new double[x.Length + tempx.Length];
                pastey = new double[y.Length + tempy.Length];

                subx1 = new double[pastePos + 1]; //need to store x[pastePos] as well
                suby1 = new double[pastePos + 1];
                //divide x into two subarray from point selectStart
                for (int i = 0; i <= pastePos; i++)
                {
                    suby1[i] = y[i];
                    subx1[i] = x[i];
                }
                subx2 = new double[x.Length - pastePos -1]; //????????????????????
                suby2 = new double[x.Length - pastePos -1];
                for (int i = pastePos+1, j = 0; i < x.Length; i++)
                {
                    suby2[j] = y[i];
                    subx2[j] = x[i];
                    j++;
                }

                suby1.CopyTo(pastey, 0);
                tempy.CopyTo(pastey, suby1.Length);
                suby2.CopyTo(pastey, suby1.Length + tempy.Length);

                subx1.CopyTo(pastex, 0);
                tempx.CopyTo(pastex, subx1.Length);
                subx2.CopyTo(pastex, subx1.Length + tempx.Length);

                for(int i= 0; i < pastex.Length; i++)
                {
                    pastex[i] = 0.125 * i;
                }

                x = pastex;
                y = pastey;
                chart1.Series["Wave"].Points.Clear();
                chart1.Series["Wave"].Points.DataBindXY(x, y);
            }
        }
    }
}
