using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.SongModels.Response;
using Repositories.AlbumRepos;
using Repositories.AlbumSongsRepo;
using Repositories.SongRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers.Resolvers.SongsResolver
{
    public class GetAlbumOfSongResolver : IValueResolver<Song, SongsResModel, List<AlbumViewResModel>>
    {
        private readonly ISongRepository _songRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IAlbumSongRepository _albumSongRepository;
        private readonly IMapper _mapper;

        public GetAlbumOfSongResolver(ISongRepository songRepository,
            IAlbumRepository albumRepository,
            IAlbumSongRepository albumSongRepository,
            IMapper mapper)
        {
            _songRepository = songRepository;
            _albumRepository = albumRepository;
            _albumSongRepository = albumSongRepository;
            _mapper = mapper;
        }

        public List<AlbumViewResModel> Resolve(Song source, SongsResModel destination, List<AlbumViewResModel> destMember, ResolutionContext context)
        {
            var currSong = _songRepository.Get(source.SongId).GetAwaiter().GetResult();

            var currAlbumsOfSongs = _albumSongRepository.GetAlbumsOfSong(currSong.SongId).GetAwaiter().GetResult();

            var currAlbums = _albumRepository.GetAlbumsByAlbumIds(currAlbumsOfSongs).GetAwaiter().GetResult();

            return _mapper.Map<List<AlbumViewResModel>>(currAlbums);
        }
    }
}
