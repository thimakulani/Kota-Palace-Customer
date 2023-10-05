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
    public class User
    {
            public string Id { get; set; } 
            public string Url { get; set; } 
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string UserType { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool PhoneNumberConfirmed { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public object LockoutEnd { get; set; }
            public long AccessFailedCount { get; set; }
    }
}