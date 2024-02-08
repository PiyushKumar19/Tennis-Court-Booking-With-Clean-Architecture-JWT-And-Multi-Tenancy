using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Features.UserFeatures.RegisterUser;
using TennisCourtBookings.Application.Repositories;
using TennisCourtBookings.Domain.Entities;

namespace TennisCourtBookings.Application.Features.UserFeatures.LoginUser
{
    public sealed class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration configuration;

        public LoginUserHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);

            var exist = await _userRepository.GetByEmail(user.Email, cancellationToken);
            if (exist == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(request.Password, exist.PasswordHash, exist.PasswordSalt))
            {
                return null;
            }

            string token = CreateToken(exist);
            var loginResponse = new LoginUserResponse();

            loginResponse.Token = token;

            return _mapper.Map<LoginUserResponse>(loginResponse);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                //new Claim(ClaimTypes.Role, "Rookie") BY
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool VerifyPasswordHash(string password, byte[] passhwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passhwordHash);
            }
        }
    }
}
