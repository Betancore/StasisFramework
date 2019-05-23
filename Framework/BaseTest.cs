using ProductX.Framework.Enums;
using ProductX.Framework.Helpers;
using log4net;
using NUnit.Framework;
using ProductX.Framework.Constants;
using ProductX.Framework.Extensions;

namespace ProductX.Framework
{
	[TestFixture]
	public class BaseTest
	{
		private const string PerformanceTestType = "performance";
		protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));
		private readonly string _baseUrl;
		public UserSteps User { get; set; }

		public BaseTest(string url)
		{
			_baseUrl = url;
		}

		public BaseTest()
		{
			_baseUrl = GoogleApiHelper.GetEnvData("Data!C3:C3");
		}

		[OneTimeSetUp]
		public void Initialize()
		{
			if (RunConfigurator.GetValue(RunValues.UpdateTestData) == "yes")
			{
				GoogleApiHelper.ExportTestDataTable();
			}
		}

		[SetUp]
		public void SetUp()
		{
			User = new UserSteps();
			Browser.GetDriver().Manage().Window.Maximize();
			Browser.GetDriver().Navigate().GoToUrl(_baseUrl);
		}

		[TearDown]
		public void TearDown()
		{
			if ((RunConfigurator.GetValue(RunValues.Zap) == "true"
				|| RunConfigurator.GetValue(RunValues.Tenant) == PerformanceTestType)
				&& TestContext.CurrentContext.Result.Outcome.Status.ToString().ToLower() == SessionStatuses.Failed.GetValue())
				ScreenshotHelper.TakeScreenshot(TestContext.CurrentContext.Test.MethodName);

			if (RunConfigurator.GetValue(RunValues.Env).ToLower() == SeleniumServerLocations.Remote)
			{
				BrowserstackHelper.SetSessionStatus(TestContext.CurrentContext.Result.Outcome.Status
														.ToString()
														.ToLower() == SessionStatuses.Failed.GetValue()
					? SessionStatuses.Failed.GetValue()
					: SessionStatuses.Passed.GetValue());
			}

			Browser.Close();
		}
	}
}