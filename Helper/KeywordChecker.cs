using System;
using System.Collections.Generic;
using System.Linq;

namespace RanorexReleaseNotesCheck.Helper
{
    public class KeywordChecker
    {
        public List<string> keywords {get;}
        private DatabaseHelper dbHelper;
        private ReportHelper reportHelper;

        public KeywordChecker()
        {
            this.dbHelper       = new DatabaseHelper();
            this.reportHelper   = new ReportHelper();
            this.keywords       = reportHelper.GetKeywords();
        }

        /// <summary>
        /// Checks the new found release notes for the set keywords. Sets criticalChange = true if at least one 
        /// keyword was found and returns a list of the critical change notes
        /// </summary>
        /// <param name="releaseNotes">A list containing lists of the new release notes</param>
        /// <returns>A list containing the critical change notes determined by the set keywords</returns>
        public List<string> checkReleaseNotes(List<List<string>> releaseNotes)
        {
            var foundKeywords = new List<string>();
            foreach (string keyword in this.keywords)
            {
                foreach (var changeNote in releaseNotes)
                {
                    foreach (string note in changeNote)
                    {
                        if (note.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (!foundKeywords.Contains(keyword))
                            {
                                foundKeywords.Add(keyword);
                            }
                        }
                    }
                }
            }
            return foundKeywords;
        }
    }
}