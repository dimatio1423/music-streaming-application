using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SongModels.Request
{
    public class SongCreateReqModel
    {
        [Required(ErrorMessage = "Music path is required.")]
        public IFormFile music_path { get; set; }

        public IFormFile image_path { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = null!;

        public string? Lyrics { get; set; }
    }
}
