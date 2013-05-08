using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fqtd.Areas.Admin.Models
{

    [Table("tbl_Properties")]
    public class PropertyModel:BaseModels
    {
        [Key]
        public int PropertyID { get; set; }
        [Required(ErrorMessage="Tên thuộc tính không được rỗng, vui lòng nhập liệu.")]
        [StringLength(250)]
        public string PropertyName { get; set; }
        [Required(ErrorMessage = "Tên thuộc tính không được rỗng, vui lòng nhập liệu.")]
        [StringLength(250)]
        public string PropertyName_EN { get; set; }
        public string Description { get; set; }
        public int ParentID { get; set; }
        public int OrderNumber { get; set; }
    }
}