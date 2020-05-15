using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.MobileControls;
using System.IO;

namespace PaymentProject
{
    class LogHelper
    {
        static string path = @"C:\Log";

        //static string path = @"D:\ningnong-service\NingnongProject\Log";

        public static string Paht
        {
            get { return path; }
            set { path = value; }
        }
        /// <summary>
        /// Write line to log file.
        /// </summary>
        public static void WriteLine()
        {
            WriteLog("-----------------------------------------------------------------------");
        }

        /// <summary>
        /// Write log with Exception
        /// </summary>
        /// <param name="er">Exception error</param>
        public static void WriteErrorLog(Exception er)
        {
            WriteErrorLog(null, "", er);
        }

        /// <summary>
        /// Write log with Exception
        /// </summary>
        /// <param name="infomation">More infomation</param>
        /// <param name="er">Exception error</param>
        public static void WriteErrorLog(string infomation, Exception er)
        {
            WriteErrorLog(null, infomation, er);
        }

        /// <summary>
        /// Write log with Exception and Form
        /// </summary>
        /// <param name="frm">Form error</param>
        /// <param name="er">Exception error</param>
        public static void WriteErrorLog(Form frm, Exception er)
        {
            WriteErrorLog(frm, "", er);
        }

        /// <summary>
        /// Write log with Exception and Form
        /// </summary>
        /// <param name="frm">Form error</param>
        /// <param name="er">Exception error</param>
        public static void WriteErrorLog(Form frm, string infomation, Exception er)
        {
            string formName = "";
            if (frm != null) formName = frm.GetType().FullName;
            if (!string.IsNullOrEmpty(formName)) WriteLog("Error!!!" + formName + ">>>" + infomation + er.TargetSite.Name + ">>>" + er.Message);
            else WriteLog("Error!!!" + infomation + " " + er.TargetSite.Name + ">>>" + er.Message);
        }

        /// <summary>
        /// Write information to log file.
        /// </summary>
        /// <param name="frm">Form write log</param>
        /// <param name="information">Information need to write</param>
        public static void WriteLog(Form frm, string information)
        {
            string formName = "";
            if (frm != null) formName = frm.GetType().FullName;
            if (!string.IsNullOrEmpty(formName)) WriteLog(formName + ">>>" + information);
            else WriteLog(information);
        }

        /// <summary>
        /// Write information to log file.
        /// </summary>
        /// <param name="information">Information need to write</param>
        public static void WriteLog(string information)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Paht);
            }
            string pathFile = path + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                FileInfo file = new FileInfo(pathFile);
                if (!file.Exists)
                {
                    FileStream fileStream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Close();
                }
                StreamWriter sw = null;
                try
                {
                    sw = File.AppendText(pathFile);//new StreamWriter(path);
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + " ==> " + information);
                }
                catch (Exception er)
                {

                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception error)
            {

            }
        }
    }
}
