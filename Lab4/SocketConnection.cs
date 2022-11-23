using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class SocketConnection : Socket
    {
        private int id;
        private HttpParser _parser;
        private IPEndPoint endpoint;
        public string response { get; set; } = "";
        public SocketConnection(int id, HttpParser parser, IPEndPoint endpoint, IPAddress IPAddress) :
            base(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        {
            this.id = id;
            _parser = parser;
            this.endpoint = endpoint;
        }



        public void BeginConnectCallback()
        {
            BeginConnect(endpoint, BeginSendCallback, this);
        }

        public void BeginSendCallback(IAsyncResult ar)
        {
            EndConnect(ar); //EndConnect completes the operation started by BeginConnect. You need to pass the IAsyncResult created by the matching BeginConnect call. EndConnect will block the calling thread until the operation is completed.
            Console.WriteLine("Connected to: " + this._parser.host);

            byte[] requestString = Encoding.ASCII.GetBytes(_parser.request);

            BeginSend(requestString, 0, requestString.Length, SocketFlags.None, BeginReceiveCallback, this);
        }

        public void BeginReceiveCallback(IAsyncResult ar)
        {
            EndSend(ar);//EndSend completes the operation started by BeginSend. You need to pass the IAsyncResult created by the matching BeginSend call.
            byte[] bytesData = new byte[1024];
            response = "";
            Console.WriteLine("Sent request to: " + this._parser.host + this._parser.url);
            BeginReceive(bytesData, 0, 1024, SocketFlags.None, ar => DownloadFileFromResponseCallback(ar, bytesData), this);
        }

        public void DownloadFileFromResponseCallback(IAsyncResult ar, byte[] bytesData)

        {
            int responseBytesLength = EndReceive(ar);//EndReceive completes the operation started by BeginReceive. You need to pass the IAsyncResult created by the matching BeginReceive call.

            response += Encoding.ASCII.GetString(bytesData, 0, responseBytesLength);
            if (!response.Contains("</html>")) // read data until we reach the end of the page
            {
                BeginReceive(bytesData, 0, 1024, SocketFlags.None, ar2 =>
                    DownloadFileFromResponseCallback(ar2, bytesData), this);
                return;
            }
            else
            {
                Console.WriteLine("Received data from: " + this._parser.host + this._parser.url);
                int start = response.IndexOf("<html", 0);
                string responseHtml = response.Substring(start);
                using (var sw = new StreamWriter(@"..\..\..\files\file" + id + ".txt"))
                {
                    sw.WriteLine("Site URL: " + this._parser.host + this._parser.url+"\n");
                    foreach (var i in response.Split("\r\n"))
                    {
                        //Console.WriteLine(i);
                        sw.WriteLine(i);
                    }


                    sw.WriteLine("Content length is: " + (_parser.GetContentLengthResponse(response) == 0 ? responseHtml.Length : _parser.GetContentLengthResponse(response)));
                }
                
                Console.WriteLine("Content length from URL " + this._parser.host + this._parser.url + " is: "+(_parser.GetContentLengthResponse(response) == 0 ? responseHtml.Length : _parser.GetContentLengthResponse(response)));



            }

            ShutdownAndClose();
            Console.WriteLine("Disconnected from: " + this._parser.host);
            Console.WriteLine("---------------------------------------------------------------");
        }


        public void BeginConnectCallbackWithTask(Action<SocketConnection> onCompleted)
        {
            BeginConnect(endpoint, ar => { EndConnect(ar); onCompleted(this); Console.WriteLine("Connected to: " + this._parser.host); }, this);
        }


        public Task BeginConnectWithTask()
        {
            var taskCompletion = new TaskCompletionSource<SocketConnection>(); //This allows a call-back based API to be consumed as a Task based API.
            BeginConnectCallbackWithTask(taskCompletion.SetResult); //SetResult completes the returned Task, and sets it's Result to the value passed into the functions's call-back.
            return taskCompletion.Task;

        }

        public void BeginSendCallbackWithTask(Action<SocketConnection> onCompleted)
        {

            byte[] requestString = Encoding.ASCII.GetBytes(_parser.request);
            BeginSend(requestString, 0, requestString.Length, SocketFlags.None, ar =>
            {
                this.EndSend(ar);
                Console.WriteLine("Sent request to: " + this._parser.host + this._parser.url);
                onCompleted(this);
            }, this);
        }
        public Task BeginSendWithTask()
        {
            var taskCompletion = new TaskCompletionSource<SocketConnection>();
            BeginSendCallbackWithTask(taskCompletion.SetResult);
            return taskCompletion.Task;
        }

        public void BeginReceiveCallbackWithTask(Action<SocketConnection> onCompleted)
        {

            var bytesData = new byte[1024];
            response = "";
            BeginReceive(bytesData, 0, 1024, SocketFlags.None, ar => DownloadFileFromResponseCallbackWithTasks(ar, bytesData, onCompleted), this);

        }
        public Task BeginReceiveWithTask()
        {
            var taskCompletion = new TaskCompletionSource<SocketConnection>();
            BeginReceiveCallbackWithTask(taskCompletion.SetResult);
            return taskCompletion.Task;
        }
        public void DownloadFileFromResponseCallbackWithTasks(IAsyncResult ar, byte[] buffer, Action<SocketConnection> onCompleted)

        {
            int responseBytesLength = EndReceive(ar);//EndReceive completes the operation started by BeginReceive. You need to pass the IAsyncResult created by the matching BeginReceive call.
            response += Encoding.ASCII.GetString(buffer, 0, responseBytesLength);
            if (!response.Contains("</html>"))
            {
                this.BeginReceive(buffer, 0, 1024, SocketFlags.None, ar2 =>
                     DownloadFileFromResponseCallbackWithTasks(ar2, buffer, onCompleted), this);
                return;
            }
            else
            {
                Console.WriteLine("Received data from: " + this._parser.host + this._parser.url);
                int start = response.IndexOf("<html", 0);
                string responseHtml = response.Substring(start);
                using (var sw = new StreamWriter(@"..\..\..\files\file" + id + ".txt"))
                {
                    sw.WriteLine("Site URL: " + this._parser.host + this._parser.url + "\n");
                    foreach (var i in response.Split("\r\n"))
                    {
                        //Console.WriteLine(i);
                        sw.WriteLine(i);
                    }


                    sw.WriteLine("Content length is: " + (_parser.GetContentLengthResponse(response) == 0 ? responseHtml.Length : _parser.GetContentLengthResponse(response)));

                }
                
                Console.WriteLine("Content length from URL "+ this._parser.host + this._parser.url + " is: " + (_parser.GetContentLengthResponse(response) == 0 ? responseHtml.Length : _parser.GetContentLengthResponse(response)));

                Console.WriteLine("---------------------------------------------------------------");


            }
            onCompleted(this);
            // ShutdownAndClose();
        }

        public void ShutdownAndClose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }

    }



}
