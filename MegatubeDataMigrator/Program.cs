using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegatubeDataMigrator.ModelNew;
using MegatubeDataMigrator.ModelOld;
using System.Data.Entity;
using System.Collections.Concurrent;

namespace MegatubeDataMigrator
{
    class Program
    {
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

        private void MigrateTable<T, K>(DbSet<T> source, DbSet<K> dest, Func<T, K> mapper)
            where T : class
            where K : class
        {
            ConcurrentQueue<K> queue = new ConcurrentQueue<K>();

            source.AsParallel().ForAll(t =>
            {
                K newRecord = mapper.Invoke(t);
                queue.Enqueue(newRecord);
            });

            dest.AddRange(queue.ToList());
        }


        #region Payment Alerts

        [ConsoleUIMethod]
        public void DropPaymentAlerts()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.PaymentAlerts);
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
                DropTable(db.Payments);
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

        #endregion

        #region Accreditations

        [ConsoleUIMethod]
        public void DropAccreditations()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.Accreditations);
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

        #endregion

        #region DataFiles

        [ConsoleUIMethod]
        public void DropDataFiles()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.DataFiles);
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

        #region Users

        [ConsoleUIMethod]
        public void DropUsers()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                DropTable(db.Users);
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

                        if (t.Payments.Count() > 0)
                            record.RegistrationDate = t.Payments.Min(x => x.Date);
                        else
                            record.RegistrationDate = DateTime.Now;

                        return record;
                    });
                }
            }
        }

        public void MigrateTutors()
        {
            using (MegatubeV2Entities newDb = new MegatubeV2Entities())
            {
                using (MegatubeEntitiesOld oldDb = new MegatubeEntitiesOld())
                {
                    MigrateTable((DbSet<ModelOld.User>)oldDb.Users.Where(x => x.TutorIban != null), newDb.Users, t => 
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

                        ModelNew.User partner         = newDb.Users.Find(t.Id);
                        partner.FiscalAdministratorId = admin.Id;

                        return admin;
                    });
                }
            }
        }

        #endregion


        #region FullProcedures

        [ConsoleUIMethod]
        public void DropDatabase()
        {
            using (MegatubeV2Entities db = new MegatubeV2Entities())
            {
                Console.Write("\nAre you sure?> ");
                if (Console.ReadLine() != "y")
                    return;

                DropTable(db.PaymentAlerts, false);
                DropTable(db.Payments, false);
                DropTable(db.Accreditations, false);
                DropTable(db.DataFiles, false);
                DropTable(db.Notes, false);
                DropTable(db.Users, false);
            }
        }

        #endregion

        static void Main(string[] args)
        {
            ConsoleUI console = new ConsoleUI(new Program(), "Megatube Data Migrator");
            console.Run();
        }
    }
}
                            //ModelNew.User admin = new ModelNew.User();
                            //admin.FullAddress   = t.TutorFullAddress;
                            //admin.CompanyName   = t.TutorCompanyName;
                            //admin.CompanyKind   = t.TutorCompanyKind;
                            //admin.IBAN          = t.TutorIban;
                            //admin.PIVAorVAT     = t.TutorPIVAORVAT;
                            //admin.PostalCode    = t.TutorPostalCode;
                            //admin.BICSWIFT      = t.TutorBICSWIFT;
                            //admin.Name          = t.TutorName;
                            //admin.LastName      = t.TutorSurname;
                            //result.Add(admin);