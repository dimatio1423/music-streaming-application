using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.PasswordModel;
using BusinessObjects.Models.RefreshTokenModel.Request;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SubscriptionModel.Response;
using BusinessObjects.Models.UserModels;
using BusinessObjects.Models.UserModels.Request;
using BusinessObjects.Models.UserModels.Response;
using CloudinaryDotNet.Actions;
using Repositories.ArtistRepos;
using Repositories.RefreshTokenRepos;
using Repositories.SubscriptionUserRepos;
using Repositories.UserRepos;
using Services.AuthenticationServices;
using Services.CloudinaryService;
using Services.EmailService;
using Services.FileServices;
using Services.Helper.CustomExceptions;
using Services.Helpers.Handler.DecodeTokenHandler;
using Services.OTPServices;
using Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IFileService _fileService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository,
            IArtistRepository artistRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IAuthenticationService authenticationService,
            IDecodeTokenHandler decodeToken,
            IFileService fileService,
            ICloudinaryService cloudinaryService,
            IEmailService emailService,
            IRefreshTokenRepository refreshTokenRepository,
            IOTPService otpService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _artistRepository = artistRepository;
            _authenticationService = authenticationService;
            _decodeToken = decodeToken;
            _fileService = fileService;
            _cloudinaryService = cloudinaryService;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
            _otpService = otpService;
            _userSubscriptionRepository = userSubscriptionRepository;
            _mapper = mapper;
        }

        public async Task<ResultModel> ChangePassword(ChangePasswordReqModel changePasswordReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Change password successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!PasswordHasher.VerifyPassword(changePasswordReq.OldPassword, currUser.Password))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The old password is incorrect";
                    return result;
                }

                if (changePasswordReq.NewPassword.Equals(changePasswordReq.OldPassword))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The new password is the same with the old password";
                    return result;
                }

                currUser.Password = PasswordHasher.HashPassword(changePasswordReq.NewPassword);

                await _userRepository.Update(currUser);

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

        public async Task<ResultModel> ForgotPassword(string email)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Sent OTP through email successfully"
            };

            try
            {
                var currUser = await _userRepository.GetUserByEmail(email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var newOtp = await _otpService.CreateOTPCodeForEmail(currUser.UserId);

                await _emailService.SendUserResetPassword(currUser.Username, currUser.Email, newOtp);

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

        public async Task<ResultModel> Login(UserLoginReqModel userLoginModel)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Login successfully"
            };

            try
            {
                var currUser = await _userRepository.GetUserByEmail(userLoginModel.Email);
                if (currUser != null)
                {
                    if (PasswordHasher.VerifyPassword(userLoginModel.Password, currUser.Password))
                    {
                        var token = _authenticationService.GenerateJWT(currUser);

                        var refreshToken = _authenticationService.GenerateRefreshToken();

                        var newRefreshToken = new RefreshToken
                        {
                            RefreshToken1 = refreshToken,
                            ExpiredAt = DateTime.Now.AddDays(1),
                            UserId = currUser.UserId
                        };

                        await _refreshTokenRepository.Insert(newRefreshToken);

                        var userLoginRes = new UserLoginResModel
                        {
                            UserId = currUser.UserId,
                            Username = currUser.Username,
                            Email = currUser.Email,
                            SubscriptionType = currUser.SubscriptionType,
                            Role = currUser.Role,
                            Token = token,
                            RefreshToken = refreshToken
                        };

                        result.Data = userLoginRes;

                        //return userLoginRes;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Code = (int)HttpStatusCode.BadRequest;
                        result.Message = "Incorrect password";
                        return result;
                        //throw new ApiException(HttpStatusCode.BadRequest, "Incorrect password");

                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                    //throw new ApiException(HttpStatusCode.NotFound, "User does not exist");
                }
            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }
            return result;
        }

        public async Task<ResultModel> Register(UserRegisterReqModel userRegisterReqModel)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Register user account successfully"

            };

            try
            {
                var currUserEmail = await _userRepository.GetUserByEmail(userRegisterReqModel.Email);

                var currUsername = await _userRepository.GetUserByUsername(userRegisterReqModel.Username);

                if (currUsername != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The username is already taken by another user";

                    return result;
                }

                if (currUserEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int) HttpStatusCode.BadRequest;
                    result.Message = "The email is already taken by another user";

                    return result;
                }

                //if (!Enum.IsDefined(typeof(RoleEnums), userRegisterReqModel.Role))
                //{
                //    result.IsSuccess = false;
                //    result.Code = (int)HttpStatusCode.BadRequest;
                //    result.Message = "Invalid role";

                //    return result;
                //}

                User newUser = new User
                {
                    Username = userRegisterReqModel.Username,
                    Email = userRegisterReqModel.Email,
                    Password = PasswordHasher.HashPassword(userRegisterReqModel.Password),
                    CreatedAt = DateTime.Now,
                    SubscriptionType = SubscriptionTypeEnums.Free.ToString(),
                    Role = RoleEnums.User.ToString(),
                    Status = StatusEnums.Active.ToString()
                };

                await _userRepository.Insert(newUser);

            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> RegisterArtist(ArtistRegisterReqModel artistRegisterReqModel)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Register artist account successfully, \n Please update your artist information"

            };

            try
            {
                var currUserEmail = await _userRepository.GetUserByEmail(artistRegisterReqModel.Email);

                var currUsername = await _userRepository.GetUserByUsername(artistRegisterReqModel.Username);

                if (currUsername != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The username is already taken by another user";

                    return result;
                }

                if (currUserEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The email is already taken by another user";

                    return result;
                }

                //if (!Enum.IsDefined(typeof(RoleEnums), userRegisterReqModel.Role))
                //{
                //    result.IsSuccess = false;
                //    result.Code = (int)HttpStatusCode.BadRequest;
                //    result.Message = "Invalid role";

                //    return result;
                //}

                User newUser = new User
                {
                    Username = artistRegisterReqModel.Username,
                    Email = artistRegisterReqModel.Email,
                    Password = PasswordHasher.HashPassword(artistRegisterReqModel.Password),
                    CreatedAt = DateTime.Now,
                    SubscriptionType = null,
                    Role = RoleEnums.Artist.ToString(),
                    Status = StatusEnums.Active.ToString()
                };

                await _userRepository.Insert(newUser);

                var newUserArtist = await _userRepository.GetUserByEmail(newUser.Email);

                Artist newArtist = new Artist
                {
                    Name = artistRegisterReqModel.ArtistName,
                    CreatedAt = DateTime.Now,
                    UserId = newUserArtist.UserId
                };


                await _artistRepository.Insert(newArtist);

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

        public async Task<ResultModel> ResetPassword(ResetPasswordReqModel resetPasswordReq)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Reset password successfully"
            };

            try
            {

                var currUser = await _userRepository.GetUserByEmail(resetPasswordReq.Email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                await _otpService.VerifyOTP(resetPasswordReq.OTP, currUser.UserId);

                currUser.Password = PasswordHasher.HashPassword(resetPasswordReq.NewPassword);

                await _userRepository.Update(currUser);

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

        public async Task<ResultModel> UpdateUserProfile(UserUpdateProfileReqModel userUpdateProfileReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update user profile account successfully"

            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.User.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                if (!currUser.Username.Equals(userUpdateProfileReq.Username))
                {
                    var currUsername = await _userRepository.GetUserByUsername(userUpdateProfileReq.Username);
                    if (currUser != null)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)HttpStatusCode.NotFound;
                        result.Message = "User does not exist";
                        return result;
                    }
                }

                

                if (userUpdateProfileReq.ImagePath != null)
                {
                    _fileService.CheckImageFile(userUpdateProfileReq.ImagePath);
                    if (currUser.ImagePath != null)
                    {
                        await _cloudinaryService.DeleteFile(currUser.ImagePath);
                    }
                }

                var imageResult = userUpdateProfileReq.ImagePath != null ? await _cloudinaryService.AddPhoto(userUpdateProfileReq.ImagePath) : null;

                currUser.ImagePath = imageResult != null ? imageResult.SecureUrl.ToString() : currUser.ImagePath;
                currUser.Username = !string.IsNullOrEmpty(userUpdateProfileReq.Username) ? userUpdateProfileReq.Username : currUser.Username;

                await _userRepository.Update(currUser);
            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewUserProfile(string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View user profile successfully"

            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.User.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                result.Data = _mapper.Map<UserViewProfileResModel>(currUser);


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

        public string GenerateOTP()
        {
            var otp = new Random().Next(100000, 999999).ToString();

            return otp;
        }

        public async Task<ResultModel> Logout(RefreshTokenReqModel refreshTokenReqModel)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Log out successfully"

            };

            try
            {
                var currRefreshToken = await _refreshTokenRepository.GetByRefreshToken(refreshTokenReqModel.RefreshToken);

                if (currRefreshToken == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Refresh Token does not exist";
                    return result;
                }

                await _refreshTokenRepository.Remove(currRefreshToken);

            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewCurrentSubsctiptionOfUser(string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get current user subscription successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.User.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }


                var currentSubscription = await _userSubscriptionRepository.GetUserSubscriptionsOfUser(currUser.UserId);

                result.Data = _mapper.Map<List<SubscriptionViewResModel>>(currentSubscription);

            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewUserInfor(string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get user successfully"
            };


            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);
                    if (currArtist == null)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)HttpStatusCode.NotFound;
                        result.Message = "Artist does not exist";
                        return result;
                    }

                    var artistInfo = _mapper.Map<ArtistViewResModel>(currArtist);

                    result.Data = artistInfo;
                    return result;
                }

                result.Data = _mapper.Map<UserViewResModel>(currUser);

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
