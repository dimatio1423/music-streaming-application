using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.PlaylistModels.Response;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SongModels.Request;
using BusinessObjects.Models.SongModels.Response;
using Microsoft.AspNetCore.Http;
using Repositories.AlbumRepos;
using Repositories.AlbumSongsRepo;
using Repositories.ArtistRepos;
using Repositories.ArtistSongRepos;
using Repositories.ListeningHistoryRepos;
using Repositories.PlayListRepos;
using Repositories.PlaylistSongRepos;
using Repositories.SongRepos;
using Repositories.UserFavoriteRepos;
using Repositories.UserQueueRepos;
using Repositories.UserRepos;
using Services.CloudinaryService;
using Services.FileServices;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IUserRepository _userRepository;
        private readonly IListeningHistoryRepository _listeningHistoryRepository;
        private readonly IUserFavoriteRepository _userFavoriteRepository;
        private readonly IPlaylistSongRepository _playlistSongRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IAlbumSongRepository _albumSongRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IArtistSongRepository _artistSongRepository;
        private readonly IUserQueueRepository _userQueueRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public SongService(ISongRepository songRepository, 
            IDecodeTokenHandler decodeToken, 
            IUserRepository userRepository,
            IListeningHistoryRepository listeningHistoryRepository,
            IUserFavoriteRepository userFavoriteRepository,
            IArtistRepository artistRepository,
            IPlaylistSongRepository playlistSongRepository,
            IPlaylistRepository playlistRepository,
            IAlbumRepository albumRepository, 
            IAlbumSongRepository albumSongRepository,
            ICloudinaryService cloudinaryService,
            IArtistSongRepository artistSongRepository,
            IUserQueueRepository userQueueRepository,
            IFileService fileService,
            IMapper mapper)
        {
            _songRepository = songRepository;
            _decodeToken = decodeToken;
            _userRepository = userRepository;
            _listeningHistoryRepository = listeningHistoryRepository;
            _userFavoriteRepository = userFavoriteRepository;
            _playlistSongRepository = playlistSongRepository;
            _playlistRepository = playlistRepository;
            _albumRepository = albumRepository;
            _albumSongRepository = albumSongRepository;
            _artistRepository = artistRepository;
            _cloudinaryService = cloudinaryService;
            _artistSongRepository = artistSongRepository;
            _userQueueRepository = userQueueRepository;

            _fileService = fileService;

            _mapper = mapper;
        }

        public async Task<ResultModel> AddNewSong(SongCreateReqModel songCreateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var permittedAudioExtensions = new[] { ".mp3", ".wav", ".aac", ".flac" };
                var musicFileExtension = Path.GetExtension(songCreateReq.music_path.FileName).ToLowerInvariant();

                _fileService.CheckMusicFile(songCreateReq.music_path);


                if (songCreateReq.image_path != null)
                {
                    _fileService.CheckImageFile(songCreateReq.image_path);
                }
                

                var resultAudio = await _cloudinaryService.AddAudio(songCreateReq.music_path);
                var resultImg = songCreateReq.image_path != null ? await _cloudinaryService.AddPhoto(songCreateReq.image_path) : null;

                var duration = _fileService.GetDurationOfMusicPath(songCreateReq.music_path);

                Song newSong = new Song
                {
                    Title = songCreateReq.Title,
                    Duration = duration,
                    FilePath = resultAudio.SecureUrl.ToString(),
                    CreatedAt = DateTime.Now,
                    Status = true.ToString(),
                    Lyrics = songCreateReq.Lyrics,
                    ImagePath = resultImg != null ? resultImg.SecureUrl.ToString() : null
                };

                var songId = await _songRepository.AddSong(newSong);

                var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";

                    return result;
                }

                ArtistSong artistSong = new ArtistSong
                {
                    ArtistId = currArtist.ArtistId,
                    SongId = songId,
                    RoleDescription = "Main artist"
                };

                await _artistSongRepository.Insert(artistSong);

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

        public async Task<ResultModel> AddSongToAlbums(int songId, int albumId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to album successfully"
            };

            try
            {

                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);
                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The artist does not exist";
                    return result;
                }

                var songOfArtist = await _songRepository.GetSongsByArtist(currArtist.ArtistId);

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                if (!songOfArtist.Contains(currSong))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The current song does not belong to current artist";
                    return result;
                }

                var albumOfArtist = await _albumRepository.GetAlbumsByArtist(currArtist.ArtistId);

                var currAlbum = await _albumRepository.Get(albumId);

                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current album does not exist";
                    return result;
                }

                if (!albumOfArtist.Contains(currAlbum))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The current album does not belong to current artist";
                    return result;
                }

                var checkAlbumSongExisting = await _albumSongRepository.CheckSongExistingInAlbum(currSong.SongId, currAlbum.AlbumId);
                if (checkAlbumSongExisting != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The current song already existed in the current album";
                    return result;
                }

                var latestTrackNumber = await _albumSongRepository.GetLatestTrackNumberOfAnAlbum(currAlbum.AlbumId);

                AlbumSong newAlbumSong = new AlbumSong
                {
                    AlbumId = currAlbum.AlbumId,
                    SongId = currSong.SongId,
                    TrackNumber = latestTrackNumber + 1,
                };

                await _albumSongRepository.Insert(newAlbumSong);


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

        public async Task<ResultModel> AddSongToPlaylist(int songId, int playlistId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to playlist successfully"
            };

            try
            {

                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                var currPlaylist = await _playlistRepository.Get(playlistId);

                if (currPlaylist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current playlist does not exist";
                    return result;
                }

                if (currUser.UserId != currPlaylist.UserId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The current playlist does not belong to current user";
                    return result;
                }

                PlaylistSong newPlaylistSong = new PlaylistSong
                {
                    PlaylistId = currPlaylist.PlaylistId,
                    SongId = currSong.SongId,
                    AddedAt = DateTime.Now
                };

               await _playlistSongRepository.Insert(newPlaylistSong);


            }catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> AddSongToUserFavorite(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to user favorite list successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                UserFavorite userFavorite = new UserFavorite
                {
                    UserId = currUser.UserId,
                    SongId = currSong.SongId,
                    AddedAt = DateTime.Now,
                };

                await _userFavoriteRepository.Insert(userFavorite);
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

        public async Task<ResultModel> AddSongToUserQueue(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to user queue list successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                UserQueue userQueue = new UserQueue
                {
                    UserId = currUser.UserId,
                    SongId = currSong.SongId,
                    AddedAt = DateTime.Now,
                };

                await _userQueueRepository.Insert(userQueue);
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

        public async Task<ResultModel> CheckSongExistingInAlbum(int songId, int albumId)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to play list successfully"
            };

            try
            {
                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                var currAlbum = await _albumRepository.GetAlbumsByAlbumId(albumId);

                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current album does not exist";
                    return result;
                }

                var albumSong = await _albumSongRepository.CheckSongExistingInAlbum(currSong.SongId, currAlbum.AlbumId);

                result.Data = albumSong != null ? true : false;

                result.Message = albumSong != null ? "The song is already existed in the album" : "";

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

        public async Task<ResultModel> CheckSongExistingInPlaylist(int songId, int playlistId)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to play list successfully"
            };

            try
            {
                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                var currPlaylist = await _playlistRepository.Get(playlistId);

                if (currPlaylist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current playlist does not exist";
                    return result;
                }

                var playlistSong = await _playlistSongRepository.CheckSongExistingInPlaylist(currSong.SongId, currPlaylist.PlaylistId);

                result.Data = playlistSong != null ? true : false;

                result.Message = playlistSong != null ? "The song is already existed in the playlist" : "";

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

        public async Task<ResultModel> CheckSongInUserFavorite(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new song to play list successfully"
            };

            try
            {

                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not exist";
                    return result;
                }

                var userFavoriteSong = await _userFavoriteRepository.CheckSongInUserFavorite(currSong.SongId, currUser.UserId);

                result.Data = userFavoriteSong != null ? true : false;

                result.Message = userFavoriteSong != null ? "The song is already existed in the favorite list" : "";

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

        public async Task<ResultModel> GetRecommendSongsForUser(int? page, int? size, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get recommend songs successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                var albums = await _songRepository.GetRecommendSongsForUser(currUser.UserId, page, size);
                result.Data = _mapper.Map<List<SongsResModel>>(albums);
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

        public async Task<ResultModel> GetSong(int songId)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get song successfully"

            };

            try
            {

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                result.Data = _mapper.Map<SongsResModel>(currSong);
            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> GetSongs(int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get song successfully"

            };

            try
            {
                var songs = await _songRepository.GetSongs(page, size);
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

        public async Task<ResultModel> GetSongsByAlbum(int albumId, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get song successfully"

            };

            try
            {

                var currAlbum = await _albumRepository.GetAlbumsByAlbumId(albumId);

                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Album does not exist";
                    return result;
                }

                var albumSongList = _mapper.Map<AlbumSongViewResModel>(currAlbum);

                var songList = _mapper.Map<List<SongsResModel>>(await _songRepository.GetSongsByAlbum(albumId, page, size));

                albumSongList.Songs = songList;

                result.Data = albumSongList;
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

        public async Task<ResultModel> GetSongsByArtist(int artistId, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get songs by artist successfully"

            };

            try
            {

                var artist = _artistRepository.Get(artistId);

                if (artist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                var songList = await _songRepository.GetSongsByArtist(artistId, page, size);
                result.Data = _mapper.Map<List<SongsResModel>>(songList);
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

        public async Task<ResultModel> GetUserFavoriteSongs(int? page, int? size, string token)
        {

            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get favorite songs of user successfully",
            };

            var decodedToken = _decodeToken.decode(token);

            var currUser = await _userRepository.Get(Int32.Parse(decodedToken.userid));

            if (currUser == null)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = "User does not exist";
                return result;
            }

            try
            {
                var userFavoriteSongs = await _userFavoriteRepository.GetUserFavorites(currUser.UserId, page, size);

                var userFavoriteSongsList = await _songRepository.GetUserFavoriteSongs(userFavoriteSongs.Select(x => x.SongId).ToList(), page, size);

                result.Data = _mapper.Map<List<SongsResModel>>(userFavoriteSongsList);

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

        public async Task<ResultModel> GetUserListeningHisotry(int? page, int? size, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get listening history songs of user successfully",
            };

            var decodedToken = _decodeToken.decode(token);

            var currUser = await _userRepository.Get(Int32.Parse(decodedToken.userid));

            if (currUser == null)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = "User does not exist";
                return result;
            }

            try
            {
                var listeningHistorySong = await _listeningHistoryRepository.GetListeningHistoriesByUser(currUser.UserId, page, size);

                var listeningHistorySongList = await _songRepository.GetUserListeningHisotry(listeningHistorySong.Select(x => (int)x.SongId).ToList(), page, size);

                result.Data = _mapper.Map<List<SongsResModel>>(listeningHistorySongList);

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

        public async Task<ResultModel> GetUserQueueSong(int? page, int? size, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get favorite songs of user successfully",
            };

            var decodedToken = _decodeToken.decode(token);

            var currUser = await _userRepository.Get(Int32.Parse(decodedToken.userid));

            if (currUser == null)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = "User does not exist";
                return result;
            }

            try
            {
                var userQueuesSong = await _userQueueRepository.GetUserQueue(currUser.UserId, page, size);

                var userQueuesSongList = await _songRepository.GetUserQueueSongs(userQueuesSong.Select(x => x.SongId).ToList(), page, size);

                result.Data = _mapper.Map<List<SongsResModel>>(userQueuesSongList);

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

        public async Task<ResultModel> PauseSong(PauseSongReqModel pauseSongReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Pause song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(pauseSongReqModel.SongId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var checkSongExistingInHistoryOfUser = await _listeningHistoryRepository.GetListeningHistoryByUserIdAndSongId(currUser.UserId, currSong.SongId);

                if (checkSongExistingInHistoryOfUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Invalid song";
                    return result;
                }

                if (pauseSongReqModel.PauseTime > currSong.Duration || !TimeOnly.TryParse(pauseSongReqModel.PauseTime.ToString(), out _))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Invalid pause time";
                    return result;

                }

                checkSongExistingInHistoryOfUser.LastPauseTime = pauseSongReqModel.PauseTime;

                await _listeningHistoryRepository.Update(checkSongExistingInHistoryOfUser);
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

        public async Task<ResultModel> PlaySong(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Play song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var checkSongExistingInHistoryOfUser = await _listeningHistoryRepository.GetListeningHistoryByUserIdAndSongId(currUser.UserId, songId);

                if (checkSongExistingInHistoryOfUser != null)
                {
                    return result;
                }

                ListeningHistory listeningHistory = new ListeningHistory
                {
                    UserId = currUser.UserId,
                    SongId = currSong.SongId,
                    PlayedAt = DateTime.Now,
                    LastPauseTime = null,
                };

                await _listeningHistoryRepository.Insert(listeningHistory);


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

        // chưa xong
        public async Task<ResultModel> RemoveSong(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                var songOfArtist = await _songRepository.GetSongsByArtist(currArtist.ArtistId);

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                if (!songOfArtist.Contains(currSong))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The current song does not belong to the current artist";
                    return result;
                }

                var artistSong = await _artistSongRepository.CheckArtistSongExisting(currArtist.ArtistId, currSong.SongId);
                if (artistSong.IsOwner is false)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Only the owner can remove the song";
                    return result;
                }

                var albumSongs = await _albumSongRepository.GetAlbumSongsBySongId(currSong.SongId);
                var artistSongs = await _artistSongRepository.GetArtistSongBySongId(currSong.SongId);
                var playlistSongs = await _playlistSongRepository.GetPlaylistSongsBySongId(currSong.SongId);

                var listeningHistoriesSongs = await _listeningHistoryRepository.GetListeningHistoriesBySongId(currSong.SongId);
                var userFavoriteSongs = await _userFavoriteRepository.GetUserFavoriteBySongId(currSong.SongId);

                foreach (var item in albumSongs)
                {
                    await _albumSongRepository.Remove(item);
                }

                foreach (var item in artistSongs)
                {
                    await _artistSongRepository.Remove(item);

                }

                foreach (var item in playlistSongs)
                {
                    await _playlistSongRepository.Remove(item);

                }

                foreach (var item in listeningHistoriesSongs)
                {
                    await _listeningHistoryRepository.Remove(item);

                }

                foreach (var item in userFavoriteSongs)
                {
                    await _userFavoriteRepository.Remove(item);

                }

                currSong.Status = StatusEnums.Inactive.ToString();
                await _songRepository.Update(currSong);

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

        public async Task<ResultModel> RemoveSongFromAlbum(int songId, int albumId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove song from playlist of user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var currAlbum = await _albumRepository.GetAlbumsByAlbumId(albumId);

                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Album does not exist";
                    return result;
                }

                if (currAlbum.Artist.User.UserId != currUser.UserId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "You can not modify another artist's album";
                    return result;
                }

                var albumSong = await _albumSongRepository.CheckSongExistingInAlbum(currSong.SongId, currAlbum.AlbumId);


                if (albumSong != null)
                {
                    await _albumSongRepository.Remove(albumSong);
                }
                else
                {
                    result.Message = "The current song does not exist in the album";
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

        public async Task<ResultModel> RemoveSongFromPlaylist(int songId, int playlistId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove song from playlist of user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var currPlaylist = await _playlistRepository.Get(playlistId);

                if (currPlaylist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Playlist does not exist";
                    return result;
                }

                if (currPlaylist.UserId != currUser.UserId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "You can not modify another user's playlist";
                    return result;
                }

                var playlistSong = await _playlistSongRepository.CheckSongExistingInPlaylist(currSong.SongId, currPlaylist.PlaylistId);


                if (playlistSong != null)
                {
                    await _playlistSongRepository.Remove(playlistSong);
                }else
                {
                    result.Message = "The current song does not exist in the playlist";
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

        public async Task<ResultModel> RemoveSongFromUserFavorite(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove song from playlist of user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var userFavorite = await _userFavoriteRepository.CheckSongInUserFavorite(currSong.SongId, currUser.UserId);

                if (userFavorite != null)
                {
                    await _userFavoriteRepository.Remove(userFavorite);
                }else
                {
                    result.Message = "The current song does not belong to current favorite list of user";
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

        public async Task<ResultModel> RemoveSongFromUserQueue(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove song from queue of user successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var userQueue = await _userQueueRepository.CheckSongInUserQueue(currUser.UserId, currSong.SongId);

                if (userQueue != null)
                {
                    await _userQueueRepository.Remove(userQueue);
                }
                else
                {
                    result.Message = "The current song does not belong to current queue list of user";
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

        public async Task<ResultModel> ReplaySong(int songId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Pause song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(songId);
                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Song does not exist";
                    return result;
                }

                var checkSongExistingInHistoryOfUser = await _listeningHistoryRepository.GetListeningHistoryByUserIdAndSongId(currUser.UserId, currSong.SongId);

                if (checkSongExistingInHistoryOfUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Invalid song";
                    return result;
                }

                checkSongExistingInHistoryOfUser.LastPauseTime = null;

                await _listeningHistoryRepository.Update(checkSongExistingInHistoryOfUser);
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

        // Cần xem thêm
        public async Task<ResultModel> Search(string searchValue, string searchBy, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Search information successfully"
            };

            try
            {
                switch(searchBy)
                {
                    case "Song":
                        var songList = await _songRepository.SearchBySongName(searchValue, page, size);
                        result.Data = songList.Count > 0 ? _mapper.Map<List<SongsResModel>>(songList) : [];
                        result.Message = songList.Count > 0 ? "Search song information successfully" : "There is no result matches";
                        break;
                    case "Playlist":
                        var playslists = await _playlistRepository.SearchPlaylistByName(searchValue, page, size);
                        result.Data = playslists.Count > 0 ? _mapper.Map<List<PlaylistViewResModel>>(playslists) : [];
                        result.Message = playslists.Count > 0 ? "Search playlist information successfully" : "There is no result matches";
                        break;
                    case "Artist":
                        var artistList = await _artistRepository.SearchByArtistName(searchValue, page, size);
                        result.Data = artistList.Count > 0 ? _mapper.Map<List<ArtistViewResModel>>(artistList) : [];
                        result.Message = artistList.Count > 0 ? "Search artist information successfully" : "There is no result matches";
                        break;
                    case "Album":
                        var albumList =  await _albumRepository.SearchByAlbumName(searchValue, page, size);
                        result.Data = albumList.Count > 0 ? _mapper.Map<List<AlbumViewResModel>>(albumList) : [];
                        result.Message = albumList.Count > 0 ? "Search album information successfully" : "There is no result matches";
                        break;
                    default:
                        result.Message = "Invalid search filter";
                        break;
                }

            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;

        }

        public async Task<ResultModel> UpdateSong(SongUpdateReqModel songUpdateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update song successfully"
            };

            try
            {
                var decodedToken = _decodeToken.decode(token);

                var currUser = await _userRepository.GetUserByEmail(decodedToken.email);

                if (currUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User does not exist";

                    return result;
                }

                if (!currUser.Role.Equals(RoleEnums.Artist.ToString()))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";

                    return result;
                }

                var songOfArtist = await _songRepository.GetSongsByArtist(currArtist.ArtistId);

                var currSong = await _songRepository.Get(songUpdateReq.songId);

                if (currSong == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The song does not exist";

                    return result;
                }

                if (!songOfArtist.Contains(currSong))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The current song does not belong to current artist";
                    return result;
                }

                var artistSong = await _artistSongRepository.CheckArtistSongExisting(currArtist.ArtistId, currSong.SongId);
                if (artistSong.IsOwner is false)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Only the owner can update the song";
                    return result;
                }

                if (songUpdateReq.music_path != null)
                {
                    _fileService.CheckMusicFile(songUpdateReq.music_path);
                    if (!string.IsNullOrEmpty(currSong.FilePath))
                    {
                        await _cloudinaryService.DeleteFile(currSong.FilePath);
                    }
                }

                if (songUpdateReq.image_path != null)
                {
                    _fileService.CheckImageFile(songUpdateReq.image_path);
                    if (!string.IsNullOrEmpty(currSong.ImagePath))
                    {
                        await _cloudinaryService.DeleteFile(currSong.ImagePath);
                    }
                }

                var resultAudio = songUpdateReq.music_path != null ? await _cloudinaryService.AddAudio(songUpdateReq.music_path) : null;
                var resultImg = songUpdateReq.image_path != null ?  await _cloudinaryService.AddPhoto(songUpdateReq.image_path) : null;


                currSong.Title = !string.IsNullOrEmpty(songUpdateReq.Title) ? songUpdateReq.Title : currSong.Title;
                currSong.Duration = songUpdateReq.music_path != null ? _fileService.GetDurationOfMusicPath(songUpdateReq.music_path) : currSong.Duration;
                currSong.FilePath = resultAudio != null ? resultAudio.SecureUrl.ToString() : currSong.FilePath;
                currSong.CreatedAt = DateTime.Now;
                currSong.Status = true.ToString();
                currSong.Lyrics = songUpdateReq.Lyrics;
                currSong.ImagePath = resultImg != null ? resultImg.SecureUrl.ToString() : currSong.ImagePath;
                

                await _songRepository.Update(currSong);

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
    }
}
