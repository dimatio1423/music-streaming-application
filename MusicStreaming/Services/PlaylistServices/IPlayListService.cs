using BusinessObjects.Models.PlaylistModels.Request;
using BusinessObjects.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PlaylistServices
{
    public interface IPlayListService
    {
        Task<ResultModel> CreatePlaylist(PlaylistCreateReqModel playlistCreateReq, string token);
        Task<ResultModel> RemovePlaylist(int playlistId, string token);
        Task<ResultModel> UpdatePlaylist(PlaylistUpdateReqModel playlistUpdateReq, string token);
        Task<ResultModel> GetPlaylistOfUser(int? page, int? size, string token);
    }
}
