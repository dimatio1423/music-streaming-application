using BusinessObjects.Models.PlaylistModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.PlaylistServices;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlayListService _playListService;

        public PlaylistController(IPlayListService playListService)
        {
            _playListService = playListService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPlaylistOfUser([FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _playListService.GetPlaylistOfUser(page, size, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistCreateReqModel playlistCreateReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _playListService.CreatePlaylist(playlistCreateReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePlaylist([FromBody] PlaylistUpdateReqModel playlistUpdateReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _playListService.UpdatePlaylist(playlistUpdateReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePlaylist([FromQuery] int playlistId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _playListService.RemovePlaylist(playlistId, token);
            return StatusCode(result.Code, result);
        }
    }
}
