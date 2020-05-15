using System;
using System.Collections.Generic;
using System.Text;

namespace BillingManagement.UI
{
    class BillingManagementDAO
    {
        private DBConnection conn;
        public BillingManagementDAO()
        {
            conn = new DBConnection();
        }
    }
}
