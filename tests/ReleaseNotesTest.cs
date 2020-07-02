using System.Collections.Generic;
using NUnit.Framework;
using RanorexReleaseNotesCheck.Abstracts;
using RanorexReleaseNotesCheck.PageObjects;
using RanorexReleaseNotesCheck.Helper;

namespace RanorexReleaseNotesCheck.tests
{
    [TestFixture]
    public class ReleaseNotesTest : AbstractTestcase
    {

        [Test]
        public void CheckForRelease()
        {
            TcCheckForRelease();
        }

        // [Test]
        public void TcCheckForRelease()
        {
            if (driver is null)
            {
                Assert.Fail("Driver not initialized");
                return;
            }

            ReleaseNotesPage releaseNotesPage   = new ReleaseNotesPage(driver);
            DatabaseHelper databaseHelper       = new DatabaseHelper();
            KeywordChecker keywordChecker       = new KeywordChecker();
            ReportHelper reportHelper           = new ReportHelper();

            var newRelease          = new List<List<string>>();
            var foundKeywords       = new List<string>();


            releaseNotesPage.ValidatePageType();

            if (releaseNotesPage.CheckForNewRelease())
            {
                newRelease = releaseNotesPage.GetNewReleaseNotes();
                databaseHelper.UpdateDatabase(newRelease);

                if (!checkKeywords)
                {
                    reportHelper.CreateReport(keywordChecker.keywords, databaseHelper.GetLastReleaseNotes(), checkKeywords);
                    Assert.Fail("Found Release of Ranorex differs from the archived: \n" +
                                "Archived Release: \t" + databaseHelper.GetLastRelease() + "\n" + 
                                "Found Release: \t" + newRelease[0][0] + "\n\n" + 
                                "Actions are needed!");
                }
                else 
                {
                    foundKeywords = keywordChecker.checkReleaseNotes(newRelease);
                }
                if (foundKeywords.Count > 0)
                {
                    reportHelper.CreateReport(foundKeywords, databaseHelper.GetLastReleaseNotes(), checkKeywords);
                    Assert.Fail("New Release contains keywords. Actions needed");
                }
                else
                {
                    Assert.Pass("There is a new release of Ranorex, but no keywords were matched");
                }
            }
            Assert.Pass("There is no new release of Ranorex");
        }
    }
}
