﻿using System;
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

        public Accreditation(string channelId, DateTime dateFrom, DateTime dateTo, decimal grossAmount, byte maintype, byte subType, int networkId)
        {
            this.ChannelId = channelId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.GrossAmmount = grossAmount;
            this.Type = maintype;
            this.SubType = subType;
            this.NetworkId = networkId;
        }

        public Accreditation(string channelId, int iUserId, DateTime dateFrom, DateTime dateTo, decimal grossAmount, byte maintype, byte subType, int networkId)
        {
            this.ChannelId      = channelId;
            this.DateFrom       = dateFrom;
            this.DateTo         = dateTo;
            this.GrossAmmount   = grossAmount;
            this.Type           = maintype;
            this.UserId         = iUserId;
            this.NetworkId      = networkId;
        }

        public override string ToString() => this.Channel.Name;

        public static IEnumerable<Accreditation> Create<T>(IGrouping<string, T> g, Channel c, decimal dollarToEuro, DateTime fileStartDate, DateTime fileEndDate, AccreditationMainType eType, int networkId, Func<T,decimal> selector)
        {
            yield return new Accreditation(c.Id, c.OwnerId.Value, fileStartDate, fileEndDate, g.Sum(x => selector.Invoke(x) * (decimal)c.PercentOwner * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Ownership, networkId);

            if(c.RecruiterId != null)
                yield return new Accreditation(c.Id, c.RecruiterId.Value, fileStartDate, fileEndDate, g.Sum(x => selector.Invoke(x) * (decimal)c.PercentRecruiter * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Recruiting, networkId);

            yield return new Accreditation(c.Id, fileStartDate, fileEndDate, g.Sum(x => selector.Invoke(x) * (decimal)c.PercentMegatube * dollarToEuro), (byte)eType, (byte)AccreditationSubType.NetworkPerformance, networkId);
        }

        //public static IEnumerable<Accreditation> Create(IGrouping<string, CsvPaidFeatures> g, Channel c, decimal dollarToEuro, DateTime fileStartDate, DateTime fileEndDate, AccreditationMainType eType, int networkId)
        //{
        //    yield return new Accreditation(c.Id, c.OwnerId.Value, fileStartDate, fileEndDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentOwner * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Ownership, networkId);

        //    if (c.RecruiterId != null)
        //        yield return new Accreditation(c.Id, c.RecruiterId.Value, fileStartDate, fileEndDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentRecruiter * dollarToEuro), (byte)eType, (byte)AccreditationSubType.Recruiting, networkId);


        //    yield return new Accreditation(c.Id, fileStartDate, fileEndDate, g.Sum(x => x.PartnerEarningsFraction * (decimal)c.PercentMegatube * dollarToEuro), (byte)eType, (byte)AccreditationSubType.NetworkPerformance, networkId);
        //}

        public enum AccreditationMainType : byte
        {
            Traffic             = 0,
            PaidFeatures        = 1,
            Extra               = 2,
        }

        public enum AccreditationSubType : byte
        {
            Ownership           = 0,
            Recruiting          = 1,            
            Manual              = 2,
            NetworkPerformance  = 3
        }

        public static string AccreditationMainTypeText(AccreditationMainType type)
        {
            switch (type)
            {
                case AccreditationMainType.Traffic: return "Traffico su canale";
                case AccreditationMainType.PaidFeatures: return "Accredito SuperChat";
                default: throw new Exception("Unknow Accreditation Type");
            }
        }

        public static string AccreditationSubTypeText(AccreditationSubType type)
        {
            switch (type)
            {
                case AccreditationSubType.Ownership: return "possesso canale";
                case AccreditationSubType.Recruiting: return "canale reclutato";
                case AccreditationSubType.NetworkPerformance: return "network performance";
                case AccreditationSubType.Manual: return "manuale";
                default: throw new Exception("Unknow Accreditation Subtype");
            }
        }

        public string ToString(string channelName)
        {
            string mainType = AccreditationMainTypeText((AccreditationMainType)this.Type);
            string subType = AccreditationMainTypeText((AccreditationMainType)this.SubType);

            return $"{mainType} {channelName} {subType}";
        }

    }
}