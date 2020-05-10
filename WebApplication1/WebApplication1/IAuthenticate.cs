using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public interface IAuthenticate
    {
        string Registe(string userName, string password, string type);
    }
}
