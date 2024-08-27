using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SongModels.Request
{
    public class SongUpdateReqModel
    {
        public int songId { get; set; }
        public IFormFile music_path { get; set; } = null!;
        public IFormFile image_path { get; set; } = null!;
        public string? Title { get; set; } = null!;
        public string? Lyrics { get; set; }
    }
}
