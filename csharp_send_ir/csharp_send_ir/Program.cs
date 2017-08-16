using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SendingModeExample
{
    class Program
    {
        const int START_LEARNING_MODE = 26;
        const int ERROR = 255;

        static SerialPort sSerialPort;

        // Past the ir code from learn example here.
        const string ircode = "152,35,181,140,247,130,95,185,73,94,94,239,253,179,78,139,68,94,94,106,96,179,78,4,64,35,18,137,174,124,6,11,253,43,26,145,182,132,14,19,5,51,34,153,190,140,22,27,13,59,42,161,198,148,30,35,21,67,50,169,206,156,38,43,29,75,58,177,214,121,35,38,24,69,35,152,204,137,18,39,8,69,51,168,188,137,34,23,24,69,51,169,205,153,18,23,24,70,18,169,204,154,35,23,7,68,34,152,205,138,35,39,24,69,50,153,204,153,18,39,23,53,50,169,204,137,19,39,24,69,51,169,205,154,35,38,24,53,51,153,189,154,35,39,8,69,50,168,188,138,37,89"; 

        static void Main(string[] args)
        {
            sSerialPort = new SerialPort("COM4");

            sSerialPort.BaudRate = 9600;
            sSerialPort.Parity = Parity.None;
            sSerialPort.StopBits = StopBits.One;
            sSerialPort.DataBits = 8;
            sSerialPort.Encoding = Encoding.ASCII;

            sSerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            sSerialPort.Open();

            Console.WriteLine("Sending IR Code ...");

            byte[] ir_array = ConvertStringArrayToByteArray(ircode);

            byte[] new_ir_array = new byte[ir_array.Length + 1]; //New Array and the size of a which is 4

            new_ir_array[0] = 227; // sending ir command.

            // copy ir code to this array starting from index 1
            for (int i = 1; i < ir_array.Length + 1; i++)
            {
                new_ir_array[i] = ir_array[i - 1];
            }
             
            sSerialPort.Write(new_ir_array, 0, new_ir_array.Length);
            Console.ReadKey();
        }

        private static byte[] ConvertStringArrayToByteArray(string str)
        {
            return str.Split(",".ToCharArray()).Select(x => byte.Parse(x.ToString())).ToArray();
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] array = new byte[sSerialPort.BytesToRead];
            sSerialPort.Read(array, 0, sSerialPort.BytesToRead);

            int resvalue = array[0];

            if (resvalue == ERROR)
            {
                Console.WriteLine("Error sending ir !");
            }
            else {
                Console.WriteLine("Error sending ir !");
            }
        }
    }
}
