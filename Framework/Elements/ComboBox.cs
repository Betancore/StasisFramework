using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ProductX.Framework.Elements
{
	public class ComboBox : BaseElement
	{
		public ComboBox(By locator, string name)
			: base(locator, name)
		{
		}

		/// <summary>
		/// Selects the by label.
		/// </summary>
		/// <param name="label">The label.</param>
		public void SelectByLabel(string label)
		{
			WaitForElementIsPresent();
			Log.Info($"Selecting option by text '{label}'");
			new SelectElement(GetElement()).SelectByText(label);
		}

		/// <summary>
		/// Selects the by value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SelectByValue(string value)
		{
			WaitForElementIsPresent();
			Log.Info($"Selecting option by value '{value}'");
			new SelectElement(GetElement()).SelectByValue(value);
		}

		/// <summary>
		/// Selects the index of the by.
		/// </summary>
		/// <param name="index">The index.</param>
		public void SelectByIndex(int index)
		{
			WaitForElementIsPresent();
			Log.Info($"Selecting option by index '{index}'");
			new SelectElement(GetElement()).SelectByIndex(index);
		}
	}
}