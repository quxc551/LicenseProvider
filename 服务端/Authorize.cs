using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace 服务端
{
    struct UserInfo
    {
        public string userName; 
        public Guid userID;

        // 这是我私自加的、用于检验令牌是否过期
        public int iat;
        public int exp;
    }

    /// <summary>
    /// 对客户端的请求进行授权
    /// </summary>
    public class Authorize
    {
        /// <summary>
        /// 服务器端的秘钥
        /// </summary>
        private string secret;

        /// <summary>
        /// 给予验证身份后的用户使用授权
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否取得授权</returns>
        public bool GetAuthorization(string token)
        {
            if (VerifyToken(token)) 
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
            String[] s = token.Split(".");

            // 由head和payload再次生成signature
            String encodedS = s[0] + '.' + s[1];
            var hs256 = new HMACSHA256(System.Text.Encoding.Default.GetBytes(secret));
            byte[] hashmessage = hs256.ComputeHash(System.Text.Encoding.Default.GetBytes(encodedS));
            String temp = "";
            for (int i = 0; i < hashmessage.Length; i++)
            {
                temp += hashmessage[i].ToString("X2");
            }

            // 比较自带signature和生成的signature
            return s[2].Equals(temp);
        }

        /// <summary>
        /// 获取令牌中的信息
        /// </summary>
        /// <param name="token">有效的令牌</param>
        /// <returns>用户信息</returns>
        private UserInfo DecodeToken(string token)
        {
            // 获取序列化的payload
            String[] s = token.Split(".");
            String temp = Encoding.ASCII.GetString(Convert.FromBase64String(s[1]));
            dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(temp);

            // 从payload中获取信息
            UserInfo u = new UserInfo();

            /*
             * 这里还需要获取guid
             */

            return u;
        }

        /// <summary>
        /// 检查授权数是否达到上限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>是否授权成功</returns>
        private bool TryAuthorize(UserInfo userInfo)
        {
            /*
             * 这里应该先验证令牌是否过期
             */

            /*
             * 然后检查授权数是否达到上限
             */

            throw new NotImplementedException();
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
    }
}
