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
    public class FileServiceTests
    {
        private readonly IMapper _mapper;
        private Mock<IFileRepository> _mockFileRepository;
        private Mock<IUnitOfWork> _mockUOW;
        private Mock<IFileSystem> _mockFileSystem;

        public FileServiceTests()
        {
            var mapperConfiguration = new MappingProfile().CreateConfiguration();
            _mapper = mapperConfiguration.CreateMapper();
        }

        [TestMethod]
        public async Task GetAsync_ValidId_ShouldRunOneTime()
        {
            //Arrange
            var expected = new File();
            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => expected);

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(()=>_mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, null);

            //Act
            await fileService.GetAsync(Guid.NewGuid());

            //Assert
            _mockFileRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAsync_InvalidId_ShouldThrowNotFoundException()
        {
            //Arrange
            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(() => _mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, null);

            //Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => fileService.GetAsync(Guid.NewGuid()));
        }

        [TestMethod]
        public async Task DeleteAsync_FileExist_DeleteFile()
        {
            //Arrange
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.File.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.File.Delete(It.IsAny<string>()));
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
            _mockFileRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new File() { Name = "Dir" });

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(() => _mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, _mockFileSystem.Object);

            //Act
            await fileService.DeleteAsync(Guid.NewGuid(), "physicalPath");

            //Assert
            _mockFileRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileSystem.Verify(x => x.File.Delete(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.File.Exists(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_FileNotExist_ThrowNotFoundException()
        {
            //Arrange
            File file = null;
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.File.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.File.Delete(It.IsAny<string>()));
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
            _mockFileRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(file);

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(() => _mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, _mockFileSystem.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => fileService.DeleteAsync(Guid.NewGuid(), "physicalPath"));

            //Assert
            _mockFileRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _mockFileSystem.Verify(x => x.File.Delete(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.File.Exists(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_FileNotExist_ThrowNotFoundException()
        {
            //Arrange
            File file = null;
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.File.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileSystem.Setup(t => t.File.Move(It.IsAny<string>(), It.IsAny<string>()));

            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository.Setup(x => x.UpdateAsync(It.IsAny<File>()));
            _mockFileRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(file);

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(() => _mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, _mockFileSystem.Object);

            //Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => fileService.UpdateAsync(new FileModel() { Name = "Name" }, "Path"));

            //Assert
            _mockFileRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileRepository.Verify(x => x.UpdateAsync(It.IsAny<File>()), Times.Never);
            _mockFileSystem.Verify(x => x.File.Exists(It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(x => x.File.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_FileExist_UpdateFile()
        {
            //Arrange
            File file = new File() { Name = "OldName" };
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileSystem.Setup(t => t.File.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(t => t.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            _mockFileSystem.Setup(t => t.File.Move(It.IsAny<string>(), It.IsAny<string>()));

            _mockFileRepository = new Mock<IFileRepository>();
            _mockFileRepository.Setup(x => x.UpdateAsync(It.IsAny<File>()));
            _mockFileRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(file);

            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(x => x.FileRepository).Returns(() => _mockFileRepository.Object);

            var fileService = new FileService(_mockUOW.Object, _mapper, _mockFileSystem.Object);

            //Act
            await fileService.UpdateAsync(new FileModel() { Name = "NewName" }, "Path");

            //Assert
            _mockFileRepository.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockFileRepository.Verify(x => x.UpdateAsync(It.IsAny<File>()), Times.Once);
            _mockFileSystem.Verify(x => x.File.Exists(It.IsAny<string>()), Times.Once);
            _mockFileSystem.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _mockFileSystem.Verify(x => x.File.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never); // old and new path the same
        }


    }
}
