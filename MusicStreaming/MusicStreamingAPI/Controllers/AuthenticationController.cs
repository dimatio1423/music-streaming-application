using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.UserModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthenticationServices;
using Services.UserServices;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel user)
        {
            ResultModel result = await _userService.Login(user);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("register/artist")]
        public async Task<IActionResult> ArtistRegister([FromBody] ArtistRegisterReqModel artistRegisterReqModel)
        {
            ResultModel result = await _userService.RegisterArtist(artistRegisterReqModel);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("register/user")]
        public async Task<IActionResult> Register([FromBody] UserRegisterReqModel userRegisterReqModel)
        {
            ResultModel result = await _userService.Register(userRegisterReqModel);
            return StatusCode(result.Code, result);
        }

    }
}
