using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class Accreditation
    {
        public Accreditation()
        {

        }

        public Accreditation(string channelId, DateTime dateFrom, DateTime dateTo, decimal grossAmount, byte maintype, byte subType)
        {
            this.ChannelId = channelId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.GrossAmmount = grossAmount;
            this.Type = maintype;
            this.SubType = subType;
        }

        public Accreditation(string channelId, int iUserId, DateTime dateFrom, DateTime dateTo, decimal grossAmount, byte maintype, byte subType)
        {
            this.ChannelId      = channelId;
            this.DateFrom       = dateFrom;
            this.DateTo         = dateTo;
            this.GrossAmmount   = grossAmount;
            this.Type           = maintype;
            this.UserId         = iUserId;
        }

        public static IEnumerable<Accreditation> Create(IGrouping<string, CsvVideo> g, Channel c, decimal dollarToEuro, DateTime fileStartDate, DateTime fileEndDate, AccreditationMainType eType)
        {
            yield return new Accreditation(c.Id, c.OwnerId.Value, fileStartDate, fileStartDate, g.Sum(x => x.PartnerRevenue * (decimal)c.PercentOwner * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Ownership);

            if(c.RecruiterId != null)
                yield return new Accreditation(c.Id, c.RecruiterId.Value, fileStartDate, fileStartDate, g.Sum(x => x.PartnerRevenue * (decimal)c.PercentRecruiter * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Recruiting);


            yield return new Accreditation(c.Id, fileStartDate, fileStartDate, g.Sum(x => x.PartnerRevenue * (decimal)c.PercentMegatube * dollarToEuro), (byte)eType, (byte)AccreditationSubType.NetworkPerformance);
        }

        public static IEnumerable<Accreditation> Create(IGrouping<string, CsvPaidFeatures> g, Channel c, decimal dollarToEuro, DateTime fileStartDate, DateTime fileEndDate, AccreditationMainType eType)
        {
            yield return new Accreditation(c.Id, c.OwnerId.Value, fileStartDate, fileStartDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentOwner * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Ownership);

            if (c.RecruiterId != null)
                yield return new Accreditation(c.Id, c.RecruiterId.Value, fileStartDate, fileStartDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentRecruiter * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Recruiting);


            yield return new Accreditation(c.Id, fileStartDate, fileStartDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentMegatube * dollarToEuro), (byte)eType, (byte)AccreditationSubType.NetworkPerformance);
        }

        public enum AccreditationMainType : byte
        {
            Traffic,
            SuperChat
        }

        public enum AccreditationSubType : byte
        {
            Ownership,
            Recruiting,
            NetworkPerformance
        }
    }
}