using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.NetworkInformation;
using GimmeProxyDotNet;

namespace ProxyHandler
{
	class MainClass
	{
		private static GimmeProxyController proxyFetcher = new GimmeProxyController();
		private static Dictionary<long, string> proxyList = new Dictionary<long, string>();

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Task mainTask = new Task(async () => 
			{
				await fetchProxies();
			});
			mainTask.Start();
			mainTask.Wait();
			Console.ReadLine();
		}


		public static async Task fetchProxies()
		{ 
			
			for (int i = 0; i <= 20; i++)
			{
				Ping pingTester = new Ping();
				PingOptions pingOptions = new PingOptions(128, true);
				GimmeProxyResult proxyResult = await proxyFetcher.DoSimpleQuery("CN","http", true, true);

				byte[] pingBuffer = new byte[32];

				PingReply pingReply = pingTester.Send(proxyResult.IpAddress, 500, pingBuffer, pingOptions);

				Console.WriteLine("Retrieved IP: {0}, Latency: {1}", proxyResult.IpAndPort, pingReply.RoundtripTime);

				if (pingReply != null) 
				{
					if (!proxyList.ContainsValue(proxyResult.IpAndPort) && !proxyList.ContainsKey(pingReply.RoundtripTime))
					{
						proxyList.Add(pingReply.RoundtripTime, proxyResult.IpAndPort);
					}
				}

			}

			var sortedProxyList = proxyList.OrderBy(i => i.Key);
			foreach (var item in sortedProxyList)
			{
				long i = item.Key;
			}
			Console.WriteLine("Length of Proxy List: {0}", sortedProxyList.Count());
		}
	}
}
