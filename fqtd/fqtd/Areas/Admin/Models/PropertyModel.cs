using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fqtd.Areas.Admin.Models
{
    public class PropertyModel
    {
        [Key]
        public int PropertyID { get; set; }

        public string PropertyName { get; set; }
    }
}