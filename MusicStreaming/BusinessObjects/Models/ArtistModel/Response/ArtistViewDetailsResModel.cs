using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.SongModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistModel.Response
{
    public class ArtistViewDetailsResModel
    {
        public int ArtistId { get; set; }

        public string Name { get; set; } = null!;

        public string Bio { get; set; }

        public DateTime CreatedAt { get; set; }

        public string imagePath { get; set; }

        public List<AlbumViewResModel> Albums { get; set; }
    }
}
