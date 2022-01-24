using AutoMapper;
using BLL.Models;
using DAL.Entities;
using System.Web;

namespace BLL.Config
{
    public class MappingProfile : Profile
    {
        public MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterModel, ApplicationUser>().ReverseMap();
                cfg.CreateMap<EditUserModel, ApplicationUser>().ReverseMap();
                cfg.CreateMap<ApplicationUser, UserModel>().ReverseMap();
                cfg.CreateMap<File, FileModel>()
                    .ForMember(dest => dest.ApplicationUserId, src => src.MapFrom(m => m.Directory.ApplicationUserId))
                    .ForMember(dest => dest.ApplicationUser, src => src.MapFrom(m => m.Directory.ApplicationUser))
                    .ReverseMap();
                cfg.CreateMap<SharedFile, SharedFileModel>().ReverseMap();
                cfg.CreateMap<LinkModel, FileShortLink>().ReverseMap();
                cfg.CreateMap<DirectoryModel, Directory>().ReverseMap();
                cfg.CreateMap<StorageItemModel, Directory>().ReverseMap();
                cfg.CreateMap<StorageItemModel, File>().ReverseMap();

                cfg.CreateMap<HttpPostedFileBase, File>()
                .ForMember(dest => dest.Name, src => src.MapFrom(m => System.IO.Path.GetFileNameWithoutExtension(m.FileName)))
                .ForMember(dest => dest.Size, src => src.MapFrom(m => m.ContentLength))
                .ForMember(dest => dest.Type, src => src.MapFrom(m => System.IO.Path.GetExtension(m.FileName)))
                .ReverseMap();

                cfg.CreateMap<HttpPostedFileBase, FileModel>()
               .ForMember(dest => dest.Name, src => src.MapFrom(m => System.IO.Path.GetFileNameWithoutExtension(m.FileName)))
               .ForMember(dest => dest.Size, src => src.MapFrom(m => m.ContentLength))
               .ForMember(dest => dest.Type, src => src.MapFrom(m => System.IO.Path.GetExtension(m.FileName)))
               .ReverseMap();
                
            });

            return config;
        }
    }
}
