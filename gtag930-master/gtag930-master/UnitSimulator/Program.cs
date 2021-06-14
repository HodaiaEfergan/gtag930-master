using System;
using System.Net.Sockets;
using System.Text;

namespace UnitSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            string toSend = "UID011106001763972UBAT4220MVOLINDExt_ONURSSI21,9 NETCON\"Partner\"MCUTMPTPM40.00EXTTMPTPS31.65LOC$GPGGA,053527.000,3149.1484,N,03515.2414,E,1,06,1.4,686.3,M, $SPEEDTAGSTID43TRSSI-51,TID48TRSSI-73,TID35TRSSI-55,TID29TRSSI-56";
            //string toSend = "UID011106005420551UBAT3874MVOLINDExt_OFFURSSI22, 9NETCON\"Partner\"MCUTMPTPM40.88EXTTMPTPS34.22LOC$GPGGA,,,,,,0,00,,,M,0.0,M,,0000*48";

            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 9870);
            NetworkStream ns = client.GetStream();
            ns.Write(Encoding.UTF8.GetBytes(toSend));
        }
    }
}