using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class Cookie
    {
        public string   Email         { get; set; }
        public string   Password      { get; set; }
        public int      NetworkId     { get; set; }

        public const string SysCookieName = "megatubev2";


        public Cookie(string email, string password, int networkId)
        {
            Email       = email.ToLower();
            Password    = password;
            NetworkId   = networkId;
        }

        public Cookie(string data)
        {            
            string[] values = data.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

            Email       = values[0].ToLower();
            Password    = values[1];
            NetworkId   = int.Parse(values[2]);
        }

        public override string ToString() => $"{Email}---{Password}---{NetworkId}";
    }
}