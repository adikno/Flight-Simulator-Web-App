using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class FirstController : Controller
    {
        private static List<Point> points = new List<Point>();
        private static Mutex mut = new Mutex();
        private static string mission;
        private static int numOfPoints = -1;
        private static string fileName;
        private static Boolean fileMode = false;
        private static int pointIndex = 0;
        //defult view
        public ActionResult Index()
        {
            
            return View();
        }

        /**
         * display view
         */
        [HttpGet]
        public ActionResult display(string ip, int port, int? rate)
        {
            Session["max"] = -1;
            string[] potentialIP = ip.Split('.');
            //check if the ip isnt leagal
            if (potentialIP.Length != 4)
            {
                fileName = ip;
                Session["rate"] = port;
                ViewBag.rate = port;
                fileMode = true;
                mission = "4";
                Session["mission"] = mission;
                ViewBag.mission = mission;
                readFromFile();
                Session["max"] = points.Count();
                return View();
            //check if the rate is null or empty
            } else if (String.IsNullOrEmpty(rate.ToString())) {
                mission = "1";
                Session["rate"] = 1;
                ViewBag.rate = 1;
            }
            //check if there is value in the rate
            if (!String.IsNullOrEmpty(rate.ToString())) {
                mission = "2";
                Session["rate"] = rate;
                ViewBag.rate = rate;
            }
            Session["mission"] = mission;
            ViewBag.mission = mission;
            //conenect to the simulator
            InfoModel.Instance.connect(ip, port, 1, 1);
            return View();
        }

        /**
         * save view - read from the simulatur data save it to a file also display the path 
         */
        public ActionResult save(string ip , int port , int rate , int seconds, string file)
        {
            
            Session["rate"] = rate;
            ViewBag.rate = rate;
            Session["mission"] = "3";
            ViewBag.mission = "3";
            numOfPoints = seconds * rate;
            Session["max"] = numOfPoints;
            fileName = file;
            //connect the simulator
            InfoModel.Instance.connect(ip, port, 1, 1);
            return View("display", InfoModel.Instance);
        }
        /**
        * this function get the postion of the plane from the simulator and r
        * return xml string of the point
        */
        [HttpPost]
        public string GetPoint()
        {
            if (!fileMode)
            {

                mut.WaitOne();

                InfoModel.Instance.readAll();
                string lat = InfoModel.Instance.Lat;
                string lon = InfoModel.Instance.Lon;
                string throttle = InfoModel.Instance.Throttle;
                string rudder = InfoModel.Instance.Rudder;

                mut.ReleaseMutex();

                Point p = new Point(lon, lat, throttle, rudder);
                //add the new point to the list
                points.Add(p);

                if (points.Count() == numOfPoints)
                {
                    writeToFile();
                } 
                return ToXml(p);
                //the forth mission return point from the file
            } else {
                mut.WaitOne();
                if (pointIndex == points.Count)
                {
                    mut.ReleaseMutex();
                    return ToXml(points[pointIndex - 1]);
                }
                pointIndex++;
                mut.ReleaseMutex();
                return ToXml(points[pointIndex - 1]);
            }
        }
        /**
        * this function write the list of the points to a file 
        *
        */
        private void writeToFile()
        {
            
            string location = System.AppDomain.CurrentDomain.BaseDirectory;
            location = location + fileName;
            //create a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@location))
            {
                for (
                    int i = 0; i < numOfPoints; i++)
                {
                    //write each point in the list
                    file.WriteLine(points[i].toFile());
                }
            }
        }
        /**
        * this function read from file all the points and add each point to the list  
        */
        public void readFromFile()
        {
            string location = System.AppDomain.CurrentDomain.BaseDirectory;
            location = location + fileName;
            string[] allPoints = System.IO.File.ReadAllLines(location);
            foreach (string p in allPoints)
            {
                if (p.Equals(""))
                {
                    continue;
                }
                string[] args = p.Split(',');
                Point point = new Point(args[0], args[1], args[2], args[3]);
                points.Add(point);
            }
            Session["max"] = points.Count();
        }
        /**
         * this function get a point and write it in a xml structure
         * and return it as a string
         */
        private string ToXml(Point p)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Point");
            p.ToXml(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
       

    }
}