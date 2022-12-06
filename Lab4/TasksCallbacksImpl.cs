using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class TasksCallbacksImpl
    {
        private List<string> urls;

        public TasksCallbacksImpl(List<string> urls)
        {
            this.urls = urls;
        }

        public void Run()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < urls.Count; i++)
            {
                int k = i;
                //StartSocket(urls[i], i);
                tasks.Add(Task.Run(()=>StartSocket(urls[k],k)));
            }

            Task.WhenAll(tasks).Wait();
        }

        public Task StartSocket(string url, int id)
        {
            var IPHostDetails = Dns.GetHostEntry(url.Split('/')[0]); // DNS entry for host
            var IPAddress = IPHostDetails.AddressList[0]; // host IP
            var endpoint = new IPEndPoint(IPAddress, 80);  // endpoint used for socket connection, port 80 default port for http, 443 for https
            var parser = new HttpParser(url.Contains("/") ? url.Substring(url.IndexOf("/")) : "/", url.Split('/')[0]);

            SocketConnection socketClient = new SocketConnection(id, parser, endpoint, IPAddress);
       
            socketClient.BeginConnectWithTask().Wait();
            socketClient.BeginSendWithTask().Wait();
            socketClient.BeginReceiveWithTask().Wait();
            socketClient.ShutdownAndClose();

            return Task.CompletedTask;
        }
       



    }
}

