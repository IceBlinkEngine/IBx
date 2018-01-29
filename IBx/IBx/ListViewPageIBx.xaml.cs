using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IBx
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPageIBx : ContentPage
    {
        public List<string> Items { get; set; }
        public TaskCompletionSource<string> tcs;

        public ListViewPageIBx(TaskCompletionSource<string> returnString, List<string> list, string headerText)
        {
            InitializeComponent();

            tcs = returnString;
            Items = list;

            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            var result = e.Item.ToString();
            await Navigation.PopModalAsync();
            // pass result
            tcs.SetResult(result);
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
