using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace waveReader
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader reader = new BinaryReader(new FileStream("386Hz2074Hz.wav", FileMode.Open));
            BinaryWriter writer = new BinaryWriter(new FileStream("sav4.wav", FileMode.Create));
            byte[] sound;
            
            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            Int16 fmtCode = reader.ReadInt16();
            Int16 channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            Int16 fmtBlockAlign = reader.ReadInt16();
            Int16 bitDepth = reader.ReadInt16();

            Console.Write("chunkID " + chunkID + "\n");
            Console.Write("fileSize " + fileSize + "\n");
            Console.Write("riffType " + riffType + "\n");
            Console.Write("fmtID " + fmtID + "\n");
            Console.Write("fmtSize " + fmtSize + "\n");
            Console.Write("fmtCode " + fmtCode + "\n");
            Console.Write("channels " + channels + "\n");
            Console.Write("sampleRate " + sampleRate + "\n");
            Console.Write("fmtAvgBPS " + fmtAvgBPS + "\n");
            Console.Write("fmtBlockAlign " + fmtBlockAlign + "\n");
            Console.Write("bitDepth " + bitDepth + "\n");

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

            if (fmtSize == 18)
            {
                // Read any extra values               
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();

            writer.Write(dataID);
            writer.Write(dataSize);

            sound = reader.ReadBytes(dataSize);


            int bytesPerSample = bitDepth / 8; //2 bytes for 16 bit
            int samples = dataSize / bytesPerSample; //how many samples there are

            Int16[] asInt16 = new Int16[samples];
            Buffer.BlockCopy(sound, 0, asInt16, 0, dataSize);
            /*float[] asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
            int c = 0;
            foreach(float temp in asFloat)
            {
                asFloat[c] = temp * 1.3f;
                c++;
            }*/

            
            Console.Write("dataID " + dataID + "\n");
            Console.Write("dataSize " + dataSize + "\n");

            int counter = 0;
            foreach (Int16 temp in asInt16)
            {
                //Console.Write(temp + " ");
                writer.Write(temp);
                counter++;
            }

            Console.Write("\n\n\n\n" + counter);

            writer.Close();
            reader.Close();
        }
    }
}
    