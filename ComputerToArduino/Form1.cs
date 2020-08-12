using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace ComputerToArduino
{
    public partial class Form1 : Form

    {

        bool isConnected = false;
        String[] ports;
        SerialPort port;


    public Form1()
        {
            InitializeComponent();
            disableControls();
            getAvailableComPorts();
   

    foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                connectToArduino();
            } else
            {
                disconnectFromArduino();
            }
        }

        void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            port.WriteLine("{'command':'START'}");
            button1.Text = "Disconnect";
            enableControls();
        }

        private void Led1CheckboxClicked(object sender, EventArgs e)

        {
            if(isConnected)
            {
            }
        }
        /*
        private void Led2CheckboxClicked(object sender, EventArgs e)

        {
            if (isConnected)
            {
                if (checkBox2.Checked)
                {
                    port.Write("#LED2ON\n");
                }
                else
                {
                    port.Write("#LED2OF\n");
                }
            }
        }

        private void Led3CheckboxClicked(object sender, EventArgs e)

        {
            if (isConnected)
            {
                if (checkBox3.Checked)
                {
                    port.Write("#LED3ON\n");
                }
                else
                {
                    port.Write("#LED3OF\n");
                }
            }
        }
        */
        private void disconnectFromArduino()
        {
            isConnected = false;
            port.WriteLine("{'command':'STOP'}");
            port.Close();
            button1.Text = "Connect";
            disableControls();
            resetDefaults();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                port.Write(textBox6.Text+"\n" );
                // { "speed":500,"degree":64,"step":10,"command":"LED1"}
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

        private void enableControls()
        {
         

        }

        private void disableControls()
        {
           
          
        }

        private void resetDefaults()
        {
          
            //textBox1.Text = "";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
      
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            double degree = float.Parse(textBox1.Text) ;
            double step = double.Parse(textBox2.Text);
            String temp = "";
            if (isConnected)
            {
                for (double i = 0; i < degree; i+=step)
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
                // { "speed":500,"degree":64,"step":10,"command":"LED1"}

                
                
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
