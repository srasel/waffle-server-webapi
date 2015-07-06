using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebAPI.Transform
{
    public class StatsQuery
    {
        root _json;
        public StatsQuery(root json)
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

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("StatsQuery", conn);
                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;
                // 3. add parameter to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@xml", xml));
                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    dt.Load(rdr);
                    // iterate through results, printing each to console
                    //while (rdr.Read())
                    //{
                    //    //yield return DetailsList.Create(rdr);
                    //    yield return StatsOutput.Create(rdr);
                    //}
                }
            }
            //return JsonConvert.SerializeObject(dt);
            return dt;

        }
    }

    #region json to object mapper
    public class root
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
        public List<string> geo { get; set; }
        public string time { get; set; }
        public string ind { get; set; }
    }
    #endregion

    #region response handler
    public class StatsOutput
    {
        [JsonProperty(PropertyName = "geo")]
        public string Geo { get; set; }

        [JsonProperty(PropertyName = "geo.name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "indicator")]
        public string Value { get; set; }



        public static StatsOutput Create(IDataReader reader)
        {
            try
            {
                string geo = reader["Geo"].ToString();
                string name = reader["Name"].ToString();
                string time = reader["Time"].ToString();
                string val = (reader["Value"] == null ? "" : reader["Value"].ToString());
                return new StatsOutput
                {
                    Geo = geo,
                    Name = name,
                    Time = time,
                    Value = val
                };
            }
            catch (Exception ex)
            {
                return new StatsOutput
                {
                    Geo = "",
                    Name = "",
                    Time = "",
                    Value = ""
                };
            }
        }
    }
    #endregion

}