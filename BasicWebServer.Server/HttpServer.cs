using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server
{ 
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        private readonly RoutingTable routingTable;

            
        public HttpServer(
            string ipAddress,
            int port,
            Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, port);// Now we need to make the browser connect to this address and port and to our app.
                                                                        // To do this, we will use the TCPListener class, which allows us to accept requests from the browser:
            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public object MapGet(string v, HtmlResponse htmlResponse)
        {
            throw new NotImplementedException();
        }

        public HttpServer(
            int port,
            Action<IRoutingTable> routingTable)
            :this("127.0.0.1", port, routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable)
            :this(8080, routingTable)
        {
        }

        public async Task Start()
        {
            serverListener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine($"Listening for requests...");

            while (true)
            {
                var connection = await serverListener.AcceptTcpClientAsync(); //We need to make our server wait for the browser to connect to it.

                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream(); //first we need to create a stream, through which data is received or sent to the browser as a byte array.

                    var requestText = await this.ReadRequest(networkStream);

                    Console.WriteLine(requestText);

                    var request = Request.Parse(requestText);

                    var response = this.routingTable.MatchRequest(request);

                    //Execute pre-render action for the response
                    if (response.PreRenderAction != null)
                    {
                        response.PreRenderAction(request, response);
                    }

                    AddSession(request, response);

                    await WriteResponse(networkStream, response);//Create a message, which will be sent

                    connection.Close(); //At the end, it is important that we close the connection to the browser or it may remain open and other connections to the server will fail.
                });

            }


        }

        private static void AddSession(Request request, Response response)
        {
            var sessionExists = request.Session
                .ContainsKey(Session.SessionCurrentDateKey);

            if (!sessionExists)
            {
                request.Session[Session.SessionCurrentDateKey]
                    = DateTime.Now.ToString();
                response.Cookies
                    .Add(Session.SessionCookieName, request.Session.Id);
            }
        }

        private async Task WriteResponse(NetworkStream networkStream, Response response) //As you can see, we have a special method for writing the response in the network stream.
        {

            var responseBytes = Encoding.UTF8.GetBytes(response.ToString()); //Write the response and convert it to a byte array.

            await networkStream.WriteAsync(responseBytes); //Use the network stream to send the response bytes to the browser.
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024; //Our buffer for reading will have a length of 1024 bytes
            var buffer = new byte[bufferLength]; //will be a byte array

            var totalBytes = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength); //we will create a buffer to read the request in parts, as our server may crash if the request is too large. 

                totalBytes += bytesRead;

                if (totalBytes > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            }
            //May not work correctly over the Internet.
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }
    }
}
