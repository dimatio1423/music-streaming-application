using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.AlbumModels.Request;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SongModels.Response;
using Newtonsoft.Json.Linq;
using Repositories.AlbumRepos;
using Repositories.AlbumSongsRepo;
using Repositories.ArtistRepos;
using Repositories.SongRepos;
using Repositories.UserRepos;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Services.AlbumServices
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IUserRepository _userRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumSongRepository _albumSongRepository;
        private readonly ISongRepository _songRepository;
        private readonly IMapper _mapper;

        public AlbumService(IAlbumRepository albumRepository, 
            IUserRepository userRepository,
            IArtistRepository artistRepository,
            IAlbumSongRepository albumSongRepository,
            ISongRepository songRepository,
            IMapper mapper,
            IDecodeTokenHandler decodeToken)
        {
            _albumRepository = albumRepository;
            _decodeToken = decodeToken;
            _userRepository = userRepository;
            _artistRepository = artistRepository;
            _albumSongRepository = albumSongRepository;
            _songRepository = songRepository;
            _mapper = mapper;
        }

        public async Task<ResultModel> CreateAlbum(AlbumCreateReqModel albumCreateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Create new album successfully",
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

                if (!currUser.Role.Equals(RoleEnums.Artist))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Unauthorized;
                    result.Message = "Do not have permission to perform this function";

                    return result;
                }

                if (!Enum.IsDefined(typeof(GenreEnums), albumCreateReq.Genre))
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Invalid genre";

                    return result;
                }

                var currArtist =  await _artistRepository.GetArtistByUserId(currUser.UserId);

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";

                    return result;
                }

                Album newAlbum = new Album
                {
                    ArtistId = currArtist.ArtistId,
                    Title = albumCreateReq.Title,
                    ReleaseDate = albumCreateReq.ReleaseDate,
                    Genre = albumCreateReq.Genre,
                    CreatedAt = DateTime.Now,
                };

                await _albumRepository.Insert(newAlbum);


            }catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<ResultModel> ViewDetailsAlbum(int albumId, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get album successfully",
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

                var songListOfAlbum = _mapper.Map<List<SongsResModel>>(await _songRepository.GetSongsByAlbum(currAlbum.AlbumId, page, size));

                var album = _mapper.Map<AlbumSongViewResModel>(currAlbum);

                album.Songs = songListOfAlbum;

                result.Data = album;
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

        public async Task<ResultModel> GetAlbums(int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get albums successfully"

            };

            try
            {
                var albums = await _albumRepository.GetAlbums(page, size);
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

        public async Task<ResultModel> GetAlbumsByArtist(int artistId, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get albums by artist successfully"

            };

            try
            {
                var artist = await _artistRepository.Get(artistId);
                if (artist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int) HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";

                    return result;
                }

                var albums = await _albumRepository.GetAlbumsByArtist(artistId,page, size);
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

        public async Task<ResultModel> GetAlbumsByGenre(string genre, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get albums by artist successfully"
            };

            try
            {
                var field = typeof(GenreEnums).GetField(genre);

                if (field == null || !field.GetValue(null).Equals(genre))
                {
                    result.IsSuccess = false;
                    result.Code = (int) HttpStatusCode.BadRequest;
                    result.Message = "Invalid genre";

                    return result;
                }

                var albums = await _albumRepository.GetAlbumsByGenre(genre, page, size);
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

        public async Task<ResultModel> GetRecommendAlbumsForUser(int? page, int? size, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get recommend albums successfully"
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

                var albums = await _albumRepository.GetRecommendAlbumsForUser(currUser.UserId, page, size);
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

        public async Task<ResultModel> RemoveAlbum(int albumId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove albums successfully"
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

                var currAlbum = await _albumRepository.GetAlbumsByAlbumId(albumId);
                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The album does not exist";
                    return result;
                }

               if (currAlbum.Artist.ArtistId != currArtist.ArtistId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "You can not remove other artist's album";
                    return result;
                }

                var albumSongList = await _albumSongRepository.GetAlbumSongsByAlbumId(currAlbum.AlbumId);

                foreach (var albumSong in albumSongList)
                {
                    await _albumSongRepository.Remove(albumSong);
                }

                await _albumRepository.Remove(currAlbum);

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

        public async Task<ResultModel> UpdateAlbum(AlbumUpdateReqModel albumUpdateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update albums successfully"
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

                var currAlbum = await _albumRepository.GetAlbumsByAlbumId(albumUpdateReq.AlbumId);
                if (currAlbum == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "The album does not exist";
                    return result;
                }

                if (currAlbum.Artist.ArtistId != currArtist.ArtistId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "You can not update other artist's album";
                    return result;
                }

                currAlbum.Title = !string.IsNullOrEmpty(albumUpdateReq.Title) ? albumUpdateReq.Title : currAlbum.Title;
                currAlbum.ReleaseDate = DateOnly.TryParse(albumUpdateReq.ReleaseDate.ToString(), out _) ? albumUpdateReq.ReleaseDate : currAlbum.ReleaseDate;
                currAlbum.Genre = !string.IsNullOrEmpty(albumUpdateReq.Genre) ? albumUpdateReq.Genre : currAlbum.Genre;

                await _albumRepository.Update(currAlbum);

            }
            catch(Exception ex)
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
