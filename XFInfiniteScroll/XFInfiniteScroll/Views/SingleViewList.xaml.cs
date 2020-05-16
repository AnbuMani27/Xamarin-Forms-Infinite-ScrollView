using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Xamarin.Forms.Xaml;
using XFInfiniteScroll.FakeDataSource;
using XFInfiniteScroll.Models;

namespace XFInfiniteScroll.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleViewList : ContentPage
    {
        private const int PageSize = 20;
        public SingleViewList()
        {
            InitializeComponent();
            Items = new InfiniteScrollCollection<Items>
            {
                OnLoadMore = async () =>
                {
                    // load the next page
                    var page = Items.Count / PageSize;
                    var items = await DataItems.GetItemsAsync(page, PageSize);
                    IsWorking = false;
                    return items;
                }
            };
            // load the initial data
            loadDataAsync();
        }
     

        public InfiniteScrollCollection<Items> Items { get; set; }

        public bool IsWorking
        {
            get; set;
        }


        private async Task loadDataAsync()
        {
            var items = await DataItems.GetItemsAsync(pageIndex: 0, pageSize: PageSize);

            Items.AddRange(items);
            ListsingleItems.ItemsSource = Items;

        }
    }
}
