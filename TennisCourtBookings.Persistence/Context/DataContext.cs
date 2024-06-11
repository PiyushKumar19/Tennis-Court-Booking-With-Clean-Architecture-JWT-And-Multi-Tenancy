using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Common;
using TennisCourtBookings.Application;
using TennisCourtBookings.Application.Repositories;
using TennisCourtBookings.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace TennisCourtBookings.Persistence.Context
{
    public class DataContext : DbContext
    {
        public string TenantId { get; private set; }
        private readonly ITenantService _tenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataContext(DbContextOptions<DataContext> options, ITenantService tenantService, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _tenantService = tenantService;
            _httpContextAccessor = httpContextAccessor;

            // Check if JWT token is available
            if (_httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated == true)
            {
                TenantId = _tenantService.GetTenant()?.TID;
            }
            else
            {
                // Use default tenant if no token is present
                TenantId = "Defaults"; // or any other default tenant ID
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CourtBookings> CourtBookings { get; set; }
        public DbSet<TennisCourt> TennisCourts { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply query filters only if TenantId is set
            //if (!string.IsNullOrEmpty(TenantId) && TenantId != "Defaults")
            //{
            //    modelBuilder.Entity<Product>().HasQueryFilter(a => a.TenantId == TenantId);
            //}

            modelBuilder.Entity<Product>().HasQueryFilter(a => a.TenantId == TenantId);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure options only if TenantId is set
            if (!string.IsNullOrEmpty(TenantId) && TenantId != "Defaults")
            {
                var tenantConnectionString = _tenantService.GetConnectionString();
                if (!string.IsNullOrEmpty(tenantConnectionString))
                {
                    var DBProvider = _tenantService.GetDatabaseProvider();
                    if (DBProvider.ToLower() == "mssql")
                    {
                        optionsBuilder.UseSqlServer(tenantConnectionString);
                    }
                }
            }
            else
            {
                // Use default connection string
                optionsBuilder.UseSqlServer(_tenantService.GetConnectionStringFromTenantId("Defaults"));
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (!string.IsNullOrEmpty(TenantId) && TenantId != "Defaults")
            {
                foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        case EntityState.Modified:
                            entry.Entity.TenantId = TenantId;
                            break;
                    }
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
