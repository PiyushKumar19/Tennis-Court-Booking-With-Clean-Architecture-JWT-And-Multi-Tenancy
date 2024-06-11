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
                // Set the default tenant when no JWT token is present
                //_currentTenant = _tenantSettings.Defaults.ConnectionString;

                //if (_currentTenant == null)
                //{
                //    throw new Exception("Default tenant not configured in tenant settings.");
                //}

                if (string.IsNullOrEmpty(_tenantSettings.Defaults.ConnectionString))
                {
                    _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
                }
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
                var isDefault = tenantId == "Defaults"; // Check if the tenantId is the default tenant identifier

                if (isDefault)
                {
                    _currentTenant = new Tenant
                    {
                        TID = "Defaults",
                        ConnectionString = _tenantSettings.Defaults.ConnectionString // Set the default connection string
                    };
                }
                else
                {
                    throw new Exception("Invalid Tenant!");
                }
            }
            else if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                // If the tenant exists but has no connection string, set the default connection string
                _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
            }
        }


        public string GetConnectionString()
        {
            return _currentTenant?.ConnectionString;
        }

        public string GetConnectionStringFromTenantId(string tenantId)
        {
            SetTenant(tenantId);
            return GetConnectionString();
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
