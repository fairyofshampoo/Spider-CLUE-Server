﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBaseManager
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SpiderClueDbEntities : DbContext
    {
        public SpiderClueDbEntities()
            //: base("name=SpiderClueDbEntities")
            :base(Environment.GetEnvironmentVariable("DATABASE"))
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<accessAccount> accessAccounts { get; set; }
        public virtual DbSet<friendList> friendLists { get; set; }
        public virtual DbSet<friendRequest> friendRequests { get; set; }
        public virtual DbSet<gamer> gamers { get; set; }
        public virtual DbSet<guessPlayer> guessPlayers { get; set; }
        public virtual DbSet<match> matches { get; set; }
        public virtual DbSet<matchResult> matchResults { get; set; }
        public virtual DbSet<report> reports { get; set; }
    }
}
