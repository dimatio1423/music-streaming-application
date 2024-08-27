using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.PlaylistModels.Request
{
    public class PlaylistCreateReqModel
    {
        [Required(ErrorMessage ="Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string title { get; set; }
    }
}
