﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Macellum.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBConnectionContainer : DbContext
    {
        public DBConnectionContainer()
            : base("name=DBConnectionContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Fisk> Fisks { get; set; }
        public virtual DbSet<Arter> Arters { get; set; }
        public virtual DbSet<Havn> Havns { get; set; }
        public virtual DbSet<BenzinPris> BenzinPris { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<ActiveSessionId> ActiveSessionIds { get; set; }
    }
}
