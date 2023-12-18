// credit: Palash Sachan
// https://github.com/PalashSachan/Internet-Access-Checker/blob/master/Internet.cs

namespace AddedContent.Firebreak
{
    /// <summary>
    /// class to check the internet access
    /// </summary>
    public static class Internet
    {
        // "What is this IP?"
        // see: https://www.reddit.com/r/pihole/comments/edvpkn/what_is_wwwmsftncsicom_its_showing_up_every/?utm_source=share&utm_medium=web2x&context=3
        private const string DEFAULT_IP = "www.github.com";
        private const int DEFAULT_TIMEOUT = 2000;

        /// <summary>
        /// Checks the Internet access with default ip = "www.github.com" 
        /// and default timeout of 2000 Milliseconds.
        /// </summary>
        public static bool IsAvailable()
        {
            return DoCheck(DEFAULT_IP, DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Checks the Internet access with IP address and default timeout.
        /// </summary>
        /// <param name="ip">IP address to check the Internet access</param>
        public static bool IsAvailable(string ip)
        {
            return DoCheck(ip, DEFAULT_TIMEOUT);
        }
        
        /// <summary>
        /// Checks the Internet access with default ip and timeout in Milliseconds.
        /// </summary>
        /// <param name="timeout">timeout to check and wait for the response</param>
        public static bool IsAvailable(int timeout)
        {
            return DoCheck(DEFAULT_IP, timeout);
        }

        /// <summary>
        /// Checks the Internet access with IP and Timeout in Milliseconds.
        /// </summary>
        /// <param name="ip">IP address to check the Internet access</param>
        /// <param name="timeout">timeout to check and wait for the response</param>
        public static bool IsAvailable(string ip, int timeout)
        {
            return DoCheck(ip, timeout);
        }

        /// <summary>
        /// Checks the Internet access with IP and Timeout in Milliseconds.
        /// </summary>
        /// <param name="ip">IP address to check the Internet access</param>
        /// <param name="timeout">timeout to check and wait for the response</param>
        private static bool DoCheck(string ip, int timeout)
        {
            try
            {
                return new System.Net.NetworkInformation.Ping().Send(ip, timeout)?.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}