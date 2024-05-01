using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InterviewTask
{
    public static class LogUtility//Log file.
    {
        public static string logfile;
        static LogUtility()//Initial definition of log file.
        {
            DateTime td = new DateTime();
            td = DateTime.Now;//Current time.
            String tds = td.Year + "" + td.Month + "" + td.Day + " T " + td.Hour + "" + td.Minute + "" + td.Second + "_" + td.Millisecond;//onvert time to text.
            logfile = @"C:\InterviewTask\Logs\s3_words_" + tds + ".txt";//Definition the file + time.
        }

        public static void AddToLog(string p)//Add to log file.
        {
            try
            {
                using (StreamWriter o = new StreamWriter(logfile, true)) //Opened file to write if not exist created new file.
                {
                    o.WriteLine(p + "\n");//Enter to file.
                    o.Close();//Closed file.
                    o.Dispose();
                }
            }
            catch(Exception e)
            {
                LogUtility.AddToLog("Exception message:" + e.Message);
                LogUtility.AddToLog("Exception StackTrace: " + e.StackTrace.ToString());
            }
        }
    }
}