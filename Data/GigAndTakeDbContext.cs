using System;
using System.Collections.Generic;
using System.Linq;
using GATIntegrations.Data.EFEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GATIntegrations.Data
{
    public partial class GigAndTakeDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public GigAndTakeDbContext(DbContextOptions<GigAndTakeDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("GATConnStr"));
            }
        }

        public DbSet<LuWorkerType> LuWorkerType { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LuWorkerType>(entity =>
            {
                entity.ToTable("LuWorkerType", tb => tb.HasTrigger("trg_LuWorkerType"));
            });

            OnModelCreatingPartial(modelBuilder);
        }
    }

}
public class WorkerIds
{
    public Guid WorkerId { get; set; }
}