using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Config;
using BLL.Exceptions;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using DAL.IdentityManagers;
using DAL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BLL.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IAuthUnitOfWork> _mockAuthUnitOfWork;
        private Mock<IAuthenticationManager> _mockAuthManager;
        private Mock<IDirectoryRepository> _mockDirRepo;
        private readonly IMapper _mapper;
        public UserServiceTests()
        {
            var mapperConfiguration = new MappingProfile().CreateConfiguration();
            _mapper = mapperConfiguration.CreateMapper();
        }

        [TestMethod]
        public async Task LoginAsync_ValidAuthModel_UserLogIn()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockAuthManager = new Mock<IAuthenticationManager>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => new ApplicationUser());
            manager.Setup(x => x.CreateIdentityAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(() => new ClaimsIdentity());
            _mockAuthManager.Setup(x => x.SignIn(It.IsAny<AuthenticationProperties>(), It.IsAny<ClaimsIdentity[]>()));


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, _mockAuthManager.Object, null);

            
            //Act
            await userService.LoginAsync(new AuthModel());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockAuthManager.Verify(x => x.SignIn(It.IsAny<AuthenticationProperties>(), It.IsAny<ClaimsIdentity[]>()), Times.Once);
        }

        [TestMethod]
        public async Task LoginAsync_UserNotExist_ThrowUserException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockAuthManager = new Mock<IAuthenticationManager>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, _mockAuthManager.Object, null);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => userService.LoginAsync(new AuthModel()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidAuthModel_CreateUserAndLogIn()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockAuthManager = new Mock<IAuthenticationManager>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null);
            manager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(() => IdentityResult.Success);
            manager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => IdentityResult.Success);
            manager.Setup(x => x.CreateIdentityAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(() => new ClaimsIdentity());
            _mockAuthManager.Setup(x => x.SignIn(It.IsAny<AuthenticationProperties>(), It.IsAny<ClaimsIdentity[]>()));


            _mockDirRepo = new Mock<IDirectoryRepository>();
            _mockDirRepo.Setup(x => x.CreateAsync(It.IsAny<Directory>()));

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, _mockAuthManager.Object, _mockDirRepo.Object);

            //Act
            await userService.RegisterAsync(new RegisterModel(), true);

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.CreateIdentityAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            _mockAuthManager.Verify(x => x.SignIn(It.IsAny<AuthenticationProperties>(), It.IsAny<ClaimsIdentity[]>()), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_UserAlreadyExist_ShouldThrowUserException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockAuthManager = new Mock<IAuthenticationManager>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => new ApplicationUser());


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            _mockDirRepo = new Mock<IDirectoryRepository>();
            _mockDirRepo.Setup(x => x.CreateAsync(It.IsAny<Directory>()));

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, _mockAuthManager.Object, _mockDirRepo.Object);

            //Act
            await Assert.ThrowsExceptionAsync<UserException>(() => userService.RegisterAsync(new RegisterModel(), true));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Logout_UserInSystem_Logout()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockAuthManager = new Mock<IAuthenticationManager>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            _mockAuthManager.Setup(x => x.SignOut());

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, _mockAuthManager.Object, null);

            //Act
            userService.Logout();

            //Assert
            _mockAuthManager.Verify(x => x.SignOut(), Times.Once);
        }

        [TestMethod]
        public void GetAllUsers_ValidUserId_ReturnListOfUsersAndGuests()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var storeRole = new Mock<IRoleStore<IdentityRole>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            var roleManager = new Mock<ApplicationRoleManager>(storeRole.Object);

            manager.Setup(x => x.Users).Returns(() => new List<ApplicationUser>() as IQueryable<ApplicationUser>);
            roleManager.Setup(x => x.Roles).Returns(() => new List<IdentityRole>() as IQueryable<IdentityRole>);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);
            _mockAuthUnitOfWork.Setup(x => x.RoleManager).Returns(roleManager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            userService.GetAllUsers(Guid.NewGuid().ToString());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.RoleManager.Roles, Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.Users, Times.Once);
        }

        [TestMethod]
        public async Task FindUserByIdAsync_ValidUserId_ReturnUser()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => new ApplicationUser());

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            await userService.FindUserByIdAsync(Guid.NewGuid().ToString());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task FindUserByIdAsync_InvalidUserId_ShouldThrowNotFoundException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => userService.FindUserByIdAsync(Guid.NewGuid().ToString()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task EditUserAsync_InvalidUserId_ShouldThrowNotFoundException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => userService.EditUserAsync(new EditUserModel()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteUserAsync_InvalidUserId_ShouldThrowNotFoundException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => userService.DeleteUserAsync(Guid.NewGuid().ToString()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetUsersByIds_ValidUserId_ReturnListOfUsersAndGuests()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);

            manager.Setup(x => x.Users).Returns(() => new List<ApplicationUser>() as IQueryable<ApplicationUser>);

            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var userService = new UserService(_mockAuthUnitOfWork.Object, _mapper, null, null);

            //Act
            userService.GetUsersByIds(new List<string>());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.Users, Times.Once);
        }
    }
}
