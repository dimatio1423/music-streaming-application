using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Request;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.ResultModels;
using Newtonsoft.Json.Linq;
using Repositories.AlbumRepos;
using Repositories.ArtistRepos;
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

namespace Services.ArtistServices
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IDecodeTokenHandler _decodeToken;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ArtistService(IArtistRepository artistRepository, 
            IUserRepository userRepository, 
            IAlbumRepository albumRepository,
            IDecodeTokenHandler decodeToken,
            ICloudinaryService cloudinaryService,
            IFileService fileService,
            IMapper mapper)
        {
            _artistRepository = artistRepository;
            _userRepository = userRepository;
            _albumRepository = albumRepository;
            _decodeToken = decodeToken;
            _cloudinaryService = cloudinaryService;
            _fileService = fileService;
            _mapper = mapper;
        }
        public async Task<ResultModel> UpdateArtistProfile(ArtistUpdateProfileReqModel artistUpdateProfileReq, string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Update artists successfully"
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

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                currArtist.Name = !string.IsNullOrEmpty(artistUpdateProfileReq.Name) ? artistUpdateProfileReq.Name : currArtist.Name;
                currArtist.Bio = !string.IsNullOrEmpty(artistUpdateProfileReq.Bio) ? artistUpdateProfileReq.Bio : currArtist.Bio;

                if (artistUpdateProfileReq.ImagePath != null)
                {
                    _fileService.CheckImageFile(artistUpdateProfileReq.ImagePath);
                    if (!string.IsNullOrEmpty(currArtist.ImagePath))
                    {
                        await _cloudinaryService.DeleteFile(currArtist.ImagePath);
                    }
                }

                var imageResult = artistUpdateProfileReq.ImagePath != null ? await _cloudinaryService.AddPhoto(artistUpdateProfileReq.ImagePath) : null;

                currArtist.ImagePath = imageResult != null ? imageResult.SecureUrl.ToString() : currArtist.ImagePath;

                await _artistRepository.Update(currArtist);

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

        public async Task<ResultModel> ViewAllArtists(int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View artists successfully"
            };

            try
            {

                var artistList = await _artistRepository.GetArtists(page, size);
                result.Data = _mapper.Map<List<ArtistViewResModel>>(artistList);

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

        public async Task<ResultModel> ViewArtistProfile(string token)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View artist profile successfully"
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

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                result.Data = _mapper.Map<ArtistViewProfileResModel>(currArtist);

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

        public async Task<ResultModel> ViewDetailsOfArtist(int artistId, int? page, int? size)
        {
            var result = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "View artist profile successfully"
            };

            try
            {
                var currArtist = await _artistRepository.GetArtist(artistId);

                if (currArtist == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Artist does not exist";
                    return result;
                }

                var album = _mapper.Map<List<AlbumViewResModel>>(await _albumRepository.GetAlbumsByArtist(currArtist.ArtistId, page, size));

                var artistDetail = _mapper.Map<ArtistViewDetailsResModel>(currArtist);

                artistDetail.Albums = album;

                result.Data = artistDetail;

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
