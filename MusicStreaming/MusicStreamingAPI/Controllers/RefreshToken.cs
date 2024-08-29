using BusinessObjects.Models.RefreshTokenModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.RefreshTokenServices;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshToken : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshToken(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> RefreshNewToken(
          [FromBody] RefreshTokenReqModel refreshTokenReqModel)
        {
            var result = await _refreshTokenService.GetRefreshToken(refreshTokenReqModel);
            return StatusCode(result.Code, result);
        }
    }
}
