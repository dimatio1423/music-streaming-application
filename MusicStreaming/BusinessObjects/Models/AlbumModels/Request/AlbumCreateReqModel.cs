using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.AlbumModels.Request
{
    public class AlbumCreateReqModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = null!;

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Release is required.")]
        public DateOnly ReleaseDate { get; set; }

        [StringLength(255, ErrorMessage = "Genre cannot exceed 255 characters.")]
        [Required(ErrorMessage = "Genre is required.")]

        public string Genre { get; set; }
    }
}
