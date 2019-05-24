using OpenQA.Selenium;

namespace Stasis.Elements
{
	public class TextBox : BaseElement
	{
		public TextBox(By locator, string name) : base(locator, name)
		{
		}

		/// <summary>
		/// Sets the text.
		/// </summary>
		/// <param name="text">The text.</param>
		public void SetText(string text)
		{
			Log.Info($"Typing text: {text}");
			WaitForElementAvailable();
			GetElement().SendKeys(text);
		}

		/// <summary>
		/// Clears field and sets text.
		/// </summary>
		/// <param name="text">The text.</param>
		public void ClearSetText(string text)
		{
			Log.Info($"Clearing and typing text: {text}");
			WaitForElementAvailable();
			GetElement().Clear();
			GetElement().SendKeys(text);
		}

		/// <summary>
		/// Clears textbox.
		/// </summary>
		public void Clear()
		{
			Log.Info($"Clearing textbox: {GetName()}'");
			GetElement().Clear();
		}

		/// <summary>
		/// Presses the enter button.
		/// </summary>
		public void PressEnter()
		{
			WaitForElementIsPresent();
			WaitForElementIsVisible();
			GetElement().SendKeys(Keys.Enter);
			Log.Info($"{GetName()} :: Press Enter'");
		}
	}
}