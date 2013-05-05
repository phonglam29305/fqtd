using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace fqtd.Areas.Admin.Models
{
    public class TimDauEntity: DbContext
    {
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<PropertyModel> Property { get; set; }
    }
}