using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisCourtBookings.Application.Features.UserFeatures.RegisterUser
{
    public sealed record RegisterUserRequest(string Email, string Username, string Password, string TenantId) : IRequest<RegisterUserResponse>;

}
