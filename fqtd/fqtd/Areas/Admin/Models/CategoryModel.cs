using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fqtd.Areas.Admin.Models
{
    [Table("tbl_Categories")]
    public class CategoryModel:BaseModels
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string CategoryName_EN { get; set; }
        public string Description_EN { get; set; }
    }
}