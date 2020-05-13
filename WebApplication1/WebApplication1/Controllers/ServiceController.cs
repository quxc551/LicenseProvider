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
            authenticate.Registe("1", "1", 1);
        }

        [HttpPost]
        [Route("~/api/mine")]
        public ActionResult Get()
        {
            var x = Payload;
            authenticate.Registe("1", "1", 1);
            var rng = new Random();
            return Ok(new
            {
                x = "123"
            }
                );
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
