using AutoMapper;
using BusinessObjects.Entities;
using BusinessObjects.Models.AlbumModels.Response;
using BusinessObjects.Models.ArtistModel.Response;
using BusinessObjects.Models.PlaylistModels.Response;
using BusinessObjects.Models.SongModels.Response;
using BusinessObjects.Models.UserModels.Response;
using Services.Helpers.Resolvers.SongsResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper.Handler.MapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            //Songs
            CreateMap<Song, SongsResModel>()
                .ForMember(dest => dest.songName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.songArtist, opt => opt.MapFrom<GetArtistOfSongResolver>())
                .ForMember(dest => dest.songAlbums, opt => opt.MapFrom<GetAlbumOfSongResolver>()).ReverseMap();

            //Albums
            CreateMap<Album, AlbumViewResModel>().ReverseMap();

            CreateMap<Album, AlbumSongViewResModel>().ReverseMap();


            //Artist
            CreateMap<Artist, ArtistViewResModel>().ReverseMap();
            CreateMap<Artist, ArtistViewDetailsResModel>().ReverseMap();
            CreateMap<Artist, ArtistViewProfileResModel>()
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User != null ? src.User.Role : null))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));


            //Users
            CreateMap<User, UserViewResModel>().ReverseMap();
            CreateMap<User, UserViewProfileResModel>().ReverseMap();


            //Playlists
            CreateMap<Playlist, PlaylistViewResModel>().ReverseMap();

        }
    }
}
