
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Services.AWS_Services.Interfaces
{
    public interface IAWS_SNSService
    {
        Task PublishMessage(AmazonSimpleNotificationServiceClient client, string topicArn, string message);
        Task<ListTopicsResponse> CreateTopic(AmazonSimpleNotificationServiceClient client, string topicName);
        Task SubcribesTopic(AmazonSimpleNotificationServiceClient client,string topicArn, string email);
        Task DeleteTopic(AmazonSimpleNotificationServiceClient client, string topicArn);
    }
}
