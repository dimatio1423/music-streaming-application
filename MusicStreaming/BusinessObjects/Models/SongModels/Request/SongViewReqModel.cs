using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SongModels.Request
{
    public class SongViewReqModel
    {
        public List<string>? ArtistName { get; set; }
        public List<string>? Status { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? sortBy { get; set; }
    }
}
