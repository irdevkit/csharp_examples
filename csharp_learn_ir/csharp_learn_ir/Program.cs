using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LearingModeExample
{
    class Program
    {
        const int START_LEARNING_MODE = 24;
        const int WAITING_IR_SIGNAL = 25;

        const int OK = 224; // Success response code from IR controller

        static int sCommandCode = 0;

        static SerialPort sSerialPort;

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

            // 224 - (E0) for learning single key
            // 225 - (E1) for learning key combination (some remotes send two ir codes for a single key)

            Console.WriteLine("Start learning mode ...");
            sCommandCode = START_LEARNING_MODE;
            sSerialPort.Write(new byte[] { 224 }, 0, 1);
            Console.ReadKey();

        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            0xff - Command error / Learning error / The infrared code is empty or invalid
            0x00 - success
            */

            if (sCommandCode == START_LEARNING_MODE)
            {
                // Response from start learning mode
                
                byte[] array = new byte[sSerialPort.BytesToRead];
                sSerialPort.Read(array, 0, sSerialPort.BytesToRead);

                int resvalue = array[0];

                if (resvalue == OK)
                {
                    Console.WriteLine("Ready to record the remote. Press any button now..");
                    sCommandCode = WAITING_IR_SIGNAL;
                }
                else
                {
                    Console.WriteLine("Errro");
                }
            }
            else if (sCommandCode == WAITING_IR_SIGNAL)
            {
                Thread.Sleep(1500); // wait till buffer full

                int len = sSerialPort.BytesToRead;

                if (len > 0) {
                    Console.WriteLine("Got for IR signal....");

                    byte[] buffer = new byte[sSerialPort.BytesToRead];
                    sSerialPort.Read(buffer, 0, sSerialPort.BytesToRead);
                    string ir_signal = "";
                    int num = 0;
                    
                    for (int idx = 0; idx < len; idx++)
                    {
                        ir_signal += buffer[idx];

                        // If not the last index, append "," to string
                        if (idx + 1 != len)
                        {
                            ir_signal += ",";
                        }

                        // Ignore the last digit in the array. It is the checksum
                        if (idx != len - 1)
                        {
                            num += buffer[idx];
                        }
                    }

                    // received data checksum
                    byte received_checksum = (byte)num;

                    // ir signal checksum is the last byte
                    int ir_signal_checksum = buffer[len - 1];
                                        
                    if (received_checksum == ir_signal_checksum)
                    {
                        Console.WriteLine("Your ir signal:");
                        Console.WriteLine(ir_signal);
                    }
                    else
                    {
                        Console.WriteLine("Invalid checksum:");
                    }
                }
            }
        }
    }
}
