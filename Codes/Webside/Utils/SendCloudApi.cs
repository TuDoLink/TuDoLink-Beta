using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace TudolinkWeb.Utils
{
    public class SendCloudApi
    {
        public static bool Send(string mail, string html)
        {
            String url = "http://sendcloud.sohu.com/webapi/mail.send.json";


            AppConfig config = new AppConfig();
            var api_user = config.AppConfigGet("SendCloudApi_User");
            var api_key = config.AppConfigGet("SendCloudApi_Key");
    
            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

                paramList.Add(new KeyValuePair<string, string>("api_user", api_user));
                return result.Contains("success");
            }
            return false;
        }

        static void Main1(string[] args)
        {
            String tos = "to1@sendcloud.org;to2@sendcloud.org";
            Console.ReadKey();
        }

    }


}
