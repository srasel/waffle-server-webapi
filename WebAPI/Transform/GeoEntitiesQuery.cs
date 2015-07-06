using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebAPI.Transform.GeoEntity
{
    public class GeoEntitiesQuery
    {
        geoentity _json;
        public GeoEntitiesQuery(geoentity json)
        {
            _json = json;
        }

        public string transformToXML(demoentity q)
        {
            return TransformUtil.JsonToXML(JsonConvert.SerializeObject(q));
        }

        public List<DataTable> getData()
        {
            List<DataTable> alldata = new List<DataTable>();
            var root = _json;//JsonConvert.DeserializeObject<root>(_json);
            foreach (query q in root.query)
            {
                var xml = transformToXML(new demoentity { query = q, lang = root.lang });
                TransformUtil.WriteLogToFile("xml>" + xml.ToString());
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(
                    System.Configuration.ConfigurationManager.
                        ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("GeoEntitiesQuery", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@xml", xml));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }
                }
                alldata.Add(dt);
            }
            //return JsonConvert.SerializeObject(dt);
            return alldata;

        }
    }

    public class demoentity
    {
        public query query { get; set; }
        public string lang { get; set; }
    }


    #region json to object mapper
    public class geoentity
    {
        public query[] query { get; set; }
        public string lang { get; set; }
    }

    public class query
    {
        public List<string> SELECT { get; set; }
        public WHERE WHERE { get; set; }
        public string FROM { get; set; }
    }

    public class WHERE
    {
        public string geo { get; set; }
        [JsonProperty(PropertyName = "geo.cat")]
        public List<string> geo_cat { get; set; }
        public string cat { get; set; }
    }
    #endregion

}