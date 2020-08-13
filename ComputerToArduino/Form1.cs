using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Management;

namespace ComputerToArduino
{
    public partial class Form1 : Form
    {

        bool isConnected = false;
        SerialPort port;

        public Form1()
        {
            InitializeComponent();
            try
            {
                ManagementScope connectionScope = new ManagementScope();
                SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

                foreach (ManagementObject item in searcher.Get())
                {

                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();
                    comboBox1.Items.Add(deviceId);
                    if (desc.Contains("Arduino"))
                    {
                        comboBox1.SelectedItem = deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            port.WriteLine("{'command':'START'}");
            button1.Text = "Disconnect";

        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.WriteLine("{'command':'STOP'}");
            port.Close();
            button1.Text = "Connect";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                port.Write(textBox6.Text + "\n");
                // { "speed":500,"degree":64,"step":10,"command":"MOVE"}
                port.ReadExisting();

                Console.WriteLine(textBox6.Text);
                Console.WriteLine();
                String s = port.ReadExisting();
                while (s == "")
                {
                    s = port.ReadExisting();
                    Console.WriteLine(s);
                    int milliseconds = 200;
                    Thread.Sleep(milliseconds);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            double degree = float.Parse(textBox1.Text);
            double step = double.Parse(textBox2.Text);
            String temp = "";
            if (isConnected)
            {
                for (double i = 0; i < degree; i += step)
                {
                    temp = String.Concat("{ 'speed':", textBox3.Text, ", 'degree':", Math.Round(step * 4.44), ", 'command':", "'MOVE'", "}");
                    label16.Text = $"{i}";
                    port.WriteLine(temp);
                    String s = port.ReadExisting();
                    while (s == "")
                    {
                        s = port.ReadExisting();


                        Thread.Sleep(200);
                    }
                    Console.WriteLine(s);

                }
                //port.WriteLine(temp);
                // { "speed":500,"degree":64,"step":10,"command":"START"}

            }
        }
    }
}
