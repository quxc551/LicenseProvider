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
using 服务端;

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
        public IServiceProvider provider;
        public Authenticate authenticate;

        public ServiceController(IServiceProvider provider,IHttpContextAccessor httpContextAccessor,Authenticate authenticate):base(httpContextAccessor)
        {
            this.authenticate = authenticate;
            authenticate.Registe("1", "1", "1");
        }

        [HttpPost]
        [Route("~/api/mine")]
        public ActionResult Get()
        {
            var rng = new Random();
            return Ok(new
            {
                x = "123"
            }
                );
        }

    }

}
