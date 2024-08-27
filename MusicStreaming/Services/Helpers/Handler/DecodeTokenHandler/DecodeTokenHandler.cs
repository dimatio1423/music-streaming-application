using BusinessObjects.Models.TokenModel;
using Services.AuthenticationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers.Handler.DecodeTokenHandler
{
    public class DecodeTokenHandler : IDecodeTokenHandler
    {
        private readonly IAuthenticationService _authenticationService;

        public DecodeTokenHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        public TokenModel decode(string token)
        {
            var roleName = _authenticationService.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType);
            var userId = _authenticationService.decodeToken(token, "userid");
            var email = _authenticationService.decodeToken(token, "email");

            return new TokenModel(userId, roleName, email);
        }
    }
}
