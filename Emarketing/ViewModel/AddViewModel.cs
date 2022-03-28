using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emarketing.ViewModel
{
    public class AddViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProductPrice { get; set; }
        public string ProductDiscription { get; set; }
        public string ProductImage { get; set; }
        public Nullable<int> FK_User { get; set; }
        public Nullable<int> FK_Category { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string UserName { get; set; }
        public string UserContact { get; set; }
        public string UserImage { get; set; }




    }
}