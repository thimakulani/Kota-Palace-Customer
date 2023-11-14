using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Initialization;
using Android.OS;
using AndroidX.AppCompat.App;
using Firebase.Messaging;
using Google.Android.Material.AppBar;
using IsmaelDiVita.ChipNavigationLib;
using Kota_Palace.Activities;
using Kota_Palace.Dialogs;
using Kota_Palace.Fragments;
using System;
using Xamarin.Essentials;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Kota_Palace
{
    [Activity(Label = "@string/app_name", MainLauncher = false)]
    public class HomeActivity : AppCompatActivity, ChipNavigationBar.IOnItemSelectedListener
    {
        private ChipNavigationBar nav_menu;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MobileAds.Initialize(this, new MobileAdsInitCompleteCallback());
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_home);
            if (savedInstanceState == null)
            {
                HomeFragment frag = new HomeFragment();
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.FragHomeHost, frag).Commit();
                frag.SpazaClicked += Frag_SpazaClicked;
            }
            ConnectViews();
            var id = Preferences.Get("Id", null);
            FirebaseMessaging
                .Instance
                .SubscribeToTopic(id);
        }

        private void ConnectViews()
        {
            nav_menu = FindViewById<ChipNavigationBar>(Resource.Id.MenuNav);
            nav_menu.SetMenuResource(Resource.Menu.nav_menu);
            nav_menu.SetItemSelected(Resource.Id.nav_home);
            nav_menu.SetOnItemSelectedListener(this);
            var toolbar_main = FindViewById<MaterialToolbar>(Resource.Id.toolbar_main);

            toolbar_main.InflateMenu(Resource.Menu.top_app_bar);
            toolbar_main.MenuItemClick += (s, e) =>
            {
                if (e.Item.ItemId == Resource.Id.toolbar_nav_cart)
                {
                    CartDialog cartDialog = new CartDialog();
                    var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                    cartDialog.Show(fragmentTransaction, null);
                }
            };
        }





        private void ImgCart_Click(object sender, System.EventArgs e)
        {
            CartDialog cartDialog = new CartDialog();
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            cartDialog.Show(fragmentTransaction, null);
        }



        private void Frag_SpazaClicked(object sender, HomeFragment.SpazaSelectedClickHandler e)
        {
            Intent intent = new Intent(this, typeof(SpazaActivity));
            intent.PutExtra("SpazaKeyId", e.KeyId);
            StartActivity(intent);
            //ActivityOptions activityOptions = ActivityOptions.MakeSceneTransitionAnimation()
        }

        public void OnItemSelected(int id)
        {
            Fragment fragment = null;
            int enterAnim = Resource.Animation.fade_in_anim;
            int exitAnim = Resource.Animation.fade_out_anim;

            switch (id)
            {
                case Resource.Id.nav_home:
                    fragment = new HomeFragment();
                    ((HomeFragment)fragment).SpazaClicked += Frag_SpazaClicked;
                    break;
                case Resource.Id.nav_order:
                    fragment = new OrderFragment();
                    break;
                case Resource.Id.nav_profile:
                    fragment = new ProfileFragment();
                    break;
                case Resource.Id.nav_logout:
                    //Firebase.Auth.FirebaseAuth.Instance.SignOut();
                    Intent intent = new Intent(Application.Context, typeof(LoginSignupActivity));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    Preferences.Clear();
                    return; // No need to commit a fragment transaction in this case
            }

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                    .SetCustomAnimations(enterAnim, exitAnim)
                    .Replace(Resource.Id.FragHomeHost, fragment)
                    .Commit();
            }
        }

        private class MobileAdsInitCompleteCallback : Java.Lang.Object, IOnInitializationCompleteListener
        {
            public void OnInitializationComplete(IInitializationStatus p0)
            {
                Console.WriteLine(p0);
            }
        }
    }


}