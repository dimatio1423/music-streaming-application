using BusinessObjects.Models.SubscriptionModel.Request;
using BusinessObjects.Models.UserModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AdminServices;
using Services.SubscriptionServices;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAdminService _adminService;

        public AdminController(ISubscriptionService subscriptionService, IAdminService adminService)
        {
            _subscriptionService = subscriptionService;
            _adminService = adminService;
        }

        [HttpPost]
        [Route("user/view")]
        [Authorize]
        public async Task<IActionResult> ViewUsers(
           [FromBody] UserViewReqModel userViewReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.ViewUsers(page, size, userViewReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("user/search")]
        [Authorize]
        public async Task<IActionResult> SearchUser(
          [FromBody] UserSearchReqModel userSearchReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.SearchUserForAdmin(page, size, userSearchReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("subscription")]
        [Authorize]
        public async Task<IActionResult> ViewSubscriptions()
        {
            //var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.ViewSubscription();
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("subscription")]

        [Authorize]
        public async Task<IActionResult> AddNewSubscription([FromBody] SubscriptionAddReqModel subscriptionAddReqModel)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.AddNewSubscription(subscriptionAddReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Route("subscription")]

        [Authorize]
        public async Task<IActionResult> UpdateSubscription([FromBody] SubscriptionUpdateReqModel subscriptionUpdateReqModel)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.UpdateSubscription(subscriptionUpdateReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Route("subscription/{subscriptionId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscription([FromRoute] int subscriptionId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _subscriptionService.DeleteSubscription(subscriptionId, token);
            return StatusCode(result.Code, result);
        }
    }
}
