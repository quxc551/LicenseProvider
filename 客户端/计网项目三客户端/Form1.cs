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
            IPEndPoint ip2 = new IPEndPoint(IPAddress.Parse("192.168.0.101"), 8910);
            IPEndPoint ip3 = new IPEndPoint(IPAddress.Parse("192.168.0.101"), 8888);
            byte[] msg = new byte[1024];
            //客户端从身份验证服务器接收序列号
            msg = Encoding.UTF8.GetBytes("6272452318");
            client.Send(msg, msg.Length, ip2);

            //客户端从身份验证服务器接收令牌
            msg = client.Receive(ref ip2);
            //s1没什么作用就是方便测试时查看msg的值
            string s1 = Encoding.UTF8.GetString(msg);
            //textBox也有大作用，就是方便测试是查看JWT的传输
            textBox1.Text = Encoding.UTF8.GetString(msg);
            //再将令牌发送给授权服务器
            msg = Encoding.UTF8.GetBytes("1." + Encoding.UTF8.GetString(msg));
            client.Send(msg, msg.Length, ip3);
            //客户端接收授权服务器验证结果
            msg = client.Receive(ref ip3);


            //弹窗显示结果
            if (Encoding.UTF8.GetString(msg).Equals("true"))
            {
                MessageBox.Show("授权成功！", "授权提示");
            }
            else if (Encoding.UTF8.GetString(msg).Equals("false"))
            {
                MessageBox.Show("授权失败！", "授权提示");
            }
        }
    }
}
