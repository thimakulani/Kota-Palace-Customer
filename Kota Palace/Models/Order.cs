using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kota_Palace.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public int BusinessId { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
        public DateTime OrderDateUtc { get; set; }  
        public string Option { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DriverId { get; set; }
    }
    public class OrderItems
    {
        public int Id { get; set; }
        public int Quantity { get; set; } //
        public string ItemName { set; get; }//product id
        public decimal Price { get; set; }
        public string Extras { get; set; }
    }

}