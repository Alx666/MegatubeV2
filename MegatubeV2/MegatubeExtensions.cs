using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public static class MegatubeExtensions
    {
        public static string FormatName(this string s)
        {
            try
            {
                return s.Trim().Substring(0, Math.Min(0, s.Length));
            }
            catch (Exception)
            {
                return null;
            }            
        }
    }
}