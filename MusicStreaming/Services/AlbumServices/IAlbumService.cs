using BusinessObjects.Models.AlbumModels.Request;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AlbumServices
{
    public interface IAlbumService
    {
        public Task<ResultModel> GetAlbums(int? page, int? size);
        public Task<ResultModel> CreateAlbum(AlbumCreateReqModel albumCreateReq, string token);
        public Task<ResultModel> GetAlbumsByArtist(int artistId, int?page, int?size);
        public Task<ResultModel> GetAlbumsByGenre(string genre, int?page, int?size);
        public Task<ResultModel> GetRecommendAlbumsForUser(int? page, int? size, string token);
        public Task<ResultModel> UpdateAlbum(AlbumUpdateReqModel albumUpdateReq, string token);
        public Task<ResultModel> RemoveAlbum(int albumId, string token);
        public Task<ResultModel> ViewDetailsAlbum(int albumId, int? page , int? size);

    }
}
