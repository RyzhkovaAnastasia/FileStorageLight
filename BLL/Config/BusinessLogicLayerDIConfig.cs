using System.IO.Abstractions;
using System.Web;
using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using DAL.Config;
using Ninject;
using Ninject.Modules;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace BLL.Config
{
    public class BusinessLogicLayerDIConfig: NinjectModule
    {
        private DataAccessLayerDIConfig _dllDI = new DataAccessLayerDIConfig();
        public NinjectModule DLLDI => _dllDI;

        public override void Load()
        {
            Bind<IUserService>().To<UserService>();
            Bind<IRoleService>().To<RoleService>();
            Bind<IFileService>().To<FileService>();
            Bind<IDirectoryService>().To<DirectoryService>();
            Bind<IStorageService>().To<StorageService>();
            Bind<IFileSystem>().To<FileSystem>();
            Bind<IAuthenticationManager>().ToMethod(x=> HttpContext.Current.GetOwinContext().Authentication);
            Unbind<ModelValidatorProvider>();

            AutoMapperConfig();
        }
        private void AutoMapperConfig()
        {
            var mapperConfiguration = new MappingProfile().CreateConfiguration();
            Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();
            Bind<IMapper>().ToMethod(ctx => new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
        }
    }
}
