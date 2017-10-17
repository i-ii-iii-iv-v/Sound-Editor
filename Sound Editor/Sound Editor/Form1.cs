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
    

 
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 form2 = null;
            Form3 form3 = null;
            form2 = new Form2(form3);
            form2.MdiParent = this;
            form2.Show();
            form2.Location = new Point(10, 10);

            form3 = new Form3(form2);
            form3.MdiParent = this;
            form3.Show();
            form3.Location = new Point(10, 300);
            form2.getForm3(form3);
            form3.getForm2(form2);
        }


        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }
        
        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
