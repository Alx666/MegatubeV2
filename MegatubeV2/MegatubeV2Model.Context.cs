﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegatubeV2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MegatubeV2Entities : DbContext
    {
        public MegatubeV2Entities()
            : base("name=MegatubeV2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Partnership> Partnerships { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Network> Networks { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<ViewNote> ViewNotes { get; set; }
        public virtual DbSet<DataFile> DataFiles { get; set; }
        public virtual DbSet<ViewChannel> ViewChannels { get; set; }
        public virtual DbSet<Accreditation> Accreditations { get; set; }
        public virtual DbSet<PaymentAlert> PaymentAlerts { get; set; }
    }
}
