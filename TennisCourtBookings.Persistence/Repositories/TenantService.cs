using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Repositories;
using TennisCourtBookings.Application;

namespace TennisCourtBookings.Persistence.Repositories
{

    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;
        private readonly IHttpContextAccessor _contextAccessor;
        private Tenant _currentTenant;

        public TenantService(IOptions<TenantSettings> tenantSettings, IHttpContextAccessor contextAccessor)
        {
            _tenantSettings = tenantSettings.Value;
            _contextAccessor = contextAccessor;

            // Check if the HTTP context is available and user is authenticated
            if (_contextAccessor.HttpContext != null && _contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var tenantId = GetTenantIdFromJwt();

                if (!string.IsNullOrEmpty(tenantId))
                {
                    SetTenant(tenantId);
                }
                else
                {
                    throw new Exception("Tenant ID claim not found in JWT token.");
                }
            }
            else
            {
                // Handle cases where there's no JWT token (e.g., during user registration)
                // You may choose to set a default tenant or handle this case as per your application logic
                _currentTenant = _tenantSettings.Tenants.FirstOrDefault(); // Adjust this as per your requirements
            }
        }

        private string GetTenantIdFromJwt()
        {
            var tenantIdClaim = _contextAccessor.HttpContext.User?.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                throw new Exception("Tenant ID claim not found in JWT token.");
            }

            return tenantIdClaim;
        }

        private void SetTenant(string tenantId)
        {
            _currentTenant = _tenantSettings.Tenants.FirstOrDefault(a => a.TID == tenantId);

            if (_currentTenant == null)
            {
                throw new Exception("Invalid Tenant!");
            }

            if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                SetDefaultConnectionStringToCurrentTenant();
            }
        }

        private void SetDefaultConnectionStringToCurrentTenant()
        {
            _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
        }

        public string GetConnectionString()
        {
            return _currentTenant?.ConnectionString;
        }

        public string GetDatabaseProvider()
        {
            return _tenantSettings.Defaults?.DBProvider;
        }

        public Tenant GetTenant()
        {
            return _currentTenant;
        }
    }

}
