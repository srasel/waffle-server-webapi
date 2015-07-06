using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using WebAPI.Transform;

namespace MvcApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public List<Employee> Get(int id)
        //{
        //    DataContext dbcontext = new DataContext();
        //    return dbcontext.employees.ToList();
        //}

        [HttpGet]
        [ActionName("stats")]
        public DataTable GetCountryDetails([FromUri]root json)
        {
            //json = "{\"query\": [{\"SELECT\": [\"geo\",\"geo.name\",\"time\",\"pop\"], \"WHERE\": {\"geo\": [\"swe\",\"nor\"],\"time\": \"2000-2015\",\"ind\":\"1\"},\"FROM\": \"humnum\"}],\"lang\": \"en\"}";
            //var json = "{\"query\": [{\"SELECT\": [\"geo\",\"geo.name\",\"time\",\"pop\"],\"FROM\": \"humnum\"}],\"lang\": \"en\"}";
            //var a = JsonConvert.DeserializeObject<root>(json);
            StatsQuery sQ = new StatsQuery(json);
            var str = sQ.getData();
            return str;

            //SELECT s = new SELECT();
            //s.select = new List<string> { "geo" };
            //FROM f = new FROM();
            //f.from = "dimTable";
            //WHERE w = new WHERE();
            //w.geo = new List<string> { "swe", "nor" };
            //w.time = "2001-2010";
            //query q = new query
            //{
            //    SELECT = new List<string> { "geo", "geo.name" },
            //    WHERE =  w ,
            //    FROM = "directtable"
            //};

            //var r = new root { query = new[] { q }, lang = "en" };
            //var j = JsonConvert.SerializeObject(r);
            return null;
            
        }

        [HttpGet]
        [ActionName("dyn")]
        public IEnumerable<DynamicSqlRow> GetDynamic()
        {
            List<DynamicSqlRow> l = new List<DynamicSqlRow>();
            SqlConnection conn = new SqlConnection(
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["DefaultConnection"].ConnectionString);
            using (var rows = new SqlCommandHelper(conn.ConnectionString.ToString(), System.Data.CommandType.Text, 
                "select * from DimCountry"))
            {
                foreach (dynamic row in rows)
                {
                    l.Add(row);
                }
                
            }
            return l;
        }


        [HttpGet]
        [ActionName("jsontoxml")]
        public IEnumerable<Object> GetJsonToXml()
        {
            DataContext dbcontext = new DataContext();
            var json = "{\"query\": [{\"SELECT\": [\"geo\",\"geo.name\",\"time\",\"pop\"],\"FROM\": \"humnum\"}],\"lang\": \"en\"}";
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "root");
            XDocument.Parse(doc.InnerXml);
            return dbcontext.indicator.ToList();
        }


        [HttpGet]
        [ActionName("indicators")]
        public IEnumerable<Object> Get()
        {
            DataContext dbcontext = new DataContext();
            return dbcontext.indicator.ToList();
        }

        [HttpGet]
        [ActionName("indicatorswdi")]
        public IEnumerable<WDI_Indicator> GetWDI()
        {
            DataContext dbcontext = new DataContext();
            return dbcontext.indicatorwdi.ToList();
        }

        [HttpGet]
        [ActionName("indicatorssub")]
        public IEnumerable<SubNationalIndicator> GetSub()
        {
            DataContext dbcontext = new DataContext();
            return dbcontext.indicatorsub.ToList();
        }

        // POST api/values
        [HttpGet]
        [ActionName("details")]
        public IEnumerable<DynamicSqlRow> Get([FromUri]Model model)
        {

            using (SqlConnection conn = new SqlConnection(
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("getAllData", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@type", model.type));
                cmd.Parameters.Add(new SqlParameter("@id", model.productId));
                cmd.Parameters.Add(new SqlParameter("@startDate", (model.startDate == 0 ? 1000 : model.startDate)));
                cmd.Parameters.Add(new SqlParameter("@endDate", (model.endDate == 0 ? 3000 : model.endDate)));

                // execute the command
                //List<DetailsList> dList = new List<DetailsList>();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        //yield return DetailsList.Create(rdr);
                        yield return new DynamicSqlRow(rdr);
                    }
                }
            }

            //List<Model> list = new List<Model>();
            //list.Add(new Model { productId = 1, startDate = 2, endDate = 3 });
            //list.Add(new Model { productId = 3, startDate = 4, endDate = 5 });
            //return list;
        }

        // POST api/values
        public void Post([FromBody]Model model)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public class Model
    {
        public int type { get; set; }
        public int productId { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
    }

    public class DetailsList
    {
        public string Geo { get; set; }
        public string Time { get; set; }
        public string Value { get; set; }
        public string Region { get; set; }

        public static DetailsList Create(IDataReader reader)
        {
            try
            {
                string g = reader["Geo"].ToString();
                string t = reader["Time"].ToString();
                string v = reader["Value"].ToString();
                string r = (reader["Region"] == null ? "" : reader["Region"].ToString());
                return new DetailsList
                {
                    Geo = g,
                    Time = t,
                    Value = v,
                    Region = r
                };
            }
            catch (Exception ex)
            {
                return new DetailsList
                {
                    Geo = "",
                    Time = "",
                    Value = "",
                    Region = ""
                }; ;
            }
        }
    }

}