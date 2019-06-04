using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication2.Models
{
    /** 
     * this class responsibole to conncect the server 
     */
    public class InfoModel
    {
        private static NetworkStream stream;

        private bool connected = false;

        TcpClient client;
        IPEndPoint ep;
        private string lonModel = "";
        private string latModel = "";
        private string rudder = "";
        private string throttle = "";
        private static InfoModel s_instance = null;
        static List<Point> points = new List<Point>();
        public static InfoModel Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new InfoModel();
                }
                return s_instance;
            }
        }

        public InfoModel() { }

        public string Lat
        {
            get { return latModel; }
            set { latModel = value; }
        }

        public string Lon
        {
            get { return lonModel; }
            set { lonModel = value; }
        }
        public string Throttle
        {
            get { return throttle; }
            set { throttle = value; }
        }
        public string Rudder
        {
            get { return rudder; }
            set { rudder = value; }
        }
        /**
         * this function connect to the simulator
         */
        public void connect(string ip, int port, int rate, int seconds)
        {
            if (connected)
            {
                disconnect();
                connected = false;
            }
            ep = new IPEndPoint(IPAddress.Parse(ip), port);
            client = new TcpClient();
            client.Connect(ep);
            connected = true;
        }
        /**
         * this function disconnect to the simulator
         */
        public void disconnect()
        {
            client.Close();
        }
        /**
         * this function request to read all the data to get the position of the plane
         */
        public void readAll()
        {
            string commandLat = "get /position/latitude-deg\r\n";
            Lat = read(commandLat);
            string commandLon = "get /position/longitude-deg\r\n";
            Lon = read(commandLon);
            string commandRudder = "get /controls/flight/rudder\r\n";
            Rudder = read(commandRudder);
            string commandThrottle = "get /controls/engines/current-engine/throttle\r\n";
            Throttle = read(commandThrottle);
        }
        /**
         * this function get a comand and send it to the simulator 
         * the simulator return a value of the specific commnad
         */
        public string read(string command)
        {
            if (connected)
            {
                NetworkStream nwStream = client.GetStream();
                byte[] byteToSend = new byte[512];
                Array.Clear(byteToSend, 0, byteToSend.Length);
                byteToSend = ASCIIEncoding.ASCII.GetBytes(command);
                //write the command to the simulator
                nwStream.Write(byteToSend, 0, byteToSend.Length);
                Array.Clear(byteToSend, 0, byteToSend.Length);

                byte[] byteToSend1 = new byte[512];
                Array.Clear(byteToSend1, 0, byteToSend1.Length);
                //read the answer
                nwStream.Read(byteToSend1, 0, byteToSend1.Length);

                try
                {
                    string value = ParseValue(Encoding.UTF8.GetString(byteToSend1, 0, byteToSend1.Length)); ;

                    if (value == "null")
                    {
                        disconnect();
                        return "";
                    }
                    //nwStream.Flush();                    
                    return value;
                } catch
                {
                    return "";
                }
            }
            return "";
        }
        /**
         * split the the data and return the disired value
         */
        public string ParseValue(string data)
        {
            //string tmp = data.Substring(data.IndexOf('\n'), data.Length);
            string[] array = data.Split('\'');
            return array[1];
        }

    }
}
