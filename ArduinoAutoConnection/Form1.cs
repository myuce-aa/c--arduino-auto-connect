using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoAutoConnection
{
    public partial class Form1 : Form
    {
        string com = null;
        string VID = "XXXX";
        string PID = "YYYY";

        // Portları tarama fonksiyonu cihazınıza göre VID ve PID değerlerini girmelisiniz. Bu bilgilere göre aşağıdaki işlemde cihazlar taranıyor
        List<string> ComPortNames()
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        
        private void arduino_connection()
        {
            List<string> names = ComPortNames();
            if (names.Count > 0)
            {
                foreach (String s in SerialPort.GetPortNames())
                {
                    if (names.Contains(s))
                    {
                        com = s;
                    }
                }
            }
            if (com != null)
            {
                try
                {
                    if (!serialPort.IsOpen && com != "")
                    {
                        /* Seri Port Ayarları */
                        serialPort.PortName = com;
                        serialPort.BaudRate = 9600;
                        serialPort.Parity = Parity.None;
                        serialPort.DataBits = 8;
                        serialPort.StopBits = StopBits.One;
                        serialPort.Open(); //Seri portu aç
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Arduino");
                }
            }
            else
                MessageBox.Show("Arduino bulunamadı!", "Arduino");
        }

        // Arduino bağlantısını kesme
        private void arduino_disconnection()
        {
            if (serialPort.IsOpen) serialPort.Close(); //Seri port açıksa kapat
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Form açılışında arduinoya direkt bağlantı
            arduino_connection();
            comPortLabel.Text = com;
            if (serialPort.IsOpen)
                statusLabel.Text = "Arduino Bağlı";
            else
                statusLabel.Text = "Arduino Bağlı Değil";
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            arduino_connection();
            if (serialPort.IsOpen)
                statusLabel.Text = "Arduino Bağlı";
            else
                statusLabel.Text = "Arduino Bağlı Değil";
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            arduino_disconnection();
            if (serialPort.IsOpen)
                statusLabel.Text = "Arduino Bağlı";
            else
                statusLabel.Text = "Arduino Bağlı Değil";
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("&"+messageTextbox.Text);
            }
        }
    }
}
