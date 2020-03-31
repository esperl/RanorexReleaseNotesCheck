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
        public static string password   = "Andagon#01";
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

	}
}