using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SongModels.Request
{
    public class PauseSongReqModel
    {
        [Required (ErrorMessage ="SongId is required")]
        public int SongId { get; set; }

        [Required(ErrorMessage ="PauseTime of song is required")]
        public TimeOnly PauseTime { get; set; }
    }
}
