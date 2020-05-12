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

    public class ClientMessage
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
            //string a = Registe("wang", "1", "1");
           // FileStream fs = new FileStream("RUser.db", FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            //StreamReader reader= new StreamReader(fs);
            registedUser.ReadFromFile();
            registedUser.WriteToFile();

            //registedUser.ReadFromFile();
            //while(!reader.EndOfStream)
            //{
            //    string regRecord = reader.ReadLine();
            //    users.Add(new UserData(regRecord.Split('\0')));
            //}


            // OnDataRecv callback = new OnDataRecv(ProcessData);
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
        public string Registe(string userName, string password, string type)
        {
            string serialNumber = string.Empty;
            for (int i = 0; i < 10; ++i)
                serialNumber += new Random().Next(1, 9).ToString();

            RegRecord regRecord1 = new RegRecord(userName, password, serialNumber, int.Parse(type));
            registedUser.ReadFromFile();
            registedUser.Add(regRecord1);
            registedUser.WriteToFile();

            //string[] regRecord = { serialNumber, userName, password, type };
            //users.Add(new UserData(regRecord));
            //FileStream fs = new FileStream("RUser.db", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            //StreamWriter writer = new StreamWriter(fs);
            //writer.WriteLine(serialNumber + '\0' + userName + '\0' + password + '\0' + type);
            //writer.Close();
            return serialNumber;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <returns>令牌</returns>
        public string LogIn(string serialNumber)
        {
            registedUser.ReadFromFile();
            if (registedUser.Contains(serialNumber))
            {
                var header = new
                {
                    typ = "JWT",
                    alg = "HS256"
                };
                var payload = new
                {
                    iat = DateTime.Now.AddHours(10),
                    userName = registedUser.GetUser(serialNumber).userName,
                    id = Guid.NewGuid()
                };
                string part1 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(header)));
                string part2 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(payload)));
                string res = part1 + part2 ;
                HMACSHA256 maker = new HMACSHA256(System.Text.Encoding.Default.GetBytes(secret));
                string signature = Encoding.Default.GetString(maker.ComputeHash(Encoding.Default.GetBytes(res)));
                string result = $"{part1}.{part2}.{signature}";
                return result;
            }
            else
                return null;

            //int pos = users.FindIndex(e => e.serialNumber == serialNumber);
            //if (pos == -1)
            //{
            //    return null;
            //}

            //else
            //{               
            //    var header = new
            //    {
            //        typ = "JWT",
            //        alg = "HS256"
            //    };
            //    var payload = new
            //    {
            //        iat = DateTime.Now,
            //        exp = 1800,
            //        userName = users[pos].userName,
            //        id = Guid.NewGuid()
            //    };

            //    //将每个用户下的guid号记录到AuthorizationData.txt中
            //    FileStream fs = new FileStream("RUser.db", FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            //    StreamReader reader = new StreamReader(fs);
            //    StreamWriter writer = new StreamWriter("D:\\AuthorizationData.txt");
            //    int FilLines = 0;
            //    int line = 0;
            //    while (!reader.EndOfStream)
            //    {
            //        string AutRecord = reader.ReadLine();
            //        ++FilLines;
            //        if (AutRecord.Substring(0, AutRecord.IndexOf('\0') - 1).Equals(payload.id))
            //            line= FilLines;
            //    }
            //    if (line==0)
            //    {
            //        writer.WriteLine(payload.userName + '\0' + payload.id);
            //    }
            //    else
            //    {
            //        string[] str = File.ReadAllLines("D:\\AuthorizationData.txt");
            //        string[] str2 = str[line].Split('\0');
            //        int i = 1;
            //        for (;i<str2.Length;++i)
            //        {
            //            if (str2[i].Equals(payload.id)) break;
            //        }
            //        if (i == str2.Length)
            //        {
            //            str[line] += '\0' + payload.id.ToString();
            //        }
            //        File.WriteAllText("D:\\AuthorizationData.txt", string.Empty);
            //        for(int j = 0; j < str.Length; ++j)
            //        {
            //            writer.WriteLine(str[j]);
            //        }
            //    }
            //    reader.Close();
            //    writer.Close();

            //    string part1 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(header)));
            //    string part2 = Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(payload)));
            //    string res = part1 + part2 + secret;
            //    HMACSHA256 maker = new HMACSHA256();
            //    string signature = Encoding.Default.GetString(maker.ComputeHash(Encoding.Default.GetBytes(res)));
            //    return part1 + part2 + signature;
            //}
        }

        public void UDPListener(object obj)
        {
            byte[] recvBuffer = new byte[1024];

            while (true)//持续监听端口
            {

                ClientMessage message = getSerialNumber();
                ProcessData(message);
            }
        }

        public void ProcessData(ClientMessage clientMessage)
        {
            string serialNum = clientMessage.serialNum;
            string token = LogIn(serialNum);
            sentToken(token, clientMessage.clientIPEndPoint);
        }

        //获得用户注册后的序列号
        public ClientMessage getSerialNumber()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvBuffer = new byte[11];
            recvBuffer = client.Receive(ref ip);
            ClientMessage clientMessage = new ClientMessage(ip, Encoding.UTF8.GetString(recvBuffer));
            return clientMessage;
        }

        //将令牌发送到客户端
        public void sentToken(string token, IPEndPoint iP)
        {
            if (token == null)
                token = "";
            byte[] sendbuffer = Encoding.Default.GetBytes(token);
            client.Send(sendbuffer, sendbuffer.Length, iP);
        }
    }
}
