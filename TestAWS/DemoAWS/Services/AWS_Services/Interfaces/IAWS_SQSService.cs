using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Services.AWS_Services.Interfaces
{
    public interface IAWS_SQSService
    {
        Task<string> CreateQueue(IAmazonSQS sqsClient, string qName, string deadLetterQueueUrl = null, string maxReceiveCount = null, string receiveWaitTime = null);
        Task DeleteQueue(IAmazonSQS sqsClient, string qUrl);
    }
}
