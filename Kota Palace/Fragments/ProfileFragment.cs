using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using FFImageLoading;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Kota_Palace.Models;
using Refractored.Controls;
using System;
using System.Net.Http;
using Xamarin.Essentials;

namespace Kota_Palace.Fragments
{
    public class ProfileFragment : Fragment
    {
        private AppCompatImageView img_edit;
        private CircleImageView profile_image;

        public TextInputEditText InputName;
        public TextInputEditText InputLastName;
        public TextInputEditText InputPhone;
        public TextInputEditText InputEmail;
        public MaterialTextView TxtUserId;
        public MaterialButton Btn_update;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.profile_fragment, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }
        private string id;
        private Context context;
        private async void ConnectViews(View view)
        {

            id = Preferences.Get("Id", null);
            img_edit = view.FindViewById<AppCompatImageView>(Resource.Id.img_edit);
            profile_image = view.FindViewById<CircleImageView>(Resource.Id.profile_image);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.InputName);
            InputLastName = view.FindViewById<TextInputEditText>(Resource.Id.InputLastName);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.InputPhone);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.InputEmail);
            Btn_update = view.FindViewById<MaterialButton>(Resource.Id.btn_update);
            TxtUserId = view.FindViewById<MaterialTextView>(Resource.Id.TxtUserId);
            InputEmail.Enabled = false;
            ViewState(false);
            img_edit.Click += (e, s) =>
            {
                ViewState(true);
            };
            Btn_update.Click += Btn_update_Click;
            try
            {

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{API.ApiUrl}/account/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var str_resp = await response.Content.ReadAsStringAsync();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(str_resp);
                    InputName.Text = data.Firstname;
                    InputLastName.Text = data.Lastname;
                    InputPhone.Text = data.PhoneNumber;
                    InputEmail.Text = data.Email;
                    TxtUserId.Text = data.Id;
                    await ImageService.Instance
                        .LoadUrl(data.Url)
                        .DownSampleInDip(512, 512)
                        .IntoAsync(profile_image);
                }
                else
                {
                    var str_resp = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, str_resp, MaskType.Black, TimeSpan.FromSeconds(2));
                }
            }
            catch (HttpRequestException ex)
            {

                AndHUD.Shared.ShowSuccess(context, ex.Message, MaskType.Black, TimeSpan.FromSeconds(2));
            }
        }

        private async void Btn_update_Click(object sender, EventArgs e)
        {
            try
            {
                UserRegister userLogin = new UserRegister()
                {
                    Email = InputEmail.Text.Trim(),
                    Firstname = InputName.Text.Trim(),
                    Lastname = InputLastName.Text.Trim(),
                    PhoneNumber = InputPhone.Text.Trim(),
                    Url = null
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);

                HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PutAsync($"{API.ApiUrl}/account/update/{id}", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var str_resp = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, str_resp, MaskType.Black, TimeSpan.FromSeconds(2));
                }
                else
                {
                    var str_resp = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, str_resp, MaskType.Black, TimeSpan.FromSeconds(2));
                }
            }
            catch (HttpRequestException ex)
            {

                AndHUD.Shared.ShowSuccess(context, ex.Message, MaskType.Black, TimeSpan.FromSeconds(2));
            }

        }

        private void ViewState(bool v)
        {
            //InputEmail.Enabled = v;
            InputName.Enabled = v;
            InputLastName.Enabled = v;
            InputPhone.Enabled = v;
            if (v)
            {
                Btn_update.Visibility = ViewStates.Visible;
                img_edit.Visibility = ViewStates.Gone;
            }
            else
            {
                Btn_update.Visibility = ViewStates.Gone;
                img_edit.Visibility = ViewStates.Visible;
            }
        }

    }
}