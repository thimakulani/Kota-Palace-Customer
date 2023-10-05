using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using ID.IonBit.IonAlertLib;
using Kota_Palace.Dialogs;
using Kota_Palace.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace Kota_Palace.Fragments
{
    public class LoginFragment : AndroidX.Fragment.App.Fragment //, IOnSuccessListener, IOnCompleteListener, IOnFailureListener
    {
        private MaterialButton BtnLogin;
        private TextView TxtCreateAccount;
        private TextView ForgotPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPassword;




        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.login_frag, container, false);
            ConnectingViews(view);

            return view;
        }
        private Context context;
        private void ConnectingViews(View view)
        {
            context = view.Context;
            BtnLogin = view.FindViewById<MaterialButton>(Resource.Id.BtnLogin);
            TxtCreateAccount = view.FindViewById<TextView>(Resource.Id.TxtCreateAccount);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.LoginInputEmail);
            InputPassword = view.FindViewById<TextInputEditText>(Resource.Id.LoginInputPassword);
            ForgotPassword = view.FindViewById<MaterialTextView>(Resource.Id.TxtForgotPassword);
            BtnLogin.Click += BtnLogin_Click;


            TxtCreateAccount.Click += (s, e) =>
            {
                BtnNav.Invoke(this, new NavEventHandler() { Id = 1 });
            };
            ForgotPassword.Click += (s, e) =>
            {
                ForgotPasswordDlgFragment forgotPasswordDlgFragment = new ForgotPasswordDlgFragment();
                forgotPasswordDlgFragment.Show(ChildFragmentManager.BeginTransaction(), "");
            };


        }




        public event EventHandler<NavEventHandler> BtnNav;
        public class NavEventHandler : EventArgs
        {
            public int Id { get; set; }
        }

        private IonAlert loadingDialog;
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                loadingDialog = new IonAlert(context, IonAlert.ProgressType);
                loadingDialog.SetSpinKit("DoubleBounce")
                    .ShowCancelButton(false)
                    .Show();
                try
                {

                    UserLogin userLogin = new UserLogin()
                    {
                        Email = InputEmail.Text.Trim(),
                        Password = InputPassword.Text.Trim(),
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);

                    using (var httpClient = new HttpClient())
                    {
                        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync($"{API.ApiUrl}/account/login", httpContent);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var user = JsonConvert.DeserializeObject<User>(result);

                            // Store user data in preferences
                            Preferences.Set("Id", user.Id);
                            Preferences.Set("e", user.Email);
                            Preferences.Set("p", userLogin.Password);

                            // Start the HomeActivity
                            var intent = new Intent(Application.Context, typeof(HomeActivity));
                            StartActivity(intent);
                        }
                        else
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            AndHUD.Shared.ShowError(context, result, MaskType.Clear, TimeSpan.FromSeconds(3));
                        }
                    };
                 


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
            if (string.IsNullOrEmpty(InputEmail.Text))
            {
                valid = false;
                InputEmail.Error = "provide your email";
            }
            if (string.IsNullOrEmpty(InputPassword.Text))
            {
                valid = false;
                InputPassword.Error = "provide your password";
            }
            return valid;
        }



       
    }
}