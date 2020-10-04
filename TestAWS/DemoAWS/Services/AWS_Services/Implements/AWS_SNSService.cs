using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DemoAWS.Services.AWS_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Services.AWS_Services.Implements
{
    public class AWS_SNSService : IAWS_SNSService
    {
        public async Task<ListTopicsResponse> CreateTopic(AmazonSimpleNotificationServiceClient client, string topicName)
        {
            CreateTopicRequest createTopicRequest = new CreateTopicRequest(topicName);
            CreateTopicResponse createTopicResponse = await client.CreateTopicAsync(createTopicRequest);
            // get all topic
            var lstTopics = await client.ListTopicsAsync(new ListTopicsRequest());
            return lstTopics;
            //Console.WriteLine("List Topics:");
            //foreach (var item in lstTopics.Topics)
            //{
            //    Console.WriteLine($"{item.TopicArn}");
            //}
        }

        public async Task DeleteTopic(AmazonSimpleNotificationServiceClient client, string topicArn)
        {
            // Delete an Amazon SNS topic.
            DeleteTopicRequest deleteTopicRequest = new DeleteTopicRequest(topicArn);
            DeleteTopicResponse deleteTopicResponse = await client.DeleteTopicAsync(deleteTopicRequest);

            // Print the request ID for the DeleteTopicRequest action.
            Console.WriteLine("DeleteTopicRequest: " + deleteTopicResponse.ResponseMetadata.RequestId);
        }

        public async Task PublishMessage(AmazonSimpleNotificationServiceClient client, string topicArn, string message)
        {
            var request = new PublishRequest
            {
                Message = message,
                TopicArn = topicArn
            };
            try
            {
                var response = await client.PublishAsync(request);

                Console.WriteLine("Message sent to topic:");
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception publishing request:");
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// create a subcriber with email--> subcribe a topic by tocpic Arn 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="topicArn"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SubcribesTopic(AmazonSimpleNotificationServiceClient client, string topicArn, string email)
        {
            // Subscribe an email endpoint to an Amazon SNS topic.
            SubscribeRequest subscribeRequest = new SubscribeRequest(topicArn, "email", email);
            SubscribeResponse subscribeResponse = await client.SubscribeAsync(subscribeRequest);
            // Print the request ID for the SubscribeRequest action.
            Console.WriteLine("SubscribeRequest: " + subscribeResponse.ResponseMetadata.RequestId);
            Console.WriteLine("To confirm the subscription, check your email.");
        }
    }
}
