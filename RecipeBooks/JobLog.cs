using System;
using System.Diagnostics;
using System.IO;

namespace RecipeBooks
{
    public class Joblog
    {
        private DateTime currentTime = new DateTime();
        private string fileName;
        private string fName;

        /// <summary>
        /// Creates a Joblob for the Program
        /// </summary>
        /// <param name="FileName">Name of the Joblog (normally the App Name)</param>
        /// <param name="daysToKeep">Number of Days to keep the logs</param>
        public Joblog(string FileName, int daysToKeep = 7)
        {
            // create a file name containing the date and time
            fName = FileName;
            currentTime = DateTime.Now;

            DeleteOldLogs(daysToKeep);

            CreateFileName(fName, currentTime);
        }
        //*********************************
        /// <summary>
        /// Informational Message to be sent to the JobLog
        /// </summary>
        /// <param name="header">Main Message</param>
        /// <param name="detail">Detail Message</param>
        public void InformationMessage(string header = "", string detail = "")
        {
            AddMessage("Information", header, detail);
        }
        //*********************************
        /// <summary>
        ///  Warning Message to be sent to the JobLog
        /// </summary>
        /// <param name="header">Main Message</param>
        /// <param name="detail">Detail Message</param>
        public void WarningMessage(string header = "", string detail = "")
        {
            AddMessage("Warning", header, detail);
        }
        //*********************************
        /// <summary>
        ///  Error Message to be sent to the JobLog
        /// </summary>
        /// <param name="header">Main Message</param>
        /// <param name="detail">Detail Message</param>
        public void ErrorMessage(string header = "", string detail = "")
        {
            AddMessage("Error", header, detail);
        }
        //*********************************
        //*********************************
        //*********************************
        /// <summary>
        /// Formats the filename to have the current Date\Time on the end of it
        /// </summary>
        /// <param name="f">Name to use</param>
        /// <param name="dt">Current Date\Time</param>
        private void CreateFileName(string f, DateTime dt)
        {
            // create a file name containing the date and time
            fileName = f + "-" + dt.ToString("yyyyMMddHHmm") + ".Log";
        }
        //*********************************
        /// <summary>
        /// Formats the text to be added to the JobLog before Printing
        /// </summary>
        /// <param name="errType">Message Type</param>
        /// <param name="header">Main Message</param>
        /// <param name="detail">Detail Message</param>
        private void AddMessage(string errType, string header, string detail)
        {
            // find out the current time
            currentTime = DateTime.Now;

            // add the current time to the log entry types and a
            string textHeader = "";
            string textDetail = "";

            // if there is a header entry add that to the log
            if (header != "")
            {
                textHeader = currentTime.ToLongTimeString() + " [" + errType + "] - ";
                textHeader += header;
            }

            // if there is a detail entry add that to the log
            if (detail != "")
            {
                textDetail = currentTime.ToLongTimeString() + " -- ";
                textDetail += detail;
            }

            PrintLogEntry(textHeader, textDetail);
        }
        //*********************************
        /// <summary>
        /// Print the messages to the JobLog
        /// </summary>
        /// <param name="header">Main Message</param>
        /// <param name="detail">Detail Message</param>
        private void PrintLogEntry(string header, string detail)
        {
            // Print the log to a text file
            using (StreamWriter outputFile = new StreamWriter(fileName, append: true))
            {
                if (header != "")
                {
                    outputFile.WriteLine(header);
                }

                if (detail != "")
                {
                    outputFile.WriteLine(detail);
                }
            }
        }
        //*********************************
        /// <summary>
        /// Delete's the Old JobLog files based on the number of days to keep them.
        /// </summary>
        /// <param name="daysToKeep">Number of days to keep the JobLogs</param>
        private void DeleteOldLogs(int daysToKeep)
        {
            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string filter = "*.log";

            string[] files = Directory.GetFiles(folder, filter);

            DateTime keepDate = DateTime.Today.AddDays(-daysToKeep);

            foreach (var file in files)
            {
                string bit = file.Substring(file.Length - 16, 8);

                int Year = int.Parse(bit.Substring(0, 4));
                int Month = int.Parse(bit.Substring(4, 2));
                int Day = int.Parse(bit.Substring(6, 2));

                DateTime fileDate = new DateTime(Year, Month, Day, 0, 0, 0);

                if (fileDate < keepDate)
                {
                    File.Delete(file);
                }
            }
        }
    }
}