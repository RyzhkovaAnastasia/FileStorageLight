using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Config;
using BLL.Exceptions;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BLL.UnitTests
{
    [TestClass]
    public class DirectoryServiceTests
    {
        private readonly IMapper _mapper;
        private Mock<IDirectoryRepository> _mockDirectoryRepository;
        private Mock<IFileSystem> _mockFileSystem;
        public DirectoryServiceTests()
        {
            var mapperConfiguration = new MappingProfile().CreateConfiguration();
            _mapper = mapperConfiguration.CreateMapper();
        }

        [TestMethod]
        public async Task GetAsync_ValidId_ShouldRunOneTime()
        {
            //Arrange
            var expected = new Directory();
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => expected);

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, null);

            //Act
            await directoryService.GetAsync(Guid.NewGuid());

            //Assert
            _mockDirectoryRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAsync_InvalidId_ShouldThrowNotFoundException()
        {
            //Arrange
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, null);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => directoryService.GetAsync(Guid.NewGuid()));
        }

        [TestMethod]
        public async Task CreateAsync_DirectoryAlreadyExist_ShouldThrowDirectoryException()
        {
            //Arrange
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.Directory.CreateDirectory(It.IsAny<string>())).Returns(It.IsAny<IDirectoryInfo>());
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.CreateAsync(It.IsAny<Directory>()));

            var directoryModel = new DirectoryModel() { Name = "Dir" };
            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<DirectoryException>(() => directoryService.CreateAsync(directoryModel, "physicalPath", "userId"));
            //Assert
            _mockDirectoryRepository.Verify(x => x.CreateAsync(It.IsAny<Directory>()), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.CreateDirectory(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_DirectoryNotExist_CreateDirectory()
        {
            //Arrange
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(t => t.Directory.CreateDirectory(It.IsAny<string>())).Returns(It.IsAny<IDirectoryInfo>());
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.CreateAsync(It.IsAny<Directory>()));

            var directoryModel = new DirectoryModel() { Name = "Dir" };
            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act
            await directoryService.CreateAsync(directoryModel, "physicalPath", "userId");

            //Assert
            _mockDirectoryRepository.Verify(x => x.CreateAsync(It.IsAny<Directory>()), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.CreateDirectory(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_DirectoryExist_DeleteDirectory()
        {
            //Arrange
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.Directory.Delete(It.IsAny<string>(), true));
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
            _mockDirectoryRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Directory() { Name = "Dir" });

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act
            await directoryService.DeleteAsync(Guid.NewGuid(), "physicalPath");

            //Assert
            _mockDirectoryRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockDirectoryRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.Delete(It.IsAny<string>(), true), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_DirectoryNotExist_ThrowNotFoundException()
        {
            //Arrange
            Directory directory = null;
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.Directory.Delete(It.IsAny<string>(), true));
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
            _mockDirectoryRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(directory);

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => directoryService.DeleteAsync(Guid.NewGuid(), "physicalPath"));

            //Assert
            _mockDirectoryRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockDirectoryRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _mockFileSystem.Verify(x => x.Directory.Delete(It.IsAny<string>(), true), Times.Never);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_DirectoryNotExist_ThrowNotFoundException()
        {
            //Arrange
            Directory directory = null;
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileSystem.Setup(t => t.Directory.Move(It.IsAny<string>(), It.IsAny<string>()));

            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Directory>()));
            _mockDirectoryRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(directory);

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => directoryService.UpdateAsync(new DirectoryModel() { Name = "Name" }, "Path"));

            //Assert
            _mockDirectoryRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockDirectoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Directory>()), Times.Never);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Directory.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_DirectoryExist_UpdateDirectory()
        {
            //Arrange
            Directory directory = new Directory() { Name = "OldName", Location = "folder" };
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.Directory.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), "OldName")).Returns(Guid.NewGuid().ToString());
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), "NewName")).Returns(Guid.NewGuid().ToString());
            _mockFileSystem.Setup(t => t.Directory.Move(It.IsAny<string>(), It.IsAny<string>()));

            _mockDirectoryRepository = new Mock<IDirectoryRepository>();
            _mockDirectoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Directory>()));
            _mockDirectoryRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(directory);

            var directoryService = new DirectoryService(_mockDirectoryRepository.Object, _mapper, _mockFileSystem.Object);

            //Act
            await directoryService.UpdateAsync(new DirectoryModel() { Name = "NewName", Location = "folder" }, "Path");

            //Assert
            _mockDirectoryRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockDirectoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Directory>()), Times.Once);
            _mockFileSystem.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.AtLeastOnce);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _mockFileSystem.Verify(x => x.Directory.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never); 
        }

    }
}
