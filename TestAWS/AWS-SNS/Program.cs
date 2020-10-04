using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AWS_SNS
{
    class Program
    {       
        /// <summary>
        /// Publish message to by Topic for all subcriptions
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="topicArn">topic full name</param>
        /// <returns></returns>
       public static async Task publishMessage(AmazonSimpleNotificationServiceClient client, string topicArn, string message)
        {            
            //string message = "Hello Ratio. This is code from C# message" + DateTime.Now.ToShortTimeString();            
            var request = new PublishRequest{
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

        public static async Task sendSMSMessage(AmazonSimpleNotificationServiceClient client, string number)
        {
            string message = $"SMS Message send from code C# {DateTime.Now.ToShortTimeString()}";

            var request = new PublishRequest
            {
                Message = message,
                PhoneNumber = number
            };

            try
            {
                var response = await client.PublishAsync(request);

                Console.WriteLine("Message sent to " + number + ":");
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception publishing request:");
                Console.WriteLine(ex.Message);
            }
        }       
        public static async Task createTopic(AmazonSimpleNotificationServiceClient client, string topicName)
        {
            CreateTopicRequest createTopicRequest = new CreateTopicRequest(topicName);
            CreateTopicResponse createTopicResponse = await client.CreateTopicAsync(createTopicRequest);
            // get all topic
            var lstTopics = await client.ListTopicsAsync(new ListTopicsRequest());
            Console.WriteLine("List Topics:");
            foreach (var item in lstTopics.Topics)
            {
                Console.WriteLine($"{item.TopicArn}");
            }
        }
        public static async Task subcribesTopic(AmazonSimpleNotificationServiceClient client,string topicArn)
        {
            // Subscribe an email endpoint to an Amazon SNS topic.
            SubscribeRequest subscribeRequest = new SubscribeRequest(topicArn, "email", "huynguyen98.clc@gmail.com");
            SubscribeResponse subscribeResponse = await client.SubscribeAsync(subscribeRequest);
            // Print the request ID for the SubscribeRequest action.
            Console.WriteLine("SubscribeRequest: " + subscribeResponse.ResponseMetadata.RequestId);
            Console.WriteLine("To confirm the subscription, check your email.");
        }
        //Note : When you delete a topic, you also delete all subscriptions to the topic.
        public static async Task deleteTopic(AmazonSimpleNotificationServiceClient client, string topicArn)
        {
            // Delete an Amazon SNS topic.
            DeleteTopicRequest deleteTopicRequest = new DeleteTopicRequest(topicArn);
            DeleteTopicResponse deleteTopicResponse =await client.DeleteTopicAsync(deleteTopicRequest);

            // Print the request ID for the DeleteTopicRequest action.
            Console.WriteLine("DeleteTopicRequest: " + deleteTopicResponse.ResponseMetadata.RequestId);
        }
        public static async Task publishMessageWithAttributes(AmazonSimpleNotificationServiceClient client, string topicArn, string messageBody)
        {
            // Initialize the example class.
            SNSMessageAttributes message = new SNSMessageAttributes(messageBody);

            // Add message attributes with string values.
            message.AddAttribute("store", "example_corp");
            message.AddAttribute("event", "order_placed");

            // Add a message attribute with a list of string values.
            List<String> interestsValues = new List<String>();
            interestsValues.Add("soccer");
            interestsValues.Add("rugby");
            interestsValues.Add("hockey");
            message.AddAttribute("customer_interests", interestsValues);

            // Add a message attribute with a numeric value.
            message.AddAttribute("price_usd", 1000);

            // Add a Boolean attribute for filtering using subscription filter policies.
            // The class applies a String.Array data type to this attribute, allowing it
            // to be evaluated by a filter policy.
            List<Boolean> encryptedVal = new List<Boolean>();
            encryptedVal.Add(false);
            message.AddAttribute("encrypted", encryptedVal);

            // Publish the message.
            String msgId =await message.Publish(client, topicArn);

            // Print the MessageId of the published message.
            Console.WriteLine("MessageId: " + msgId);
        }
       static async Task Main(string[] args)
        {

            var request = new ListTopicsRequest();
            var response = new ListTopicsResponse();
            var credentials = new BasicAWSCredentials("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");
            var client = new AmazonSimpleNotificationServiceClient(credentials,RegionEndpoint.USWest2);
            //await createTopic(client, "AWS-Ratio");            
            // Send SMS Messages             

            //do
            //{
            //    // get list topic 
            response = await client.ListTopicsAsync(request);

            foreach (var topic in response.Topics)
            {
                Console.WriteLine("Topic: {0}", topic.TopicArn);

                // get list subs by topic
                var subs = await client.ListSubscriptionsByTopicAsync(
                  new ListSubscriptionsByTopicRequest
                  {
                      TopicArn = topic.TopicArn
                  });

                var ss = subs.Subscriptions;

                if (ss.Any())
                {
                    Console.WriteLine("Start send message:");
                    await publishMessage(client, topic.TopicArn, "Hello Ratio. This is message to test AWS SQS");
                }

                //        var attrs = client.GetTopicAttributesAsync(
                //          new GetTopicAttributesRequest
                //          {
                //              TopicArn = topic.TopicArn
                //          });


                Console.WriteLine();
            }

            //    request.NextToken = response.NextToken;

            //} while (!string.IsNullOrEmpty(response.NextToken));
        }
    }
}
