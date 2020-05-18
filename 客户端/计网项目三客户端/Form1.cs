using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;



namespace 计网项目三客户端
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
            //IPEndPoint（）中如果第一个参数用IPAddress.Any，我的电脑会报错，然后网上说换成本机的具体ip就可。
            IPEndPoint ip_recvFrom = new IPEndPoint(IPAddress.Any, 0);
            IPEndPoint ip2 = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 8910);
            IPEndPoint ip3 = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 8888);
            byte[] msg = new byte[1024];
            //发送序列号至身份验证服务器
            string serialNumber = textBox2.Text == string.Empty ? "4852433483" : textBox2.Text;
            msg = Encoding.UTF8.GetBytes(serialNumber);
            client.Send(msg, msg.Length, ip2);

            //客户端从身份验证服务器接收令牌
            msg = client.Receive(ref ip_recvFrom);
            textBox1.Text += $"时间：{DateTime.Now}\t来自{ip_recvFrom}的回应：\n";
            textBox1.Text += Encoding.UTF8.GetString(msg);
            textBox1.Text += "\n";

            //再将令牌发送给授权服务器
            msg = Encoding.UTF8.GetBytes("1." + Encoding.UTF8.GetString(msg));
            client.Send(msg, msg.Length, ip3);
            msg = client.Receive(ref ip_recvFrom);
            textBox1.Text += $"时间：{DateTime.Now} 来自{ip_recvFrom}的回应：\n";
            textBox1.Text += Encoding.UTF8.GetString(msg) + "\n";

            //弹窗显示结果
            if (Encoding.UTF8.GetString(msg) == "true")
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
