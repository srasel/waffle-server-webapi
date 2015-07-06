using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    [Table("SubNationalIndicator")]
    public class SubNationalIndicator
    {
        public int ID { get; set; }
        public string indicator { get; set; }
    }
}