using BusinessObjects.Models.RefreshTokenModel.Request;
using BusinessObjects.Models.RefreshTokenModel.Response;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.UserModels.Response;
using Repositories.RefreshTokenRepos;
using Repositories.UserRepos;
using Services.AuthenticationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Services.RefreshTokenServices
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IAuthenticationService authenticationService, IUserRepository userRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }
        public async Task<ResultModel> GetRefreshToken(RefreshTokenReqModel refreshTokenReq)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Refresh token successfully",
            };

            try
            {
                var currRefreshToken = await _refreshTokenRepository.GetByRefreshToken(refreshTokenReq.RefreshToken);

                if (currRefreshToken == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Refresh token not found";
                    return result;
                }

                if (currRefreshToken.ExpiredAt < DateTime.Now)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Refresh token is expired";
                    return result;
                }

                var currUser = await _userRepository.Get(currRefreshToken.UserId);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User not found";
                    return result;
                }

                var token = _authenticationService.GenerateJWT(currUser);

                var newRefreshToken = _authenticationService.GenerateRefreshToken();

                currRefreshToken.RefreshToken1 = newRefreshToken;
                currRefreshToken.ExpiredAt = DateTime.Now.AddDays(1);
                await _refreshTokenRepository.Update(currRefreshToken);


                result.Data = new RefreshTokenResModel
                {
                    AccessToken = token,
                    RefreshToken = newRefreshToken
                };


            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }
    }
}
