using System.Collections.Generic;
using OpenQA.Selenium;
using RanorexReleaseNotesCheck.Abstracts;
using RanorexReleaseNotesCheck.Helper;

namespace RanorexReleaseNotesCheck.PageObjects
{
    public class ReleaseNotesPage : AbstractWebPage
    {
        private DatabaseHelper dbHelper;
        private bool rqItemsLoaded;
		private List<By> requiredForClass;

        public ReleaseNotesPage(IWebDriver wd) :base(wd)
        {
            this.dbHelper = new DatabaseHelper();
            rqItemsLoaded = false;
            requiredForClass  = new List<By>()
            {
                By.XPath("//div[@class='page-header']//h1[text()='Release Notes']")
            };
        }

        public override List<By> GetRequiredItems()
        {
			List<By> temp = base.GetRequiredItems();
			if (rqItemsLoaded)
            {
                return temp;
            }
			foreach (By element in requiredForClass)
            {
				temp.Add(element);
			}
			rqItemsLoaded = true;
			return temp;
		}


        /// <summary>
        /// Gets all listed releases
        /// </summary>
        /// <returns>A list with all names as IWebElements</returns>
        public List<IWebElement> GetAllReleases()
        {
            var result = new List<IWebElement>();
            string releasePath = "//h5[contains(@class,'title')]";

            WaitUntilElementExists(By.XPath(releasePath));
            var releaseList = wd.FindElements(By.XPath(releasePath));

            result.AddRange(releaseList);

            return result;
        }

        /// <summary>
        /// Gets the name of the most upper listed release
        /// </summary>
        /// <returns>IWebElement of the last release</returns>
        public IWebElement GetNewRelease()
        {
            string releasePath = "//h5[contains(@class,'title')]";
            WaitUntilElementExists(By.XPath(releasePath));
            return wd.FindElement(By.XPath(releasePath));
        }

        /// <summary>
        /// Gets the last release and compares it to the last archived one
        /// </summary>
        /// <returns>True if there is a new release, false if not</returns>
        public bool CheckForNewRelease()
        {
            string archivedRelease  = dbHelper.GetLastRelease();
            string newRelease       = GetNewRelease().Text;
            return !archivedRelease.Equals(newRelease);
        }

        /// <summary>
        /// Opens the chosen release tab and saves the changes in a list.
        /// First list is the release name, then it alternates between a list of the change header and a list of the corresponding change notes.
        /// Example: {Release_Name, Change_Header1, Change_Notes1, Change_Header2, Change_Notes2, Change_Header3, ...}
        /// </summary>
        /// <returns>A list of a list of strings</returns>
        public List<List<string>> GetReleaseNotes(IWebElement releaseName)
        {
            var release = new List<string> {releaseName.Text};
            var result  = new List<List<string>> {release};
            
            releaseName.Click();

            string headLinesXPath = "//h5[contains(@class,'title') and contains(text(), '" + releaseName.Text + "')]//..//p";
            WaitUntilElementExists(By.XPath(headLinesXPath));
            var headLines = wd.FindElements(By.XPath(headLinesXPath));

            foreach (IWebElement headline in headLines)
            {
                var changeHeaders = new List<string> {headline.Text};
                var changeNotes = new List<string>();

                string changesXpath = headLinesXPath + "//strong[contains(text(),'" + headline.Text + "')]//parent::p//following-sibling::ul[1]/li";
                var changes = wd.FindElements(By.XPath(changesXpath));

                foreach (IWebElement change in changes)
                {
                    changeNotes.Add(change.Text);
                }
                result.Add(changeHeaders);
                result.Add(changeNotes);
            }
            return result;
        }

        /// <summary>
        /// Opens the newest release tab by selecting the uppermost release and saves the changes in a list.
        /// First list is the release name, then it alternates between a list of the change header and a list of the corresponding change notes.
        /// Example: {Release_Name, Change_Header1, Change_Notes1, Change_Header2, Change_Notes2, Change_Header3, ...}
        /// </summary>
        /// <returns>A list of a list of strings</returns>
        public List<List<string>> GetNewReleaseNotes()
        {
            var newRelease = GetNewRelease();
            return GetReleaseNotes(newRelease);
        }
    }
}