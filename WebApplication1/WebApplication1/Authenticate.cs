using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using WebApplication1;

namespace WebApplication1
{

    public struct ClientMessage
    {
        /// <summary>
        /// 客户端地址信息
        /// </summary>
        /// 
        public IPEndPoint clientIPEndPoint;
        /// <summary>
        /// 客户端发来的序列号
        /// </summary>
        public string serialNum;

        public ClientMessage(IPEndPoint point, string mess)
        {
            clientIPEndPoint = new IPEndPoint(point.Address, point.Port);
            serialNum = mess;
        }
    }

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
    public class Authenticate
    {
        public delegate void OnDataRecv(byte[] data);
        private UdpClient client = new UdpClient(8910, AddressFamily.InterNetwork);
        private string secret = "75012";
        //private List<UserData> users=new List<UserData>();
        private RegistedUser registedUser = new RegistedUser();

        public Authenticate()
        {
            registedUser.ReadFromFile();

            Thread th = new Thread(UDPListener);
            th.Start();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="type">许可证类型</param>
        /// <returns>序列号</returns>
        public string Registe(string userName, string password, int type,int time)
        {
            if (registedUser.ContainsUserName(userName))
            {
                return "用户已存在";
            }

            string serialNumber = string.Empty;
            do
            {
                for (int i = 0; i < 10; ++i)
                    serialNumber += new Random().Next(1, 9).ToString();
            } while (registedUser.Contains(serialNumber));

            RegRecord regRecord1 = new RegRecord(userName, password, serialNumber, type,time);
            registedUser.ReadFromFile();
            registedUser.Add(regRecord1);
            registedUser.WriteToFile();
            return serialNumber;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <returns>令牌</returns>
        public string LogIn(string serialNumber)
        {
            if (registedUser.Contains(serialNumber))
            {
                var header = new
                {
                    typ = "JWT", //type
                    alg = "HS256" //algorithm
                };
                var payload = new
                {
                    iss = "一Ping就通",  //Issuer
                    iat = DateTime.Now, //Issued At
                    aud = registedUser.GetUser(serialNumber).userName, //Audience
                    exp = DateTime.Now.AddMinutes(registedUser.GetUser(serialNumber).AvailableTime), //Expiration Time
                    jti = Guid.NewGuid() //JWT ID
                };
                string part1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header)));
                string part2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                string res = $"{part1}.{part2}";
                HMACSHA256 maker = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                string signature = Convert.ToBase64String(maker.ComputeHash(Encoding.UTF8.GetBytes(res)));
                string jwt = $"{part1}.{part2}.{signature}";
                return jwt;
            }
            else
                return null;
        }

        public void UDPListener(object obj)
        {
            byte[] recvBuffer = new byte[1024];

            while (true)//持续监听端口
            {

                ClientMessage message = GetSerialNumber();
                ProcessData(message);
            }
        }

        public void ProcessData(ClientMessage clientMessage)
        {
            string serialNum = clientMessage.serialNum;
            string token = LogIn(serialNum);
            SentToken(token, clientMessage.clientIPEndPoint);
        }

        //获得用户注册后的序列号
        public ClientMessage GetSerialNumber()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvBuffer = new byte[11];
            recvBuffer = client.Receive(ref ip);
            ClientMessage clientMessage = new ClientMessage(ip, Encoding.UTF8.GetString(recvBuffer));
            return clientMessage;
        }

        //将令牌发送到客户端
        public void SentToken(string token, IPEndPoint iP)
        {
            if (token == null)
                token = "";
            byte[] sendbuffer = Encoding.UTF8.GetBytes(token);
            client.Send(sendbuffer, sendbuffer.Length, iP);
        }

        public List<RegRecord> GetUserInfo()
        {
            return registedUser.GetAllinfo();
        }

        public string DeleteUser(string serialNumber)
        {
            return registedUser.Remove(serialNumber);
        }
    }
}
