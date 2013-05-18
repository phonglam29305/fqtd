//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace fqtd.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Brands
    {
        public Brands()
        {
            this.tbl_Brand_Items = new HashSet<BrandItems>();
        }
    
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandName_EN { get; set; }
        public string Description { get; set; }
        public string Description_EN { get; set; }
        public Nullable<int> BrandTypeID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyUser { get; set; }
        public Nullable<System.DateTime> DeleteDate { get; set; }
        public string DeleteUser { get; set; }
    
        public virtual ICollection<BrandItems> tbl_Brand_Items { get; set; }
        public virtual BrandType tbl_BrandType { get; set; }
        public virtual Categories tbl_Categories { get; set; }
    }
}
