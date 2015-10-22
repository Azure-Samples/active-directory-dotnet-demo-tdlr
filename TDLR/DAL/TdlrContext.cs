using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tdlr.Models;

namespace Tdlr.DAL
{
    public class TdlrContext : DbContext
    {
        public TdlrContext() : base("TdlrContext") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>().HasMany<AadObject>(t => t.SharedWith).WithMany(a => a.Tasks).Map(m =>
            {
                m.MapLeftKey("TaskID");
                m.MapRightKey("AadObjectID");
                m.ToTable("Shares");
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<AadObject> AadObjects { get; set; }
        public DbSet<TokenCacheEntry> TokenCacheEntries { get; set; }
    }
}