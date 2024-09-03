using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.AlbumModels.Request;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SongModels.Request;
using BusinessObjects.Models.SongModels.Response;
using BusinessObjects.Models.UserModels.Request;
using BusinessObjects.Models.UserModels.Response;
using Repositories.AlbumRepos;
using Repositories.AlbumSongsRepo;
using Repositories.ArtistRepos;
using Repositories.ListeningHistoryRepos;
using Repositories.PlayListRepos;
using Repositories.PlaylistSongRepos;
using Repositories.SongRepos;
using Repositories.SubscriptionUserRepos;
using Repositories.UserFavoriteRepos;
using Repositories.UserQueueRepos;
using Repositories.UserRepos;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ISongRepository _songRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IPlaylistSongRepository _playlistSongRepository;
        private readonly IUserFavoriteRepository _userFavoriteRepository;
        private readonly IListeningHistoryRepository _listeningHistoryRepository;
        private readonly IAlbumSongRepository _albumSongRepository;
        private readonly IUserQueueRepository _userQueueRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IDecodeTokenHandler _decodeToken;

        public AdminService(IUserRepository userRepository, 
            ISongRepository songRepository, 
            IArtistRepository artistRepository, 
            IAlbumRepository albumRepository, 
            IAlbumSongRepository albumSongRepository, 
            IPlaylistRepository playlistRepository, 
            IPlaylistSongRepository playlistSongRepository,
            IUserFavoriteRepository userFavoriteRepository,
            IUserQueueRepository userQueueRepository,
            IListeningHistoryRepository listeningHistoryRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IDecodeTokenHandler decodeToken,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _playlistRepository = playlistRepository;
            _playlistSongRepository = playlistSongRepository;
            _userFavoriteRepository = userFavoriteRepository;
            _listeningHistoryRepository = listeningHistoryRepository;
            _albumSongRepository = albumSongRepository;
            _userQueueRepository = userQueueRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _decodeToken = decodeToken;
            _mapper = mapper;


        }
        public async Task<ResultModel> DeleteUser(int userId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var deletedUser = await _userRepository.Get(userId);
                
                if (deletedUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (deletedUser.Status.Equals(StatusEnums.Inactive.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The user has already been deleted";
                    return result;
                }

                
                if (deletedUser.Role.Equals(RoleEnums.User.ToString())) {

                    var playlistOfUser = await _playlistRepository.GetPlaylistsByUserId(deletedUser.UserId);

                   

                    var favoriteListOfUser = await _userFavoriteRepository.GetUserFavorites(deletedUser.UserId);

                    var queueListOfUser = await _userQueueRepository.GetUserQueue(deletedUser.UserId);

                    var listeningHistoryOfUser = await _listeningHistoryRepository.GetListeningHistoriesByUser(deletedUser.UserId);

                    var userSubscription = await _userSubscriptionRepository.GetUserSubscriptionsOfUser(deletedUser.UserId);

                    foreach (var item in playlistOfUser)
                    {

                        var playListSongOfUsers = await _playlistSongRepository.GetPlaylistSongsByPlaylistId(item.PlaylistId);

                        foreach (var playlistSong in playListSongOfUsers)
                        {
                            await _playlistSongRepository.Remove(playlistSong);
                        }

                        await _playlistRepository.Remove(item);
                    }

                    foreach (var item in favoriteListOfUser)
                    {
                        await _userFavoriteRepository.Remove(item);
                    }

                    foreach (var item in queueListOfUser)
                    {
                        await _userQueueRepository.Remove(item);
                    }

                    foreach (var item in listeningHistoryOfUser)
                    {
                        await _listeningHistoryRepository.Remove(item);
                    }

                    foreach (var item in userSubscription)
                    {
                        await _userSubscriptionRepository.Remove(item);
                    }

                    deletedUser.Status = StatusEnums.Inactive.ToString();

                    await _userRepository.Update(deletedUser);

                }else if (deletedUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    var currArtist = await _artistRepository.GetArtistByUserId(deletedUser.UserId);
                    if (currArtist == null)
                    {
                        result.IsSuccess = false;
                        result.Code = (int)HttpStatusCode.NotFound;
                        result.Message = "Artist does not exist";
                        return result;
                    }

                    var albumsOfArtist = await _albumRepository.GetAlbumsByArtist(currArtist.ArtistId);

                    foreach (var item in albumsOfArtist)
                    {
                        var albumSongs = await _albumSongRepository.GetAlbumSongsByAlbumId(item.AlbumId);

                        foreach (var albumSong in albumSongs)
                        {
                            await _albumSongRepository.Remove(albumSong);
                        }

                        await _albumRepository.Remove(item);
                    }

                    var songsOfArtist = await _songRepository.GetSongsByArtist(currArtist.ArtistId);

                    var playListsContainsSong = await _playlistSongRepository.GetPlaylistSongsBySongIds(songsOfArtist.Select(x => x.SongId).ToList());

                    var favoriteContainsSong = await _userFavoriteRepository.GetUserFavoriteBySongIds(songsOfArtist.Select(x => x.SongId).ToList());

                    var queueContainSong = await _userQueueRepository.GetUserQueueBySongIds(songsOfArtist.Select(x => x.SongId).ToList());

                    var listeningHistoryContainsSong = await _listeningHistoryRepository.GetListeningHistoriesBySongIds(songsOfArtist.Select(x => x.SongId).ToList());

                    foreach (var item in playListsContainsSong)
                    {
                        await _playlistSongRepository.Remove(item);
                    }

                    foreach (var item in favoriteContainsSong)
                    {
                        await _userFavoriteRepository.Remove(item);
                    }

                    foreach (var item in queueContainSong)
                    {
                        await _userQueueRepository.Remove(item);
                    }

                    foreach (var item in listeningHistoryContainsSong)
                    {
                        await _listeningHistoryRepository.Remove(item);
                    }

                    foreach (var song in songsOfArtist)
                    {
                        song.Status = StatusEnums.Inactive.ToString();
                        await _songRepository.Update(song);
                    }


                    deletedUser.Status = StatusEnums.Inactive.ToString();
                    await _userRepository.Update(deletedUser);
                }else
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Can not delete user with role admin";
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> SearchAlbumForAdmin(int? page, int? size, AlbumSearchReqModel albumSearchReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Search albums successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var albums = await _albumRepository.GetAlbums(page, size);

                if (!string.IsNullOrEmpty(albumSearchReqModel.searchValue))
                {
                    albums = await _albumRepository.SearchByAlbumName(albumSearchReqModel.searchValue, page, size);
                }

                albums = FilterFeatureForViewAlbum(albums, new AlbumViewReqModel
                {
                    ArtistName = albumSearchReqModel.ArtistName,
                    Genre = albumSearchReqModel.Genre,
                    StartDate = albumSearchReqModel.StartDate,
                    EndDate = albumSearchReqModel.EndDate
                });

                albums = SortFeatureForViewAlbum(albums, albumSearchReqModel.sortBy);

                result.Data = _mapper.Map<List<AlbumViewResModel>>(albums);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> SearchArtistForAdmin(int? page, int? size, ArtistSearchReqModel artistSearchReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Search artist successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var artists = await _artistRepository.GetArtists(page, size);

                if (!string.IsNullOrEmpty(artistSearchReqModel.searchValue))
                {
                    artists = await _artistRepository.SearchByArtistName(artistSearchReqModel.searchValue, page, size);
                }

                artists = FilterFeatureForViewArtist(artists, new ArtistViewReqModel{
                    StartDate = artistSearchReqModel.StartDate,
                    EndDate = artistSearchReqModel.EndDate
                });

                artists = SortFeatureForViewArtist(artists, artistSearchReqModel.sortBy);

                result.Data = _mapper.Map<List<ArtistViewResModel>>(artists);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> SearchSongForAdmin(int? page, int? size, SongSearchReqModel songSearchReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Search song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var songs = await _songRepository.GetSongs(page, size);

                if (!string.IsNullOrEmpty(songSearchReqModel.searchValue))
                {
                    songs = await _songRepository.SearchBySongName(songSearchReqModel.searchValue, page, size);
                }

                songs = FilterFeatureForViewSong(songs, new SongViewReqModel
                {
                    ArtistName = songSearchReqModel.ArtistName,
                    Status = songSearchReqModel.Status,
                    StartDate = songSearchReqModel.StartDate,
                    EndDate = songSearchReqModel.EndDate,
                });

                songs = SortFeatureForViewSong(songs, songSearchReqModel.sortBy);

                result.Data = _mapper.Map<List<SongsResModel>>(songs);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> SearchUserForAdmin(int? page, int? size, UserSearchReqModel userSearchReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Search user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var users = await _userRepository.GetAll(page, size);

                if (!string.IsNullOrEmpty(userSearchReqModel.searchValue))
                {
                    users = await _userRepository.SearchByUserName(userSearchReqModel.searchValue, page, size);
                }

                users = FilterFeatureForViewUser(users, new UserViewReqModel
                {
                    subscription = userSearchReqModel.subscription,
                    role = userSearchReqModel.role,
                    status = userSearchReqModel.status,
                    StartDate = userSearchReqModel.StartDate,
                    EndDate = userSearchReqModel.EndDate,
                });

                users = SortFeatureForViewUser(users, userSearchReqModel.sortBy);

                result.Data = _mapper.Map<List<UserViewResModel>>(users);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewAlbums(int? page, int? size, AlbumViewReqModel albumViewReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View albums successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var albums = await _albumRepository.GetAlbums(page, size);

                albums = FilterFeatureForViewAlbum(albums, albumViewReqModel);

                albums = SortFeatureForViewAlbum(albums, albumViewReqModel.sortBy);

                result.Data = _mapper.Map<List<AlbumViewResModel>>(albums);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewArtists(int? page, int? size, ArtistViewReqModel artistViewReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View artists successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);
                
                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var artists = await _artistRepository.GetArtists(page, size);

                artists = FilterFeatureForViewArtist(artists, artistViewReqModel);

                artists = SortFeatureForViewArtist(artists, artistViewReqModel.sortBy);

                result.Data = _mapper.Map<List<ArtistViewResModel>>(artists);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewSongs(int? page, int? size, SongViewReqModel songViewReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View songs successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var songs = await _songRepository.GetSongs(page, size);

                songs = FilterFeatureForViewSong(songs, songViewReqModel);

                songs = SortFeatureForViewSong(songs, songViewReqModel.sortBy);

                result.Data = _mapper.Map<List<SongsResModel>>(songs);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewUsers(int? page, int? size, UserViewReqModel userViewReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currAdmin = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currAdmin == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Admin does not exist";
                    return result;
                }

                if (!currAdmin.Role.Equals(RoleEnums.Admin.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var users = await _userRepository.GetAll(page, size);

                users = FilterFeatureForViewUser(users, userViewReqModel);

                users = SortFeatureForViewUser(users, userViewReqModel.sortBy);

                result.Data = _mapper.Map<List<UserViewResModel>>(users);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public List<User> FilterFeatureForViewUser(List<User> list, UserViewReqModel userViewReqModel)
        {
            if (userViewReqModel.subscription is not null && userViewReqModel.subscription.Any())
            {
                list = list.Where(x => userViewReqModel.subscription.Contains(x.SubscriptionType)).ToList();
            }

            if (userViewReqModel.role is not null && userViewReqModel.role.Any())
            {
                list = list.Where(x => userViewReqModel.role.Contains(x.Role)).ToList();
            }

            if (userViewReqModel.status is not null && userViewReqModel.status.Any())
            {
                list = list.Where(x => userViewReqModel.status.Contains(x.Status)).ToList();
            }

            if (userViewReqModel.StartDate != null && userViewReqModel.EndDate != null)
            {

                DateTime startDate = userViewReqModel.StartDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDate = userViewReqModel.EndDate.Value.ToDateTime(TimeOnly.MinValue);

                list = list.Where(c => c.CreatedAt >= startDate  && c.CreatedAt <= endDate).ToList();
            }

            return list;
        }

        public List<User> SortFeatureForViewUser(List<User> list, string sortBy)
        {
            switch(sortBy)
            {
                case "name_asc":
                    list = list.OrderBy(x => x.Username).ToList();
                    break;
                case "name_desc":
                    list = list.OrderByDescending(x => x.Username).ToList();
                    break;
                case "date_asc":
                    list = list.OrderBy(x => x.CreatedAt).ToList();
                    break;
                case "date_desc":
                    list = list.OrderByDescending(x => x.Username).ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.Username).ToList();
                    break;
            }

            return list;
        }

        public List<Album> FilterFeatureForViewAlbum(List<Album> list, AlbumViewReqModel albumViewReqModel)
        {
            if (albumViewReqModel.ArtistName is not null && albumViewReqModel.ArtistName.Any())
            {
                list = list.Where(x => albumViewReqModel.ArtistName.Contains(x.Artist.Name)).ToList();
            }

            if (albumViewReqModel.Genre is not null && albumViewReqModel.Genre.Any())
            {
                list = list.Where(x => albumViewReqModel.Genre.Contains(x.Genre)).ToList();
            }

            if (albumViewReqModel.StartDate != null && albumViewReqModel.EndDate != null)
            {

                list = list.Where(c => c.ReleaseDate >= albumViewReqModel.StartDate && c.ReleaseDate <= albumViewReqModel.EndDate).ToList();
            }

            return list;
        }

        public List<Album> SortFeatureForViewAlbum(List<Album> list, string sortBy)
        {
            switch (sortBy)
            {
                case "title_asc":
                    list = list.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    list = list.OrderByDescending(x => x.Title).ToList();
                    break;
                case "release_date_asc":
                    list = list.OrderBy(x => x.ReleaseDate).ToList();
                    break;
                case "release_date_desc":
                    list = list.OrderByDescending(x => x.ReleaseDate).ToList();
                    break;
                case "genre_asc":
                    list = list.OrderBy(x => x.Genre).ToList();
                    break;
                case "genre_desc":
                    list = list.OrderByDescending(x => x.Genre).ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.Title).ToList();
                    break;
            }

            return list;
        }

        public List<Song> FilterFeatureForViewSong(List<Song> list, SongViewReqModel songViewReqModel)
        {
            if (songViewReqModel.ArtistName is not null && songViewReqModel.ArtistName.Any())
            {
                list = list.Where(x => x.ArtistSongs
                           .Select(artist => artist.Artist.Name)
                           .Any(artistName => songViewReqModel.ArtistName.Contains(artistName)))
               .ToList();
            }

            if (songViewReqModel.StartDate is not null && songViewReqModel.Status.Any())
            {
                list = list.Where(x => songViewReqModel.Status.Contains(x.Status)).ToList();
            }

            if (songViewReqModel.StartDate != null && songViewReqModel.EndDate != null)
            {
                DateTime startDate = songViewReqModel.StartDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDate = songViewReqModel.EndDate.Value.ToDateTime(TimeOnly.MinValue);

                list = list.Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate).ToList();
            }

            return list;
        }

        public List<Song> SortFeatureForViewSong(List<Song> list, string sortBy)
        {
            switch (sortBy)
            {
                case "title_asc":
                    list = list.OrderBy(x => x.Title).ToList();
                    break;
                case "title_desc":
                    list = list.OrderByDescending(x => x.Title).ToList();
                    break;
                case "created_date_asc":
                    list = list.OrderBy(x => x.CreatedAt).ToList();
                    break;
                case "created_date_desc":
                    list = list.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
                case "duration_asc":
                    list = list.OrderBy(x => x.Duration).ToList();
                    break;
                case "duration_desc":
                    list = list.OrderByDescending(x => x.Duration).ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.Title).ToList();
                    break;
            }

            return list;
        }

        public List<Artist> FilterFeatureForViewArtist(List<Artist> list, ArtistViewReqModel artistViewReqModel)
        {

            if (artistViewReqModel.StartDate != null && artistViewReqModel.EndDate != null)
            {
                DateTime startDate = artistViewReqModel.StartDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDate = artistViewReqModel.EndDate.Value.ToDateTime(TimeOnly.MinValue);

                list = list.Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate).ToList();
            }

            return list;
        }

        public List<Artist> SortFeatureForViewArtist(List<Artist> list, string sortBy)
        {
            switch (sortBy)
            {
                case "name_asc":
                    list = list.OrderBy(x => x.Name).ToList();
                    break;
                case "name_desc":
                    list = list.OrderByDescending(x => x.Name).ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.Name).ToList();
                    break;
            }

            return list;
        }
    }
}
