using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using Kota_Palace.Frag_Tabs;

namespace Kota_Palace.Fragments
{
    public class OrderFragment : Fragment
    {
        private ViewPager2 viewpager;
        private TabLayout tabLayout;
        private TabsAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.order_fragment, container, false);

            viewpager = view.FindViewById<ViewPager2>(Resource.Id.PagerHost);
            tabLayout = view.FindViewById<TabLayout>(Resource.Id.TabHost);

            adapter = new TabsAdapter(this);
            adapter.AddFragment(new TabOrder(), "ORDER");
            adapter.AddFragment(new TabOrderHistory(), "HISTORY");

            viewpager.Adapter = adapter;

            new TabLayoutMediator(tabLayout, viewpager, new TabLayoutMediatorCallback(adapter)).Attach();

            return view;
        }
    }

    public class TabsAdapter : FragmentStateAdapter
    {
        private readonly List<Fragment> fragments = new List<Fragment>();
        private readonly List<string> fragmentTitles = new List<string>();

        public TabsAdapter(Fragment parentFragment) : base(parentFragment)
        {
            
        }

        public void AddFragment(Fragment fragment, string title)
        {
            fragments.Add(fragment);
            fragmentTitles.Add(title);
        }

        public override int ItemCount => fragments.Count;

        public override Fragment CreateFragment(int position)
        {
            return fragments[position];
        }

        public string GetFragmentTitle(int position)
        {
            return fragmentTitles[position];
        }
    }

    public class TabLayoutMediatorCallback : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
    {
        private readonly TabsAdapter adapter;

        public TabLayoutMediatorCallback(TabsAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(adapter.GetFragmentTitle(position));
        }
    }
}
