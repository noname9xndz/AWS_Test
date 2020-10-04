using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoMapper;
using DemoAWS.Models;
using DemoAWS.Models.ViewModels;
using DemoAWS.Services.AWS_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DemoAWS.Controllers
{
    public class AWSSNSController : Controller
    {

        private IAWS_SNSService _aWS_SNSService;
        private IAWS_SQSService _aWS_SQSService;
        private static BasicAWSCredentials credentials = new BasicAWSCredentials("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");
        private static AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.USWest2);
        private readonly IMapper _mapper;

        public AWSSNSController(IAWS_SNSService aWS_SNSService, IAWS_SQSService aWS_SQSService, IMapper mapper)
        {
            _aWS_SNSService = aWS_SNSService;
            _aWS_SQSService = aWS_SQSService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var lstTopics = await client.ListTopicsAsync(new ListTopicsRequest());
            var model=_mapper.Map<List<TopicViewModel>>(lstTopics.Topics);                        
            return View("Index",model);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TopicViewModel topicViewModel)
        {
            if (ModelState.IsValid)
            {
               var lstTopics= await _aWS_SNSService.CreateTopic(client, topicViewModel.TopicName);
                return RedirectToAction("Index");
            }
            return View(topicViewModel);
        }
        public async Task<IActionResult> DeleteTopic(string Id)
        {
            await _aWS_SNSService.DeleteTopic(client, Id);
            var lstTopics = await client.ListTopicsAsync(new ListTopicsRequest());
            return View("Index");
        }
        public async Task<IActionResult> ListSubscriptionOfTopic(string Id)
        {
            //
            var subs = await client.ListSubscriptionsByTopicAsync(
                new ListSubscriptionsByTopicRequest
                {
                    TopicArn = Id
                });

            var ss = subs.Subscriptions;
            return View("ListSubscriptionOfTopic", ss);
        }
        [HttpGet]
        public async Task<IActionResult> CreateSubscription(string Id)
        {
            ViewBag.TopicArn = Id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubscription(TopicViewModel topicViewModel)
        {            

            await _aWS_SNSService.SubcribesTopic(client, topicViewModel.Id, topicViewModel.SubcriptionEmail);
            return RedirectToAction("ListSubscriptionOfTopic", "AWSSNS", new { Id= topicViewModel.Id });
        }
    }
}
