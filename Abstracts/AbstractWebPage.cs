using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using RanorexReleaseNotesCheck.Helper;

namespace RanorexReleaseNotesCheck.Abstracts
{
    public class AbstractWebPage
    {
        protected IWebDriver wd;
        private const int PAGELOADTIME = 30;
		private const int IMPLICITWAIT = 10;
		private const int SCRIPTTIMEOUT = 7;
        private List<By> requiredItems = new List<By>();

        protected AbstractWebPage(IWebDriver webDriver)
        {
			this.wd = webDriver;
            this.wd.Manage().Timeouts().PageLoad 				= TimeSpan.FromSeconds(PAGELOADTIME);
			this.wd.Manage().Timeouts().AsynchronousJavaScript 	= TimeSpan.FromSeconds(SCRIPTTIMEOUT);
			wd.Manage().Timeouts().ImplicitWait 				= TimeSpan.FromSeconds(IMPLICITWAIT);
        }

        public virtual List<By> GetRequiredItems()
        {
			return this.requiredItems;
		}

        public void ValidatePageType() {
			bool result = true;
			List<By> rqItems = this.GetRequiredItems();

			for (int i = 0; i < rqItems.Count; i++)
            {
				By element = rqItems[i];
				try
                {
					this.GetElement(element);
				}
				catch (Exception) 
                {
					result = false;
				}
			}
			Assert.AreEqual(true, result, "Validation for PageObjects (required items) failed");
		}

        protected IWebElement? GetElement(By by, int secondsToWait = 60) {
			return WaitUntilElementExists(by, secondsToWait);
		}

        /// <summary>
		/// /// Checks if specified Element exists.
		/// </summary>
		/// <param name="by">Element to search</param>
		/// <returns>Found Element, otherwise null.</returns>
		public IWebElement? FindElementOrDefault(By by)
		{
			try
			{
				return  wd.FindElement(by);
			}
			catch (StaleElementReferenceException)
			{
				return default;
			}
			catch (NoSuchElementException)
			{
				return default;
			}
		}

        /// <summary>
		/// Lets you wait for an Element to be existing.
		/// </summary>
		/// <param name="by">Element to be checked</param>
		/// <param name="SecondsToTimeOut">Time in seconds until method times out.</param>
		/// <returns>The first found WebELement, if existing. Otherwise return null.</returns>
		public IWebElement? WaitUntilElementExists(By by, int SecondsToTimeOut = 10)
		{
			using (WithImplicitWait.FromSeconds(wd, 0))
			{
				return GetWait(SecondsToTimeOut).Until(_ => FindElementOrDefault(by));
			}
		}

        public WebDriverWait GetWait(int SecondsToTimeOut)
        {
			return new WebDriverWait(wd, TimeSpan.FromSeconds(SecondsToTimeOut));
		}

        public WebDriverWait GetWait()
        {
			return GetWait(PAGELOADTIME);
		}
    }
}