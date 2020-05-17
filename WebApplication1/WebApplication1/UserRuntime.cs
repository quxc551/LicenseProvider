using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

public struct UserInfo
{
    public DateTime expiringTime;
    public string userName;
    public Guid userID;

}

namespace WebApplication1
{
    public class UserRuntime
    {
        private Dictionary<string, List<UserInfo>> userList = new Dictionary<string, List<UserInfo>>();

        public UserRuntime(string filePath = "DataFile.dat")
        {
            ReadFromFile(filePath);
        }
        public List<UserInfo> GetUserList()
        {
            List<UserInfo> temp = new List<UserInfo>();
            foreach(var value in userList.Values)
            {
                temp.AddRange(value);
            }
            return temp;
        }
        public int UserCountByName(string name)
        {
            return userList.ContainsKey(name) ? userList[name].Count : 0;
        }
        public void ReadFromFile(string filePath= "DataFile.dat")
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                object o = JsonConvert.DeserializeObject<Dictionary<string, List<UserInfo>>>(json);
                userList = (Dictionary<string, List<UserInfo>>)o;
            }
        }
        public void WriteToFile(string filePath = "DataFile.dat")
        {
            string json = JsonConvert.SerializeObject(userList);
            File.WriteAllText(filePath, json);
        }
        public bool AuthorizeUser(UserInfo userInfo)
        {
            clean();//首先清除过期的令牌用户
            if (userInfo.expiringTime < DateTime.Now) return false;
            //检查该用户名的用户组是否有授权
            if(userList.ContainsKey(userInfo.userName))
            {
                //检查该用户是否已经被授权
                int index = userList[userInfo.userName].FindIndex(e => e.userID == userInfo.userID);
                if (index > -1)
                {
                    userList[userInfo.userName][index] = userInfo;
                    return true;
                }
                else
                {
                    //看授权数是否已满
                    if (userList[userInfo.userName].Count < 10)
                    {
                        userList[userInfo.userName].Add(userInfo);
                        return true;
                    }
                    else
                    {
                        //这里应该检查是否有过期授权
                        return false;
                    }
                }
            }
            else
            {
                List<UserInfo> temp = new List<UserInfo>();
                temp.Add(userInfo);
                userList.Add(userInfo.userName, temp);
                return true;
            }
        }

        public void DeleteSubUser(UserInfo userInfo)
        {
            if (userList.ContainsKey(userInfo.userName))
            {
                int index = userList[userInfo.userName].FindIndex(e => e.userID == userInfo.userID);
                if(index>-1)
                {
                    userList[userInfo.userName].RemoveAt(index);
                }
            }
            WriteToFile();
        }

        public void UpdateUserState(UserInfo userInfo1,UserInfo userinfo2)
        {
            if (userList.ContainsKey(userInfo1.userName))
            {
                int index = userList[userInfo1.userName].FindIndex(e => e.userID == userInfo1.userID);
                if (index > -1)
                {
                    userList[userInfo1.userName].RemoveAt(index);
                    userList[userinfo2.userName].Add(userinfo2);
                }
            }
            WriteToFile();
        }
        public void DeleteUser(string userName)
        {
            if(userList.ContainsKey(userName))
            {
                userList.Remove(userName);
            }
        }
        /// <summary>
        /// 清楚令牌超时的用户
        /// </summary>
        public void clean()
        {
            foreach(List<UserInfo> userInfos in userList.Values)
            {
                for(int i=userInfos.Count-1;i>0;i--)
                {
                    if(userInfos[i].expiringTime<DateTime.Now)
                    {
                        userInfos.Remove(userInfos[i]);
                    }
                }
            }
            WriteToFile();
        }
    }
}
