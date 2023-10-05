using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using FloatingActionButton = Google.Android.Material.FloatingActionButton.FloatingActionButton;

namespace Kota_Palace.Adapters
{
    class FabAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MenuAdapterClickEventArgs> ItemClick;
        public event EventHandler<MenuAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<MenuAdapterClickEventArgs> FabClick;
        int[] items;

        public FabAdapter(int[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.fab_menu_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new MenuAdapterViewHolder(itemView, OnClick, OnLongClick, OnFabClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        private string[] color = { "#6877cd", "#cdbd68", "#68a9cd", "#8b68cd", "#bd68cd" };
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MenuAdapterViewHolder;
            holder.Fab.SetImageResource(items[position]);
            holder.Fab.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(color[position]));
            //holder.TextView.Text = items[position];
        }

        public override int ItemCount => items.Length;

        void OnFabClick(MenuAdapterClickEventArgs args) => FabClick?.Invoke(this, args);
        void OnClick(MenuAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MenuAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MenuAdapterViewHolder : RecyclerView.ViewHolder
    {
        public FloatingActionButton Fab { get; set; }


        public MenuAdapterViewHolder(View itemView, Action<MenuAdapterClickEventArgs> clickListener,
                            Action<MenuAdapterClickEventArgs> longClickListener, Action<MenuAdapterClickEventArgs> fabClickListener) : base(itemView)
        {
            //TextView = v;
            Fab = itemView.FindViewById<FloatingActionButton>(Resource.Id.RowFab);

            Fab.Click += (sender, e) => fabClickListener(new MenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new MenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }


    }

    public class MenuAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}