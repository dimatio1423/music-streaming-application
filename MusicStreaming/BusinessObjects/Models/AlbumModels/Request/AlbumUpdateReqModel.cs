using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.AlbumModels.Request
{
    public class AlbumUpdateReqModel
    {
        [Required (ErrorMessage ="AlbumId is required")]
        public int AlbumId { get; set; }

        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateOnly ReleaseDate { get; set; }

        [StringLength(255, ErrorMessage = "Genre cannot exceed 255 characters.")]

        public string Genre { get; set; }
    }
}
