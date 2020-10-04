using Amazon.SQS;
using Amazon.SQS.Model;
using DemoAWS.Services.AWS_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Services.AWS_Services.Implements
{
    public class AWS_SQSService : IAWS_SQSService
    {       
        public async Task<string> CreateQueue(IAmazonSQS sqsClient, string qName, string deadLetterQueueUrl = null, string maxReceiveCount = null, string receiveWaitTime = null)
        {
            var attrs = new Dictionary<string, string>();

            // If a dead-letter queue is given, create a message queue
            if (!string.IsNullOrEmpty(deadLetterQueueUrl))
            {
                attrs.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, receiveWaitTime);
                attrs.Add(QueueAttributeName.RedrivePolicy,
                  $"{{\"deadLetterTargetArn\":\"{await GetQueueArn(sqsClient, deadLetterQueueUrl)}\"," +
                  $"\"maxReceiveCount\":\"{maxReceiveCount}\"}}");
            }

            CreateQueueResponse responseCreate = await sqsClient.CreateQueueAsync(
              new CreateQueueRequest()
              {
                  QueueName = qName,
                  Attributes = attrs
              });
            return responseCreate.QueueUrl;
        }
        private static async Task<string> GetQueueArn(IAmazonSQS sqsClient, string qUrl)
        {
            GetQueueAttributesResponse responseGetAtt = await sqsClient.GetQueueAttributesAsync(qUrl, new List<string> { QueueAttributeName.QueueArn });
            return responseGetAtt.QueueARN;
        }
        public async Task DeleteQueue(IAmazonSQS sqsClient, string qUrl)
        {
            Console.WriteLine($"Deleting queue {qUrl}...");
            await sqsClient.DeleteQueueAsync(qUrl);
            Console.WriteLine($"Queue {qUrl} has been deleted.");
        }
    }
}
