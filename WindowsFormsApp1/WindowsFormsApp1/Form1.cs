using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace WindowsFormsApp1
{

    
    public partial class Form1 : Form
    {
        [DllImport("Record.dll")]
        public static extern Int32 StartDialog();
        public Form1()
        {
            InitializeComponent();
        }
    

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
