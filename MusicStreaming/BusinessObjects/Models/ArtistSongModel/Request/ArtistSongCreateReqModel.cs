using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistSongModel.Request
{
    public class ArtistSongCreateReqModel
    {
        [Required(ErrorMessage ="ArtistId is required")]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "SongId is required")]
        public int SongId { get; set; }

        [Required(ErrorMessage = "Role description is required")]
        public string RoleDescription { get; set; }
    }
}
