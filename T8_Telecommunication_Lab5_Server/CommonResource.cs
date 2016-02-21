using System.Collections.Generic;

namespace T8_Telecommunication_Lab5_Server
{
    public class CommonResource
    {
        public List<Client> Clients = new List<Client>();
        public List<List<byte>> Matrix = new List<List<byte>>();
        public List<int> CompletedRows = new List<int>();
        public List<int> FreeRows = new List<int>();
    }
}
