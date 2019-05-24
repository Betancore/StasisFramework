using log4net;
using OpenQA.Selenium;
using ProductX.Framework.Elements;
using ProductX.Framework.Enums;
using ProductX.Framework.Helpers;

namespace ProductX.Framework.Forms
{
	public class BaseForm
	{
		protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseForm));

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseForm" /> class.
		/// </summary>
		/// <param name="locator">The locator.</param>
		/// <param name="name">The name.</param>
		protected BaseForm(By locator, string name)
		{
			new Button(locator, name).WaitForElementIsPresent();
		}

		public BaseForm()
		{
		}

		/// <summary>
		/// Navigates to specified url.
		/// </summary>
		/// <param name="url">Url to navigate to.</param>
		public void GoToUrl(string url)
		{
			Log.Info($"Navigating to: {url}");
			Browser.GetDriver().Navigate().GoToUrl(url);
		}

		/// <summary>
		/// Navigate to base page.
		/// </summary>
		public void GoToMainPage()
		{
			var mainPageUrl = SettingsHelper.GetValue(SettingsValues.BaseUrl);
			Log.Info($"Navigating to {mainPageUrl}");
			Browser.GetDriver().Navigate().GoToUrl(mainPageUrl);
		}

		/// <summary>
		/// Asserts the text is present.
		/// </summary>
		/// <param name="text">The text.</param>
		public void WaitForTextIsPresent(string text)
		{
			Log.Info($"Waiting for text: {text}");
			new Button(By.XPath($"//*[contains(text(),'{text}')]"), text + " label").WaitForElementIsPresent();
		}

		/// <summary>
		/// Waits for text is disappear.
		/// </summary>
		/// <param name="text">The text.</param>
		public void WaitForTextDisappear(string text)
		{
			Log.Info($"Waiting for text to disappear: {text}");
			new Button(By.XPath($"//*[contains(text(),'{text}')]"), text + " label").WaitForElementDisappear();
		}

		/// <summary>
		/// Checks if text is present.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>System.Boolean.</returns>
		public bool IsTextPresent(string text)
		{
			Log.Info($"Is text present: {text}");

			return Browser.GetDriver().FindElements(By.XPath($"//*[contains(text(),'{text}')]")).Count > 0;
		}

		/// <summary>
		/// Refreshes current browser page.
		/// </summary>
		public void Refresh()
		{
			Log.Info("Refreshing page");
			Browser.GetDriver().Navigate().Refresh();
		}
	}
}