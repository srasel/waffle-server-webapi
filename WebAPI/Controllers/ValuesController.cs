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
using StatsQuery = WebAPI.Transform.Stats.StatsQuery;
using stat = WebAPI.Transform.Stats.stat;
using dimension = WebAPI.Transform.Dimension.dimension;
using DimensionQuery = WebAPI.Transform.Dimension.DimensionQuery;
using quantity = WebAPI.Transform.Quantity.quantity;
using QuantityQuery = WebAPI.Transform.Quantity.QuantityQuery;
using geoentity = WebAPI.Transform.GeoEntity.geoentity;
using GeoEntitiesQuery = WebAPI.Transform.GeoEntity.GeoEntitiesQuery;
using System.Web.Http.Cors;
using System.Threading.Tasks;

namespace MvcApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {

        public async Task<List<DataTable>> GetGeoEntitiesAsync(string json)
        {
            var jObj = JsonConvert.DeserializeObject<geoentity>(json);
            GeoEntitiesQuery sQ = new GeoEntitiesQuery(jObj);
            var str = sQ.getData();
            var js = JsonConvert.SerializeObject(str);
            //TransformUtil.WriteLogToFile("return from geo" + js);
            return str;
        }
        public async Task<List<DataTable>> GetStatsAsync(stat body)
        {
            StatsQuery sQ = new StatsQuery(body);
            var str = sQ.getData();
            var js = JsonConvert.SerializeObject(str);
            //TransformUtil.WriteLogToFile("return from stat" + js);
            return str;
        }

        public async Task<List<DataTable>> GetStatQueryPostAsync(stat body)
        {
            try
            {
                return await GetStatsAsync(body);
                
            }
            catch (Exception ex)
            {
                TransformUtil.WriteLogToFile("in actual task>" + ex.Message.ToString());
                return new List<DataTable>();
            }

        }

        [HttpPost]
        [ActionName("waffle")]
        public async Task<List<DataTable>> GetFromQueryEngineAsync([FromBody] stat body)
        {
            TransformUtil.WriteLogToFile("in waffle>" + JsonConvert.SerializeObject(body));
            Task<List<DataTable>> getresult = GetStatQueryPostAsync(body);
            return await getresult;
            //return null;
            //return await Task.Run(() => { return GetStatQueryPostAsync(body); });
        }


        [HttpGet]
        [ActionName("dimension")]
        public DataTable GetDimQuery([FromUri]string json)
        {
            var jObj = JsonConvert.DeserializeObject<dimension>(json);
            DimensionQuery sQ = new DimensionQuery(jObj);
            var str = sQ.getData();
            return str;
        }

        [HttpPost]
        [ActionName("dimension")]
        public DataTable GetDimQueryPost([FromBody]dimension json)
        {
            //var jObj = JsonConvert.DeserializeObject<dimension>(json);
            DimensionQuery sQ = new DimensionQuery(json);
            var str = sQ.getData();
            return str;
        }

        [HttpGet]
        [ActionName("quantity")]
        public DataTable GetQuantityQuery([FromUri]string json)
        {
            var jObj = JsonConvert.DeserializeObject<quantity>(json);
            QuantityQuery sQ = new QuantityQuery(jObj);
            var str = sQ.getData();
            return str;
        }

        [HttpPost]
        [ActionName("quantity")]
        public DataTable GetQuantityQueryPost([FromBody]quantity json)
        {
            //var jObj = JsonConvert.DeserializeObject<quantity>(json);
            QuantityQuery sQ = new QuantityQuery(json);
            var str = sQ.getData();
            return str;
        }

        [HttpGet]
        [ActionName("categoriesofgeo")]
        public DataTable GetCatOfTypeGeoQuery([FromUri]string json)
        {
            var str = WebAPI.Transform.Dimension.Utility.getDataFromSP("CategoriesOfTypeGeo");
            return str;
        }

        [HttpPost]
        [ActionName("categoriesofgeo")]
        public DataTable GetCatOfTypeGeoQueryPost([FromBody]string json)
        {
            var str = WebAPI.Transform.Dimension.Utility.getDataFromSP("CategoriesOfTypeGeo");
            return str;
        }

        [HttpGet]
        [ActionName("datasource")]
        public DataTable GetDataSource()
        {
            var str = WebAPI.Transform.Dimension.Utility.getData("select * from DimDataSource");
            return str;
        }

        [HttpPost]
        [ActionName("datasource")]
        public DataTable GetDataSourcePost()
        {
            var str = WebAPI.Transform.Dimension.Utility.getData("select * from DimDataSource");
            return str;
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

        [HttpGet]
        [ActionName("details")]
        public DataTable Get([FromUri]Model model)
        {
            DataTable dt = new DataTable();
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
                    dt.Load(rdr);
                    
                }
                return dt;
            }

        }


        [HttpGet]
        [ActionName("getindicatorsofsoruce")]
        public DataTable GetDataSource([FromUri]string source)
        {
            var str = WebAPI.Transform.Dimension.Utility.getData("select * from [dbo].[DimIndicators] where [DataSourceID] =" + source);
            return str;
        }


    }

    public class Model
    {
        public int type { get; set; }
        public int productId { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
    }


}