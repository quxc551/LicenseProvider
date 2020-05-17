using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : MyController
    {
        //身份验证
        //private Authenticate authenticate = new Authenticate();
        //授权
        //private Authorize authorize = new Authorize();
        public Authenticate authenticate;
        public Authorize authorize;

        public ServiceController(Authorize authorize, IHttpContextAccessor httpContextAccessor,Authenticate authenticate):base(httpContextAccessor)
        {
            this.authenticate = authenticate;
            this.authorize = authorize;
        }

        [HttpGet]
        [Route("~/api/test")]
        public ActionResult Test()
        {
            return Ok(new
            {
                success = true,
                msg = "测试成功"
            });
        }

        [HttpPost]
        [Route("~/api/getLicenseStatus")]
        public ActionResult GetLicenseStatus()
        {
            List<RegRecord> records = authenticate.GetUserInfo();
            return Ok(new
            {
                success = true,
                msg = "",
                data = records.Select(e => new
                {
                    e.userName,
                    e.serialNumber,
                    usingCount = authorize.UserCountByName(e.userName),
                    licenseCount = 10
                })
            });
        }

        [HttpPost]
        [Route("~/api/regist")]
        public ActionResult GetToRegist()
        {
            string userName = Payload["userName"];
            string password = Payload["password"];
            string type = Payload["type"];
            //string userName=Payload[""]
            string result=authenticate.Registe(userName, password, int.Parse(type));
            if(result==("用户已存在"))
            {
                return Ok(new
                {
                    success = false,
                    msg = result,
                    license = ""

                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    msg = "注册成功",
                    license = result
                });
            }
        }

        [HttpPost]
        [Route("~/api/getRegList")]
        public ActionResult GetRegistInfo()
        {
            return Ok(new
            {
                success = true,
                msg = "注册信息",
                data = authenticate.GetUserInfo()
        }
            );
        }

        [HttpPost]
        [Route("~/api/delUsers")]
        public ActionResult DeleteUser()
        {
            string license = Payload["license"];

            string result = authenticate.DeleteUser(license);
            if (result == "不存在此用户")
            {
                return Ok(new
                {
                    success = false,
                    msg = result
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    msg = result
                });
            }
        }

        [HttpPost]
        [Route("~/api/GetUserList")]
        public ActionResult GetUserList()
        {
            return Ok(new
            {
                success = true,
                msg = "",
                data = authorize.GetUserList()
            });
        }
    }

}
