using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.Chip;
using Google.Android.Material.Internal;
using Google.Android.Material.TextView;
using Kota_Palace.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using static Xamarin.Io.OpenCensus.Metrics.Export.Summary;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;

namespace Kota_Palace.Adapters
{
    public class CartAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CartAdapterClickEventArgs> ItemClick;
        public event EventHandler<CartAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<CartAdapterClickEventArgs> ItemOptionClick;
        private readonly List<CartModel> Items = new List<CartModel>();
        

        public CartAdapter(List<CartModel> data)
        {
            Items = data;
            
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.cart_items;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new CartAdapterViewHolder(itemView, OnClick, OnLongClick, OnOptionClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public  override  void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];
            // Replace the contents of the view with that element
            var holder = viewHolder as CartAdapterViewHolder;


            var menu = Items[position];
            holder.TxtName.Text = menu.ItemName.ToUpper();
            holder.TxtCounter.Text = Items[position].Quantity.ToString().ToUpper();
            double price = (double)(menu.Price * Items[position].Quantity);
            holder.TxtPrice.Text = $"R{price}";
            if (string.IsNullOrEmpty(Items[position].Note))
            {
                holder.TxeNote.Text = Items[position].Note.ToUpper();
            }

            holder.ChipAddOns.RemoveAllViews();
            var inflator = LayoutInflater.From(holder.ItemView.Context);
            if (Items[position].Extras != null)
            {
                var extras = Items[position].Extras.Split("#");
                foreach (var item in extras)
                {
                    //holder.txtAddOns.Text += item.Value + ", ";
                    var chip = inflator.Inflate(Resource.Layout.chip_item, null, false) as Chip;
                    chip.Clickable = false; 
                    
                    chip.Text = $"{item}".ToUpper();
                    holder.ChipAddOns.AddView(chip);
                }
            }
            holder.ChipAddOns.Enabled = false;

        }

        public override int ItemCount => Items.Count;

        void OnClick(CartAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnOptionClick(CartAdapterClickEventArgs args) => ItemOptionClick?.Invoke(this, args);
        void OnLongClick(CartAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class CartAdapterViewHolder : RecyclerView.ViewHolder
    {
        public MaterialTextView TxtName { get; set; }
        public MaterialTextView TxeNote { get; set; }
        public MaterialTextView TxtPrice { get; set; }
        public MaterialTextView TxtCounter { get; set; }
        public ChipGroup ChipAddOns { get; set; }
        public CheckableImageButton BtnMore { get; set; }


        public CartAdapterViewHolder(View itemView, Action<CartAdapterClickEventArgs> clickListener,
                            Action<CartAdapterClickEventArgs> longClickListener,
                            Action<CartAdapterClickEventArgs> optionClickListener) : base(itemView)
        {
            //TextView = v;

            TxtCounter = itemView.FindViewById<MaterialTextView>(Resource.Id.cart_item_count);
            TxeNote = itemView.FindViewById<MaterialTextView>(Resource.Id.cart_item_noted);
            TxtName = itemView.FindViewById<MaterialTextView>(Resource.Id.cart_item_name);
            TxtPrice = itemView.FindViewById<MaterialTextView>(Resource.Id.cart_item_price);
            BtnMore = itemView.FindViewById<CheckableImageButton>(Resource.Id.cart_btn_more);
            ChipAddOns = itemView.FindViewById<ChipGroup>(Resource.Id.cart_row_add_ons);
            BtnMore.Click += (sender, e) => optionClickListener(new CartAdapterClickEventArgs { View = itemView, Btn = BtnMore, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new CartAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new CartAdapterClickEventArgs { View = itemView,Position = AbsoluteAdapterPosition });
        }
    }

    public class CartAdapterClickEventArgs : EventArgs
    {
        public CheckableImageButton Btn { get; set; }

        public View View { get; set; }
        public int Position { get; set; }
    }
}