using BusinessObjects.Models.RefreshTokenModel.Request;
using BusinessObjects.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RefreshTokenServices
{
    public interface IRefreshTokenService
    {
        Task<ResultModel> GetRefreshToken(RefreshTokenReqModel refreshTokenReq);
    }
}
