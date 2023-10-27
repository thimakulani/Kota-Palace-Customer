using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Kota_Palace.Models;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using static Android.Resource;

namespace Kota_Palace.Adapters
{
    public class SpazaAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ShopsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ShopsAdapterClickEventArgs> ItemLongClick;
        private readonly List<SpazaModel> Items = new List<SpazaModel>();

        public SpazaAdapter(List<SpazaModel> data)
        {
            Items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.spaza_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new ShopsAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as ShopsAdapterViewHolder;
            holder.TxtName.Text = Items[position].Name.ToUpper();
            if (Items[position].Online == "ONLINE")
            {
                holder.TxtDescription.Text = Items[position].Online.ToUpper();
            }
            else
            {
                holder.ItemView.Enabled = false;
                holder.TxtDescription.Text = "OFFLINE".ToUpper();
            }
            if (!string.IsNullOrEmpty(Items[position].ImgUrl))
            {
                holder.ImgIcon.SetScaleType(ImageView.ScaleType.CenterCrop);
                ImageService.Instance.LoadUrl(Items[position].ImgUrl)
                    .DownSampleInDip(512, 512)
                    .Retry(3, 500)
                    .Into(holder.ImgIcon);
            }
            else
            {
                ImageService.Instance.LoadUrl("https://static.wikia.nocookie.net/p__/images/b/b8/The_Amazing_World_of_Gumball_logo.png/revision/latest/scale-to-width-down/1000?cb=20220713233005&path-prefix=protagonist")
                    .DownSampleInDip(512, 512)
                    .Retry(3, 500)
                    .Into(holder.ImgIcon);
                //holder.imgIcon.SetImageResource(Resource.Mipmap.logo_food);
            }
            ViewAnimationUtils(holder.ItemView, position);
            /*if (Items[position].Coordinates !=null )
            {
                var latlang = Items[position].Coordinates.Split('/');
                double latitude = double.Parse(latlang[0].Trim());
                double longitude = double.Parse(latlang[1].Trim());
                try
                {

                    var currentLocation = await Geolocation.GetLocationAsync();
                    double distance = -1;
                    distance = LocationExtensions.CalculateDistance(currentLocation, latitude, longitude, DistanceUnits.Kilometers);
                    holder.TxtLocation.Text = $"{Items[position].BusinessAddress} [{Math.Round(distance, 2)} KM away]".ToUpper();
                    holder.Load_progress.Visibility = ViewStates.Gone;
                    holder.Img_location.Visibility = ViewStates.Visible;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(holder.ItemView.Context, ex.Message.ToUpper(), ToastLength.Long).Show();
                }

            }*/

        }
        private void ViewAnimationUtils(View itemView, int position)
        {
            if (position % 2 == 0)
            {
                var animation = AnimationUtils.LoadAnimation(itemView.Context, Resource.Animation.Side_in_right);
                itemView.StartAnimation(animation);
            }
            else
            {
                var animation = AnimationUtils.LoadAnimation(itemView.Context, Resource.Animation.Side_in_left);
                itemView.StartAnimation(animation);
            }
        }

        public override int ItemCount => Items.Count;

        void OnClick(ShopsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ShopsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }
    public class ShopsAdapterViewHolder : RecyclerView.ViewHolder
    {

        public TextView TxtDescription { get; set; }
        public TextView TxtName { get; set; }
        public TextView TxtLocation { get; set; }
        public AppCompatImageView ImgIcon { get; set; }
        public AppCompatImageView Img_location { get; set; }
        public ProgressBar Load_progress { get; set; }
        public ShopsAdapterViewHolder(View itemView, Action<ShopsAdapterClickEventArgs> clickListener,
                            Action<ShopsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            TxtDescription = itemView.FindViewById<TextView>(Resource.Id.RowtxtDescription);
            TxtName = itemView.FindViewById<TextView>(Resource.Id.RowtxtSpazaName);
            TxtLocation = itemView.FindViewById<TextView>(Resource.Id.RowtxtSpazaAddress);
            ImgIcon = itemView.FindViewById<AppCompatImageView>(Resource.Id.RowSpazaImg);
            Img_location = itemView.FindViewById<AppCompatImageView>(Resource.Id.img_location);
            Load_progress = itemView.FindViewById<ProgressBar>(Resource.Id.load_progress);

            itemView.Click += (sender, e) => clickListener(new ShopsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ShopsAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }
    public class ShopsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}