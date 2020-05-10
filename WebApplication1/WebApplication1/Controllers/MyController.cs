using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        public SortedDictionary<string, string> Payload
        {
            get; set;
        } = new SortedDictionary<string, string>();

        public byte[] PayloadBytes
        {
            get; set;
        }

        public string PostStream
        {
            get; set;
        } = string.Empty;


        public MyController(IHttpContextAccessor httpContextAccessor)
        {
            construct(httpContextAccessor);
        }

        public async void construct(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            httpContext.Request.EnableBuffering();
            #region 读取请求流
            if (httpContext.Request.ContentLength > 0)
            {
                int bodyLength = (int)httpContext.Request.ContentLength;

                if (bodyLength < 5 * 1024 * 1024) // 小于5MB才考虑是否转换字符串
                {
                    PayloadBytes = new byte[bodyLength];
                    await httpContext.Request.Body.ReadAsync(PayloadBytes, 0, bodyLength);
                    httpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        PostStream = Encoding.UTF8.GetString(PayloadBytes);
                        //在转换为PostStream后，测试能否转为JSON
                        JObject obj = JObject.Parse(PostStream);
                        Payload = new SortedDictionary<string, string>();
                        foreach (var token in obj)
                        {
                            if (token.Value != null)
                            {
                                Payload.Add(token.Key, token.Value.ToString());
                            }
                        }
                    }
                    catch
                    {
                        // 有问题就不管了，因为有可能输入的字符串而不是JSON
                    }
                }
            }
            #endregion
        }
    }
}
