using BusinessObjects.Models.AlbumModels.Request;
using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.SongModels.Request;
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

        [HttpPost]
        [Route("song/view")]
        [Authorize]
        public async Task<IActionResult> ViewSongs(
          [FromBody] SongViewReqModel songViewReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.ViewSongs(page, size, songViewReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("song/search")]
        [Authorize]
        public async Task<IActionResult> SearchSong(
          [FromBody] SongSearchReqModel songSearchReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.SearchSongForAdmin(page, size, songSearchReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("album/view")]
        [Authorize]
        public async Task<IActionResult> ViewAlbums(
         [FromBody] AlbumViewReqModel albumViewReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.ViewAlbums(page, size, albumViewReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("album/search")]
        [Authorize]
        public async Task<IActionResult> SearchAlbums(
          [FromBody] AlbumSearchReqModel albumSearchReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.SearchAlbumForAdmin(page, size, albumSearchReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("artist/view")]
        [Authorize]
        public async Task<IActionResult> ViewArtists(
        [FromBody] ArtistViewReqModel artistViewReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.ViewArtists(page, size, artistViewReqModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("artist/search")]
        [Authorize]
        public async Task<IActionResult> SearchArtist(
          [FromBody] ArtistSearchReqModel artistSearchReqModel, [FromQuery] int? page = 1, int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _adminService.SearchArtistForAdmin(page, size, artistSearchReqModel, token);
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
