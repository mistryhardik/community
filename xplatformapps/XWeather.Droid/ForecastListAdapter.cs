using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XWeather.Droid
{
    public class ForecastListAdapter : BaseAdapter<string>
    {
        string[] forecastItems;
        Context context;

        public ForecastListAdapter(Context context, string[] forecastItems)
        {
            this.context = context;
            this.forecastItems = forecastItems;
        }

        public override string this[int position]
        {
            get { return forecastItems[position]; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return forecastItems[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ForecastListAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ForecastListAdapterViewHolder;

            if (holder == null)
            {
                holder = new ForecastListAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.ForecastListItem, parent, false);
                holder.ForecastItemText = view.FindViewById<TextView>(Resource.Id.ForecastListItemTextView);
                view.Tag = holder;
            }


            //fill in your items
            holder.ForecastItemText.Text = forecastItems[position];

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return forecastItems.Length;
            }
        }
    }

    public class ForecastListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView ForecastItemText { get; set; }
    }
}