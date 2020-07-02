using NUnit.Framework;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;

namespace RanorexReleaseNotesCheck.Abstracts
{
    [TestFixture]
	public class AbstractTestcase
	{
		protected IWebDriver? driver;
        //Default: true
        public bool checkKeywords       = true;
        //Default: "127.0.0.1"
        public static string adress     = "127.0.0.1";
        //Default: "3306"
        public static string port       = "3306";
        //Default: "root"
        public static string username   = "root";
        //Default: ""
        public static string password   = "";
        //Default: "release_notes_check"
        public static string database   = "release_notes_check";

        [SetUp]
        public void Init()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("intl.accept_languages", "en");
            driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);

            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.ranorex.com/release-notes/");
        }

        [TearDown]
        public void Cleanup()
        {
            driver?.Quit();
        }

        public void runTest(Action testFunction) 
        {
			try 
            {
				testFunction();
			}
			catch (Exception e) 
            {
				//The following code checks if the session got lost and the current window is a login-screen (which should never happen in the focus())
				//			 	if (wd.FindElements(By.XPath(".//*[@id='Button_CD']/span[text()='Log In']")).Count>0){
				if (driver!.FindElements(By.XPath(".//*[@id='submitButton']")).Count > 0) 
                {
                    Assert.Fail("!!Login-Screen displayed!! Original Exception in next Message");
					// Rep.Error("!!Login-Screen displayed!! Original Exception in next Message");
				}

                Assert.Fail("Test failed with uncatched exception, see next message", true);
                Assert.Fail(e.Message + " Stacktrace: " + e.StackTrace + " Inner Exception:" + e.InnerException);
				// Rep.Error("Test failed with uncatched exception, see next message", true);
				// Rep.Error(e.Message + " Stacktrace: " + e.StackTrace + " Inner Exception:" + e.InnerException);
#if DEBUG
				throw;
#else
				Assert.Fail();
#endif
			}
		}

	}
}