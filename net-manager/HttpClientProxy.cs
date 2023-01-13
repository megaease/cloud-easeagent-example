namespace net.manager
{
    public class HttpClientProxy
    {
        public static HttpClientProxy CLIENT { get; set; }
        public String addEndpoint;
        public String deleteEndpoint;

        private IHttpClientFactory clientFactory;

        public HttpClientProxy(IHttpClientFactory clientFactory)
        {
            string? endpoint = Environment.GetEnvironmentVariable("USER_ENDPOINT");
            if (endpoint == null)
            {
                this.addEndpoint = "http://127.0.0.1:18888/user/add/";
                this.deleteEndpoint = "http://127.0.0.1:18888/user/delete/";
            }
            else
            {
                this.addEndpoint = endpoint + "/add/";
                this.deleteEndpoint = endpoint + "/delete/";

            }
            this.clientFactory = clientFactory;
        }
        public async void CallAddAsync(string name)
        {
            await callGetAsync(addEndpoint + name);
        }

        public async void CallDeleteAsync(string name)
        {
            await callGetAsync(deleteEndpoint + name);
        }

        private async Task callGetAsync(string url)
        {
            using (var httpClient = clientFactory.CreateClient("UserManager"))
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("call url({0}) fail {1}", url, response.ToString());
                    }
                    else
                    {
                        Console.WriteLine("call url({0}) success {1}", url, response.ToString());
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("call url({0}) error {1}", url, e.ToString());
                }
            }
        }
    }
}