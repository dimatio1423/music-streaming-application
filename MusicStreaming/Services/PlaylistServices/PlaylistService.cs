using AutoMapper;
using Azure;
using BusinessObjects.Entities;
using BusinessObjects.Models.PlaylistModels.Request;
using BusinessObjects.Models.PlaylistModels.Response;
using BusinessObjects.Models.ResultModels;
using Repositories.PlayListRepos;
using Repositories.PlaylistSongRepos;
using Repositories.UserRepos;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.PlaylistServices
{
    public class PlaylistService : IPlayListService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly IPlaylistSongRepository _playlistSongRepository;
        private readonly IMapper _mapper;

        public PlaylistService(IPlaylistRepository playlistRepository,
            IUserRepository userRepository,
            IDecodeTokenHandler decodeToken,
            IPlaylistSongRepository playlistSongRepository,
            IMapper mapper)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _decodeToken = decodeToken;
            _playlistSongRepository = playlistSongRepository;
            _mapper = mapper;
        }

        public Task<ResultModel> CheckSongExistingInPlaylist(int songId, int playlistId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultModel> CreatePlaylist(PlaylistCreateReqModel playlistCreateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Create new playlist successfully"
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

                Playlist newPlaylist = new Playlist
                {
                    UserId = currUser.UserId,
                    Title = playlistCreateReq.title,
                    CreatedAt = DateTime.Now,
                };

                await _playlistRepository.Insert(newPlaylist);


            }catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;

                return result;
            }

            return result;
        }

        public async Task<ResultModel> GetPlaylistOfUser(int? page, int? size, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Get playlist of user successfully"
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

                var playlists = await _playlistRepository.GetPlaylistsByUserId(currUser.UserId, page, size);

                result.Data = _mapper.Map<List<PlaylistViewResModel>>(playlists);

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

        public async Task<ResultModel> RemovePlaylist(int playlistId, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove playlist of user successfully"
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
                    result.Message = "You can not remove another user's playlist";
                    return result;
                }
                var playlistSongs = await _playlistSongRepository.GetPlaylistSongsByPlaylistId(currPlaylist.PlaylistId);

                foreach (var item in playlistSongs)
                {
                    await _playlistSongRepository.Remove(item);
                }

                await _playlistRepository.Remove(currPlaylist);

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

        public async Task<ResultModel> UpdatePlaylist(PlaylistUpdateReqModel playlistUpdateReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Remove playlist of user successfully"
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

                var currPlaylist = await _playlistRepository.Get(playlistUpdateReq.playlistId);

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
                    result.Message = "You can not update another user's playlist";
                    return result;
                }

                currPlaylist.Title = playlistUpdateReq.title;

                await _playlistRepository.Update(currPlaylist);

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
