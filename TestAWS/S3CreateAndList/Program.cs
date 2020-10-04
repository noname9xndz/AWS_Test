using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace S3CreateAndList
{
    class Program
    {
        static void Main(string[] args)
        {

            //string topicArn = "arn:aws:sns:us-west-2:069629214577:AWS-Notification";
            //string message = "Hello Ratio, This is message send from code " + DateTime.Now.ToShortTimeString();

            //var client = new AmazonSimpleNotificationServiceClient("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");

            //var request = new PublishRequest
            //{
            //    Message = message,
            //    TopicArn = topicArn
            //};

            //try
            //{
            //    var response = client.Publish(request);

            //    Console.WriteLine("Message sent to topic:");
            //    Console.WriteLine(message);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Caught exception publishing request:");
            //    Console.WriteLine(ex.Message);
            //}

            var client = new AmazonSimpleNotificationServiceClient("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");
            var request = new ListTopicsRequest();
            var response = new ListTopicsResponse();

            do
            {
                response = client.ListTopics(request);

                foreach (var topic in response.Topics)
                {
                    Console.WriteLine("Topic: {0}", topic.TopicArn);

                    var subs = client.ListSubscriptionsByTopic(
                      new ListSubscriptionsByTopicRequest
                      {
                          TopicArn = topic.TopicArn
                      });

                    var ss = subs.Subscriptions;

                    if (ss.Any())
                    {
                        Console.WriteLine("  Subscriptions:");

                        foreach (var sub in ss)
                        {
                            Console.WriteLine("    {0}", sub.SubscriptionArn);
                        }
                    }

                    var attrs = client.GetTopicAttributes(
                      new GetTopicAttributesRequest
                      {
                          TopicArn = topic.TopicArn
                      }).Attributes;

                    if (attrs.Any())
                    {
                        Console.WriteLine("  Attributes:");

                        foreach (var attr in attrs)
                        {
                            Console.WriteLine("    {0} = {1}", attr.Key, attr.Value);
                        }
                    }

                    Console.WriteLine();
                }

                request.NextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(response.NextToken));
        }

    }
}
