using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace Nero_ETA
{
    public static class BL
    {
        private static int ratioSpeed ;
        public static int _fullTime { get; private set; }
        static SerialPort comPort = new SerialPort();
        public static event EventHandler<EventArgsSerial> PcbDataChanged;
        private static readonly string filePath = "time.ini";
        private static readonly string filePathR = "ratio.ini";
        private static int[] data = new int[2];

        private static string speedPCB;

        public static string SpeedPCB
        {
            get
            {
                speedPCB = File.ReadAllText(filePath);
                ratioSpeed = Convert.ToInt32(File.ReadAllText(filePathR));
                return speedPCB;
            }
            set
            {
                try
                {
                    float f = Convert.ToSingle(value);
                    if (f>0.264f)
                    {
                        SetFullTime(f);

                        if (comPort.IsOpen)
                        {
                            File.WriteAllText(filePath, value);
                            MessageBox.Show("Настройки скорости успешно применены");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Скорость должна быть не менее 0.265 !!!");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
            }
        }

        public static bool Running()
        {
            string[] portName = SerialPort.GetPortNames();
            comPort.ReadTimeout = 500;
            for (int i = 0; i < portName.Length; i++)
            {
                comPort.Close();
                comPort.PortName = portName[i];
                try
                {
                    comPort.Open();
                    if (comPort.IsOpen)
                    {
                        comPort.DiscardInBuffer();
                        comPort.WriteLine("W");
                        string st = "t" + Convert.ToString(_fullTime);
                        if (comPort.ReadLine() == "stm32")
                        {
                            comPort.WriteLine(st);
                            Thread.Sleep(100);
                            comPort.WriteLine(st);
                            comPort.DataReceived += Read;
                            return comPort.IsOpen;
                        }

                    }
                }
                catch (Exception e)
                {
                    comPort.Close();
                    Debug.WriteLine(e);
                    return false;
                }
            }

            return false;
        }
        
        public static void SendMinus(string str)
        {
            if (comPort.IsOpen)
            {
                comPort.WriteLine(str);
            }
        }


        private static void Read(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = comPort.ReadLine();
                comPort.DiscardInBuffer();
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strPars = str.Split(':');
                    if (strPars.Length == 2)
                    {
                        data[0] = Convert.ToInt32(strPars[0]);
                        data[1] = Convert.ToInt32(strPars[1]);
                        PcbDataChanged(null, new EventArgsSerial(data));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


        }

        private static void SetFullTime(float speed)
        {
            _fullTime = (int) (ratioSpeed / speed);
            if(comPort.IsOpen)
            { comPort.WriteLine("t" + Convert.ToString(_fullTime));}
        }
    }
}



public class EventArgsSerial:EventArgs
{
    public int[] data;
    public EventArgsSerial(int[] data)
    {
        this.data = data;
    }

}