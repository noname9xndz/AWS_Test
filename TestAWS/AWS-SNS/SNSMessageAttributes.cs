using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AWS_SNS
{
    class SNSMessageAttributes
    {
        private String message;
        private Dictionary<String, MessageAttributeValue> messageAttributes;

        public SNSMessageAttributes(String message)
        {
            this.message = message;
            messageAttributes = new Dictionary<string, MessageAttributeValue>();
        }

        public string Message
        {
            get => this.message;
            set => this.message = value;
        }

        public void AddAttribute(String attributeName, String attributeValue)
        {
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "String",
                StringValue = attributeValue
            };
        }

        public void AddAttribute(String attributeName, float attributeValue)
        {
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "Number",
                StringValue = attributeValue.ToString()
            };
        }

        public void AddAttribute(String attributeName, int attributeValue)
        {
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "Number",
                StringValue = attributeValue.ToString()
            };
        }

        public void AddAttribute(String attributeName, List<String> attributeValue)
        {
            String valueString = "[\"" + String.Join("\", \"", attributeValue.ToArray()) + "\"]";
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "String.Array",
                StringValue = valueString
            };
        }

        public void AddAttribute(String attributeName, List<float> attributeValue)
        {
            String valueString = "[" + String.Join(", ", attributeValue.ToArray()) + "]";
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "String.Array",
                StringValue = valueString
            };
        }

        public void AddAttribute(String attributeName, List<int> attributeValue)
        {
            String valueString = "[" + String.Join(", ", attributeValue.ToArray()) + "]";
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "String.Array",
                StringValue = valueString
            };
        }

        public void AddAttribute(String attributeName, List<Boolean> attributeValue)
        {
            String valueString = "[" + String.Join(", ", attributeValue.ToArray()) + "]";
            messageAttributes[attributeName] = new MessageAttributeValue
            {
                DataType = "String.Array",
                StringValue = valueString
            };
        }

        public async Task<String> Publish(AmazonSimpleNotificationServiceClient snsClient, String topicArn)
        {
            PublishRequest request = new PublishRequest
            {
                TopicArn = topicArn,
                MessageAttributes = messageAttributes,
                Message = message
            };
            PublishResponse result =await snsClient.PublishAsync(request);
            return result.MessageId;
        }
    }
}
