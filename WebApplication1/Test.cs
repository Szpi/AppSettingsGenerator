using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class MySettings
    {
        IConfiguration _configuration;
        public MySettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string My_Test => _configuration.GetValue<string>("My.Test");
    }
}
