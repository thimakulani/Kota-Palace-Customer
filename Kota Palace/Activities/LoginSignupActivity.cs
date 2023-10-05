using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Kota_Palace.Fragments;
using System;

namespace Kota_Palace.Activities
{
    [Activity(Label = "Landing", MainLauncher = false)]
    public class LoginSignupActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.landing_page);
            LoginFragment frag = new LoginFragment();
            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations(Resource.Animation.Side_in_left,
                Resource.Animation.Side_out_right, Resource.Animation.Side_in_right,
                Resource.Animation.Side_out_left)
                .Add(Resource.Id.fragLandingPageHost, frag)
                .Commit();
            frag.BtnNav += Frag_BtnNav;
        }

        private void Frag_BtnNav(object sender, LoginFragment.NavEventHandler e)
        {
            SignupFragment frag = new SignupFragment();
            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left, Resource.Animation.Side_in_left, Resource.Animation.Side_out_right)
                .Replace(Resource.Id.fragLandingPageHost, frag)
                // .AddSharedElement(e.Btn, e.Btn.TransitionName)
                .Commit();
            frag.BtnLoginHandler += Frag_BtnLoginHandler;
        }

        private void Frag_BtnLoginHandler(object sender, EventArgs e)
        {
            LoginFragment frag = new LoginFragment();
            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations(Resource.Animation.Side_in_left,
                Resource.Animation.Side_out_right, Resource.Animation.Side_in_right,
                Resource.Animation.Side_out_left)
                .Replace(Resource.Id.fragLandingPageHost, frag)
                .Commit();
            frag.BtnNav += Frag_BtnNav;
        }
    }
}
public class UserLogin
{

    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserRegister
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PhoneNumber { get; set; }
    public string Url { get; set; }
}