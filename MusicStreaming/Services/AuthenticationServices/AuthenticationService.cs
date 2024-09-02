﻿using BusinessObjects.Entities;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.UserModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.RefreshTokenRepos;
using Repositories.UserRepos;
using Services.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationService(IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _tokenHandler = new JwtSecurityTokenHandler();
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string decodeToken(string jwtToken, string nameClaim)
        {
            Claim? claim = _tokenHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));

            //_tokenHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
            return claim != null ? claim.Value : "Error!!!";
        }

        public string GenerateJWT(User User)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:JwtKey"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new()
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role),
                new Claim("userid", User.UserId.ToString()),
                new Claim("email", User.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credential
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        public string GenerateRefreshToken()
        {
            var newRefreshToken = Guid.NewGuid().ToString();
            return newRefreshToken;
        }
    }
}
