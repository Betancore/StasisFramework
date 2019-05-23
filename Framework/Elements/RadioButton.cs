using System;
using OpenQA.Selenium;

namespace ProductX.Framework.Elements
{
	public class RadioButton : BaseElement
	{
		public RadioButton(By locator, string name) : base(locator, name)
		{
		}

		/// <summary>
		/// Waits for element is selected.
		/// </summary>
		/// <param name="timeout">Int timeout value in milliseconds.</param>
		public void WaitForSelected(int? timeout = null) =>
			Browser.Wait(TimeSpan.FromMilliseconds(timeout ?? DefaultTimeout)).Until(waiting => IsSelected());

		/// <summary>
		/// Check if element is selected.
		/// </summary>
		/// <returns>System.Bool.</returns>
		public bool IsSelected()
		{
			WaitForElementIsPresent();

			return GetElement().Selected;
		}

		/// <summary>
		/// Waits for element is deselected.
		/// </summary>
		/// <param name="timeout">Int timeout value in milliseconds.</param>
		public void WaitForDeselected(int? timeout = null) =>
			Browser.Wait(TimeSpan.FromMilliseconds(timeout ?? DefaultTimeout)).Until(waiting => !IsSelected());
	}
}