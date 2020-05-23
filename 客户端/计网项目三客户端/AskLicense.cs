using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 计网项目三客户端
{
    public class AskLicense
    {
        private static IPEndPoint ip2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8910);
        private static IPEndPoint ip3 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        private byte[] msg= new byte[1024];
        private UdpClient client = new UdpClient();
        public string jwt = string.Empty;
        public string serialNumber = string.Empty;

        public AskLicense(string serialNumber)
        {
            this.serialNumber = serialNumber;
            ReadFromFile();
        }

        private string GetNewJWT()
        {
            msg = Encoding.UTF8.GetBytes(serialNumber);
            for(int i=0;i<3;++i)
            {
                client.Send(msg, msg.Length, ip2);
                Task<UdpReceiveResult> res = client.ReceiveAsync();
                if (res.Wait(3000))
                {
                    msg = res.Result.Buffer;
                    jwt = Encoding.UTF8.GetString(res.Result.Buffer);
                    break;
                }
            }
            return jwt;
        }

        private bool ReqAuthorize()
        {
            msg = Encoding.UTF8.GetBytes("1." + jwt);
            for (int i = 0; i < 3; ++i)
            {
                client.Send(msg, msg.Length, ip3);
                Task<UdpReceiveResult> res = client.ReceiveAsync();
                if (res.Wait(3000))
                {
                    return Encoding.UTF8.GetString(res.Result.Buffer) == "true";
                }
            }
            return false;
        }

        private bool UpdateAuthorize()
        {
            string oldJWT = jwt;
            string newJWT = GetNewJWT();
            msg = Encoding.UTF8.GetBytes($"3.{oldJWT}.{newJWT}");
            for (int i = 0; i < 3; ++i)
            {
                client.Send(msg, msg.Length, ip3);
                Task<UdpReceiveResult> res = client.ReceiveAsync();
                if (res.Wait(3000))
                {
                    return Encoding.UTF8.GetString(res.Result.Buffer) == "true";
                }
            }
            return false;
        }

        public bool LogIn()
        {
            if(string.IsNullOrEmpty(jwt))
            {
                _ = GetNewJWT();
                return ReqAuthorize();
            }
            else
            {
                return UpdateAuthorize();
            }
        }

        public bool LogOut()
        {
            msg = Encoding.UTF8.GetBytes($"2.{jwt}");
            for (int i = 0; i < 3; ++i)
            {
                client.Send(msg, msg.Length, ip3);
                Task<UdpReceiveResult> res = client.ReceiveAsync();
                if (res.Wait(3000))
                {
                    return Encoding.UTF8.GetString(res.Result.Buffer) == "true";
                }
            }
            return false;
        }

        public void ReadFromFile(string filepath="SaveFile.dat")
        {
            if(File.Exists(filepath))
            {
                jwt = File.ReadAllText(filepath);
            }
            

        }
        
        public void WriteToFile(string filepath="SaveFile.dat")
        {
            File.WriteAllText(filepath, jwt);
        }
    }
}
