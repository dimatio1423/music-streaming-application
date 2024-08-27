using BusinessObjects.Models.SongModels.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.SongServices;
using System.ComponentModel.DataAnnotations;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllSongs([FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetSongs(page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("{songId}")]
        public async Task<IActionResult> GetSong([FromRoute] int songId)
        {
            var result = await _songService.GetSong(songId);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("album/{albumId}")]
        [Authorize]
        public async Task<IActionResult> GetSongsByAlbum([FromRoute] int albumId,
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetSongsByAlbum(albumId, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("artist/{artistId}")]
        [Authorize]
        public async Task<IActionResult> GetSongsByArtist([FromRoute] int artistId,
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetSongsByArtist(artistId, page, size);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("history")]
        [Authorize]
        public async Task<IActionResult> GetUserListeningHisotry(
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetUserListeningHisotry(page, size, token);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("favorite")]
        [Authorize]
        public async Task<IActionResult> GetUserFavoriteSongs(
            [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetUserFavoriteSongs(page, size, token);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("recommend-song")]
        [Authorize]
        public async Task<IActionResult> GetRecommendSongsForUser(
           [FromQuery] int? page = 1, [FromQuery] int? size = 10)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.GetRecommendSongsForUser(page, size, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("album/add-song")]
        [Authorize]
        public async Task<IActionResult> AddSongToAlbum(
           [FromQuery] [Required] int songId, [FromQuery] [Required] int albumId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.AddSongToAlbums(songId, albumId, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("playlist/add-song")]
        [Authorize]
        public async Task<IActionResult> AddSongToPlaylist(
           [FromQuery][Required] int songId, [FromQuery][Required] int playlistId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.AddSongToPlaylist(songId, playlistId, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Route("favorite/add-song")]
        [Authorize]
        public async Task<IActionResult> AddSongToFavorite(
           [FromQuery][Required] int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.AddSongToUserFavorite(songId, token);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("album/check-song")]
        [Authorize]
        public async Task<IActionResult> CheckSongExistingInAlbum(
           [FromQuery][Required] int songId, [FromQuery][Required] int albumId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.CheckSongExistingInAlbum(songId, albumId);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("playlist/check-song")]
        [Authorize]
        public async Task<IActionResult> CheckSongExistingInPlaylist(
           [FromQuery][Required] int songId, [FromQuery][Required] int playlistId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.CheckSongExistingInPlaylist(songId, playlistId);
            return StatusCode(result.Code, result);
        }

        [HttpGet]
        [Route("favorite/check-song")]
        [Authorize]
        public async Task<IActionResult> CheckSongExistingInUserFavorite(
           [FromQuery][Required] int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.CheckSongInUserFavorite(songId, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Route("album/remove-song")]
        [Authorize]
        public async Task<IActionResult> RemoveSongFromAlbum(
           [FromQuery][Required] int songId, [FromQuery][Required] int albumId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.RemoveSongFromAlbum(songId, albumId, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Route("playlist/remove-song")]
        [Authorize]
        public async Task<IActionResult> RemoveSongFromPlaylist(
          [FromQuery][Required] int songId, [FromQuery][Required] int albumId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.RemoveSongFromPlaylist(songId, albumId, token);
            return StatusCode(result.Code, result);
        }

        [HttpDelete]
        [Route("favorite/remove-song")]
        [Authorize]
        public async Task<IActionResult> RemoveSongFromUserFavorite(
          [FromQuery][Required] int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.RemoveSongFromUserFavorite(songId, token);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNewSong(
           [Required] IFormFile music_path,
           IFormFile image_path,
           string title,
           string lyrics)
        {
            var songCreateReq = new SongCreateReqModel
            {
                music_path = music_path,
                image_path = image_path,
                Title = title,
                Lyrics = lyrics
            };

            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.AddNewSong(songCreateReq, token);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateSong(
           IFormFile music_path,
           IFormFile image_path,
           string title,
           string lyrics)
        {
            var songUpdateReq = new SongUpdateReqModel
            {
                music_path = music_path,
                image_path = image_path,
                Title = title,
                Lyrics = lyrics
            };

            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.UpdateSong(songUpdateReq, token);
            return StatusCode(result.Code, result);
        }


        [HttpDelete]
        [Route("{songId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSong(
           [FromRoute] int songId)
        {

            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            var result = await _songService.RemoveSong(songId, token);
            return StatusCode(result.Code, result);
        }
    }
}
