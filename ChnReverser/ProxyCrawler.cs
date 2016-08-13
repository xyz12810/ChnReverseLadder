using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace ChnReverser
{
	public class ProxyCrawler
	{
		public ProxyCrawler()
		{

		}

		public string[] LoadChinaList(string address = "http://www.ipdeny.com/ipblocks/data/countries/cn.zone")
		{
			// TODO: Change everything below to async.
			HttpClient client = new HttpClient();
			HttpResponseMessage returnMessage = client.GetAsync(address).Result;
			HttpContent returnContent = returnMessage.Content;

			string contentStr = returnContent.ReadAsStringAsync().Result;
			string[] contentArray = contentStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			Console.WriteLine("Got {0} IP ranges in China! Awaiting to crawl!", contentArray.Length);

			return contentArray;
		}

		public async Task<bool> IsValidProxy(string proxyIp, int port = 80)
		{
			try
			{
				using (HttpClient client = clientWithProxy(proxyIp, port))
				using (HttpResponseMessage response = await client.GetAsync("http://taobao.com"))
				using (HttpContent content = response.Content)
				{
					
					string result = await content.ReadAsStringAsync();

					// ... Display the result.
					if (result != null)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}
		}

		private HttpClient clientWithProxy(string proxyIp, int port)
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				Proxy = new WebProxy("http://" + proxyIp + ":" + port.ToString(), false),
				UseProxy = true
			};

			HttpClient client = new HttpClient(handler);
			return client;
		}
	}
}