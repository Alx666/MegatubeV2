using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegatubeDataMigrator.ModelNew;
using MegatubeDataMigrator.ModelOld;
using System.Data.Entity;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;

namespace MegatubeDataMigrator
{
    class Program
    {

        #region Misc        

        private void OutPercentage(int current, int total)
        {
            float result = ((float)current / total) * 100f;            

            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(result.ToString("n2")+ "%" +  new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void DropTable<T>(DbSet<T> dataset, bool confirm = true) where T : class
        {
            if (confirm)
            {                
                Console.Write("\nAre you sure?> ");
                if (Console.ReadLine() != "y")
                    return;
            }

            Console.Write($"Dropping {dataset.Count()} {dataset.GetType().GetGenericArguments()[0].Name}...");
            dataset.RemoveRange(dataset);            
            Console.WriteLine("Done");
        }

        private void EnumerateTable<T>(DbSet<T> dataset) where T : class
        {
            Console.WriteLine();
            dataset.ToList().ForEach(x => Console.WriteLine(x.ToString()));
            Console.WriteLine($"{dataset.Count()} records");
        }

        private void MigrateTable<T, K>(IEnumerable<T> source, DbSet<K> dest, Func<T, K> mapper, MegatubeV2Entities db, Action<int,int,K> OnSave)
            where T : class
            where K : class
        {            
            List<T> records = source.ToList();

            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    K newRecord = mapper.Invoke(records[i]);
                    dest.Add(newRecord);
                    db.SaveChanges();
                    OnSave(i + 1, records.Count, newRecord);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadLine();
                }
            }
        }

#endregion

        #region Payment Alerts

        [ConsoleUIMethod]
        public void DropPaymentAlerts()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.PaymentAlerts);
                db.SaveChanges();
            }
        }

        [ConsoleUIMethod]
        public void EnumeratePaymentAlerts()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.PaymentAlerts);
            }
        }

        #endregion

        #region Payments

        [ConsoleUIMethod]
        public void DropPayments()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                Console.Write("\nAre you sure?> ");
                if (Console.ReadLine() != "y")
                    return;

                db.Database.ExecuteSqlCommand("delete [Payment]");                

                Console.WriteLine("Done!");
            }
        }

        [ConsoleUIMethod]
        public void EnumeratePayments()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.Payments);
            }
        }

        [ConsoleUIMethod]
        public void MigratePayments()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    MigrateTable(oldDb.Payments, newDb.Payments, t =>
                    {
                        ModelNew.Payment record = new ModelNew.Payment();
                        record.Id               = t.Id;
                        record.DateFrom         = t.DateFrom;
                        record.DateTo           = t.DateTo;
                        record.Date             = t.Date;
                        record.Gross            = t.Amount;
                        record.Net              = PaymentMethodFactory.GetNetFromGrossF((short)t.PaymentType, record.Gross);
                        record.PaymentType      = t.PaymentType;
                        record.ReceiptCount     = 0;
                        record.UserId           = t.UserId;
                        record.AdministratorId  = newDb.Users.Find(record.UserId).FiscalAdministratorId;
                        
                        
                        return record;

                    }, newDb, (index, total, entity) => OutPercentage(index, total));
                }
            }
        }

        #endregion

        #region Accreditations

        [ConsoleUIMethod]
        public void DropAccreditations()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                Console.Write("\nAre you sure?> ");
                if (Console.ReadLine() != "y")
                    return;

                db.Database.ExecuteSqlCommand("delete [Accreditation]");
                Console.WriteLine("Done!");
            }
        }

        [ConsoleUIMethod]
        public void EnumerateAccreditations()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {                
                EnumerateTable(db.Accreditations);
            }
        }
        [ConsoleUIMethod]
        public void MigrateAccreditations()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    var mapping = new Dictionary<int, Tuple<byte, byte>>
                    {
                        { 0, new Tuple<byte, byte>(0, 0) }, //Traffic, Ownership
                        { 1, new Tuple<byte, byte>(0, 1) }, //Traffic, Recruiting
                        { 2, new Tuple<byte, byte>(2, 2) }, //Custom Accreditation
                        { 3, new Tuple<byte, byte>(1, 0) }, //Paid-Features Ownership
                        { 4, new Tuple<byte, byte>(1, 1) }  //Paid-Features Recruiting
                    };



                    //var accrUser = (from a in oldDb.Accreditations
                    //                where !a.Payed
                    //                group a by a.UserId into g
                    //                select new
                    //                {
                    //                    UserId = g.Key,
                    //                    PFO = g.Where(x => x.Type == 3).ToList(),
                    //                    PFR = g.Where(x => x.Type == 4).ToList(),
                    //                    TO = g.Where(x => x.Type == 0).ToList(),
                    //                    TR = g.Where(x => x.Type == 1).ToList()
                    //                }).ToList();



                    List<int> users = newDb.Users.Select(x => x.Id).ToList();

                    var oldOnes = (from a in oldDb.Accreditations
                                   where !a.Payed && users.Contains(a.UserId)
                                   group a by a.UserId into g
                                   select new
                                   {
                                       UserId = g.Key,
                                       DateFrom = g.Min(x => x.DateFrom),
                                       DateTo = g.Max(x => x.DateTo),
                                       GrossAmount = g.Sum(x => x.GrossAmmount)

                                   }).ToList();

                    //oldDb.Accreditations.Where(x => !x.Payed && users.Contains(x.UserId)).ToList().gr





                    //MigrateTable(oldDb.Accreditations.Where(x => !x.Payed && users.Contains(x.UserId)), newDb.Accreditations, t =>
                    //{
                    //    ModelNew.Accreditation record = new ModelNew.Accreditation();
                    //    record.Id = t.Id;
                    //    record.DateFrom = t.DateFrom;
                    //    record.DateTo = t.DateTo;
                    //    //record.ChannelId = t.ChannelId;
                    //    record.GrossAmmount = t.GrossAmmount;
                    //    record.UserId = t.UserId;
                    //    record.Type = mapping[t.Type].Item1;
                    //    record.SubType = mapping[t.Type].Item2;
                    //    record.PaymentId = null;

                    //    return record;

                    //}, newDb, (index, total, entity) => OutPercentage(index, total));
                }
            }
        }

        #endregion

        #region DataFiles

        [ConsoleUIMethod]
        public void DropDataFiles()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.DataFiles);
                db.SaveChanges();
            }
        }

        [ConsoleUIMethod]
        public void EnumerateDataFiles()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.DataFiles);
            }
        }

        #endregion

        #region Notes

        [ConsoleUIMethod]
        public void DropNotes()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.Notes);
            }
        }

        [ConsoleUIMethod]
        public void EnumerateNotes()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.Notes);
            }
        }

        #endregion

        #region Contracts

        [ConsoleUIMethod]
        public void DropContracts()
        {
        }

        #endregion

        #region Users

        [ConsoleUIMethod]
        public void CheckChannels()
        {
            List<string> messages = new List<string>();

            using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
            {
                using (MegatubeV2Entities newDb = new MegatubeV2Entities())
                {
                    //Utenze da controllare
                    List<ModelNew.User> newUsers    = newDb.Users.ToList();                    

                    foreach (var newUser in newUsers)
                    {
                        Console.WriteLine($"Checking {newUser.LastName} {newUser.Name}");

                        List<ModelNew.Channel> ownedChannels          = newUser.OwnedChannels.ToList();
                        List<ModelNew.Channel> recruitedChannels      = newUser.RecruitedChannels.ToList();

                        ModelOld.User oldUser = oldDb.Users.Where(x => x.Id == newUser.Id).SingleOrDefault();

                        Partner     oldPartnership = null;
                        Recruiter   oldRecruiting  = null;

                        if (ownedChannels.Count() > 0)
                        {
                            oldPartnership = oldUser.Partner;

                            if (oldPartnership == null)
                            {
                                string mex = $"{newUser.LastName} {newUser.Name}: Ha canali associati (Partnership) ma non risulta partner sul vecchio database";
                                Console.WriteLine(mex);
                                messages.Add(mex);
                            }
                        }

                        if(oldPartnership != null)
                        {
                            var oldChannels = oldPartnership.ChannelOwnerships;

                            int newOwnedChannelsCount = ownedChannels.Count();
                            int oldOwnedChannelsCount = oldChannels.Count();

                            if (newOwnedChannelsCount != oldOwnedChannelsCount)
                            {                                
                                string mex = $"{newUser.LastName} {newUser.Name}: ha una discrepanza sul numero canali (Partnership) {newOwnedChannelsCount}/{oldOwnedChannelsCount} (nuovi/vecchi)";
                                Console.WriteLine(mex);
                                messages.Add(mex);
                            }

                            foreach (var channel in oldChannels)
                            {                               
                                if (ownedChannels.Where(x => x.Id == channel.ChannelId).SingleOrDefault() == null)
                                {
                                    string mex = $"{newUser.LastName} {newUser.Name}: Canale mancante {channel.ChannelId} => {channel.Channel.Name}";
                                    Console.WriteLine(mex);
                                    messages.Add(mex);
                                }
                            }

                            foreach (var channel in ownedChannels)
                            {
                                if (oldChannels.Where(x => x.ChannelId == channel.Id).SingleOrDefault() == null)
                                {
                                    string mex = $"{newUser.LastName} {newUser.Name}: Canale erroneamente associato? {channel.Id} => {channel.Name}";
                                    Console.WriteLine(mex);
                                    messages.Add(mex);
                                }
                            }
                        }


                        if (recruitedChannels.Count() > 0)
                        {
                            oldRecruiting = oldUser.Recruiter;

                            if (oldRecruiting == null)
                            {
                                string mex = $"{newUser.LastName} {newUser.Name}: Ha canali associati (Recruiting) ma non risulta Recruiter sul vecchio database";
                                Console.WriteLine(mex);
                                messages.Add(mex);
                            }
                        }

                        if (oldRecruiting != null)
                        {
                            var oldChannels = oldRecruiting.ChannelRecruitings;

                            int newRecruitedChannelsCount = recruitedChannels.Count();
                            int oldRecruitedChannelsCount = oldChannels.Count();

                            if (newRecruitedChannelsCount != oldRecruitedChannelsCount)
                            {
                                string mex = $"{newUser.LastName} {newUser.Name}: ha una discrepanza sul numero canali (Recruiting) {newRecruitedChannelsCount}/{oldRecruitedChannelsCount} (nuovi/vecchi)";
                                Console.WriteLine(mex);
                                messages.Add(mex);
                            }

                            foreach (var channel in oldChannels)
                            {
                                if (recruitedChannels.Where(x => x.Id == channel.ChannelId).SingleOrDefault() == null)
                                {
                                    string mex = $"{newUser.LastName} {newUser.Name}: Canale mancante {channel.ChannelId} => {channel.Channel.Name}";
                                    Console.WriteLine(mex);
                                    messages.Add(mex);
                                }
                            }

                            foreach (var channel in recruitedChannels)
                            {
                                if (oldChannels.Where(x => x.ChannelId == channel.Id).SingleOrDefault() == null)
                                {
                                    string mex = $"{newUser.LastName} {newUser.Name}: Canale erroneamente associato? {channel.Id} => {channel.Name}";
                                    Console.WriteLine(mex);
                                    messages.Add(mex);
                                }
                            }
                        }


                        #region Comments

                        //Recruiter recruiting = oldUser.Recruiter;
                        //if (newUserRecruitedChannels.Count > 0 && recruiting == null)
                        //{
                        //    string message = $"{newUser.LastName} {newUser.Name}: Discrepanza accesso con vecchio DB (Recruiting)";
                        //    messages.Add(message);
                        //    continue;
                        //}


                        //List<ModelOld.ChannelRecruiting> oldUserRecruitedChannels = recruiting.ChannelRecruitings.ToList();

                        //if (newUserRecruitedChannels.Count() != oldUserRecruitedChannels.Count())
                        //{
                        //    string message = $"{newUser.LastName} {newUser.Name}: Discrepanza Conteggio Canali Reclutati";
                        //    messages.Add(message);
                        //}
                        //else
                        //{
                        //    List<string> newChannelIds = newUserRecruitedChannels.Select(x => x.Id).ToList();
                        //    List<string> oldChannelIds = oldUserRecruitedChannels.Select(x => x.ChannelId).ToList();

                        //    oldChannelIds.All(x => newChannelIds.Contains(x));

                        //    string message = $"{newUser.LastName} {newUser.Name}: canali errati (Recruiting)";
                        //    messages.Add(message);
                        //}

                        #endregion

                        File.WriteAllLines("output.txt", messages.ToArray());

                    }                    
                }
            }
        }



        [ConsoleUIMethod]
        public void NormalizeNames()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                List<ModelNew.User> users = db.Users.ToList();

                for (int i = 0; i < users.Count; i++)
                {
                    ModelNew.User  current = users[i];

                    if (!string.IsNullOrWhiteSpace(current.Name))
                    {
                        current.Name        = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(current.Name);
                        current.Name        = current.Name.Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(current.LastName))
                    {
                        current.LastName    = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(current.LastName);
                        current.LastName    = current.LastName.Trim();
                    }

                    db.SaveChanges();
                    OutPercentage(i, users.Count);
                }                
            }
        }

        [ConsoleUIMethod]
        public void DropUser(int id)
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                ModelNew.User u = db.Users.Find(id);
                u.AdministratorOf.Clear();
                db.Users.Remove(u);
                db.SaveChanges();
            }
        }

        [ConsoleUIMethod]
        public void EnumerateUsers()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.Users);
            }
        }

        [ConsoleUIMethod]
        public void MigrateUsers()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    MigrateTable(oldDb.Users, newDb.Users, t =>
                    {
                        ModelNew.User record        = new ModelNew.User();
                        record.Id                   = t.Id;
                        record.Name                 = t.Name;
                        record.LastName             = t.Surname;
                        record.BICSWIFT             = t.BICSWIFT;
                        record.BirthDate            = t.BirthDate;
                        record.BirthPlace           = t.BirthPlace;
                        record.CompanyKind          = t.CompanyKind;
                        record.CompanyName          = t.TutorCompanyName;
                        record.EMail                = t.Email;
                        record.FullAddress          = t.Full_Address;
                        record.IBAN                 = t.Iban;
                        record.Mobile               = t.Phone_Number;
                        record.Password             = t.Password;
                        record.PIVAorVAT            = t.PIVAORVAT;
                        record.PostalCode           = t.PostalCode;
                        record.Skype                = t.Skype;
                        record.PaymentMethod        = t.PaymentMethod;
                        record.Role                 = 2;

                        if (t.Payments.Count() > 0)
                            record.RegistrationDate = t.Payments.Min(x => x.Date);
                        else
                            record.RegistrationDate = DateTime.Now;

                        return record;
                    }, newDb, (index, total, entity) => OutPercentage(index, total));
                }
            }
        }

        [ConsoleUIMethod]
        public void MigrateTutors()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    MigrateTable(oldDb.Users.Where(x => 
                        !(x.TutorIban == null || x.TutorIban.Trim() == string.Empty) ||
                        !(x.TutorName == null || x.TutorName.Trim() == string.Empty) ||
                        !(x.TutorSurname == null || x.TutorSurname.Trim() == string.Empty) ||
                        !(x.TutorBirthPlace == null || x.TutorBirthPlace.Trim() == string.Empty) ||
                        !(x.TutorBICSWIFT == null || x.TutorBICSWIFT.Trim() == string.Empty) ||
                        !(x.TutorPostalCode == null || x.TutorPostalCode.Trim() == string.Empty) ||
                        !(x.TutorPostalCode == null || x.TutorPostalCode.Trim() == string.Empty) ||
                        !(x.TutorPIVAORVAT == null || x.TutorPIVAORVAT.Trim() == string.Empty) ||
                        !(x.TutorFullAddress == null || x.TutorFullAddress.Trim() == string.Empty) ||
                        x.TutorBirthDate.HasValue), newDb.Users, t =>
                    {
                        ModelNew.User admin           = new ModelNew.User();
                        admin.Id                      = newDb.Users.Max(x => x.Id) + 1;
                        admin.FullAddress             = t.TutorFullAddress;
                        admin.CompanyName             = t.TutorCompanyName;
                        admin.CompanyKind             = t.TutorCompanyKind;
                        admin.IBAN                    = t.TutorIban;
                        admin.PIVAorVAT               = t.TutorPIVAORVAT;
                        admin.PostalCode              = t.TutorPostalCode;
                        admin.BICSWIFT                = t.TutorBICSWIFT;
                        admin.Name                    = t.TutorName;
                        admin.LastName                = t.TutorSurname;
                        admin.PaymentMethod           = t.PaymentMethod;
                        admin.Role                    = 2;
                        
                        ModelNew.User partner         = newDb.Users.Find(t.Id);
                        partner.FiscalAdministratorId = admin.Id;
                        admin.RegistrationDate        = partner.RegistrationDate;

                        return admin;
                    }, newDb, (index, total, entity) => OutPercentage(index, total));
                }
            }

            Console.WriteLine("WARNING: Check for bad User Records (Some had whitespaces in fields)");
        }

        #endregion

        #region Channel

        [ConsoleUIMethod]
        public void EnumerateChannels()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                EnumerateTable(db.Channels);
            }
        }

        [ConsoleUIMethod]
        public void MigrateChannels()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    MigrateTable(oldDb.Channels, newDb.Channels, t =>
                    {
                        ModelNew.Channel record = new ModelNew.Channel();
                        record.Id               = t.Id;
                        record.Name             = t.Name;
                        record.OwnerId          = t.ChannelOwnership != null ? (int?)t.ChannelOwnership.PartnerId : null;
                        record.RecruiterId      = t.ChannelRecruiting != null ? (int?)t.ChannelRecruiting.RecruiterId : null;
                        record.RegistrationDate = t.RegistrationDate;
                        record.IsActive         = true;
                        record.LatestActivity   = DateTime.Now;
                        record.PercentMegatube  = t.PercentMegatube;
                        record.PercentOwner     = t.ChannelOwnership != null ? t.ChannelOwnership.PercentPartner : 0.0;
                        record.PercentRecruiter = t.ChannelRecruiting != null ? t.ChannelRecruiting.PercentRecruiter : 0.0;

                        return record;
                    }, newDb, (index, total, entity) => Console.WriteLine($"{index}/{total}"));
                }
            }
        }
        #endregion



        //var item = (Mixer.GetType().GetProperty("exposedParameters").GetValue(Mixer, null) as IEnumerable<object>).Select(x => new Parameter(x.GetType().GetField("name").GetValue(x) as string, Mixer))

        //public enum AccreditationMainType : byte
        //{
        //    Traffic = 0,
        //    PaidFeatures = 1
        //    Extra = 2,
        //}

        //public enum AccreditationSubType : byte
        //{
        //    Ownership = 0,
        //    Recruiting = 1,
        //    Manual = 2,
        //    NetworkPerformance = 3
        //}

        [ConsoleUIMethod]
        public void CleanDatabase()
        {                        
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                List<string> idsToSave  = File.ReadAllLines("CurrentChannels.txt").Select(x => x.Substring(2)).ToList();
                List<int> peapoleToSAve = new List<int>();

                foreach (ModelNew.Channel channel in db.Channels.ToList())
                {
                    if (idsToSave.Contains(channel.Id))
                    {
                        if (channel.Owner != null)
                        {
                            peapoleToSAve.Add(channel.OwnerId.Value);

                            if (channel.Owner.FiscalAdministrator != null)
                                peapoleToSAve.Add(channel.Owner.FiscalAdministratorId.Value);
                        }

                        if (channel.Recruiter != null)
                        {
                            peapoleToSAve.Add(channel.RecruiterId.Value);

                            if (channel.Recruiter.FiscalAdministrator != null)
                                peapoleToSAve.Add(channel.Recruiter.FiscalAdministratorId.Value);
                        }
                    }
                    else
                    {
                        try
                        {
                            db.Channels.Remove(channel);
                            db.SaveChanges();
                            Console.WriteLine("Channel Removed => " + channel.Name);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }

                foreach (ModelNew.User user in db.Users.ToList())
                {
                    try
                    {
                        if (peapoleToSAve.Contains(user.Id))
                            continue;

                        db.Users.Remove(user);
                        db.SaveChanges();
                        Console.WriteLine("User Removed => " + user.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }


        [ConsoleUIMethod]
        public void CreateAdministrator()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                try
                {
                    ModelNew.User admin = new ModelNew.User();
                    admin.Role = 1;
                    admin.Name = "Admin";
                    admin.LastName = "Admin";
                    admin.EMail = "admin";
                    admin.Password = "2464b94ceb49c2115c4238f51be98d8b";
                    db.Users.Add(admin);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
   

        #region FullProcedures

        [ConsoleUIMethod]
        public void DropDatabase()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                Console.Write("\nAre you sure?> ");
                if (Console.ReadLine() != "y")
                    return;

                db.Database.ExecuteSqlCommand("delete [PaymentAlert]");
                db.Database.ExecuteSqlCommand("delete [Accreditation]");
                db.Database.ExecuteSqlCommand("delete [Payment]");
                db.Database.ExecuteSqlCommand("delete [DataFile]");
                db.Database.ExecuteSqlCommand("delete [Note]");
                db.Database.ExecuteSqlCommand("delete [Channel]");
                db.Database.ExecuteSqlCommand("delete [User]");

                Console.WriteLine("Done!");
            }
        }

        [ConsoleUIMethod]
        public void MigrateDatabase()
        {
            Console.WriteLine("Migrating Users");
            MigrateUsers();

            Console.WriteLine("Migrating Administrators");
            MigrateTutors();

            Console.WriteLine("Migrating Channels");
            MigrateChannels();

            Console.WriteLine("Migrating Payments");
            MigratePayments();

            Console.WriteLine("Migrating Accreditations");
            MigrateAccreditations();
        }

        #endregion

        static void Main(string[] args)
        {
            ConsoleUI console = new ConsoleUI(new Program(), "Megatube Data Migrator");
            console.Run();
        }
    }
}