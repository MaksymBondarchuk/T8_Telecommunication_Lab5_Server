using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace T8_Telecommunication_Lab5_Server
{
    public class Server
    {
        public CommonResource Data = new CommonResource();

        public async Task Run()
        {
            await Task.Run((Action)Accepter);
        }

        public void Accepter()
        {
            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetworkV6,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    string clientName = null;

                    // Get client name
                    var bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    clientName += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    int selectedIndex;
                    lock (Data)
                    {
                        // Check if have that name already
                        var aliases = Data.Clients.Count(client => client.Name.Contains(clientName));
                        if (aliases != 0)
                            clientName += $"({aliases + 1})";

                        Data.Clients.Add(new Client
                        {
                            Name = clientName,
                            Socket = handler
                        });

                        lock (Data)
                        {
                            Data.Log.Add($"{DateTime.Now} Received client from PC with name : {clientName}");
                        }

                        // If no rows left
                        if (Data.FreeRows.Count == 0)
                        {
                            lock (Data)
                            {
                                Data.Log.Add($"{DateTime.Now} No work for {clientName}");
                            }

                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            continue;
                        }

                        // Send row to client
                        selectedIndex = Data.FreeRows[0];
                        var msg = Data.Matrix[selectedIndex].ToArray();
                        handler.Send(msg);
                    }

                    // Receive sorted row
                    lock (Data)
                    {
                        Data.Log.Add($"{DateTime.Now} {clientName} sorted row {selectedIndex}");
                        Data.Log.Add(null);
                    }
                    handler.Receive(bytes);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    lock (Data)
                    {
                        for (var i = 0; i < Data.Matrix[selectedIndex].Count; i++)
                            Data.Matrix[selectedIndex][i] = bytes[i];
                        Data.CompletedRows.Add(selectedIndex);
                        Data.FreeRows.Remove(selectedIndex);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
