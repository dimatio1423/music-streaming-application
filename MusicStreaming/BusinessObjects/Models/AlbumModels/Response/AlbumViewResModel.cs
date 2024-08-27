using BusinessObjects.Models.ArtistModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.AlbumModels.Response
{
    public class AlbumViewResModel
    {
        public int AlbumId { get; set; }

        public ArtistViewResModel Artist { get; set; }

        public string Title { get; set; } = null!;

        public string Genre { get; set; }
    }
}
