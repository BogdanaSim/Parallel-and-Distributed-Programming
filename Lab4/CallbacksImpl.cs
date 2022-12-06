using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class CallbacksImpl
    {

        private List<string> urls;

        public CallbacksImpl(List<string> urls)
        {
            this.urls = urls;
        }

        public void Run()
        {
            for (int i = 0 ; i < urls.Count; i++)
            {
                StartSocket(urls[i],i);
            }
        }

        public void StartSocket(string url,int id)
        {
            var IPHostDetails = Dns.GetHostEntry(url.Split('/')[0]); // DNS entry for host
            var IPAddress = IPHostDetails.AddressList[0]; // host IP
            var endpoint = new IPEndPoint(IPAddress, 80); // endpoint used for socket connection, port 80 default port for http, 443 for https
            var parser = new HttpParser(url.Contains("/") ? url.Substring(url.IndexOf("/")) : "/",url.Split('/')[0] );
         
            SocketConnection socketClient =new SocketConnection(id, parser, endpoint, IPAddress);
            socketClient.BeginConnectCallback();
            //Thread.Sleep(1000);
            do
            {
                Thread.Sleep(1000); //some sockets might not finish receiving all data so we wait until they are shut down
            }
            while (socketClient.Connected);
            // socketClient.ShutdownAndClose();
        }
       


    }
}
