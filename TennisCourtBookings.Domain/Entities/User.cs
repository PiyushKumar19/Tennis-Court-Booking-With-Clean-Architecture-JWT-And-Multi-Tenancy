using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Domain.Common.Entities;

namespace TennisCourtBookings.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public string Email { get; set; }
        public string TenantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DOB { get; set; }
    }
}
