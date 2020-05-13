using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebApplication1
{

    public class ClientMessage2
    {
        /// <summary>
        /// 客户端地址信息
        /// </summary>
        /// 
        public IPEndPoint clientIPEndPoint;

        /// <summary>
        /// 客户端发来的令牌信息
        /// </summary>
        /// 
        public string token;

        /// <param name="point"></param>
        public ClientMessage2(IPEndPoint point, string message)
        {
            clientIPEndPoint = new IPEndPoint(point.Address, point.Port);
            token = message;
        }
    }


    /// <summary>
    /// 对客户端的请求进行授权
    /// </summary>
    public class Authorize
    {
        /// <summary>
        /// 服务器端的秘钥
        /// </summary>
        private string secret = "75012";
        private UdpClient client = new UdpClient(8888, AddressFamily.InterNetwork);
        private UserRuntime userRuntime = new UserRuntime();

        public Authorize()
        {
            userRuntime.ReadFromFile();
            userRuntime.WriteToFile();
            Thread th = new Thread(UdpListener);
            th.Start();
        }


        //从客户端获得令牌
        public ClientMessage2 getToken()
        {
            byte[] recvBuffer = new byte[1024];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            recvBuffer = client.Receive(ref ip);
            ClientMessage2 message = new ClientMessage2(ip, Encoding.Default.GetString(recvBuffer));
            return message;
        }

        //监听发来的令牌消息
        public void UdpListener()
        {
            while (true)
            {
                ClientMessage2 message = getToken();
                SendResult(message);
            }
        }


        //发送授权结果给客户端
       public void SendResult(ClientMessage2 clientMessage)
        {
            byte[] sendBuffer = Encoding.Default.GetBytes("true");
            IPEndPoint ip = clientMessage.clientIPEndPoint;
            if (GetAuthorization(clientMessage.token))
                client.Send(sendBuffer, sendBuffer.Length, ip);
            else
            {
                sendBuffer = Encoding.Default.GetBytes("false");
                client.Send(sendBuffer, sendBuffer.Length, ip);
            }
        }

        /// <summary>
        /// 给予验证身份后的用户使用授权
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否取得授权</returns>
        public bool GetAuthorization(string token)
        {
            if (token!=""&&VerifyToken(token)) 
                return TryAuthorize(DecodeToken(token));

            // 令牌无效
            return false;
        }

        /// <summary>
        /// 验证令牌是否有效
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否有效</returns>
        private bool VerifyToken(string token)
        {
            string[] s = token.Split('.');

            // 由head和payload再次生成signature
            string encodedS = s[0]  + s[1];

            HMACSHA256 maker = new HMACSHA256(System.Text.Encoding.Default.GetBytes(secret));
            string signature = Encoding.Default.GetString(maker.ComputeHash(Encoding.Default.GetBytes(encodedS)));
            //var hs256 = new HMACSHA256(System.Text.Encoding.Default.GetBytes(secret));
            //byte[] hashmessage = hs256.ComputeHash(System.Text.Encoding.Default.GetBytes(encodedS));
            //string temp = "";
            //for (int i = 0; i < hashmessage.Length; i++)
            //{
            //    temp += hashmessage[i].ToString("X2");
            //}

            // 比较自带signature和生成的signature
            return s[2].Equals(signature);
        }

        /// <summary>
        /// 获取令牌中的信息
        /// </summary>
        /// <param name="token">有效的令牌</param>
        /// <returns>用户信息</returns>
        private UserInfo DecodeToken(string token)
        {
            // 获取序列化的payload
            String[] s = token.Split('.');
            String temp = Encoding.ASCII.GetString(Convert.FromBase64String(s[1]));
            dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(temp);

            // 从payload中获取信息
            UserInfo u = new UserInfo();
            u.expiringTime = payload.iat;
            u.userName = payload.userName;
            u.userID = payload.id;
            return u;
        }

        /// <summary>
        /// 检查授权数是否达到上限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>是否授权成功</returns>
        private bool TryAuthorize(UserInfo userInfo)
        {

            //以授权用户为10个人为例
            userRuntime.ReadFromFile();
            bool a=userRuntime.AuthorizeUser(userInfo);
            userRuntime.WriteToFile();

            return a;
            //StreamReader reader = new StreamReader("D:\\AuthorizationData.txt");
            //while (!reader.EndOfStream)
            //{
            //    string[] AutRecord = reader.ReadLine().Split('\0');
            //    if (AutRecord[0].Equals(userInfo.userName) && AutRecord.Length < 11)
            //        return true;
            //}
            //return false;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 生成新令牌
        /// </summary>
        /// <param name="part1"></param>
        /// <param name="part2"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string MakeNewToken(string part1,string part2,string secret)
        {
            String head = Convert.ToBase64String(Encoding.ASCII.GetBytes(part1)); // JWT中的head
            String payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(part2)); // JWT中的payload
            String encryptString = head + '.' + payload; // 签名由head+'.'+payload生成

            // hs256生成signature
            var hs256 = new HMACSHA256(System.Text.Encoding.Default.GetBytes(secret));
            byte[] encrypt = hs256.ComputeHash(System.Text.Encoding.Default.GetBytes(encryptString));
            String signature = "";
            for (int i = 0; i < encrypt.Length; i++)
            {
                signature += encrypt[i].ToString("X2");
            }

            return encryptString + '.' + signature;
        }

        public string GetUserList()
        {
            return userRuntime.GetUserList();
        }
    }
}
