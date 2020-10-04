using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAWS.Models.Entity
{
    public class Product
    {
        public Product( string name, int quantity, DateTime expireDate, double price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Quantity = quantity;
            ExpireDate = expireDate;
            Price = price;
        }
        public Product()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }        
        public string Image { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpireDate { get; set; }
        public double Price { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category{ get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        public string TopicArn { get; set; }

    }
}
