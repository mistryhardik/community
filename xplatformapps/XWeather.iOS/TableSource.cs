using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;

namespace XWeather.iOS
{
    public class TableSource : UITableViewSource
    {
        string[] items;
        string cellId = "ForecastListCell";

        ViewController controller;

        public TableSource(string[] _items, ViewController _controller)
        {
            items = _items;
            controller = _controller;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellId);

            var item = items[indexPath.Row];

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellId);
            }

            cell.TextLabel.Text = item;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return items.Length;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            var item = items[indexPath.Row];

            controller.ShowForecastDetail(item);
        }
    }
}
