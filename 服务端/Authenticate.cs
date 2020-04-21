using System;
using System.Collections.Generic;
using System.Text;

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
        public string Registe(string userName,string password,string type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <returns>令牌</returns>
        public string LogIn(string serialNumber)
        {
            throw new NotImplementedException();
        }
    }
}
