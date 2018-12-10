using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Communication.HttpBridge
{
   public class BridgeClientRequest
    {
        public event Action<String> ReceiveResponseHandler;
        public event Action<int> ErrorBridgeHandler;
        public BridgeClientRequest() { }
        public async Task<object> PostCallAPI(string url, object jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent("hello", Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        ReceiveResponseHandler(jsonString);
                        Console.WriteLine(jsonString);
                        return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
            return null;
        }
        public async Task<String> GetCallAPI(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        ReceiveResponseHandler(jsonString);
                        Console.WriteLine(jsonString);
                        return jsonString;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
