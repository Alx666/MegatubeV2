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
        LoginSuccess = 0,
        LoginFailed = 1,

        UserChanged = 2,
        PaymentCreated = 3,
        FileUploaded = 4,
        NoteEdited = 5,
        NoteDeleted = 6,
        ApplicationError = 666, //никогда не забывайте
        AddCredit = 667,
    }
}