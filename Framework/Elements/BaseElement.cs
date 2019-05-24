using System;
using System.Linq;
using System.Threading;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using ProductX.Framework.Enums;
using ProductX.Framework.Helpers;

namespace ProductX.Framework.Elements
{
	public abstract class BaseElement
	{
		private readonly By _locator;
		private readonly string _name;
		private readonly IWebElement _element;
		protected readonly int DefaultTimeout = Convert.ToInt32(SettingsHelper.GetValue(SettingsValues.Timeout));
		protected readonly int DefaultPollingInterval = Convert.ToInt32(SettingsHelper.GetValue(SettingsValues.PollingInterval));
		protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseElement));
		private IWebElement Element => _element ?? Browser.GetDriver().FindElements(_locator).FirstOrDefault();

		protected BaseElement(By locator, string name)
		{
			_name = name;
			_locator = locator;
		}

		protected BaseElement(IWebElement element, string name)
		{
			_element = element;
			_name = name;
		}

		/// <summary>
		/// Determines whether element is clickable.
		/// </summary>
		/// <returns>System.Boolean.</returns>
		public bool IsClickable() => GetElement().Enabled;

		/// <summary>
		/// Waits for element exists in DOM.
		/// </summary>
		public void WaitForElementIsPresent() => WaitForElementExistence();

		/// <summary>
		/// Waits for element exists in DOM.
		/// </summary>
		/// <param name="timeout">Timeout of wait.</param>
		public void WaitForElementIsPresent(int timeout) => WaitForElementExistence(timeout);

		/// <summary>
		/// Waits for element exists in DOM.
		/// </summary>
		/// <param name="timeout">Timeout of wait.</param>
		/// <param name="count">Count of elements to wait.</param>
		/// <param name="pollingInterval">Polling interval.</param>
		public void WaitForElementIsPresent(int timeout, int count, int? pollingInterval = null)
			=> WaitForElementExistence(timeout, pollingInterval, count);

		/// <summary>
		/// Waits for element disappear from DOM.
		/// </summary>
		public void WaitForElementDisappear()
			=> WaitForElementExistence(isPresent: false);

		/// <summary>
		/// Waits for element disappear from DOM.
		/// </summary>
		/// <param name="timeout">Timeout of wait.</param>
		/// /// <param name="pollingInterval">Polling interval.</param>
		public void WaitForElementDisappear(int timeout, int? pollingInterval = null)
			=> WaitForElementExistence(isPresent: false, timeout: timeout, pollingInterval: pollingInterval);

		/// <summary>
		///     Waits for element is visible.
		/// </summary>
		/// <param name="timeout">Milliseconds until timeout.</param>
		public void WaitForElementIsVisible(int? timeout = null) => Browser.Wait(
				TimeSpan.FromMilliseconds(timeout ?? DefaultTimeout),
				TimeSpan.FromMilliseconds(DefaultPollingInterval))
			.Until(waiting => IsPresent() && Element.Displayed);

		/// <summary>
		/// Waits for element is invisible.
		/// </summary>
		/// <param name="timeout">Milliseconds until timeout.</param>
		public void WaitForElementIsInvisible(int? timeout = null) => Browser.Wait(TimeSpan.FromMilliseconds(timeout ?? DefaultTimeout))
			.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(_locator));

		/// <summary>
		/// Waits for element is clickable.
		/// </summary>
		public void WaitForElementIsClickable() => Browser.Wait(
			TimeSpan.FromMilliseconds(DefaultTimeout),
			TimeSpan.FromMilliseconds(DefaultPollingInterval)).Until(waiting => IsPresent() && Element.Enabled);

		/// <summary>
		/// Gets element`s InnerText.
		/// </summary>
		/// <param name="acceptEmptyText">Will wait for not empty InnerText.</param>
		/// <returns>System.String.</returns>
		public string GetText(bool acceptEmptyText = true)
		{
			if (!acceptEmptyText)
			{
				WaitForTextIsNotEmpty();
			}

			return GetElement().Text.Trim();
		}

		/// <summary>
		/// Finds element on the current page or returns _element value.
		/// </summary>
		/// <returns>Element on the current page/_element/null.</returns>
		public IWebElement GetElement()
		{
			WaitForElementIsPresent();

			return Element;
		}

		/// <summary>
		/// Waits for enabled/displayed, hovers over element to avoid tooltip overlaying and clicks.
		/// </summary>
		public void Click()
		{
			Browser.Wait().Until(waiting =>
			{
				try
				{
					WaitForElementAvailable();
					Element.Click();
					Log.Info($"{GetName()} :: click");

					return true;
				}
				catch (Exception)
				{
					return false;
				}
			});
		}

		/// <summary>
		/// Waits for enabled/displayed and hovers over element.
		/// </summary>
		public void HoverOver()
		{
			WaitForElementAvailable();
			Hover();
		}

		/// <summary>
		/// Waits for present in DOM and hovers over element.
		/// </summary>
		public void HoverOverInvisibleElement()
		{
			WaitForElementIsPresent();
			Hover();
		}

		/// <summary>
		/// Gets the attribute value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>System.String.</returns>
		public string GetAttributeValue(string value)
		{
			var result = GetElement()
				.GetAttribute(value);

			return result?.Trim() ?? string.Empty;
		}

		/// <summary>
		/// Determines whether element is displayed.
		/// </summary>
		/// <returns>System.Boolean.</returns>
		public bool IsDisplayed(int timeout = 0)
		{
			if (timeout == 0)
			{
				return IsPresent() && Element.Displayed;
			}

			try
			{
				WaitForElementIsVisible(timeout);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Check if element.
		/// </summary>
		/// <param name="timeout">Milliseconds until timeout.</param>
		/// <returns></returns>
		public bool IsPresent(int? timeout = null, int? pollingInterval = null)
		{
			if (timeout == null)
			{
				return Element != null;
			}

			pollingInterval = pollingInterval ?? DefaultPollingInterval;

			try
			{
				Browser.Wait(TimeSpan.FromMilliseconds((int)timeout), TimeSpan.FromMilliseconds((int)pollingInterval))
					.Until(waiting => Browser.GetDriver().FindElements(_locator).Count > 0);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Waits for element animation is completed and it`s location is static.
		/// </summary>
		public void WaitForAnimationIsCompleted()
		{
			Browser.Wait()
				.Until(waiting =>
				{
					var previousPosition = GetElement()?.Location;
					Thread.Sleep(100);
					var currentPosition = GetElement()?.Location;

					return currentPosition != null && previousPosition.Equals(currentPosition);
				});
		}

		protected string GetName() => _name;

		protected By GetLocator() => _locator;

		protected void WaitForTextIsNotEmpty() => Browser.Wait().Until(brw => GetElement().Text != string.Empty);

		protected void WaitForElementAvailable()
		{
			WaitForElementIsPresent();
			WaitForElementIsVisible();
			WaitForElementIsClickable();
		}

		private void WaitForElementExistence(int? timeout = null, int? pollingInterval = null, int? count = null, bool isPresent = true)
		{
			timeout = timeout ?? DefaultTimeout;
			pollingInterval = pollingInterval ?? DefaultPollingInterval;

			if (_locator == null)
			{
				return;
			}

			try
			{
				Browser.Wait(TimeSpan.FromMilliseconds((int)timeout), TimeSpan.FromMilliseconds((int)pollingInterval)).Until(waiting =>
				{
					var webElements = Browser.GetDriver().FindElements(_locator);

					if (!isPresent)
					{
						return webElements.Count == 0;
					}

					if (count != null)
					{
						return webElements.Count == count;
					}

					return webElements.Count != 0;
				});
			}
			catch (Exception)
			{
				Log.Fatal(isPresent
					? $"Element with locator: '{_locator}' does not exists!"
					: $"Element with locator: '{_locator}' does not disappear!");

				throw;
			}
		}

		private void Hover()
		{
			new Actions(Browser.GetDriver())
				.MoveToElement(GetElement())
				.Build()
				.Perform();
			Log.Info($"Mouse pointer hover over '{GetName()}'");
		}
	}
}