using OpenQA.Selenium;
using Stasis.Elements;
using Stasis.Forms;

namespace DummyTests.Forms
{
	public class FakeForm : BaseForm
	{
		private static readonly By TitleLocator = By.Id("main");
		private static TextBox SearchTextBox => new TextBox(By.XPath("//input[contains(@role, 'combobox')]"), "Search textbox");
		private static Button SearchButton => new Button(By.XPath("//input[not(ancestor::*[@style = 'display: none;']) and contains(@name, 'btnK')]"), "Search button");
		public FakeForm() : base(TitleLocator, "Main Google form")
		{
		}

		public void SetSearch(string searchText) => SearchTextBox.ClearSetText(searchText);

		public void Search() => SearchButton.Click();
	}
}
