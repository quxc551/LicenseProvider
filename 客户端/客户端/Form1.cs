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
            client = new UdpClient(6000, AddressFamily.InterNetwork);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] ipAndPort = textBox1.Text.Split(':');
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]));
            string regMsg = textBox3.Text;
            byte[] msg = Encoding.Default.GetBytes(regMsg);
            client.Send(msg, msg.Length, ip);
            //客户端从身份验证服务器接收序列号
            msg = client.Receive(ref ip);
            client.Send(msg, msg.Length, ip);
            //客户端从身份验证服务器接收令牌
            msg = client.Receive(ref ip);
            //再将令牌发送给授权服务器

            IPEndPoint ip2 = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 8888);
            client.Send(msg, msg.Length, ip2);
            //客户端接收授权服务器验证结果
            msg = client.Receive(ref ip2);


            //弹窗显示结果
            if (msg.ToString().Equals("true"))
            {
                MessageBox.Show("授权成功！", "授权提示");
            }
            else
            {
                MessageBox.Show("授权失败！", "授权提示");
            }
        }
    }
}
