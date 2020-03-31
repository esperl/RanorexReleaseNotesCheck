using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using RanorexReleaseNotesCheck.Abstracts;

namespace RanorexReleaseNotesCheck.Helper
{
    public class DatabaseHelper
    {
        private MySqlConnection connectionDB;
        private MySqlCommand? commandDB;
        private MySqlDataReader? reader;

        public DatabaseHelper()
        {
            string connectionData = "datasource="  + AbstractTestcase.adress   + ";" +
                                    "port="        + AbstractTestcase.port     + ";" +
                                    "username="    + AbstractTestcase.username + ";" +
                                    "password="    + AbstractTestcase.password + ";" +
                                    "database="    + AbstractTestcase.database + ";";
            connectionDB = new MySqlConnection(connectionData);
        }

        /// <summary>
        /// Gets the last known release from the database
        /// </summary>
        /// <returns>A string of the release name</returns>
        public string GetLastRelease()
        {
            string lastReleaseCommand = "SELECT release_name FROM release_notes " +
                                        "WHERE id=(" +
                                        "SELECT MAX(id) FROM release_notes);";
            var result = ExecuteCommand(lastReleaseCommand);
            return result[0];
        }

        /// <summary>
        /// Gets the last release from the database. First element is the release name, followed by alternating between the header
        /// and a list of the notes from the corresponding header. {release_name, change_header1, change_notes1, change_header2, changes_notes2, ...}
        /// </summary>
        /// <returns>The last release as a list containing lists of strings</returns>
        public List<List<string>> GetLastReleaseNotes()
        {
            var lastReleaseNotes = new List<List<string>> {
                new List<string>{GetLastRelease()}
            };
            
            string headerCommand = "SELECT DISTINCT change_header FROM release_notes " + 
                                   "WHERE release_name=\"" + lastReleaseNotes[0][0] + "\";";
            var changeHeader = ExecuteCommand(headerCommand);

            foreach (string header in changeHeader)
            {
                lastReleaseNotes.Add(new List<string>{header});
                
                string noteCommand = "SELECT change_note FROM release_notes " + 
                                     "WHERE change_header=\"" + header + "\"" +
                                     "AND release_name=\"" + lastReleaseNotes[0][0] + "\";";
                lastReleaseNotes.Add(ExecuteCommand(noteCommand));
            }

            return lastReleaseNotes;
        }

        /// <summary>
        /// Updates the database with the new found release and the corresponding notes
        /// </summary>
        /// <param name="newRelease">List of the new Release notes</param>
        public void UpdateDatabase(List<List<string>> newRelease)
        {
            for (int i = 2; i < newRelease.Count; i = i + 2)
            {
                foreach (string note in newRelease[i])
                {
                    string updateCommand = "INSERT INTO release_notes (release_name, change_header, change_note) " +
                                           "VALUES (\"" + newRelease[0][0] + "\", \"" + newRelease[i - 1][0] +  "\", \"" + note + "\");";
                    ExecuteCommand(updateCommand);
                }
            }
        }

        /// <summary>
        /// Enables connection to the database and executes a sql command
        /// </summary>
        /// <param name="commandString">A string containing a sql command</param>
        /// <returns>A string containing the results</returns>
        private List<string> ExecuteCommand (string commandString)
        {
            commandDB = new MySqlCommand(commandString, connectionDB);
            List<string> result = new List<string>();
            try
            {
                //Open the database
                connectionDB.Open();

                //Execute the query
                reader = commandDB.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //As our database, the array will contain : ID 0, RELEASE_NAME 1, CHANGE_HEADER 2, CHANGE_NOTE 3
                        result.Add(reader.GetString(0));
                    }
                }
                connectionDB.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            return result;
        }
    }
}