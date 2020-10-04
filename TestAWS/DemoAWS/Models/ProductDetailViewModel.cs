using DemoAWS.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Models
{
    public class SNS_Message
    {
        public Product Product{ get; set; }
        public string message { get; set; }
    }
}
