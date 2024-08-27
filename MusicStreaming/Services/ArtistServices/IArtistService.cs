using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ResultModels;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ArtistServices
{
    public interface IArtistService
    {
        Task<ResultModel> ViewAllArtists(int? page, int? size);
        Task<ResultModel> ViewArtistProfile(string token);
        Task<ResultModel> UpdateArtistProfile(ArtistUpdateProfileReqModel artistUpdateProfileReq, string token);
        Task<ResultModel> ViewDetailsOfArtist(int artistId, int? page, int? size);
    }
}
