using System;
using OpenQA.Selenium;

namespace RanorexReleaseNotesCheck.Helper
{
	internal class WithImplicitWait : IDisposable
    {
		private TimeSpan defaultImplicitWait;
		public readonly IWebDriver driver;

		private WithImplicitWait(IWebDriver driver, TimeSpan waitTime)
        {
			this.driver =  driver;
			this.defaultImplicitWait = driver.Manage().Timeouts().ImplicitWait;
			driver.Manage().Timeouts().ImplicitWait = waitTime;
		}

		public void Dispose()
		{
			driver.Manage().Timeouts().ImplicitWait = this.defaultImplicitWait;
		}
		
		public static WithImplicitWait FromSeconds(IWebDriver driver, double seconds)
        {
			return new WithImplicitWait(driver, TimeSpan.FromSeconds(seconds));
		}
	}
}
