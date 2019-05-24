using System;
using ProductX.Framework.Enums;
using ProductX.Framework.Helpers;
using log4net;
using NUnit.Framework;
using ProductX.Framework.Constants;
using ProductX.Framework.Extensions;
using ProductX.Framework.Models.Google;

namespace ProductX.Framework
{
	[TestFixture]
	public class BaseTest
	{
		protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));
		private readonly string _baseUrl;

		public BaseTest(string url)
		{
			_baseUrl = url;
		}

		[OneTimeSetUp]
		public void Initialize()
		{
			if (SettingsHelper.GetValue(SettingsValues.UpdateTestData) == "yes")
			{
				// Mocked to avoid compilation level errors
				GoogleApiHelper.ExportTestDataTable(new GoogleApiUser(), "dummy spreadsheet id");
			}
		}

		[SetUp]
		public void SetUp()
		{
			Browser.GetDriver().Manage().Window.Maximize();
			Browser.GetDriver().Navigate().GoToUrl(_baseUrl);
		}

		[TearDown]
		public void TearDown()
		{
			if (SettingsHelper.GetValue(SettingsValues.Env).ToLower() == SeleniumServerLocations.Remote)
			{
				BrowserstackHelper.SetSessionStatus(
					TestContext.CurrentContext.Result.Outcome.Status.ToString().ToLower() == SessionStatuses.Failed.GetValue()
					? SessionStatuses.Failed.GetValue()
					: SessionStatuses.Passed.GetValue());
			}
			else
			{
				if (SettingsHelper.GetValue(SettingsValues.EnableScreenshoting).Equals("yes", StringComparison.InvariantCultureIgnoreCase)
					&& TestContext.CurrentContext.Result.Outcome.Status.ToString().ToLower() == SessionStatuses.Failed.GetValue())
				{
					ScreenshotHelper.TakeScreenshot(TestContext.CurrentContext.Test.MethodName);
				}
			}

			Browser.Close();
		}
	}
}