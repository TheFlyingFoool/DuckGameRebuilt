using System;
using System.IO;
using System.Net;
using System.Text;

namespace CrashWindow// this is a replacement for the class DG used to use inside the CrashWindow.exe felt weird to have it be circularly depent 
{
    public static class CrashWindow
    {
        public static string SQLEncode(string plainText)
        {
            return plainText.Replace("+", "@").Replace("/", "_");
        }

        public static string SQLDecode(string plainText)
        {
            return plainText.Replace("@", "+").Replace("_", "/");
        }

        public static string Base64SQLEncode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText)).Replace("+", "@").Replace("/", "_");
        }

        public static string Base64SQLDecode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData.Replace("@", "+").Replace("_", "/"));
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void SendBugReport(string pVersion, string pMods, string pAssembly, string pException, string pLogMessage, string pComment)
        {
            SendBugReportBase64(pVersion, Base64SQLEncode(pMods), Base64SQLEncode(pAssembly), Base64SQLEncode(pException), Base64SQLEncode(pLogMessage), Base64SQLEncode(pComment));
        }

        public static void SendBugReportBase64(string pVersionNonB64, string pModsB64, string pAssemblyB64, string pExceptionB64, string pLogMessageB64, string pCommentB64)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.wonthelp.info/DuckWeb/logBug.php");
            httpWebRequest.Method = "POST";
            string data = "sendRequest=DGBugLogger";
            data = data + "&version=" + pVersionNonB64.ToString();
            data = data + "&mods=" + pModsB64;
            data = data + "&assembly=" + pAssemblyB64;
            data = data + "&error=" + pExceptionB64;
            data = data + "&log=" + pLogMessageB64;
            data = data + "&comment=" + pCommentB64;
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            httpWebRequest.ContentLength = byteArray.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(byteArray, 0, byteArray.Length);
            requestStream.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
    }
}
