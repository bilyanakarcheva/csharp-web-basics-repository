using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var iPAddress = IPAddress.Parse("127.0.0.1");
            var port = 8080;

            var serverListener = new TcpListener(iPAddress, port); // Now we need to make the browser connect to this address and port and to our app.
                                                                   // To do this, we will use the TCPListener class, which allows us to accept requests from the browser:
            serverListener.Start();

            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine($"Listening for requests...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient(); //We need to make our server wait for the browser to connect to it.

                var networkStream = connection.GetStream(); //first we need to create a stream, through which data is received or sent to the browser as a byte array.

                var content = "Hello from the server"; //Create a message, which will be sent
                var contentLength = Encoding.UTF8.GetByteCount(content); //get its length in bytes (bytes length is often different from the string length).

                var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength} 

{content}"; //Then, let’s construct our response. It should be in HTML format and have the "Content-Type" and "Content-Length" headers.
            //Note that you should not have excessive spaces in the response, as they break the HTML format.

                var responseBytes = Encoding.UTF8.GetBytes(response); //Write the response and convert it to a byte array.

                networkStream.Write(responseBytes); //Use the network stream to send the response bytes to the browser.

                connection.Close(); //At the end, it is important that we close the connection to the browser or it may remain open and other connections to the server will fail.
            }


        }
    }
}
