using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public partial class EventLog
    {
        public static EventLog Create(MegatubeV2Entities hDb, User hUser, EventLogType eType, string sText)
        {
            EventLog hNew = new EventLog();

            try
            {
                hNew.User = hDb.Users.Find(hUser.Id);
            }
            catch (Exception)
            {
                hNew.User = null;
            }
            finally
            {
                hNew.Type = (short)eType;
                hNew.Description = sText;
                hNew.Date = DateTime.Now;
            }

            return hNew;
        }

        public static void Log(MegatubeV2Entities hDb, User hUser, EventLogType eType, string sText, bool bAutosave = false)
        {
            EventLog hNew = Create(hDb, hUser, eType, sText);
            hDb.EventLogs.Add(hNew);

            if (bAutosave)
                hDb.SaveChanges();
        }
    }


    public enum EventLogType : short
    {
        LoginSuccess             = 0,
        LoginFailed              = 1,
        UserCreated              = 2,
        UserChanged              = 3,
        ChannelEdited            = 4,
        PaymentCreated           = 5,
        PaymentReverted          = 6,
        FileUploaded             = 7,
        NoteEdited               = 8,
        NoteDeleted              = 9,
        ContractUpload           = 10,
        AddCredit                = 11,
        RemoveCredit             = 12,
        ОшибкаПриложения         = 666, //никогда не забывайте                
    }
}