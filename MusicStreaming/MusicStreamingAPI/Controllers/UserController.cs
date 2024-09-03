using BusinessObjects.Models.PasswordModel;
using BusinessObjects.Models.RefreshTokenModel.Request;
using BusinessObjects.Models.UserModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.SubscriptionServices;
using Services.UserServices;
using System.ComponentModel.DataAnnotations;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;

        public UserController(IUserService userService, ISubscriptionService subscriptionService)
        {
            _userService = userService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _userService.ViewUserProfile(token);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserUpdateProfileReqModel userUpdateProfileReqModel)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _userService.UpdateUserProfile(userUpdateProfileReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var result = await _userService.ForgotPassword(email);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordReqModel changePasswordReq)
        {

            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _userService.ChangePassword(changePasswordReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenReqModel refreshTokenReqModel)
        {
            var result = await _userService.Logout(refreshTokenReqModel);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordReqModel resetPasswordReq)
        {

            //var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _userService.ResetPassword(resetPasswordReq);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("subscription")]
        public async Task<IActionResult> ViewSubscriptions(string? filterBy)
        {
            //var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.ViewSubscription(filterBy);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("subscription/{subscriptionId}")]
        public async Task<IActionResult> ViewDetailsSubscription(int subscriptionId)
        {
            //var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.ViewDetailsSubscription(subscriptionId);
            return StatusCode(result.Code, result);
        }

    }
}
