using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class Contract
    {
        public Contract(int userId, string filename, DateTime? expireDate)
        {
            this.UserId = userId;
            this.UploadDate = DateTime.Now;
            this.FilenName = filename;            
        }

        public Contract()
        {

        }
    }
}