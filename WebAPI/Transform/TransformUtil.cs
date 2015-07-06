using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

namespace WebAPI.Transform
{
    public class TransformUtil
    {
        public static string JsonToXML(string json)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "root");
            //XDocument.Parse(doc.InnerXml);
            return doc.InnerXml;
        }

        public static void WriteLogToFile(string content)
        {
            try
            {
                string fileLoc = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.txt", "Log"));
                using (StreamWriter sw = File.AppendText(fileLoc))
                {
                    sw.WriteLine(DateTime.Now + "\t" + content);
                }  
            }
            catch(Exception)
            {
                ;
            }
        }
    }
}