using BusinessObjects.Entities;
using BusinessObjects.Enums;
using BusinessObjects.Models.ArtistSongModel.Request;
using BusinessObjects.Models.ResultModels;
using Repositories.ArtistRepos;
using Repositories.ArtistSongRepos;
using Repositories.SongRepos;
using Repositories.UserRepos;
using Services.Helpers.Handler.DecodeTokenHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.ArtistSongServices
{
    public class ArtistSongService : IArtistSongService
    {
        private readonly IUserRepository _userRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IArtistSongRepository _artistSongRepository;
        private readonly ISongRepository _songRepository;
        private readonly IDecodeTokenHandler _decodeToken;

        public ArtistSongService(IUserRepository userRepository, 
            IArtistRepository artistRepository, 
            IArtistSongRepository artistSongRepository, 
            ISongRepository songRepository,
            IDecodeTokenHandler decodeToken)
        {
            _userRepository = userRepository;
            _artistRepository = artistRepository;
            _artistSongRepository = artistSongRepository;
            _songRepository = songRepository;
            _decodeToken = decodeToken;
        }
        public async Task<ResultModel> AddNewArtistToSong(ArtistSongCreateReqModel artistSongCreateReqModel, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Add new artist to song successfully"
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
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Do not have permission to perform this function";
                    return result;
                }

                var currArtist = await _artistRepository.GetArtistByUserId(currUser.UserId);

                var addedArtist = await _artistRepository.GetArtistByUserId(artistSongCreateReqModel.ArtistId);

                if (currArtist == null || addedArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                var currSong = await _songRepository.Get(artistSongCreateReqModel.SongId);

                var songOfArtist = await _songRepository.GetSongsByArtist(currArtist.ArtistId);

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
                    result.Message = "Song does not belong to current artist";
                    return result;
                }

                ArtistSong artistSong = new ArtistSong
                {
                    ArtistId = addedArtist.ArtistId,
                    SongId = currSong.SongId,
                    RoleDescription = artistSongCreateReqModel.RoleDescription
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
    }
}
