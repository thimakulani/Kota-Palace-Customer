using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CloudFirestore.Attributes;

namespace Kota_Palace.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public string Customer_Id { get; set; }
        public string Status { get; set; }
        public int BusinessId { get; set; }
        //cartiems
        public int Quantity { get; set; } //
        public string ItemName { get; set; }
        public string Note { get; set; }
        public decimal Price { get; set; }
        //cart extras
        public string Extras { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } //
        public int CartId { set; get; }//
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string Extras { get; set; }
        public string Note { get; set; }
    }

    public class CartItemsExtras
    {
        public int Id { set; get; }
        public string Title { get; set; }
    }
}
