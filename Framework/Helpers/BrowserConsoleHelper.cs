using System.Linq;
using OpenQA.Selenium;

namespace Stasis.Helpers
{
	public static class BrowserConsoleHelper
	{
		/// <summary>
		/// Checks if browser console contains specified error.
		/// </summary>
		/// <param name="errorMessage">Error message.</param>
		/// <returns>System.Boolean.</returns>
		public static bool IsConsoleErrorExist(string errorMessage)
		{
			var logs = Browser.GetDriver().Manage().Logs.GetLog(LogType.Browser);

			if (logs.Count == 0)
			{
				return false;
			}

			return logs.Any(log => log.Message.ToLower()
				.Contains(errorMessage));
		}
	}
}
