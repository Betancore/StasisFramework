using System;
using System.IO;
using OpenQA.Selenium;

namespace Stasis.Helpers
{
	public static class ScreenshotHelper
	{
		/// <summary>
		/// Takes screenshot and saves it in working directory.
		/// </summary>
		/// <param name="screenshotName">Name for screenshot.</param>
		public static void TakeScreenshot(string screenshotName)
		{
			var screenshot = ((ITakesScreenshot)Browser.GetDriver()).GetScreenshot();
			screenshot.SaveAsFile(Path.GetFullPath(
				$"{Directory.GetCurrentDirectory()}\\{screenshotName}{DateTime.UtcNow:MM_dd_HH_mm_ss}.png"),
				ScreenshotImageFormat.Png);
		}

	}
}
