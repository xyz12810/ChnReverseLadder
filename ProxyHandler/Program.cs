using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using GimmeProxyDotNet;

namespace ProxyHandler
{
	class MainClass
	{
		private static GimmeProxyController proxyFetcher = new GimmeProxyController();
		private static Dictionary<long, string> proxyList = new Dictionary<long, string>();


		// Original WebClient cannot specify timeout,
		// here is a workaround for this issue.
		// See here: http://stackoverflow.com/questions/1789627/how-to-change-the-timeout-on-a-net-webclient-object
		private class WebClientWithTimeOut : WebClient
		{
			protected override WebRequest GetWebRequest(Uri address)
			{
				WebRequest w = base.GetWebRequest(address);
				w.Timeout = 10 * 1000;
				return w;
			}
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Task mainTask = new Task(async () => 
			{
				await fetchProxies();
			});
			mainTask.Start();
			mainTask.Wait();
			Console.ReadKey();
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

				Console.WriteLine("[INFO] Retrieved IP: {0}, Latency: {1}", proxyResult.IpAndPort, pingReply.RoundtripTime);

				if (pingReply != null) 
				{
					if (testNetEaseMusic(proxyResult.IpAndPort) 
					    && !proxyList.ContainsValue(proxyResult.IpAndPort) 
					    && !proxyList.ContainsKey(pingReply.RoundtripTime))
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("[SUCCESS] Wrote one new proxy {0} to list!", proxyResult.IpAndPort);
						Console.ResetColor();
						proxyList.Add(pingReply.RoundtripTime, proxyResult.IpAndPort);
					}
				}

			}

			var sortedProxyList = proxyList.OrderBy(i => i.Key);
			string proxiesCombined = string.Empty;

			// Add user-specified proxies first
			proxiesCombined += getOwnProxies();

			foreach (var item in sortedProxyList)
			{
				long itemKey = item.Key;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("[SUCCESS] Will write proxy {0} to PAC!", "PROXY " + item.Value + ";");
				Console.ResetColor();
				proxiesCombined += " PROXY " + item.Value + "; ";
			}

			// Finally, add origial Unblock-Youku's proxy to the end.
			proxiesCombined += "PROXY proxy.uku.im:443;";
			writeToPac(proxiesCombined, "http://pac.uku.im/pac.pac");
			Console.WriteLine("Length of Proxy List: {0}", sortedProxyList.Count());
		}

		private static void writeToPac(string ipAddrs, string origPacFileUrl)
		{
			string origContent = string.Empty;
			using (WebClient pacGrabber = new WebClient())
			{
				origContent = pacGrabber.DownloadString(origPacFileUrl);
			}

			string patchedContent = origContent.Replace("PROXY proxy.uku.im:443;", ipAddrs);
			File.WriteAllText("autopac.pac", patchedContent);
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("[DONE] Done all works. Check \"autopac.pac\" file in your directory!");
			Console.ResetColor();
		}

		private static bool testNetEaseMusic(string proxyAddr)
		{
			string testResultStr = string.Empty;
			bool testResult = false;
			try
			{
				// Some private proxies does not work with public services.
				// Maybe it will return a 403 forbidden code or 500 internal service error code.
				// So we need to "filter" them here.
				using (WebClientWithTimeOut pacGrabber = new WebClientWithTimeOut())
				{
					pacGrabber.Proxy = new WebProxy(proxyAddr);
					testResultStr = pacGrabber.DownloadString("http://ipservice.163.com/isFromMainland");
				}
			}
			catch
			{
				// Do nothing because it's unnecessary
			}

			// Sometimes the Netease server may be blocked in some network environments, and return an empty or invalid result.
			// So we need a judgement before parsing the result.
			if (testResultStr != null || testResultStr.Length >= 4)
			{
				bool.TryParse(testResultStr, out testResult);
			}
			else
			{
				testResult = false;
			}

			return testResult;
		}

		private static string getOwnProxies(string proxyFile = "ownproxies.txt")
		{
			return File.ReadAllText(proxyFile);
		}
			
	}
}
