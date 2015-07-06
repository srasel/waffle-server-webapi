using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace WebAPI.Transform.Quantity
{
    public class QuantityQuery
    {
        quantity _json;
        public QuantityQuery(quantity json)
        {
            _json = json;
        }

        public string transformToXML()
        {
            return TransformUtil.JsonToXML(JsonConvert.SerializeObject(_json));
        }

        public DataTable getData()
        {
            var root = _json;//JsonConvert.DeserializeObject<root>(_json);
            var xml = transformToXML();

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("QuantityQuery", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@xml", xml));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    dt.Load(rdr);
                }
            }
            //return JsonConvert.SerializeObject(dt);
            return dt;

        }
    }

    #region json to object mapper
    public class quantity
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
        public List<string> quantity { get; set; }
    }
    #endregion
}