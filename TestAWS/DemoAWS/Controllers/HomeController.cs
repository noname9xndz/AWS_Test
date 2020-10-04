using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DemoAWS.Models;
using Amazon.S3;
using DemoAWS.Models.Entity;

namespace DemoAWS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        List<Product> products = new List<Product>(){
            new Product("Iphone 10",10,DateTime.Parse("2020-9-9"),1000),
            new Product("Samsung Note 10", 10, DateTime.Parse("2020-9-9"), 690),
            new Product("Iphone 8", 10, DateTime.Parse("2020-9-9"), 550)
            };
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            //S3Client = s3Client;
        }

     
       

        public IActionResult Index()
        {                        
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
