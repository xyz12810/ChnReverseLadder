using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ChnReverser
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Task mainTask = new Task(() => 
			{
				ProxyCrawler crawler = new ProxyCrawler();
				string[] ipRangeArray = crawler.LoadChinaList();
				Console.WriteLine(ipRangeArray);
				Parallel.ForEach(ipRangeArray, new ParallelOptions { MaxDegreeOfParallelism = 50 }, (string currentIp) =>
			 	{
					 workerThread(currentIp);
			 	});
			});

			mainTask.Start();
			mainTask.Wait();
			Console.ReadLine();
		}

		private static StreamWriter streamWriter = new StreamWriter("result.txt", false, Encoding.UTF8);
		private static void workerThread(string sourceIP = "192.168.0.0/24")
		{
			// Solution copied and edited from: http://stackoverflow.com/questions/32028166/convert-cidr-notation-into-ip-range

			ProxyCrawler proxyCrawler = new ProxyCrawler();
			string[] parts = sourceIP.Split('.', '/');

			uint ipNum = (Convert.ToUInt32(parts[0]) << 24) |
				(Convert.ToUInt32(parts[1]) << 16) |
				(Convert.ToUInt32(parts[2]) << 8) |
				Convert.ToUInt32(parts[3]);

			int maskbits = Convert.ToInt32(parts[4]);
			uint mask = 0xffffffff;
			mask <<= (32 - maskbits);

			uint ipStart = ipNum & mask;
			uint ipEnd = ipNum | (mask ^ 0xffffffff);
			uint ipLength = ipEnd - ipStart;

			Console.WriteLine("[INFO] Thread #{0}: Range of IP address: {1} - {2}", Thread.CurrentThread.ManagedThreadId, convertToIp(ipStart), convertToIp(ipEnd));
			Console.WriteLine("[INFO] Thread #{0}: IP Start Position: {0}, End position: {1}", ipStart, ipEnd);

			// We cannot use unsigned 32 bit int here, and signed int32 isn't long enough.
			// So, here we use signed long (int64) instead.
			Parallel.For(0, ipLength, new ParallelOptions { MaxDegreeOfParallelism = 500 }, async (long ipIndex) =>
			 {
				 for (int proxyPort = 80; proxyPort <= 9000; proxyPort++)
				 {
					 Task<bool> validateProxy = proxyCrawler.IsValidProxy(convertToIp(ipStart + (uint)ipIndex), proxyPort);
					 bool proxyTestResult = await validateProxy;

					 if (proxyTestResult)
					 {
						 string proxyPath = convertToIp(ipStart + (uint)ipIndex) + ":" + proxyPort.ToString();
						 streamWriter.WriteLine(proxyPath);
						 streamWriter.Flush();
						 Console.ForegroundColor = ConsoleColor.Green;
						 Console.WriteLine("[SUCCESS] Found HTTP Proxy at {0}", proxyPath);
						 Console.ResetColor();
					 }
					 else
					 {
						 string proxyPath = convertToIp(ipStart + (uint)ipIndex) + ":" + proxyPort.ToString();
						 Console.ForegroundColor = ConsoleColor.Yellow;
						 Console.WriteLine("[FAIL] Not an HTTP Proxy at {0}", proxyPath);
						 Console.ResetColor();
					}
				}
			});

		}

		private static string convertToIp(uint ip)
		{
			return String.Format("{0}.{1}.{2}.{3}", ip >> 24, (ip >> 16) & 0xff, (ip >> 8) & 0xff, ip & 0xff);
		}



	}
}
