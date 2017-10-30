using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class DataFile
    {
        public DataFile(string fileName, DateTime date, ProcessingType eType, int newChannels, int totalChannels, int unactiveChannels)
        {
            this.Name               = fileName;
            this.UploadDate         = date;
            this.ProcessingType     = (byte)eType;
            this.NewChannels        = newChannels;
            this.UnactiveChannels   = unactiveChannels;
            this.TotalChannels      = totalChannels;
        }

        public DataFile()
        {

        }
    }
}