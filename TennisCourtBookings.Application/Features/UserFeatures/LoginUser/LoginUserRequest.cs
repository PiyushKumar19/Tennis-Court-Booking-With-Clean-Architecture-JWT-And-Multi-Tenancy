using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Features.UserFeatures.RegisterUser;

namespace TennisCourtBookings.Application.Features.UserFeatures.LoginUser
{
    public sealed record LoginUserRequest(string Email, string Password, string TenantId) : IRequest<LoginUserResponse>;

}
