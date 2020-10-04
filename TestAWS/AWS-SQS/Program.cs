using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AWS_SQS
{
    class Program
    {
        private static async Task ShowQueues(IAmazonSQS sqsClient)
        {
            ListQueuesResponse responseList = await sqsClient.ListQueuesAsync("");
            Console.WriteLine();
            foreach (string qUrl in responseList.QueueUrls)
            {
                // Get and show all attributes. Could also get a subset.
                await ShowAllAttributes(sqsClient, qUrl);
            }
        }
        // Method to show all attributes of a queue
        private static async Task ShowAllAttributes(IAmazonSQS sqsClient, string qUrl)
        {
            var attributes = new List<string> { QueueAttributeName.All };
            GetQueueAttributesResponse responseGetAtt =
              await sqsClient.GetQueueAttributesAsync(qUrl, attributes);
            Console.WriteLine($"Queue: {qUrl}");
            foreach (var att in responseGetAtt.Attributes)
                Console.WriteLine($"\t{att.Key}: {att.Value}");
        }
       /// <summary>
       /// Create Queue AWS SQS
       /// </summary>
       /// <param name="sqsClient"></param>
       /// <param name="qName"></param>
       /// <param name="deadLetterQueueUrl"></param>
       /// <param name="maxReceiveCount"></param>
       /// <param name="receiveWaitTime"></param>
       /// <returns></returns>
        private static async Task<string> CreateQueue(IAmazonSQS sqsClient, string qName, string deadLetterQueueUrl = null,
            string maxReceiveCount = null, string receiveWaitTime = null)
        {
            var attrs = new Dictionary<string, string>();

            // If a dead-letter queue is given, create a message queue
            if (!string.IsNullOrEmpty(deadLetterQueueUrl))
            {
                attrs.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, receiveWaitTime);
                attrs.Add(QueueAttributeName.RedrivePolicy,
                  $"{{\"deadLetterTargetArn\":\"{await GetQueueArn(sqsClient, deadLetterQueueUrl)}\"," +
                  $"\"maxReceiveCount\":\"{maxReceiveCount}\"}}");
                // Add other attributes for the message queue such as VisibilityTimeout
            }

            // If no dead-letter queue is given, create one of those instead
            //else
            //{
            //  // Add attributes for the dead-letter queue as needed
            //  attrs.Add();
            //}

            // Create the queue
            CreateQueueResponse responseCreate = await sqsClient.CreateQueueAsync(
                new CreateQueueRequest() {
                    QueueName=qName,
                    Attributes=attrs
                });
            return responseCreate.QueueUrl;
        }
        // Method to get the ARN of a queue
        private static async Task<string> GetQueueArn(IAmazonSQS sqsClient, string qUrl)
        {
            GetQueueAttributesResponse responseGetAtt = await sqsClient.GetQueueAttributesAsync(
              qUrl, new List<string> { QueueAttributeName.QueueArn });
            return responseGetAtt.QueueARN;
        }
        //
        // Method to delete an SQS queue
        private static async Task DeleteQueue(IAmazonSQS sqsClient, string qUrl)
        {
            Console.WriteLine($"Deleting queue {qUrl}...");
            await sqsClient.DeleteQueueAsync(qUrl);
            Console.WriteLine($"Queue {qUrl} has been deleted.");
        }
        static async Task Main(string[] args)
        {
            var credentials = new BasicAWSCredentials("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");
            var sqsClient = new AmazonSQSClient(credentials,RegionEndpoint.USWest2);            
            Console.WriteLine("--------------------------------------------------");
            //Console.WriteLine("Start create queue sqs");
            //string result=await CreateQueue(sqsClient, "Ratio-Queue3", null, null, "20");
            //Console.WriteLine($"{result}");

            // remove queue has url : https://sqs.us-west-2.amazonaws.com/069629214577/Ratio-Queue3
            await DeleteQueue(sqsClient, "https://sqs.us-west-2.amazonaws.com/069629214577/Ratio-Queue3");
        }
    }
}
