using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Repositories;
using TennisCourtBookings.Persistence.Context;

namespace TennisCourtBookings.Persistence.Services
{
    //public interface IAdminTenantService
    //{
    //    Task DeleteUserDataAsync(string tenantId, string userId);
    //}

    //public class AdminTenantService : IAdminTenantService
    //{
    //    private readonly ITenantService _tenantService;

    //    public AdminTenantService(ITenantService tenantService)
    //    {
    //        _tenantService = tenantService;
    //    }

    //    public async Task DeleteUserDataAsync(string tenantId, string userId)
    //    {
    //        var connectionString = _tenantService.GetConnectionStringFromTenantId(tenantId);

    //        using (var context = new DataContext(connectionString))
    //        {
    //            // Perform deletion
    //            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    //            if (user != null)
    //            {
    //                context.Users.Remove(user);
    //                await context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //}

}
