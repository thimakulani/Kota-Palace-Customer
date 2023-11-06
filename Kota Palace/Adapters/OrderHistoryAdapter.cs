using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Chip;
using Google.Android.Material.TextView;
using Kota_Palace.Models;
using System;
using System.Collections.Generic;

namespace Kota_Palace.Adapters
{
    internal class OrderHistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<OrderHistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<OrderHistoryAdapterClickEventArgs> ItemLongClick;
        private readonly List<Order>items = new List<Order>();

        public OrderHistoryAdapter(List<Order> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.order_history_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new OrderHistoryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
           // var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as OrderHistoryAdapterViewHolder;
            //holder.TextView.Text = items[position];

            decimal price = 0;

            foreach (var item in items[position].OrderItems)
            {
                Chip chip = new Chip(holder.ItemView.Context)
                {
                    Text = $"{item.ItemName.ToUpper()}(*{item.Quantity})",
                    Checkable = false
                };
                price += (item.Price * item.Quantity);
                holder.Items.AddView(chip);
            }

            //fill in your items
            holder.Quantity.Text = $"QUANTITY: {items[position].OrderItems.Count + 1}";
            holder.Price.Text = $"PRICE:R{price}";
            holder.Status.Text = $"ORDER STATUS: {items[position].Status}".ToUpper();
            holder.ItemName.Text = $"ORDER NUMBER {items[position].Id}";

        }

        public override int ItemCount => items.Count;

        void OnClick(OrderHistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(OrderHistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class OrderHistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }

        public MaterialTextView Quantity { get; set; }
        public MaterialTextView Price { get; set; }
        public MaterialTextView Status { get; set; }
        public MaterialTextView ItemName { get; set; }
        public ChipGroup Items { get; set; }
        public OrderHistoryAdapterViewHolder(View itemView, Action<OrderHistoryAdapterClickEventArgs> clickListener,
                            Action<OrderHistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            Quantity = itemView.FindViewById<MaterialTextView>(Resource.Id.order_history_item_quantity);
            Status = itemView.FindViewById<MaterialTextView>(Resource.Id.order_hietory_item_status);
            Items = itemView.FindViewById<ChipGroup>(Resource.Id.order_hietory_item_products);
            Price = itemView.FindViewById<MaterialTextView>(Resource.Id.order_history_item_total_price);
            ItemName = itemView.FindViewById<MaterialTextView>(Resource.Id.order_history_item_name);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new OrderHistoryAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new OrderHistoryAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class OrderHistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}