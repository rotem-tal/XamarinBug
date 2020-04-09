using System;
using Nito.AsyncEx;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {

        static HttpClient client = new HttpClient();
        static string dbLink = "https://zulital3.wixsite.com/tute-meshek-tal/_functions/xamarin/";
        
        static void Main(string[] args)
        // A main function to enter the Async main method
        {
            AsyncContext.Run(() => MainA());
        }


        static async void MainA()
        // A main method that can run async
        {

            string[] arr = await GetId();
            if (arr.Length == 0)
            {
                if (await CreateId())
                {
                    arr = await GetId();
                }
                else
                {
                    Console.WriteLine("Unable to create ID's");
                    System.Environment.Exit(1);
                }
            }
            var response = await client.DeleteAsync(dbLink + arr[0]);
            Console.WriteLine(response.StatusCode);
        }

        private static async Task<string[]> GetId()
        //Returns a list of ID's available for deletion on the server
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

        private static async Task<bool> CreateId(int quantity = 10)
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
