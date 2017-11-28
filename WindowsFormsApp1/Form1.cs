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

    
    public unsafe partial class Form1 : Form
    {
        private uint length;
        private unsafe byte** pData;
        private unsafe uint* pLength;
        private unsafe byte[] data;
        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern Int32 StartDialog(byte** data, uint* plength);

        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint* getLength();

        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte** getData();
        public unsafe Form1()
        {
            InitializeComponent();
            pData = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            
            fixed(uint* tempLength = &length)
            {
                StartDialog(pData, tempLength);
            }
            
        }

        private unsafe void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //pLength = &length;
            //copy into array
            pData = getData();
            data = new byte[*(getLength())];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = *((*pData)+i);
            }
            for(int i = 0; i < 100; i++)
            {
                Console.Write(data[i] + " ");
            }
            Console.WriteLine(*(getLength()));
        }
    }
}
