using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.AlbumModels.Request
{
    public class AlbumViewReqModel
    {
        public List<string>? ArtistName { get; set; }
        public List<string>? Genre { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? sortBy { get; set; }
    }
}
