using OpenQA.Selenium;

namespace ProductX.Framework.Helpers
{
	public static class JsHelper
	{
		private const string IsJqueryActiveScript = "return !!window.jQuery && window.jQuery.active == 0";
		private static IJavaScriptExecutor JavaScriptExecutor => (IJavaScriptExecutor) Browser.GetDriver();

		/// <summary>
		/// Waiting until JQuery execution is completed. (Added just as example of what helper should do).
		/// </summary>
		public static void WaitForJQueryExecuted()
		{
			Browser
				.Wait()
				.Until(waiting => JavaScriptExecutor.ExecuteScript(IsJqueryActiveScript));
		}
	}
}
