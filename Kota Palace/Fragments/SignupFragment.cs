using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using Java.Util;
using Kota_Palace.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Kota_Palace.Fragments
{
    public class SignupFragment : AndroidX.Fragment.App.Fragment//, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
    {
        
        private TextInputEditText InputName;
        private TextInputEditText InputLast;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPhone;
        private TextInputEditText InputPassword;
        private MaterialButton BtnSignup;
        private MaterialButton BtnSignIn;

     


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.signup_frag, container, false);
            ConnectingViews(view);
            context = view.Context;
            return view;
        }

        private void ConnectingViews(View view)
        {
            BtnSignIn = view.FindViewById<MaterialButton>(Resource.Id.BtnSignIn);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.SignupInputNames);
            InputLast = view.FindViewById<TextInputEditText>(Resource.Id.SignupInputLastName);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.SignupInputEmail);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.SignupInputPhoneNr);
            InputPassword = view.FindViewById<TextInputEditText>(Resource.Id.SignupInputPassword);
            BtnSignup = view.FindViewById<MaterialButton>(Resource.Id.Register);

            BtnSignup.Click += BtnSignup_Click;


            BtnSignIn.Click += BtnSignIn_Click;
        }

        public event EventHandler BtnLoginHandler;
        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            BtnLoginHandler(sender, e);
        }

        
        private IonAlert loadingDialog;
        private async void BtnSignup_Click(object sender, EventArgs e)
        {

            if (IsValid())
            {
                loadingDialog = new IonAlert(context, IonAlert.ProgressType);
                loadingDialog.SetSpinKit("DoubleBounce")
                    .ShowCancelButton(false)
                    .Show();

                try
                {
                    UserRegister userLogin = new UserRegister()
                    {
                        Email = InputEmail.Text.Trim(),
                        Password = InputPassword.Text.Trim(),
                        Firstname = InputName.Text.Trim(),
                        Lastname = InputLast.Text.Trim(),
                        PhoneNumber = InputPhone.Text.Trim(),
                        Url = null
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);

                    HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    HttpClient httpClient = new HttpClient();
                    var response = await httpClient.PostAsync($"{API.ApiUrl}/account/signup", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        AndHUD.Shared.ShowSuccess(context, "Your account has been successfully created", MaskType.Clear, TimeSpan.FromSeconds(3));
                        BtnSignIn.PerformClick();
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        AndHUD.Shared.ShowError(context, result, MaskType.Clear, TimeSpan.FromSeconds(3));
                    }


                }
                catch (Exception ex)
                {

                    AndHUD.Shared.ShowError(context, ex.Message, MaskType.Clear, TimeSpan.FromSeconds(3));

                }
                finally
                {
                    loadingDialog.Dismiss();
                }
            }
        }
        
        private bool IsValid()
        {
            bool valid = true;
            if (string.IsNullOrEmpty(InputName.Text))
            {
                InputName.Error = "provide your name";
                valid = false;
            }
            if (string.IsNullOrEmpty(InputLast.Text))
            {
                InputLast.Error = "provide your last name";
                valid = false;
            }
            if (string.IsNullOrEmpty(InputPhone.Text))
            {
                InputPhone.Error = "provide your phone number";
                valid = false;

            }
            if (string.IsNullOrEmpty(InputEmail.Text))
            {
                InputEmail.Error = "provide your email";
                valid = false;
            }
            if (string.IsNullOrEmpty(InputPassword.Text))
            {
                InputPassword.Error = "provide your password";
                valid = false;
            }
           
            
            return valid;
        }

   
    }

    public class Customer
    {
        public string Names { get;  set; }
        public string Phone { get;  set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}