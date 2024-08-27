using BusinessObjects.Enums;
using BusinessObjects.Models.AlbumModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AlbumServices;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet]
        [Route("{albumId}")]
        public async Task<IActionResult> ViewDetailsAlbum([FromRoute] int albumId,[FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var result = await _albumService.ViewDetailsAlbum(albumId, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAlbums([FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.GetAlbums(page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("artist/{artistId}")]
        [Authorize]
        public async Task<IActionResult> GetAlbumsByArtist([FromRoute] int artistId, 
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.GetAlbumsByArtist(artistId, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("genre")]
        [Authorize]
        public async Task<IActionResult> GetAlbumsByGenre([FromQuery] string genre = "Pop",
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.GetAlbumsByGenre(genre, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("recommend-album")]
        [Authorize]
        public async Task<IActionResult> GetRecommendAlbumsForUser(
           [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.GetRecommendAlbumsForUser(page, size, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewAlbum(
           [FromBody] AlbumCreateReqModel albumCreateReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.CreateAlbum(albumCreateReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAlbum(
           [FromBody] AlbumUpdateReqModel albumUpdateReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.UpdateAlbum(albumUpdateReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Route("{albumId}")]
        [Authorize]
        public async Task<IActionResult> RemoveAlbum(
           [FromRoute] int albumId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _albumService.RemoveAlbum(albumId, token);
            return StatusCode(result.Code, result);
        }
    }
}
