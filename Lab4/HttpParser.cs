using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class HttpParser
    {


        public string url { get; }

        public string host { get; }

        public string request { get; }


        public HttpParser(string url, string host)
        {
            this.url = url;
            this.host = host;

            request = "GET " + url + " HTTP/1.1\r\n" +
                   "Host: " + host + "\r\n" +
                   "Content-Type: text/html\r\n"+
                   "Content-Length: 0\r\n\r\n";
        }

        public int GetContentLengthResponse(string response)
        {
            int length = 0;
            string[] dataResponse = response.Split('\r', '\n');
            foreach (string data in dataResponse)
            {
                string[] headerNameDetails = data.Split(':');

                if (headerNameDetails[0] == "Content-Length")
                {
                    length = int.Parse(headerNameDetails[1]);
                }
            }

            return length;
        }

    }
}
