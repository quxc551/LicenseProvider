using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 客户端
{
    public partial class Form1 : Form
    {
        private UdpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new UdpClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 8910);
            string regMsg = "songrunhan" + '\0' + "123456" + '\0' + "normal";
            byte[] msg = Encoding.Default.GetBytes(regMsg);
            client.Send(msg, msg.Length, ip);
        }
    }
}
