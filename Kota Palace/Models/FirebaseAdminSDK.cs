using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kota_Palace.Models
{
    public static class FirebaseAdminSDK
    {
        public static FirebaseMessaging GetFirebaseMessaging(Stream input)
        {
            FirebaseMessaging messaging;

            FirebaseAdmin.AppOptions options = new FirebaseAdmin.AppOptions()
            {
                Credential = GoogleCredential.FromStream(input),
                ProjectId = "local-kota-15d60",
                ServiceAccountId = "firebase-adminsdk-6p3vo@local-kota-15d60.iam.gserviceaccount.com",

            };
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                var app = FirebaseAdmin.FirebaseApp.Create(options);
                messaging = FirebaseMessaging.GetMessaging(app);
            }
            else
            {
                messaging = FirebaseMessaging.DefaultInstance;//.FirebaseAuth.DefaultInstance;
            }

            return messaging;
        }
    }
}