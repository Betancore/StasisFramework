using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Stasis.Constants;
using Stasis.Enums;
using Stasis.Helpers;

namespace Stasis
{
	public static class Browser
	{
		private const string DriverPath = "../../resources/";
		private static IWebDriver _driver;
		private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(int.Parse(SettingsHelper.GetValue(SettingsValues.Timeout)));
		private static readonly TimeSpan PollingInterval = TimeSpan.FromMilliseconds(int.Parse(SettingsHelper.GetValue(SettingsValues.PollingInterval)));
		private static readonly string PlatformType = SettingsHelper.GetValue(SettingsValues.BsPlatform);
		private static readonly string DriverType =
			SettingsHelper.GetValue(PlatformType.ToLower() == "desktop" ? SettingsValues.BsBrowser : SettingsValues.BsBrowserName);
		private static readonly string BrowserType = SettingsHelper.GetValue(SettingsValues.Browser);
		private static readonly string BsUser = SettingsHelper.GetValue(SettingsValues.BsUser);
		private static readonly string BsKey = SettingsHelper.GetValue(SettingsValues.BsKey);
		private static readonly string BsBrowserName = SettingsHelper.GetValue(SettingsValues.BsBrowserName);
		private static readonly string BsOsPlatform = SettingsHelper.GetValue(SettingsValues.BsOsPlatform);
		private static readonly string BsOsVersion = SettingsHelper.GetValue(SettingsValues.BsOsVersion);
		private static readonly string BsDevice = SettingsHelper.GetValue(SettingsValues.BsDevice);
		private static readonly string BsBrowserType = SettingsHelper.GetValue(SettingsValues.BsBrowser);
		private static readonly string BsBrowserVersion = SettingsHelper.GetValue(SettingsValues.BsBrowserVersion);
		private static readonly string BsOs = SettingsHelper.GetValue(SettingsValues.BsOs);
		private static readonly string BsResolution = SettingsHelper.GetValue(SettingsValues.BsResolution);
		private static readonly string BsRemoteServer = SettingsHelper.GetValue(SettingsValues.BsRemoteServer);

		/// <summary>
		/// Gets the driver.
		/// </summary>
		/// <returns>IWebDriver.</returns>
		public static IWebDriver GetDriver() => _driver ?? (_driver = SetupBrowser());

		/// <summary>
		/// Quits driver.
		/// </summary>
		public static void Close()
		{
			if (_driver == null)
			{
				return;
			}

			_driver.Quit();
			_driver = null;
		}

		/// <summary>
		/// Creates instance of WebDriverWait.
		/// </summary>
		/// <param name="timeout">Timeout.</param>
		/// <param name="pollingInterval">Polling interval.</param>
		/// <returns>WebDriverWait.</returns>
		public static WebDriverWait Wait(TimeSpan timeout = default, TimeSpan pollingInterval = default)
		{
			timeout = timeout.Ticks == 0 ? Timeout : timeout;
			pollingInterval = pollingInterval.Ticks == 0 ? PollingInterval : pollingInterval;

			return new WebDriverWait(_driver, timeout)
			{
				PollingInterval = pollingInterval
			};
		}

		private static IWebDriver SetupBrowser()
		{
			var environment = SettingsHelper.GetValue(SettingsValues.Environment);

			switch (environment)
			{
				case SeleniumServerLocations.Remote:
					{
						var options = GetRemoteDriverOptions();

						return new RemoteWebDriver(new Uri(BsRemoteServer), options);
					}
				case SeleniumServerLocations.Local:
					{
						var options = GetLocalOptions();
						switch (BrowserType.ToLower())
						{
							case BrowserTypes.Chrome:
								return new ChromeDriver(Path.GetFullPath(DriverPath), (ChromeOptions)options);

							case BrowserTypes.Firefox:
								return new FirefoxDriver((FirefoxOptions)options);

							case BrowserTypes.Ie:
								return new InternetExplorerDriver(Path.GetFullPath(DriverPath), (InternetExplorerOptions)options);

							default:
								return new ChromeDriver(Path.GetFullPath(DriverPath));
						}
					}

				default:
					return new ChromeDriver(Path.GetFullPath(DriverPath));
			}
		}

		#region Remote options
		private static DriverOptions GetRemoteDriverOptions()
		{
			DriverOptions options;

			switch (DriverType.ToLower())
			{
				case BrowserTypes.Chrome:
					options = GetRemoteDesktopDriverOptions(new ChromeOptions());
					break;
				case BrowserTypes.Firefox:
					options = GetRemoteDesktopDriverOptions(new FirefoxOptions());
					break;
				case BrowserTypes.Ie:
					options = GetRemoteDesktopDriverOptions(new InternetExplorerOptions());
					break;
				case BrowserTypes.Safari:
					options = GetRemoteDesktopDriverOptions(new SafariOptions());
					break;
				case MobilePlatforms.Android:
					options = GetRemoteDeviceDriverOptions(new ChromeOptions());
					break;
				case MobilePlatforms.Iphone:
					options = GetRemoteDeviceDriverOptions(new ChromeOptions());
					break;
				default:
					options = GetRemoteDesktopDriverOptions(new ChromeOptions());
					break;
			}

			return options;
		}

		/// <summary>
		/// Instead of creating elegant and small method I have to develop this because AddAdditionalCapability does not set isGlobalCapability == true
		/// implicitly for some browsers. So for Chrome, IE, Firefox you should do it manually, for Safari, Edge - not (AddAdditionalCapability does not
		/// even have such parameter, same as base DriverOptions).
		/// </summary>
		/// <param name="options">Driver options.</param>
		/// <returns>DriverOptions.</returns>
		private static DriverOptions GetRemoteDesktopDriverOptions(DriverOptions options)
		{
			var capabilities = GetRemoteDesktopCapabilitiesSet();

			if (options.GetType() == typeof(ChromeOptions))
			{
				var chromeOptions = (ChromeOptions)options;
				chromeOptions.AddArgument(BrowserArguments.DisableWebSecurity);
				chromeOptions.AddArgument(BrowserArguments.DisableSiteIsolationTrials);
				capabilities.ToList().ForEach(pair => chromeOptions.AddAdditionalCapability(pair.Key, pair.Value, true));

				return options;
			}

			if (options.GetType() == typeof(FirefoxOptions))
			{
				var firefoxOptions = (FirefoxOptions)options;
				capabilities.ToList().ForEach(pair => firefoxOptions.AddAdditionalCapability(pair.Key, pair.Value, true));

				return options;
			}

			if (options.GetType() == typeof(InternetExplorerOptions))
			{
				var internetExplorerOptions = (InternetExplorerOptions)options;
				capabilities.ToList().ForEach(pair => internetExplorerOptions.AddAdditionalCapability(pair.Key, pair.Value, true));

				return options;
			}

			capabilities.ToList().ForEach(pair => options.AddAdditionalCapability(pair.Key, pair.Value));

			return options;
		}

		private static DriverOptions GetRemoteDeviceDriverOptions(ChromeOptions options)
		{
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.User, BsUser, true);
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.Key, BsKey, true);
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.BrowserName, BsBrowserName, true);
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.Platform, BsOsPlatform, true);
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.Device, BsDevice, true);
			options.AddAdditionalCapability(BrowserStackCapabilityKeys.RealMobile, true.ToString().ToLower(), true);

			return options;
		}
		#endregion

		#region Local options
		private static DriverOptions GetLocalOptions()
		{
			switch (BrowserType.ToLower())
			{
				case BrowserTypes.Chrome:

					var options = new ChromeOptions();
					options.AddUserProfilePreference(BrowserArguments.SafeBrowsingArgument, true);
					options.AddArgument(BrowserArguments.DisableWebSecurity);
					options.AddArgument(BrowserArguments.DisableSiteIsolationTrials);
					options.AddArgument(BrowserArguments.IgnoreCertificatesArgument);

					return options;

				case BrowserTypes.Firefox:
					return new FirefoxOptions();

				case BrowserTypes.Ie:
					return new InternetExplorerOptions();

				default:
					return new ChromeOptions();
			}
		}
		#endregion

		private static Dictionary<string, string> GetRemoteDesktopCapabilitiesSet()
		{
			var capabilities = new Dictionary<string, string>
			{
				{BrowserStackCapabilityKeys.User, BsUser},
				{BrowserStackCapabilityKeys.Key, BsKey},
				{BrowserStackCapabilityKeys.Browser, BsBrowserType},
				{BrowserStackCapabilityKeys.BrowserVersion, BsBrowserVersion},
				{BrowserStackCapabilityKeys.Os, BsOs},
				{BrowserStackCapabilityKeys.OsVersion, BsOsVersion},
				{BrowserStackCapabilityKeys.Resolution, BsResolution},
				{BrowserStackCapabilityKeys.Console, "errors"},
				{BrowserStackCapabilityKeys.Name, TestContext.CurrentContext.Test.Name},
				{BrowserArguments.SafeBrowsingArgument, true.ToString().ToLower()}
			};

			return capabilities;
		}
	}
}