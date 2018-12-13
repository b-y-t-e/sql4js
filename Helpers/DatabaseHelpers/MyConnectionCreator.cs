using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Database;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Else.HttpService;
using sql4js.Helpers;

namespace Database
{
    public class MyConnectionCreator
    {
        public static MyConnection Create()
        {
            //throw new NotImplementedException();
            return new MyConnection(SettingsHelper.Get("sqlConnectionString")); // Globals.CONNECTION_STRING, Globals.ODBC);
        }
    }
}
