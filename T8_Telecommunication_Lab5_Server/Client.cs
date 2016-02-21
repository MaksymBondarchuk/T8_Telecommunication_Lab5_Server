using System.Net.Sockets;

namespace T8_Telecommunication_Lab5_Server
{
    public class Client
    {
        public string Name;
        public Socket Socket;
        public bool IsFree = true;
        public int MatrixIndex = -1;
    }
}
