using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GimmeProxyDotNet;

namespace ProxyHandler
{
	class MainClass
	{
		private static GimmeProxyController proxyFetcher = new GimmeProxyController();

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			List<string> proxyList = new List<string>();
			Task mainTask = new Task(async () => 
			{
				for (int i = 0; i <= 10; i++)
				{
					proxyList.Add(await getProxyAddr());
				}
			});
			mainTask.Start();
			mainTask.Wait();
			Console.ReadLine();
		}

		public static async Task<string> getProxyAddr()
		{
			GimmeProxyResult proxyResult = await proxyFetcher.DoQuery("CN");
			return proxyResult.IpAndPort;
		}
	}
}
