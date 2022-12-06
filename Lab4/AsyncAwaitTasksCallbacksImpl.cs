using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class AsyncAwaitTasksCallbacksImpl
    {
        private List<string> urls;

        public AsyncAwaitTasksCallbacksImpl(List<string> urls)
        {
            this.urls = urls;
        }

        public void Run()
        {
            List<Task> tasks = new List<Task>();
            for (int k = 0; k < urls.Count; k++)
            {
                int i = k;
                //StartSocket(urls[k], k);
                tasks.Add(Task.Run(() => StartSocket(urls[i], i)));
            }

            Task.WhenAll(tasks).Wait();
        }

        public async Task StartSocket(string url, int id)
        {
            var IPHostDetails = Dns.GetHostEntry(url.Split('/')[0]); // DNS entry for host
            var IPAddress = IPHostDetails.AddressList[0]; // host IP
            var endpoint = new IPEndPoint(IPAddress, 80);  // endpoint used for socket connection, port 80 default port for http, 443 for https
            var parser = new HttpParser(url.Contains("/") ? url.Substring(url.IndexOf("/")) : "/", url.Split('/')[0]);
     
            SocketConnection socketClient = new SocketConnection(id, parser, endpoint, IPAddress);
            await socketClient.BeginConnectWithTask();
            await socketClient.BeginSendWithTask();
            await socketClient.BeginReceiveWithTask();

            socketClient.ShutdownAndClose();
        }


    }
}
