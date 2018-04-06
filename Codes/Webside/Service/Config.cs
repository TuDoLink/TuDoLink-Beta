using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TudolinkWeb.Service
{
    class Config
    {
        public static string DatabaseFile = "";
        public static string DataSource
        {
            get
            {
                return string.Format("data source={0}", ConfigurationManager.AppSettings["SQLiteDb"]);
            }
        }
    }
}