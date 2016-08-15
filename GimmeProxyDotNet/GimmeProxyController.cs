using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GimmeProxyDotNet
{
	public class GimmeProxyController
	{
		public GimmeProxyController()
		{

		}

		private const string apiBaseUrl = "http://gimmeproxy.com";
		private const string baseStr = "/api/getProxy?";
		private const string messageStr = "country={0}&protocol={1}&get={2}&post={3}&cookies={4}&referer={5}&user-agent={6}&supportsHttps={7}&anonymityLevel={8}&port={9}&maxCheckPeriod={10}&apikey={11}";
		private const string simplifiedMessageStr = "country={0}&protocol={1}&get={2}&post={3}";

		private HttpClient GetHttpClient()
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(apiBaseUrl);
			Debug.WriteLine("REQUEST HTTP ADDR: " + apiBaseUrl);
			return client;
		}

		private async Task<T> ExecuteAsync<T>(string query)
		{
			string CallUrl = apiBaseUrl + baseStr + query;

			Debug.WriteLine("REQUEST HTTP ADDR: " + CallUrl);

			using (var client = GetHttpClient())
			{
				var json = await client.GetStringAsync(CallUrl);
				var result = JsonConvert.DeserializeObject<T>(json);
				return result;
			}
		}

		public async Task<GimmeProxyResult> DoFullQuery(string Country = "", string Protocol = "http", bool AllowGet = true,
									bool AllowPost = true, bool AllowCookies = true, bool AllowReferer = true,
									bool AllowUserAgent = true, bool SupportHttps = false, int AnonymityLevel = 0,
									string Port = "", int MaxCheckPeriod = 86400, string ApiKey = "")
		{
			var proxyQuery = string.Format(messageStr, Country, Protocol, 
			                               AllowGet, AllowPost, AllowCookies, 
			                               AllowReferer, AllowUserAgent, SupportHttps, 
			                               AnonymityLevel, Port, MaxCheckPeriod, ApiKey);

			Debug.WriteLine("Query: {0}", proxyQuery);
			
			var result = await this.ExecuteAsync<GimmeProxyResult>(proxyQuery);
			return result;
		}


		public async Task<GimmeProxyResult> DoSimpleQuery(string Country = "", string Protocol = "http", bool AllowGet = true, bool AllowPost = true)
		{
			var proxyQuery = string.Format(simplifiedMessageStr, Country, Protocol, AllowGet, AllowPost);
			Debug.WriteLine("Query: {0}", proxyQuery);
			var result = await this.ExecuteAsync<GimmeProxyResult>(proxyQuery);
			return result;
		}

	}
}

