using System;
using System.Collections.ObjectModel;
using AutoMapper;
using BLL.Config;
using BLL.Services;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace BLL.UnitTests
{
    [TestClass]
    public class StorageServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        public StorageServiceTests()
        {
            var mapperConfiguration = new MappingProfile().CreateConfiguration();
            _mapper = mapperConfiguration.CreateMapper();
        }

        [TestMethod]
        public async Task GetAsync_ValidId_ShouldRunOneTime()
        {
            var mockFileRepo = new Mock<IFileRepository>();
            var mockDirectoryRepo = new Mock<IDirectoryRepository>();

            mockDirectoryRepo.Setup(x => x.GetChildDirectories(It.IsAny<Guid>()))
                .Returns(() => new Collection<Directory>());
            mockDirectoryRepo.Setup(x => x.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(() => null);
            mockDirectoryRepo.Setup(x => x.GetDefaultDirAsync(It.IsAny<string>()))
               .ReturnsAsync(() => new Directory());

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(x => x.FileRepository).Returns(() => mockFileRepo.Object);
            _mockUnitOfWork.Setup(x => x.DirectoryRepository).Returns(() => mockDirectoryRepo.Object);

            var storageService = new StorageService(_mockUnitOfWork.Object, _mapper);


            //Act
            await storageService.GetItemsByUserId(Guid.NewGuid(), "userId");

            //Assert
            _mockUnitOfWork.Verify(x => x.DirectoryRepository.GetChildDirectories(It.IsAny<Guid>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.DirectoryRepository.GetAsync(It.IsAny<Guid>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.DirectoryRepository.GetDefaultDirAsync(It.IsAny<string>()), Times.Once);
        }

    }
}
