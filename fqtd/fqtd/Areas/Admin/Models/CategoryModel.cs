using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fqtd.Areas.Admin.Models
{
    [Table("tbl_Categories")]
    public class CategoryModel
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string CategoryName_EN { get; set; }
        public string Description_EN { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUser { get; set; }
        public DateTime DeleteDate { get; set; }
        public int DeleteUser { get; set; }
    }
}