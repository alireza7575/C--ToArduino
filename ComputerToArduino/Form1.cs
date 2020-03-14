using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using AForge.Video.DirectShow;
using AForge.Video;

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

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;
        private ArrayList listCamera = new ArrayList();
        public string pathFolder = Application.StartupPath + @"\ImageCapture\";

        private Stopwatch stopWatch = null;
        private static bool needSnapshot = false;

        #region test
      


            private static string _usbcamera;
            public string usbcamera
            {
                get { return _usbcamera; }
                set { _usbcamera = value; }
            }

            private void button10_Click(object sender, EventArgs e)
            {
                OpenCamera();
            }

            #region Open Scan Camera
            private void OpenCamera()
            {
                try
                {
                    usbcamera = comboBox2.SelectedIndex.ToString();
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                    if (videoDevices.Count != 0)
                    {
                        // add all devices to combo
                        foreach (FilterInfo device in videoDevices)
                        {
                            listCamera.Add(device.Name);

                        }
                    }
                    else
                    {
                        MessageBox.Show("Camera devices found");
                    }

                    videoDevice = new VideoCaptureDevice(videoDevices[Convert.ToInt32(usbcamera)].MonikerString);
                    snapshotCapabilities = videoDevice.SnapshotCapabilities;
                    if (snapshotCapabilities.Length == 0)
                    {
                        //MessageBox.Show("Camera Capture Not supported");
                    }

                    OpenVideoSource(videoDevice);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }

            }
            #endregion


            //Delegate Untuk Capture, insert database, update ke grid 
            public delegate void CaptureSnapshotManifast(Bitmap image);
            public void UpdateCaptureSnapshotManifast(Bitmap image)
            {
                try
                {
                    needSnapshot = false;
                    captureImageBox.Image = image;
                captureImageBox.Update();


                    string namaImage = "sampleImage";
                    string nameCapture = namaImage + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";

                    if (Directory.Exists(pathFolder))
                    {
                    captureImageBox.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                        Console.WriteLine(pathFolder + nameCapture);
                    }
                    else
                    {
                        Directory.CreateDirectory(pathFolder);
                    captureImageBox.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                    }

                }

                catch { }

            }

            public void OpenVideoSource(IVideoSource source)
            {
                try
                {
                    // set busy cursor
                    this.Cursor = Cursors.WaitCursor;

                    // stop current video source
                    CloseCurrentVideoSource();

                    // start new video source
                    videoSourcePlayer1.VideoSource = source;
                    videoSourcePlayer1.Start();

                    // reset stop watch
                    stopWatch = null;


                    this.Cursor = Cursors.Default;
                }
                catch { }
            }

            private void getListCameraUSB()
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count != 0)
                {
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        comboBox2.Items.Add(device.Name);

                    }
                }
                else
                {
                    comboBox2.Items.Add("No DirectShow devices found");
                }

                comboBox2.SelectedIndex = 0;

            }

            public void CloseCurrentVideoSource()
            {
                try
                {

                    if (videoSourcePlayer1.VideoSource != null)
                    {
                        videoSourcePlayer1.SignalToStop();

                        // wait ~ 3 seconds
                        for (int i = 0; i < 30; i++)
                        {
                            if (!videoSourcePlayer1.IsRunning)
                                break;
                            System.Threading.Thread.Sleep(100);
                        }

                        if (videoSourcePlayer1.IsRunning)
                        {
                            videoSourcePlayer1.Stop();
                        }

                        videoSourcePlayer1.VideoSource = null;
                    }
                }
                catch { }
            }

            private void button20_Click(object sender, EventArgs e)
            {
                needSnapshot = true;
            }

            private void videoSourcePlayer1_NewFrame_1(object sender, ref Bitmap image)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    Graphics g = Graphics.FromImage(image);

                    // paint current time
                    SolidBrush brush = new SolidBrush(Color.Red);
                    g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
                    brush.Dispose();
                    if (needSnapshot)
                    {
                        this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), image);
                    }
                    g.Dispose();
                }
                catch
                { }
            }
        
    

    #endregion
    public Form1()
        {
            InitializeComponent();
            disableControls();
            getAvailableComPorts();
    getListCameraUSB();

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
                needSnapshot = true;

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
                    needSnapshot = true;
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
