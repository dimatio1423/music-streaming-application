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
    public class SearchController : ControllerBase
    {
        private readonly ISongService _songService;

        public SearchController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> PauseSong(
        [FromQuery] string searchValue, [FromQuery] string searchFilter, [FromQuery] int? page, [FromQuery] int? size)
        {
            var result = await _songService.Search(searchValue, searchFilter, page, size);
            return StatusCode(result.Code, result);
        }
    }
}
