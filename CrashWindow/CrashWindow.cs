using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrashWindow
{
    public static class CrashWindow
    {
        public static string SQLEncode(string plainText)
        {
            return plainText.Replace("+", "@").Replace("/", "_");
        }
        public static string SQLDecode(string plainText)
        {
            return plainText.Replace("@", "+").Replace("_", "/"); ;
        }
        public static string Base64SQLEncode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string str = Convert.ToBase64String(plainTextBytes).Replace("+", "@").Replace("/", "_");
            return str;
        }
        public static string Base64SQLDecode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData.Replace("@", "+").Replace("_", "/"));
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void SendBugReport(string pVersion, string pMods, string pAssembly, string pException, string pLogMessage, string pComment)
        {
            SendBugReportBase64(pVersion, Base64SQLEncode(pMods), Base64SQLEncode(pAssembly), Base64SQLEncode(pException), Base64SQLEncode(pLogMessage), Base64SQLEncode(pComment));
        }

        public static void SendBugReportBase64(string pVersionNonB64, string pModsB64, string pAssemblyB64, string pExceptionB64, string pLogMessageB64, string pCommentB64)
        {
            string sendString = "http://www.wonthelp.info/DuckWeb/logBug.php";
            HttpWebRequest sendRequest = (HttpWebRequest)WebRequest.Create(sendString);
            sendRequest.Method = "POST";

            string data = "sendRequest=DGBugLogger";
            data += "&version=" + pVersionNonB64.ToString();
            data += "&mods=" + pModsB64;
            data += "&assembly=" + pAssemblyB64;
            data += "&error=" + pExceptionB64;
            data += "&log=" + pLogMessageB64;
            data += "&comment=" + pCommentB64;

            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            sendRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            sendRequest.ContentLength = byteArray.Length;

            Stream dataStream = sendRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            sendRequest.GetResponse();
        }
    }
}
