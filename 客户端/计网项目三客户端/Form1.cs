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
        AskLicense clientside = new AskLicense("6352342471");
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = clientside.LogIn().ToString();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = clientside.LogOut().ToString();
        }
    }
}
