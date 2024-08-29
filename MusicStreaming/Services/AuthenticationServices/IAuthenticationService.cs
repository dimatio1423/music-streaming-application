using BusinessObjects.Entities;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        string GenerateJWT(User User);
        string GenerateRefreshToken();
        string decodeToken(string jwtToken, string nameClaim);
    }
}
