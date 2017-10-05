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
        public Form2(Form3 form3)
        {
            InitializeComponent();
            this.form3 = form3;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {
            
            double[] x = new double[8];
            double[] y = new double[8];
            for (int i = 0; i < 8; i++)
            {
                x[i] = i / 8.0;
                y[i] = Math.Sin(2 * Math.PI * x[i]);
            }
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 10;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;

            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = 0.5;
            /* chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
             chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
             chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
             chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
             chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;*/
            chart1.Series["Wave"].Points.DataBindXY(x, y);
            //form3.dft(x, y);
        }
       
        public void getForm3(Form3 form)
        {
            form3 = form;
        }
    }
}
