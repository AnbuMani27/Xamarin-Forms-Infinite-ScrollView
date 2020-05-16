using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class GroupViewList : ContentPage
    {
        private int ListItemsCount = 0;
        private const int PageSize = 20;
        public InfiniteScrollCollection<ObservableGroupCollection<string, Items>> Items;
        public bool IsWorking
        {
            get; set;
        }

        public GroupViewList()
        {
            InitializeComponent();
            //BindingContext = new GroupListViewModel();
            Items = new InfiniteScrollCollection<ObservableGroupCollection<string, Items>>
            {
                OnLoadMore = async () =>
                {
                    // load the next page
                    IsWorking = true;
                    ListItemsCount = 0;
                    foreach (var _itemslist in Items)
                    {
                        ListItemsCount += _itemslist.Count;
                    }
                    double _pagecount = (double)ListItemsCount / PageSize;
                    var page = Convert.ToInt32(Math.Ceiling(_pagecount));
                    var items = await DataItems.GetItemsAsync(page, PageSize);
                    var _items = items.GroupBy(e => e.Title).Select(e => new ObservableGroupCollection<string, Items>(e)).ToList();
                    if (_items.Count > 0)
                    {
                        foreach (var getGroupItems in _items.GroupBy(i => i.Header).ToList())
                        {

                            var _GetExistingGroupItems = Items.LastOrDefault(i => i.Header == getGroupItems.Key);
                            if (_GetExistingGroupItems != null && _GetExistingGroupItems.Count > 0)
                            {
                                // this is an existing group, so add the items to that
                                foreach (var _ExistingGroupedItems in getGroupItems.ToList())
                                {
                                    foreach (var _GroupListitems in _ExistingGroupedItems.ToList())
                                    {
                                        _GetExistingGroupItems.Add(_GroupListitems);  //Update items
                                    }
                                }
                            }
                            else
                            {
                                //Add new Group
                                Items.AddRange(getGroupItems);
                                GroupItems.ItemsSource = Items;
                            }

                        }
                    }
                    
                    IsWorking = false; 
                    return null;
                }
            };
            loadDataAsync();
        }
        private async Task loadDataAsync()
        {
            IsWorking = true;
            var items = await DataItems.GetItemsAsync(pageIndex: 0, pageSize: PageSize);
            var groupItems = items.GroupBy(e => e.Title).Select(e => new ObservableGroupCollection<string, Items>(e)).ToList();
            Items.AddRange(groupItems);
            OnPropertyChanged("Items");
            GroupItems.ItemsSource = Items;
            IsWorking = false;

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class ObservableGroupCollection<K, T> : ObservableCollection<T>
        {
            private readonly K _header;

            public ObservableGroupCollection(IGrouping<K, T> group)
                : base(group)
            {
                _header = group.Key;
            }

            public K Header
            {
                get { return _header; }
            }
        }
    }
}
