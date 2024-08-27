using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.PasswordModel;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.UserModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public interface IUserService
    {
        Task<ResultModel> Register(UserRegisterReqModel userRegisterReqModel);
        Task<ResultModel> RegisterArtist(ArtistRegisterReqModel artistRegisterReqModel);
        Task<ResultModel> Login(UserLoginReqModel userLoginModel);
        Task<ResultModel> ChangePassword(ChangePasswordReqModel changePasswordReq, string token);
        Task<ResultModel> ForgotPassword(string email);
        Task<ResultModel> ResetPassword(ResetPasswordReqModel resetPasswordReq,string token);
        Task<ResultModel> ViewUserProfile(string token);
        Task<ResultModel> UpdateUserProfile(UserUpdateProfileReqModel userUpdateProfileReq, string token);
    }
}
