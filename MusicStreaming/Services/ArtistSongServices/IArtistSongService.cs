using BusinessObjects.Models.ArtistSongModel.Request;
using BusinessObjects.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ArtistSongServices
{
    public interface IArtistSongService
    {
        Task<ResultModel> AddNewArtistToSong(ArtistSongCreateReqModel artistSongCreateReqModel, string token);
    }
}
