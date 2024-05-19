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
        public string TenantId { get; set; }
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
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CourtBookings> CourtBookings { get; set; }
        public DbSet<TennisCourt> TennisCourts { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!string.IsNullOrEmpty(TenantId))
            {
                modelBuilder.Entity<Product>().HasQueryFilter(a => a.TenantId == TenantId);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_httpContextAccessor.HttpContext?.User.Identity.IsAuthenticated == true)
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
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (!string.IsNullOrEmpty(TenantId))
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
