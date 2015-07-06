using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebAPI.Transform.Dimension
{
    public class DimensionQuery
    {
        dimension _json;
        public DimensionQuery(dimension json)
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
                SqlCommand cmd = new SqlCommand("select * from vDimensions", conn);
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.Add(new SqlParameter("@xml", xml));
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
    public class dimension
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
        public List<string> dimension { get; set; }
    }
    #endregion

    public class Utility
    {
        public static DataTable getData(string query)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.Add(new SqlParameter("@xml", xml));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    dt.Load(rdr);
                }
            }
            //return JsonConvert.SerializeObject(dt);
            return dt;
        }

        public static DataTable getDataFromSP(string spName)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(new SqlParameter("@xml", xml));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    dt.Load(rdr);
                }
            }
            //return JsonConvert.SerializeObject(dt);
            return dt;
        }
    }
}