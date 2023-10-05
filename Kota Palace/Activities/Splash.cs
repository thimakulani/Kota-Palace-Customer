using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Initialization;
using Android.OS;
using Android.Runtime;
using AndroidHUD;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Kota_Palace.Activities;
using Kota_Palace.Models;
using Plugin.CloudFirestore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Kota_Palace
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class Splash : AppCompatActivity, IOnInitializationCompleteListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                string e = Preferences.Get("e", null);
                string p = Preferences.Get("p", null);
                try
                {
                    if (string.IsNullOrWhiteSpace(e) || string.IsNullOrWhiteSpace(p))
                    {
                        Intent intent = new Intent(Application.Context, typeof(LoginSignupActivity));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                        return;
                    }
                    UserLogin userLogin = new UserLogin()
                    {
                        Email = e,
                        Password = p,
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);



                    HttpClient httpClient = new HttpClient();
                    HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{API.ApiUrl}/account/login", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        Intent intent = new Intent(Application.Context, typeof(HomeActivity));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    }
                    else
                    {
                        Intent intent = new Intent(Application.Context, typeof(LoginSignupActivity));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    }
                }
                catch (Exception)
                {
                    Intent intent = new Intent(Application.Context, typeof(LoginSignupActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                }

            });

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnInitializationComplete(IInitializationStatus p0)
        {

        }
    }
}