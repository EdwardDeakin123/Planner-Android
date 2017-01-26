using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Front_End.Models;

namespace Front_End
{
    // This class handles the custom layout in the navigation drawer's list view.
    class MenuAdapter : BaseAdapter<MenuItemModel>
    {
        List<MenuItemModel> _Items;
        Activity _Context;

        public MenuAdapter(Activity context, List<MenuItemModel> items) 
            : base()
        {
            _Items = items;
            _Context = context;
        }

        public override MenuItemModel this[int position]
        {
            get
            {
                return _Items[position];
            }
        }

        public override int Count
        {
            get
            {
                return _Items.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MenuItemModel item = _Items[position];
            View view = convertView;

            if(view == null)
            {
                // The view hasn't been created yet.
                view = _Context.LayoutInflater.Inflate(Resource.Layout.MenuListItem, null);
            }

            // Assign the item's data to the view.
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Text;
            view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResource);

            return view;
        }
    }
}