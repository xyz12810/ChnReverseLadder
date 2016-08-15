using System;
using Newtonsoft.Json;
namespace GimmeProxyDotNet
{
	[JsonObject]
	public class GimmeProxyResult
	{
		[JsonProperty(PropertyName = "get")]
		public bool AllowGet { get; set; }

		[JsonProperty(PropertyName = "post")]
		public bool AllowPost { get; set; }

		[JsonProperty(PropertyName = "cookies")]
		public bool AllowCookies { get; set; }

		[JsonProperty(PropertyName = "referer")]
		public bool Referer { get; set; }

		[JsonProperty(PropertyName = "user-agent")]
		public bool UserAgent { get; set; }

		[JsonProperty(PropertyName = "anonymityLevel")]
		public int AnonymityLevel { get; set; }

		[JsonProperty(PropertyName = "supportsHttps")]
		public bool SupportsHttps { get; set; }

		[JsonProperty(PropertyName = "protocol")]
		public string Protocol { get; set; }

		[JsonProperty(PropertyName = "ip")]
		public string IpAddress { get; set; }

		[JsonProperty(PropertyName = "port")]
		public string Port { get; set; }

		[JsonProperty(PropertyName = "country")]
		public string Country { get; set; }

		[JsonProperty(PropertyName = "tsChecked")]
		public Int64 CheckedTime { get; set; }

		[JsonProperty(PropertyName = "curl")]
		public string CurlAddr { get; set; }

		[JsonProperty(PropertyName = "ipPort")]
		public string IpAndPort { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string ProxyType { get; set; }
	}
}

