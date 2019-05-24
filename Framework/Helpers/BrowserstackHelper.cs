using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;
using Stasis.Enums;

namespace Stasis.Helpers
{
	public static class BrowserstackHelper
	{
		internal const string BrowserstackSessionConfigUrl = "https://www.browserstack.com/automate/sessions/{0}.json";

		/// <summary>
		/// Sets session status to mark test as passed/failed in Browserstack.
		/// </summary>
		/// <param name="status">Test status.</param>
		/// <param name="reason">Reason if the status.</param>
		public static void SetSessionStatus(string status, string reason = null)
		{
			if (SettingsHelper.GetValue(SettingsValues.Environment).ToLower() != "remote")
			{
				throw new ArgumentException("Webdriver is not set up as remote. Check run.xml to configure remote run properly");
			}

			var driver = (RemoteWebDriver) Browser.GetDriver();
			var requestData = Encoding.UTF8.GetBytes(new JObject(new JProperty("status", status), new JProperty("reason", reason)).ToString());
			var uri = new Uri(string.Format(BrowserstackSessionConfigUrl, driver.SessionId));
			var webRequest = WebRequest.Create(uri);
			var httpWebRequest = (HttpWebRequest)webRequest;
			webRequest.ContentType = "application/json";
			webRequest.Method = WebRequestMethods.Http.Put;
			webRequest.ContentLength = requestData.Length;

			using (var st = webRequest.GetRequestStream())
			{
				st.Write(requestData, 0, requestData.Length);
			}

			var networkCredential = new NetworkCredential(SettingsHelper.GetValue("bsUser"), SettingsHelper.GetValue("bsKey"));
			var credentialCache = new CredentialCache {{uri, "Basic", networkCredential}};
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = credentialCache;
			webRequest.GetResponse().Close();
		}
	}
}
