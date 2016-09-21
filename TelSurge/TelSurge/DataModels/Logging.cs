using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TelSurge.DataModels
{
    public class Logging
    {
        public enum StatusTypes { Error, Running, Stopped }
        public string StatusMessage { get; set; }
        public string StatusDetail { get; set; }
        public StatusTypes StatusType { get; set; }
        public string URL { get; set; }

        public Logging()
        {
            StatusMessage = "";
            StatusDetail = "";
        }
        public Logging(string statusMessage, string statusDetail, string url, StatusTypes statusType)
        {
            StatusMessage = statusMessage;
            StatusDetail = statusDetail;
            URL = url;
            StatusType = statusType;
        }
        public bool Record()
        {
            if (URL == null || URL == "")
                return false;
            else
            {
                Record(URL);
                return true;
            }
        }
        public void Record(string url)
        {
            try
            {
                string[] logText = { StatusType.ToString().Replace(",", "-"), StatusMessage.Replace(",", "-"), StatusDetail.Replace(",", "-"), DateTime.Now.ToString() };
                if (!File.Exists(url))
                    CreateNewFile(url);
                File.AppendAllText(url, String.Join(",", logText) + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception)
            {

            }
        }
        private void CreateNewFile(string url)
        {
            string[] columns = { "Type", "Message", "Detail", "Date" };
            File.WriteAllText(url, String.Join(",", columns)+Environment.NewLine);
        }

        public static void WriteToFile(string txt, string url) 
        {
            if (!File.Exists(url))
                File.WriteAllText(url, "Time"+Environment.NewLine);
            File.AppendAllText(url, txt+Environment.NewLine);
        }
    }
}
