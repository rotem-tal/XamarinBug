using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        HttpClient client = new HttpClient();
        string dbLink = "https://zulital3.wixsite.com/tute-meshek-tal/_functions/xamarin";
        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            string[] idArray = await GetId();
            if (idArray.Length == 0)
            {
                if (await CreateId())
                {
                    idArray = await GetId();
                }
                else
                {
                    throw new Exception("Unable to create IDs");
                }
            }

            var response = await client.DeleteAsync(dbLink + idArray[0]);
            await DisplayAlert(response.StatusCode.ToString(),idArray[0], "ok");
        }

        private async Task<string[]> GetId()
        {
            List<RequestItem> Items = new List<RequestItem>();
            var response = await client.GetAsync(dbLink);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                content = content.Substring(9, content.Length - 10);

                Items = JsonConvert.DeserializeObject<List<RequestItem>>(content);
            }
            return Items.Select(item => item._id).ToArray();
        }
        private async Task<bool> CreateId(int quantity = 10)
        // Creates more IDs, parameter "quantity" is to determine how much
        {
            bool notEmpty = false;
            for (int i = 0; i < quantity; i++)
            {
                var res = await client.PostAsync(dbLink, new StringContent(""));
                if (res.IsSuccessStatusCode) { notEmpty = true; }
            }
            return notEmpty;
        }
    }
}
