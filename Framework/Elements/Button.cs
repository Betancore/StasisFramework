using OpenQA.Selenium;

namespace ProductX.Framework.Elements
{
	public class Button : BaseElement
	{
		public Button(By locator, string name) : base(locator, name)
		{
		}

		public Button(IWebElement element, string name) : base(element, name)
		{
		}
	}
}