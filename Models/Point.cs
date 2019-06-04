using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml;

namespace WebApplication2.Models
{
    public class Point
    {
       

        public Point(string x, string y, string throttle , string rudder)
        {
            X = x;
            Y= y;
            Throttle = throttle;
            Rudder = rudder;
        }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "X")]
        public string X
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Y")]
        public string Y
        {
            get;
            set;

        }
        
        public string Throttle
        {
            get;
            set;

        }
        
        public string Rudder
        {
            get;
            set;

        }
        /**
t        * this function write the data of the point in one string
        */
        public string toFile()
        {
            return X + "," + Y + "," + Throttle + "," + Rudder;
        }

       /**
        * this function write the data of the point in xml 
        */
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Point");
            writer.WriteElementString("Lon", X);
            writer.WriteElementString("Lat", Y);
            writer.WriteEndElement();
        }
    }
}