using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Models.ViewModels
{
    public class TopicViewModel
    {
        public string TopicName { get; set; }
        // topic arn
        public string Id { get; set; }
        public string SendMessage { get; set; }
        public string SubcriptionEmail { get; set; }
    }
}
