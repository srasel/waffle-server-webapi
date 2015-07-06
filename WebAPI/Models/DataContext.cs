using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPI.Models;

namespace MvcApplication1.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {

        }
        public DbSet<Indicator> indicator { get; set; }
        public DbSet<WDI_Indicator> indicatorwdi { get; set; }
        public DbSet<SubNationalIndicator> indicatorsub { get; set; }
    }
}