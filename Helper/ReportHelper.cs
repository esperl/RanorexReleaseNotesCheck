using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace RanorexReleaseNotesCheck.Helper
{
    public class ReportHelper
    {
        private ExcelPackage excelPackage;
        private FileInfo? excelFile;
        private string reportDir;
        private string keywordsDir;

        public ReportHelper()
        {
            this.reportDir = System.IO.Directory.CreateDirectory(Path.Combine("..", "..", "..", "Reports")).FullName;
            this.keywordsDir = System.IO.Directory.CreateDirectory(Path.Combine("..", "..", "..", "Keywords")).FullName;

            this.excelPackage = new ExcelPackage();
        }

        /// <summary>
        /// Create an excel report of the new found 
        /// </summary>
        /// <param name="keywords">A list of words to check the notes with</param>
        /// <param name="changeNotes">A list of the new change notes</param>
        public void CreateReport(List<string> keywords, List<List<string>> changeNotes, bool doCheckKeywords = false)
        {
            var date = DateTime.Now.ToString().Replace(":", "-");
            var reportPath = Path.GetFullPath(Path.Combine(reportDir, date + "_ReleaseNotesReport.xlsx"));

            this.excelFile  = new FileInfo(reportPath);
            this.excelPackage.Workbook.Worksheets.Add("Release Notes");
            var reportWorksheet = excelPackage.Workbook.Worksheets["Release Notes"];

            var headerRow = new List<string[]> { new string[] {"Release", "Change Headline", "Change Notes"} };
            var headerRange = "A1:" + char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

            reportWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
            reportWorksheet.Cells[2, 1].LoadFromArrays(new List<string[]> {changeNotes[0].ToArray()});

            int row = 2;
            for (int i = 1; i < changeNotes.Count; i++)
            {
                if (i % 2 == 1)
                {
                    if (doCheckKeywords && (keywords.Any(keyword => changeNotes[i][0].IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)))
                    {
                        HighlightNote(reportWorksheet, row, 2);
                    }
                    reportWorksheet.Cells[row++, 2].Value = changeNotes[i][0];
                }
                else
                {
                    for (int j = 0; j < changeNotes[i].Count; j++)
                    {
                        if (doCheckKeywords && (keywords.Any(keyword => changeNotes[i][j].IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)))
                        {
                            HighlightNote(reportWorksheet, row, 3);
                        }
                        reportWorksheet.Cells[row++, 3].Value = changeNotes[i][j];
                    }
                }
            }
            AutoFitColumns(reportWorksheet, headerRow[0].Length);
            excelPackage.SaveAs(excelFile);
        }

        /// <summary>
        /// Creates a list of keywords based on the keywords.xlsx file
        /// </summary>
        /// <returns>A list of keywords</returns>
        public List<string> GetKeywords()
        {
            List<string> result = new List<string>();

            var keywordsPath = Path.GetFullPath(Path.Combine(keywordsDir, "Keywords.xlsx"));
            this.excelFile = new FileInfo(@keywordsPath);
            this.excelPackage = new ExcelPackage(excelFile);
            var keywordsWorksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();

            // both variables return the first found value (most upperleft) in the excel file
            int firstRow    = keywordsWorksheet.Dimension.Start.Row;
            int firstColumn = keywordsWorksheet.Dimension.Start.Column;

            // Since firstRow / -Column return the first found value both row and column must be subtracted by one (Excel file rows/columns start by 1 not 0) 
            int rows        = firstRow      + keywordsWorksheet.Dimension.Rows - 1;
            int columns     = firstColumn   + keywordsWorksheet.Dimension.Columns - 1;

            for (int i = firstRow; i <= rows; i++)
            {
                for (int j = firstColumn; j <= columns; j++)
                {
                    var keyword = keywordsWorksheet.Cells[i, j].GetValue<string?>();
                    if (keyword != null)
                    {
                        result.Add(keyword);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Fits the columns to the inserted data.
        /// </summary>
        /// <param name="reportWorksheet">The worksheet you want to edit</param>
        /// <param name="columnCount">The count of columns to autofit</param>
        private void AutoFitColumns(ExcelWorksheet reportWorksheet, int columnCount)
        {
            for (int i = 1; i <= columnCount; i++)
            {
                reportWorksheet.Column(i).AutoFit();
            }
        }

        /// <summary>
        /// Sets text color to red and if text is a 'change header' also sets font to 'bold'
        /// </summary>
        /// <param name="worksheet">The worksheet you want to edit</param>
        /// <param name="row">Affected row in worksheet</param>
        /// <param name="column">Affected column in worksheet</param>
        private void HighlightNote(ExcelWorksheet worksheet, int row, int column)
        {
            worksheet.Cells[row, column].Style.Font.Color.SetColor(System.Drawing.Color.Red);
            if (column == 2)
            {
                worksheet.Cells[row, column].Style.Font.Bold = true;
            }
        }
    }
}