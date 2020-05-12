using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebApplication1;

namespace UnitTestProject1
{
    [TestClass]
    public class TestUserRuntime
    {
        [TestMethod("写入到文件")]
        public void Write()
        {
            UserRuntime userRuntime = new UserRuntime();
            UserInfo user;
            user.expiringTime = DateTime.Now.AddDays(1);
            user.userID = Guid.NewGuid();
            user.userName = "songrunhan";
            userRuntime.AuthorizeUser(user);

            userRuntime.WriteToFile();
        }

        [TestMethod("从文件读取")]
        public void Read()
        {
            UserRuntime userRuntime = new UserRuntime();
            userRuntime.ReadFromFile();
        }

        [TestMethod("正确用户")]
        public void AU()
        {
            UserRuntime userRuntime = new UserRuntime();
            UserInfo user;
            user.expiringTime = DateTime.Now.AddDays(1);
            user.userID = Guid.NewGuid();
            user.userName = "songrunhan";

            Assert.IsTrue(userRuntime.AuthorizeUser(user));
        }

        [TestMethod("过期用户")]
        public void AU_E1()
        {
            UserRuntime userRuntime = new UserRuntime();
            UserInfo user;
            user.expiringTime = DateTime.Now.AddDays(1);
            user.userID = Guid.NewGuid();
            user.userName = "badgay";

            Assert.IsTrue(userRuntime.AuthorizeUser(user));
        }

        [TestMethod("过多用户")]
        public void AU_E2()
        {
            UserRuntime userRuntime = new UserRuntime();
            UserInfo user;
            user.expiringTime = DateTime.Now.AddDays(1);
            user.userName = "verybadgay";
            for (int i = 0; i < 10; ++i)
            {
                user.userID = Guid.NewGuid();
                userRuntime.AuthorizeUser(user);
            }
            user.userID = Guid.NewGuid();

            Assert.IsFalse(userRuntime.AuthorizeUser(user));
        }

    }

    [TestClass]
    public class TestRegistedUser
    {
        [TestMethod("写入到文件")]
        public void Write()
        {
            RegistedUser registedUser = new RegistedUser();
            RegRecord regRecord;
            regRecord.password = "123456";
            regRecord.serialNumber = "1234567845";
            regRecord.type = 1;
            regRecord.userName = "songrunhan";
            registedUser.Add(regRecord);
            registedUser.WriteToFile();
        }

        [TestMethod("从文件读取")]
        public void Read()
        {
            RegistedUser registedUser = new RegistedUser();
            registedUser.ReadFromFile();
        }

        [TestMethod("正确用户")]
        public void AU()
        {
            RegistedUser registedUser = new RegistedUser();
            RegRecord regRecord;
            regRecord.password = "123456";
            regRecord.serialNumber = "1234567845";
            regRecord.type = 1;
            regRecord.userName = "songrunhan";
            registedUser.Add(regRecord);
        }
    }
}
