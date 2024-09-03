using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ArtistSongModel.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ArtistSongRepos;
using Services.ArtistServices;
using Services.ArtistSongServices;
using System.ComponentModel.DataAnnotations;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IArtistSongService _artistSongService;

        public ArtistController(IArtistService artistService, IArtistSongService artistSongService)
        {
            _artistService = artistService;
            _artistSongService = artistSongService;
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> GetArtistProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _artistService.ViewArtistProfile(token);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("{artistId}")]
        
        public async Task<IActionResult> ViewDetailsArtist([FromRoute] int artistId, [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var result = await _artistService.ViewDetailsOfArtist(artistId, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Authorize]

        public async Task<IActionResult> UpdateArtistProfile([Required] IFormFile imagePath, string name, string bio)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var artistUpdateModel = new ArtistUpdateProfileReqModel
            {
                Bio = bio,
                Name = name,
                ImagePath = imagePath
            };

            var result = await _artistService.UpdateArtistProfile(artistUpdateModel, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Authorize]
        [Route("Add-artist-to-song")]

        public async Task<IActionResult> AddNewArtistToSong([FromBody] ArtistSongCreateReqModel artistSongCreateReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _artistSongService.AddNewArtistToSong(artistSongCreateReq, token);
            return StatusCode(result.Code, result);
        }
    }
}
