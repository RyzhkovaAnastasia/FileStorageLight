using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Effort;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DAL.UnitTests
{
    [TestClass]
    public class DirectoryRepositoryTests : IDisposable
    {
        private readonly FileStorageDbContext _context;
        private readonly IDirectoryRepository _directoryRepo;
        private readonly string[] _userIds = { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        public DirectoryRepositoryTests()
        {
            var connection = DbConnectionFactory.CreateTransient();
            _context = new FileStorageDbContext(connection.DataSource);
            _directoryRepo = new DirectoryRepository(_context);
        }

        [TestInitialize()]
        public void Initialize()
        {
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.Create(
                new ApplicationUser() { Id = _userIds[0], UserName = "UserName1", Email = "UserName1@mail.com" },
                "123456");
            userManager.Create(
                new ApplicationUser() { Id = _userIds[1], UserName = "UserName2", Email = "UserName2@mail.com" },
                "123456");

            _context.Directories.AddRange(
                new List<Directory>()
                {
                    new Directory()
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestDir",
                        ApplicationUserId = _userIds[0],
                        Location = ""
                    },
                    new Directory()
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestDir2",
                        ApplicationUserId = _userIds[0],
                        Location = "TestDir"
                    },
                    new Directory()
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestDir",
                        ApplicationUserId = _userIds[1],
                        Location = ""
                    },
                    new Directory()
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestDir2",
                        ApplicationUserId = _userIds[1],
                        Location = ""
                    }
                });
        }

        [TestCleanup()]
        public void Cleanup()
        {
            _context.Database.Delete();
        }

        [TestMethod]
        public async Task Create_ValidValues_AddDirectoryToList()
        {
            //Arrange

            Directory dirToList = new Directory()
            {
                Id = Guid.NewGuid(),
                Name = "NewDir",
                UploadDate = DateTime.Now,
                ApplicationUserId = _userIds[0]
            };
            var expected = _context.Directories.Local.ToList();
            expected.Add(dirToList);

            //Act
            await _directoryRepo.CreateAsync(dirToList);

            //Assert
            var actual = _context.Directories.Local.ToList();

            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Create_NullFile_DontAddToFiles()
        {
            //Arrange

            Directory directoryToList = null;
            var expected = _context.Directories.Local.ToList();

            //Act
            await _directoryRepo.CreateAsync(directoryToList);

            //Assert
            var actual = _context.Directories.Local.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Delete_ValidFileId_DeleteFromFiles()
        {
            //Arrange
            const int index = 0;
            var directoryToDeleteId = _context.Directories.Local[index].Id;

            var expected = _context.Directories.Local.ToList();
            expected.RemoveAt(index);

            //Act
            await _directoryRepo.DeleteAsync(directoryToDeleteId);

            //Assert
            var actual = _context.Directories.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Delete_InvalidFileId_DontDelete()
        {
            //Arrange
            var directoryToDeleteId = Guid.NewGuid();
            var expected = _context.Directories.Local.ToList();

            //Act
            await _directoryRepo.DeleteAsync(directoryToDeleteId);

            //Assert
            var actual = _context.Directories.Local.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Get_ValidFileId_ReturnFile()
        {
            //Arrange
            const int index = 0;
            var directoryToGetId = _context.Directories.Local[index].Id;
            var expected = _context.Directories.Local[index];

            //Act
            var actual = await _directoryRepo.GetAsync(directoryToGetId);

            //Assert
            Assert.AreEqual(expected.Id, actual.Id);
        }

        [TestMethod]
        public async Task Get_InvalidFileId_ReturnNull()
        {
            //Arrange
            var directoryToGetId = Guid.NewGuid();
            Directory expected = null;

            //Act
            var actual = await _directoryRepo.GetAsync(directoryToGetId);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task GetAll_ReturnAllFiles()
        {
            //Arrange
            var expected = _context.Directories.Local.ToList();

            //Act
            var actual = await _directoryRepo.GetAll();

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void GetAllByUserId_ValidUserId_ReturnAllUserFiles()
        {
            //Arrange
            var userId = _userIds[0];
            var expected = new List<Directory>()
            {
                _context.Directories.Local[0],
                _context.Directories.Local[1]
            };

            //Act
            var actual = _directoryRepo.GetAllByUserId(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void GetAllByUserId_InvalidUserId_ReturnEmptyList()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var expected = new List<Directory>();
            //Act
            var actual = _directoryRepo.GetAllByUserId(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

     

        [TestMethod]
        public async Task Update_UpdatesFileName_UpdateFile()
        {
            //Arrange
            var directoryToUpdate = new Directory()
            {
                Id = _context.Directories.Local[0].Id,
                Name = "NewName",
                UploadDate = DateTime.Now,
                ApplicationUserId = _userIds[0],
                ApplicationUser = null
            };

            //Act
            await _directoryRepo.UpdateAsync(directoryToUpdate);

            //Assert
            var actual = _context.Directories.Local[0];
            Assert.AreEqual(directoryToUpdate.Id, actual?.Id);
            Assert.AreEqual(directoryToUpdate.Name, actual?.Name);
        }
        public void Dispose()
        {
            try
            {
                _context.Database.Delete();
            }
            catch
            {
                // ignored
            }

            _context.Dispose();
        }

        ~DirectoryRepositoryTests()
        {
            Dispose();
        }
    }
}
