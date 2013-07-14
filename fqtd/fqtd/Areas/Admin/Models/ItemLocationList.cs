using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace fqtd.Areas.Admin.Models
{

    public class ItemLocationList
    {
        public ItemLocationList(int ID) { this.ID = ID; }
        [Key]
        public int ID { get; set; }
    }
}
