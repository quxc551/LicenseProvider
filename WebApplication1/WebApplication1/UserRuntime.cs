using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

[Serializable]
public struct UserInfo
{
    public string userName;
    public Guid userID;
    public DateTime expiringTime;
}

namespace WebApplication1
{
    public class UserRuntime
    {
        private Dictionary<string, List<UserInfo>> userList = new Dictionary<string, List<UserInfo>>();

        public UserRuntime(string filePath = "DataFile.dat")
        {

        }

        public void ReadFromFile(string filePath= "DataFile.dat")
        {
            string json = File.ReadAllText(filePath);
            object o = JsonConvert.DeserializeObject<Dictionary<string, List<UserInfo>>>(json);
            userList = (Dictionary<string, List<UserInfo>>)o;
        }
        public void WriteToFile(string filePath = "DataFile.dat")
        {
            string json = JsonConvert.SerializeObject(userList);
            File.WriteAllText(filePath, json);
        }
        public bool AuthorizeUser(UserInfo userInfo)
        {
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
    }
}
