using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;


namespace Sound_Editor
{
    


    public unsafe partial class Form1 : Form
    {
        //writing binrary datas to wav file
        private BinaryWriter writer;
        //reading binary datas from wav file
        private BinaryReader reader;
        //file dialog
        OpenFileDialog ofd;

        //array to store wav file data part 
        protected byte[] sound;
        //array to store wav file data part when 16bits per sample
        protected Int16[] sound16;

        //window that displays time domain & frequency domain, respectively
        private Form2 form2;
        private Form3 form3;


        //current data type
        //0: no data
        //1: read from wav 
        //2: from recorder
        int dataCurrent;

        //wav file header info
        private int chunkID;
        private int fileSize;
        private int riffType;
        private int fmtID;
        private int fmtSize;
        private Int16 fmtCode;
        private Int16 channels;
        private int sampleRate;
        private int fmtAvgBPS;
        private Int16 fmtBlockAlign;
        private Int16 bitDepth;
        private int dataID;
        private int dataSize;
        //file header ends here

        //points to data recorded by dll record dialog
        protected unsafe byte** pData;
        //length of the data recorded **in bytes i think**
        protected uint length;
        //copy of data
        protected unsafe byte[] data;
        private bool recordOpen;

        //Exported function in dll
        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern Int32 StartDialog(byte** data, uint* plength);

        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint* getLength();

        [DllImport("Record.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte** getData();

        public Form1()
        {
            InitializeComponent();
            
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            dataCurrent = 0;
            recordOpen = false;
            /*form2 = new Form2(form3);
            form2.MdiParent = this;
            form2.Show();
            form2.Location = new Point(10, 10);

            form3 = new Form3(form2);
            form3.MdiParent = this;
            form3.Show();
            form3.Location = new Point(10, 300);
            form2.getForm3(form3);
            form3.getForm2(form2);*/
        }

        /// <summary>
        /// opens dialog box to select file to open and reads that file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path;
            ofd = new OpenFileDialog();
            ofd.Filter = "WAV|*.wav";
            
            //checks if selection was successful
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                reader = new BinaryReader(new FileStream(path, FileMode.Open));
                dataCurrent = 1;
            }
            
            else
            {
                //fail
                return;
            }

            readHeader();
            readData();
            
            //test purpose
            foreach (Int16 temp in sound16)
            {
                Console.WriteLine(temp + " ");
            }
            reader.Close();

            if(form2 != null)
            {
                form2.Close();
            }
            if(bitDepth == 16)
                form2 = new Form2(sound16, this);
            else if(bitDepth == 8)
                form2 = new Form2(sound, this);
            form2.MdiParent = this;
            form2.Show();
        }

        /// <summary>
        /// reads header block of the wav file
        /// </summary>
        private void readHeader()
        {
            chunkID = reader.ReadInt32();
            fileSize = reader.ReadInt32();
            riffType = reader.ReadInt32();
            fmtID = reader.ReadInt32();
            fmtSize = reader.ReadInt32();
            fmtCode = reader.ReadInt16();
            channels = reader.ReadInt16();
            sampleRate = reader.ReadInt32();
            fmtAvgBPS = reader.ReadInt32();
            fmtBlockAlign = reader.ReadInt16();
            bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values               
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            dataID = reader.ReadInt32();
            dataSize = reader.ReadInt32();
        }

        /// <summary>
        /// reads data block of the wav file
        /// </summary>
        private void readData()
        {
            sound = reader.ReadBytes(dataSize);
            int bytesPerSample = bitDepth / 8; // how many bytes for each sample
            int samples = dataSize / bytesPerSample; //number of Samples
            if(bytesPerSample == 2)
            {
                sound16 = new Int16[samples];
                Buffer.BlockCopy(sound, 0, sound16, 0, dataSize);
            }

        }

        /// <summary>
        /// Writes header data to .wav file when saving
        /// </summary>
        private void writeDataR()
        {
            writer.Write(chunkID);
            writer.Write(fileSize);
            writer.Write(riffType);
            writer.Write(fmtID);
            writer.Write(fmtSize);
            writer.Write(fmtCode);
            writer.Write(channels);
            writer.Write(sampleRate);
            writer.Write(fmtAvgBPS);
            writer.Write(fmtBlockAlign);
            writer.Write(bitDepth);
            writer.Write(dataID);
            writer.Write(dataSize);
        }

        /// <summary>
        /// Event where user clicks save from the file menu
        /// saves current data as .wav file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //maybe printf out message box?
            if (dataCurrent == 0)
                return;

            string path;
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                path = sfd.FileName;
                writer = new BinaryWriter(new FileStream(path, FileMode.Create));

                   
                if (dataCurrent == 1)
                {
                    setupHeader(1);
                    writeDataR();
                    foreach (Int16 temp in sound16)
                    {
                        writer.Write(temp);
                    }
                }
                else
                {
                    foreach (byte temp in sound)
                    {
                        writer.Write(temp);
                    }
                }

                writer.Close();
            }
        }

        private void setupHeader(int state)
        {
                      
            if (state == 1)
                bitDepth = 16;
            else if (state == 2)
                bitDepth = 8;
            else return;

            chunkID = 1179011410;
            fileSize = 44138;
            riffType = 1163280727;
            fmtID = 544501094;
            fmtSize = 16;
            fmtCode = 1;
            channels = 1;
            sampleRate = 22050;
            fmtAvgBPS = 44100;
            fmtBlockAlign = 2;
            
            dataID = 1635017060;
            dataSize = 44102;
        }

        /// <summary>
        /// event wher user selects Record from the menu bar
        /// loads up the record dialog box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(recordOpen== true)
            {
                return;//message box "open already"
            }
            dataCurrent = 2;
            recordOpen = true;
            StartDialog(null, null);
            recordOpen = false;
        }

        /// <summary>
        /// Event when Draw from menu bar is selected
        /// gets data from record & passing it to time domain graph
        /// or gets data from opened file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //recorded data
            if(dataCurrent == 2 && recordOpen == true)
            {
                //bring data;
                length = *getLength();
                pData = getData();
                data = new byte[length];

                for(int i = 0; i < data.Length; i++)
                {
                    data[i] = *((*pData) + i);
                }

                if (form2 != null)
                {
                    form2.Close();
                }
                form2 = new Form2(data, this);
                form2.MdiParent = this;
                form2.Show();
            }
        }
    }
}
