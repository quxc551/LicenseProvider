using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace 服务端
{
    public struct UserData
    {
        public string serialNumber;
        public string userName;
        public string password;
        public string type;
        public UserData(string[] value)
        {
            serialNumber = value[0];
            userName = value[1];
            password = value[2];
            type = value[3];
        }
    }

    /// <summary>
    /// 验证用户身份
    /// </summary>
    class Authenticate
    {
        public delegate void OnDataRecv(byte[] data);
        private UdpClient client = new UdpClient(8910, AddressFamily.InterNetwork);
        private string secret = "75012";
        private List<UserData> users;

        Authenticate()
        {
            StreamReader reader= new StreamReader("RegistedUser.db");
            while(!reader.EndOfStream)
            {
                string regRecord = reader.ReadLine();
                users.Add(new UserData(regRecord.Split('\0')));
            }
            OnDataRecv callback = new OnDataRecv(ProcessData);
            Thread th = new Thread(UDPListener);
            th.Start(callback);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="type">许可证类型</param>
        /// <returns>序列号</returns>
        public string Registe(string userName, string password, string type)
        {
            string serialNumber = string.Empty;
            for (int i = 0; i < 10; ++i)
                serialNumber += new Random().Next(1, 9).ToString();
            string[] regRecord = { serialNumber, userName, password, type };
            users.Add(new UserData(regRecord));
            StreamWriter writer = new StreamWriter("RegistedUser.db", true);
            writer.WriteLine(serialNumber + '\0' + userName + '\0' + password + '\0' + type);
            writer.Close();
            return serialNumber;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <returns>令牌</returns>
        public string LogIn(string serialNumber)
        {
            int pos = users.FindIndex(e => e.serialNumber == serialNumber);
            if (pos == -1)
            {
                return null;
            }

            else
            {               
                var header = new
                {
                    typ = "JWT",
                    alg = "HS256"
                };
                var payload = new
                {
                    iat = DateTime.Now,
                    exp = 1800,
                    userName = users[pos].userName,
                    id = Guid.NewGuid()
                };
                string part1 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(header)));
                string part2 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(payload)));
                string res = part1 + part2 + secret;
                HMACSHA256 maker = new HMACSHA256();
                string signature = maker.ComputeHash(Encoding.Default.GetBytes(res)).ToString();
                return part1 + part2 + signature;
            }
        }

        public void UDPListener(object obj)
        {
            byte[] recvBuffer = new byte[1024];
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 8888);
            client.Receive(ref ip);
            OnDataRecv CallBack = obj as OnDataRecv;
            CallBack(recvBuffer);
        }

        public void ProcessData(byte[] data)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("192.168.18.3"), 6000);
            string userinfo = data.ToString();
            string[] arr = userinfo.Split('\0');
            byte[] sendbuffer = Encoding.Default.GetBytes(Registe(arr[0], arr[1], arr[2]));
            client.Send(sendbuffer, sendbuffer.Length, ip);
        }
    }
}
