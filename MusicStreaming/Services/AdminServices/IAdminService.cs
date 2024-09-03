using BusinessObjects.Models.AlbumModels.Request;
using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SongModels.Request;
using BusinessObjects.Models.UserModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AdminServices
{
    public interface IAdminService
    {
        Task<ResultModel> ViewUsers(int? page, int?size, UserViewReqModel userViewReqModel, string token);
        Task<ResultModel> ViewAlbums(int? page, int?size, AlbumViewReqModel albumViewReqModel, string token);
        Task<ResultModel> ViewSongs(int? page, int?size, SongViewReqModel songViewReqModel, string token);
        Task<ResultModel> ViewArtists(int? page, int?size, ArtistViewReqModel artistViewReqModel, string token);
        Task<ResultModel> SearchUserForAdmin(int? page, int? size, UserSearchReqModel userSearchReqModel, string token);
        Task<ResultModel> SearchArtistForAdmin(int? page, int? size, ArtistSearchReqModel artistSearchReqModel, string token);
        Task<ResultModel> SearchAlbumForAdmin(int? page, int? size, AlbumSearchReqModel albumSearchReqModel, string token);
        Task<ResultModel> SearchSongForAdmin(int? page, int? size, SongSearchReqModel songSearchReqModel, string token);
        Task<ResultModel> DeleteUser(int userId, string token);

    }
}
