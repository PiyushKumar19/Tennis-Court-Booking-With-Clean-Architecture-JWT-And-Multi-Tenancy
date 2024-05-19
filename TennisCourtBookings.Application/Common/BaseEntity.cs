using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisCourtBookings.Application.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; private set; }
    }
}
