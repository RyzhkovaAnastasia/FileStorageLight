using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Exceptions;
using BLL.Services;
using DAL.Entities;
using DAL.Interfaces;
using DAL.IdentityManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BLL.UnitTests
{
    [TestClass]
    public class RoleServiceTests
    {
        private Mock<IAuthUnitOfWork> _mockAuthUnitOfWork;

        [TestMethod]
        public async Task EditAsync_ValidUserIdNewRoles_ShouldEdit()
        {
            //Arrange
            var user = new ApplicationUser();
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => user);
            manager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(new List<string>());
            manager.Setup(x => x.AddToRolesAsync(It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync(()=> IdentityResult.Success);
            manager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync(() => IdentityResult.Success);


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var roleService = new RoleService(_mockAuthUnitOfWork.Object);

            //Act
            await roleService.EditAsync(Guid.NewGuid().ToString(), new List<string>());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.GetRolesAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.AddToRolesAsync(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.RemoveFromRolesAsync(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
        }

        [TestMethod]
        public async Task EditAsync_InvalidUserIdNewRoles_ShouldThrowUserException()
        {
            //Arrange
            var user = new ApplicationUser();
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => user);
            manager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(new List<string>());
            manager.Setup(x => x.AddToRolesAsync(It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync(() => IdentityResult.Failed());
            manager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync(() => IdentityResult.Failed());


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var roleService = new RoleService(_mockAuthUnitOfWork.Object);

            //Act
            await Assert.ThrowsExceptionAsync<UserException>(() => roleService.EditAsync(Guid.NewGuid().ToString(), new List<string>()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.GetRolesAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.AddToRolesAsync(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.RemoveFromRolesAsync(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
        }

        [TestMethod]
        public async Task EditAsync_InvalidUserIdNewRoles_ShouldThrowNotFoundException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var roleService = new RoleService(_mockAuthUnitOfWork.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => roleService.EditAsync(Guid.NewGuid().ToString(), new List<string>()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetRolesAsync_ValidUserId_GetRoles()
        {
            //Arrange
            var user = new ApplicationUser();
            var store = new Mock<IUserStore<ApplicationUser>>();
            var storeRole = new Mock<IRoleStore<IdentityRole>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            var roles = new Mock<ApplicationRoleManager>(storeRole.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => user);
            manager.Setup(x => x.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(() => new List<string>());

            roles.Setup(x => x.Roles).Returns(()=>new List<IdentityRole>() as IQueryable<IdentityRole>);


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);
            _mockAuthUnitOfWork.Setup(x => x.RoleManager).Returns(roles.Object);

            var roleService = new RoleService(_mockAuthUnitOfWork.Object);

            //Act
            await roleService.GetRolesAsync(Guid.NewGuid().ToString());

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockAuthUnitOfWork.Verify(x => x.UserManager.GetRolesAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetRolesAsync_InvalidUserId_ShouldThrowNotFoundException()
        {
            //Arrange
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<ApplicationUserManager>(store.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);


            _mockAuthUnitOfWork = new Mock<IAuthUnitOfWork>();
            _mockAuthUnitOfWork.Setup(x => x.UserManager).Returns(manager.Object);

            var roleService = new RoleService(_mockAuthUnitOfWork.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => roleService.GetRolesAsync(Guid.NewGuid().ToString()));

            //Assert
            _mockAuthUnitOfWork.Verify(x => x.UserManager.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
