using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoAWS.Models;
using DemoAWS.Models.Entity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DemoAWS.Services.AWS_Services.Interfaces;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon;
using System.Text;
using Amazon.SimpleNotificationService.Model;
using DemoAWS.Models.ViewModels;
using AutoMapper;

namespace DemoAWS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Context _context;
        public IWebHostEnvironment _env;
        private IAWS_SNSService _aWS_SNSService;
        private IAWS_SQSService _aWS_SQSService;
        private readonly IMapper _mapper;
        private static BasicAWSCredentials credentials = new BasicAWSCredentials("AKIARANRZP5YWCFMZ7G3", "8H1zw/nwYAY7APEuiXHbCIAPZn7OfWobmqRP6nSX");
        private static AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(credentials,RegionEndpoint.USWest2);

        public ProductsController(Context context, IWebHostEnvironment env, IAWS_SNSService aWS_SNSService, IAWS_SQSService aWS_SQSService, IMapper mapper)
        {
            _context = context;
            _env = env;
            _aWS_SNSService = aWS_SNSService;
            _aWS_SQSService = aWS_SQSService;
            _mapper = mapper;
        }



        // GET: Products
        public async Task<IActionResult> Index()
        {
            var context = _context.Products.Include(p => p.Category);
            return View(await context.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }            
            var lstTopics = await client.ListTopicsAsync(new ListTopicsRequest());
            var model = _mapper.Map<List<TopicViewModel>>(lstTopics.Topics);
            ViewData["TopicId"] = new SelectList(model, "Id", "TopicName");
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            StringBuilder message = new StringBuilder();
            if (product != null)
            {
                message.AppendLine($"Name: {product.Name} |");
                message.AppendLine($"Quantity: {product.Quantity} |");
                message.AppendLine($"ExpireDate: {product.ExpireDate} |");
                message.AppendLine($"Price: {product.Price} |");
                message.AppendLine($"Category: {product.Category.Name}");
            }            
            ViewBag.Mess = message.ToString();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        // 
        [HttpPost]
        public async Task<IActionResult> CreateSNSTopic()
        {
            string topicName = Request.Form["Name"].ToString();
            await _aWS_SNSService.CreateTopic(client, topicName);
            return RedirectToAction("Index");
        }
        //
        [HttpPost]
        public async Task<IActionResult> SendMessageToSNS(string publishMessage, Product product)
        {
            if (product == null) return NotFound();
            await _aWS_SNSService.PublishMessage(client, product.TopicArn, publishMessage);

            return RedirectToAction("Index");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Name,Quantity,ExpireDate,Price,CategoryId,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile?.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = $"{_env.WebRootPath}\\content\\images\\{product.ImageFile.FileName}"; //we are using Temp file name just for the example. Add your own file path. 
                    
                    //Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                        product.Image = product.ImageFile.FileName;
                    }
                }
                //
                product.Id = Guid.NewGuid();
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Image,Name,Quantity,ExpireDate,Price,CategoryId,ImageFile")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile?.Length > 0)
                    {
                        // full path to file in temp location
                        var filePath = $"{_env.WebRootPath}\\content\\images\\{product.ImageFile.FileName}"; 

                        //Path.Combine(Server.MapPath("~/Content/Images"), _FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(stream);
                            product.Image = product.ImageFile.FileName;
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        // SNS

    }
}
