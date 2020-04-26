using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace 服务端
{
    /// <summary>
    /// 验证用户身份
    /// </summary>
    class Authenticate
    {
        /// <summary>
        /// 服务器端的秘钥
        /// </summary>
        private string secret;

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="type">许可证类型</param>
        /// <returns>序列号</returns>
        public string Registe(string userName, string password, string type)
        {
            //throw new NotImplementedException();
            string serialNumber="";
            for (int i = 0; i < 10; ++i)
                serialNumber += new Random().Next(1, 9).ToString();
            StreamWriter streamWriter = new StreamWriter("UserData.txt");
            streamWriter.WriteLine(serialNumber + "," + userName + "," + password + "," + type);
            streamWriter.Close();
            return serialNumber;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <returns>令牌</returns>
        public string LogIn(string serialNumber)
        {
            /*
             * 对序列号解密，合乎规则 则生成登录令牌
             */

            // 序列号错误
            if (false)
                return null; 

            else
            {               
                int now = (int)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;

                var header = new
                {
                    typ = "JWT",
                    alg = "HS256"
                };
                var payload = new
                {
                    iat = now, // 令牌签发时间,Unix时间戳(从1970年1月1日开始所经过的秒数，不考虑闰秒)
                    exp = now + 1800, // 令牌过期时间,比如30min

                    /*
                     * 宋润涵说这部分有用户的Guid，但我不知道是哪个标签。。。
                     */

                    //userName =  // 加密后的许可证或其它唯一标识符，用于获取许可证人数、限制授权
                };

                /*
                 * 然后这里我觉得应该调用Authorize的MakeNewToken
                 * 参数依次为Newtonsoft.Json.JsonConvert.SerializeObject(header),Newtonsoft.Json.JsonConvert.SerializeObject(payload),secret
                 */
                return null;
            }
        }
    }
}
