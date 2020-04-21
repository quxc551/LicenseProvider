using System;
using System.Collections.Generic;
using System.Text;

namespace 服务端
{
    struct UserInfo
    {
        string userName;
        Guid userID;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 验证令牌是否有效
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否有效</returns>
        private bool VerifyToken(string token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取令牌中的信息
        /// </summary>
        /// <param name="token">有效的令牌</param>
        /// <returns>用户信息</returns>
        private UserInfo DecodeToken(string token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 检查授权数是否达到上限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>是否授权成功</returns>
        private bool TryAuthorize(UserInfo userInfo)
        {
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
            throw new NotImplementedException();
        }
    }
}
