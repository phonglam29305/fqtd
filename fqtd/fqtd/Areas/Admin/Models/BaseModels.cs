using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fqtd.Areas.Admin.Models
{
	public class BaseModels
	{
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUser { get; set; }
        public DateTime DeleteDate { get; set; }
        public int DeleteUser { get; set; }
	}
}