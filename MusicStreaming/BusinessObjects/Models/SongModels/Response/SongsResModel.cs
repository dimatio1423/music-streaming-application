using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SongModels.Response
{
    public class SongsResModel
    {
        public int songId { get; set; }

        public string songName { get; set; }

        public TimeOnly? Duration { get; set; }

        public string FilePath { get; set; } = null!;

        public string? Lyrics { get; set; }

        public string? ImagePath { get; set; }

        public List<AlbumViewResModel> songAlbums { get; set; }

        public List<ArtistViewResModel> songArtist { get; set; }
    }
}
