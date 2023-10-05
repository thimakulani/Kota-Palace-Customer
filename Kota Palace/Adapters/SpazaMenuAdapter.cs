using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Google.Android.Material.Button;
using Google.Android.Material.Chip;
using Kota_Palace.Models;
using System;
using System.Collections.Generic;

namespace Kota_Palace.Adapters
{
    public class SpazaMenuAdapter : RecyclerView.Adapter
    {
        public event EventHandler<SpazaMenuAdapterClickEventArgs> ItemClick;
        public event EventHandler<SpazaMenuAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<SpazaMenuAdapterClickEventArgs> ItemAddToCartClick;
        private readonly List<MenuModel> items = new List<MenuModel>();

        public SpazaMenuAdapter(List<MenuModel> data)
        {
            items = data;
        }
        public override int GetItemViewType(int position)
        {
            if (items.Count == 1) return 0; //show it full width
            else
            {
                if (items.Count % 2 == 0)
                    return 1;
                else
                    return (position > 1 && position == items.Count - 1) ? 0 : 1;
            }
        }
        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.menu_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new SpazaMenuAdapterViewHolder(itemView, OnClick, OnLongClick, OnAddToCartClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as SpazaMenuAdapterViewHolder;
            holder.TxtAddOns.Visibility = ViewStates.Gone;
            holder.ChipAddOns.RemoveAllViews();
            holder.ChipAddOns.Visibility = ViewStates.Gone;

            //var inflator = LayoutInflater.From(holder.ItemView.Context);
            //if (items[position].Extras != null)
            //{
            //    foreach (var item in items[position].Extras)
            //    {
            //        //holder.txtAddOns.Text += item.Value + ", ";
            //        var chip = inflator.Inflate(Resource.Layout.chip_item, null, false) as Chip;
            //        chip.Text = $"{item.Title}";
            //        holder.chipAddOns.AddView(chip);
            //    }
            //   //holder.txtAddOns.Text = holder.txtAddOns.Text.Substring(0, holder.txtAddOns.Text.Length-2);
            //}
            //holder.txtAddOns.Visibility = ViewStates.Gone;
            holder.TxtName.Text = items[position].Name.ToUpper();
             if(items[position].Status)
            {
                holder.TxtStatus.Text = "Available".ToUpper();
            }
            else
            {
                holder.TxtStatus.Text = "Not Available".ToUpper();
            }
            holder.TxtPrice.Text = $"R{items[position].Price}";
            holder.TxtStatus.Visibility = ViewStates.Gone;
            if (!string.IsNullOrEmpty(items[position].Url))
            {

                ImageService.Instance.LoadUrl(items[position].Url)
                    .Retry(3, 500)
                    .DownSample(200, 200)
                    .Into(holder.Img);
            }
            else
            {
                holder.Img.SetImageResource(Resource.Mipmap.logo_food);
            }
            holder.Img.Alpha = 0.6f;
            ViewAnimationUtils(holder.ItemView, position);
        }
        private void ViewAnimationUtils(View itemView, int position)
        {
            if (position % 2 == 1)
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
        public override int ItemCount => items.Count;

        void OnClick(SpazaMenuAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnAddToCartClick(SpazaMenuAdapterClickEventArgs args) => ItemAddToCartClick?.Invoke(this, args);
        void OnLongClick(SpazaMenuAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class SpazaMenuAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TxtName { get; set; }
        public TextView TxtStatus { get; set; }
        public TextView TxtPrice { get; set; }
        public TextView TxtAddOns { get; set; }
        public ImageView Img { get; set; }
        public ChipGroup ChipAddOns { get; set; }
        public MaterialButton BtnAddToCart { get; set; }


        public SpazaMenuAdapterViewHolder(View itemView, Action<SpazaMenuAdapterClickEventArgs> clickListener,
                            Action<SpazaMenuAdapterClickEventArgs> longClickListener,
                            Action<SpazaMenuAdapterClickEventArgs> addToCartClickListener) : base(itemView)
        {
            //TextView = v;

            TxtName = itemView.FindViewById<TextView>(Resource.Id.menu_row_name_of_spatlo);
            TxtStatus = itemView.FindViewById<TextView>(Resource.Id.menu_row_status);
            TxtPrice = itemView.FindViewById<TextView>(Resource.Id.menu_row_price);
            TxtAddOns = itemView.FindViewById<TextView>(Resource.Id.menu_row_add_ons);
            Img = itemView.FindViewById<ImageView>(Resource.Id.menu_row_img);
            BtnAddToCart = itemView.FindViewById<MaterialButton>(Resource.Id.menu_row_BtnAddToCart);
            ChipAddOns = itemView.FindViewById<ChipGroup>(Resource.Id.chipAddOns);
            BtnAddToCart.Click += (sender, e) => addToCartClickListener(new SpazaMenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new SpazaMenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new SpazaMenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class SpazaMenuAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}