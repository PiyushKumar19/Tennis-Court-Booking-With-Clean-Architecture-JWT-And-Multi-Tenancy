using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisCourtBookings.Application.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> Save(CancellationToken cancellationToken);
    }
}
