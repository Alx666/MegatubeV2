using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class DataFile
    {
        public DataFile(string fileName, DateTime date, byte filetype)
        {
            this.Name = fileName;
            this.UploadDate = date;
            this.FileType = filetype;
        }

        public DataFile()
        {

        }
    }
}