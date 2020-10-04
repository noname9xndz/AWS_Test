using Amazon.SimpleNotificationService.Model;
using AutoMapper;
using DemoAWS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Models.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Topic, TopicViewModel>()
                  .ForMember(dest =>
                     dest.Id,
                     opt => opt.MapFrom(src => src.TopicArn))
                  .ForMember(dest =>
                     dest.TopicName,
                     opt => opt.MapFrom(src => src.TopicArn.GetTopicName()))
                  .ReverseMap();               
        }        
    }
    public  static class Helper
    {
        public static string GetTopicName(this string topicArn)
        {
            var word = topicArn.Split(":").LastOrDefault();
            return word;
        }
    }


}
