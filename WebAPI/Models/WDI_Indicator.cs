using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    [Table("WDI_Indicator")]
    public class WDI_Indicator
    {
        public int ID { get; set; }
        public string indicator { get; set; }
    }
}