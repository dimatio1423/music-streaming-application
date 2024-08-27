using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserFavoriteRepos
{
    public interface IUserFavoriteRepository : IGenericRepository<UserFavorite>
    {
        Task<List<UserFavorite>> GetUserFavorites(int userId, int? page, int? size);

        Task<UserFavorite> CheckSongInUserFavorite(int songId, int userId);
        Task<List<UserFavorite>> GetUserFavoriteBySongId(int songId);

    }
}
