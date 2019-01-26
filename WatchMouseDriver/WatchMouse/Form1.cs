using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchMouse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool running = false;
        public static int amount = 20;
        public Mode mode = Mode.Mouse;
        private TcpListener server;
        private Thread detector;
        private TcpClient phone = null;
        bool XY = false;
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        public static string hostname = "";
        public List<String> Adapters()
        {
            List<String> values = new List<String>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                values.Add(nic.Name);
            }
            return values;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ConnectDisconnect();
        }
        private void ConnectDisconnect()
        {
            if (running == false)
            {
                Form2 f = new Form2(Adapters().ToArray());
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Dictionary<string, IPAddress> id = new Dictionary<string, IPAddress>();
                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            string n = ni.Name;
                            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    try
                                    {
                                        id.Add(n, ip.Address.MapToIPv4());
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    IPAddress ia = null;
                    foreach (KeyValuePair<string, IPAddress> i in id)
                    {
                        if (i.Key == hostname) ia = i.Value;
                    }
                    textBox1.Text = ia.ToString();
                    server = new TcpListener(ia, 5000);
                    server.Start();
                    detector = new Thread(() =>
                    {
                        NetworkStream s = null;
                        while (true)
                        {
                            if (phone == null)
                            {
                                try
                                {
                                    phone = server.AcceptTcpClient();
                                    s = phone.GetStream();
                                }
                                catch
                                {
                                    MessageBox.Show("Error");
                                    break;
                                }
                            }
                            else
                            {
                                string data = "";
                                while(s.DataAvailable)
                                {
                                    data += ((char)s.ReadByte()).ToString();
                                }
                                if (data.Length > 0)
                                {
                                    data = data.Remove(data.Length - 1, 1);
                                    ProcessInput(data);
                                }
                            }
                        }
                    });
                    detector.Start();
                    running = true;
                    button1.Text = "Disconnect";
                    disconnectToolStripMenuItem.Text = "Disconnect";
                }
            }
            else
            {
                running = false;
                server.Stop();
                phone = null;
                button1.Text = "Start server";
                disconnectToolStripMenuItem.Text = "Connect";
            }
        }
        public static string[] CustomKeys = new string[]
        {
            "{ENTER}", "{ENTER}", "{ENTER}", "{ENTER}"
        };
        private void ProcessInput(string d)
        {
            if (mode != Mode.Trackpad)
            {
                if (d == "up")
                {
                    if (mode == Mode.Mouse)
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - amount);
                    else if (mode == Mode.Arrow)
                        SendKeys.SendWait("{UP}");
                    else if (mode == Mode.WASD)
                        SendKeys.SendWait("{W}");
                }
                else if (d == "do")
                {
                    if (mode == Mode.Mouse)
                        Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + amount);
                    else if (mode == Mode.Arrow)
                        SendKeys.SendWait("{DOWN}");
                    else if (mode == Mode.WASD)
                        SendKeys.SendWait("{S}");
                }
                else if (d == "le")
                {
                    if (mode == Mode.Mouse)
                        Cursor.Position = new Point(Cursor.Position.X - amount, Cursor.Position.Y);
                    else if (mode == Mode.Arrow)
                        SendKeys.SendWait("{LEFT}");
                    else if (mode == Mode.WASD)
                        SendKeys.SendWait("{A}");
                }
                else if (d == "ri")
                {
                    if (mode == Mode.Mouse)
                        Cursor.Position = new Point(Cursor.Position.X + amount, Cursor.Position.Y);
                    else if (mode == Mode.Arrow)
                        SendKeys.SendWait("{RIGHT}");
                    else if (mode == Mode.WASD)
                        SendKeys.SendWait("{D}");
                }
                else if (d == "cl")
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    Thread.Sleep(50);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                }
                else if (d == "sc")
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    Thread.Sleep(50);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                }
                else if (d == "en")
                {
                    SendKeys.SendWait("{ENTER}");
                }
                else if (d == "f5")
                {
                    SendKeys.SendWait("{F5}");
                }
                else if (d == "f11")
                {
                    SendKeys.SendWait("{F11}");
                }
                else if (d == "es")
                {
                    SendKeys.SendWait("{ESC}");
                }
                else if (d == "bs")
                {
                    SendKeys.SendWait("{BACKSPACE}");
                }
                else if (d == "sp")
                {
                    SendKeys.SendWait(" ");
                }
                else if (d == "c1")
                {
                    SendKeys.SendWait(CustomKeys[0]);
                }
                else if (d == "c2")
                {
                    SendKeys.SendWait(CustomKeys[1]);
                }
                else if (d == "c3")
                {
                    SendKeys.SendWait(CustomKeys[2]);
                }
                else if (d == "c4")
                {
                    SendKeys.SendWait(CustomKeys[3]);
                }
                else if (d == "am")
                {
                    mode = Mode.Arrow;
                }
                else if (d == "wm")
                {
                    mode = Mode.WASD;
                }
                else if (d == "it")
                {
                    Keyboard.AltTab();
                }
                else if (d == "9s")
                {
                    amount = 200;
                }
                else if (d == "5s")
                {
                    amount = 50;
                }
                else if (d == "2s")
                {
                    amount = 20;
                }
                else if (d == "1s")
                {
                    amount = 10;
                }
                else if (d == "tr")
                {
                    mode = Mode.Trackpad;
                }
            }
            else
            {
                if (d == "tr")
                    mode = Mode.Mouse;
                else if(d == "cl")
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    Thread.Sleep(50);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                }
                else if(d == "sc")
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    Thread.Sleep(50);
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                }
                else
                {
                    try
                    {
                        string[] spl = d.Split(' ');
                        int x = int.Parse(spl[0]);
                        int y = int.Parse(spl[1]);
                        int xm = (int)Map(x, 60, 180, 0, 1920);
                        int ym = (int)Map(y, 60, 180, 0, 1080);
                        Cursor.Position = new Point(xm, ym);
                    }
                    catch
                    {

                    }
                }
            }
        }
        decimal Map(decimal value, decimal fromSource, decimal toSource,decimal fromTarget, decimal toTarget)
        {
            decimal i = ((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
            return i;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDisconnect();
        }

        private void mapKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyMapper km = new KeyMapper();
            km.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            KeyMapper km = new KeyMapper();
            km.Show();
        }
    }
    public enum Mode
    {
        Mouse = 0,
        Arrow = 1,
        WASD = 2,
        Trackpad = 3
    }

}
