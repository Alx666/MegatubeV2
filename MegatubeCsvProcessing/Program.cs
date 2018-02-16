using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegatubeV2;
using System.IO;

namespace MegatubeCsvProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Stream fs = File.OpenRead("YouTube_id0iPcHVVMh4ASTXBUYkOA_M_20171201_20171231_videoclaim_rawdata_v1-0.csv"))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    SmartParser<CsvVideo> parser = new SmartParser<CsvVideo>(sr, "Video ID,Content Type,Policy,Video Title,Video Duration (sec),Username,Uploader,Channel Display Name,Channel ID,Claim Type,Claim Origin,Multiple Claims?,Category,Asset ID,Asset Labels,Asset Channel ID,Custom ID,Owned Views,Owned Views : Watch Page,Owned Views : Embedded Player,Owned Views : Channel Page,Owned Views : Live,Owned Views : On Demand,Owned Views : Ad-Enabled,Owned Views : Ad-Requested,YouTube Revenue Split : AdSense Served YouTube Sold,YouTube Revenue Split : DoubleClick Served YouTube Sold,YouTube Revenue Split : DoubleClick Served Partner Sold,YouTube Revenue Split : Partner Served Partner Sold,YouTube Revenue Split,Partner Revenue : AdSense Served YouTube Sold,Partner Revenue : DoubleClick Served YouTube Sold,Partner Revenue : DoubleClick Served Partner Sold,Partner Revenue : Partner Served Partner Sold,Partner Revenue,Estimated RPM");
                    parser.Map<string>("Video ID", (r, v)         => r.VideoId = v);
                    parser.Map<string>("Channel ID", (r, v)       => r.ChannelID = v);
                    parser.Map<string>("Asset Channel ID", (r, v) => r.AssetChannelId = v);
                    parser.Map<decimal>("Partner Revenue", (r, v) => r.PartnerRevenue = v);
                    parser.Map<string>("Content Type", (r, v)     => r.ContentType = v);

                    IEnumerable<CsvVideo> videos = parser.ReadAllLines();

                    decimal result = (from v in videos
                                      where v.GetOwnerReference() == "HZl_sLl4kGZSkrPBrWb_aQ"
                                      select v).Sum(x => x.PartnerRevenue) * 0.83739m;

                    decimal mt = result * 0.25m;
                    decimal re = result * 0.25m;
                    decimal pt = result * 0.95m;

                    Console.WriteLine(mt);
                    Console.WriteLine(re);
                    Console.WriteLine(pt);
                    Console.ReadLine();
                }
            }
        }
    }
}
