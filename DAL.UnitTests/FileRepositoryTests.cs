using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class FileRepositoryTests : IDisposable
    {
        private readonly FileStorageDbContext _context;
        private readonly IFileRepository _fileRepo;
        private readonly string[] _userIds = { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        public FileRepositoryTests()
        {
            var connection = DbConnectionFactory.CreateTransient();
            _context = new FileStorageDbContext(connection.DataSource);
            _fileRepo = new FileRepository(_context);
        }

        [TestInitialize()]
        public void Initialize()
        {
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.Create(new ApplicationUser() { Id = _userIds[0], UserName = "UserName1", Email = "UserName1@mail.com" }, "123456");
            userManager.Create(new ApplicationUser() { Id = _userIds[1], UserName = "UserName2", Email = "UserName2@mail.com" }, "123456");

            _context.Files.AddRange(new List<File>()
            {
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    IsImportant = true,
                    DirectoryId = Guid.NewGuid(),
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[0] }
                },
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName2",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    Location = "TestDirectory",
                    Recipients = new List<SharedFile>(){ new SharedFile(){ApplicationUserId =  _context.Users.Local[1].Id}},
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[0] }
                },
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName3",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[0] }

                },
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[1] }
                },
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName2",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[1] }
                },
                new File()
                {
                    Id = Guid.NewGuid(),
                    Name = "TestName3",
                    UploadDate = DateTime.Now,
                    Size = 10,
                    Type = ".png",
                    Directory = new Directory(){ Name = "Dir",  ApplicationUserId = _userIds[1] }
                }
            });

            _context.FileShortLinks.AddRange(new List<FileShortLink>()
            {
                new FileShortLink()
                {
                    Id = _context.Files.Local[2].Id,
                    FullLink = "fullLink",
                    ShortLink = "shortLink",
                    File = _context.Files.Local[2]
                }
            });

            _context.SharedFiles.AddRange(new List<SharedFile>()
            {
                new SharedFile()
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = _userIds[1],
                    FileId = _context.Files.Local[1].Id
                }
            });
        }

        [TestCleanup()]
        public void Cleanup()
        {
            _context.Database.Delete();
        }

        [TestMethod]
        public async Task Create_ValidFile_AddToFiles()
        {
            //Arrange

            File fileToList = new File()
            {
                Id = Guid.NewGuid(),
                Name = "FileToList",
                UploadDate = DateTime.Now,
                Size = 10,
                Type = ".png"
            };
            var expected = _context.Files.Local.ToList();
            expected.Add(fileToList);

            //Act
            await _fileRepo.CreateAsync(fileToList);

            //Assert
            var actual = _context.Files.Local.ToList();

            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Create_NullFile_DontAddToFiles()
        {
            //Arrange

            File fileToList = null;
            var expected = _context.Files.Local.ToList();

            //Act
            await _fileRepo.CreateAsync(fileToList);

            //Assert
            var actual = _context.Files.Local.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Create_ValidFileList_AddToFiles()
        {
            //Arrange

            var fileList = new List<File>()
            {
                new File(){
                Id = Guid.NewGuid(),
                Name = "FileToList",
                UploadDate = DateTime.Now,
                Size = 10,
                Type = ".png"
                }
            };
            var expected = _context.Files.Local.ToList();
            expected.AddRange(fileList);

            //Act
            await _fileRepo.CreateAsync(fileList);

            //Assert
            var actual = _context.Files.Local.ToList();

            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Delete_ValidFileId_DeleteFromFiles()
        {
            //Arrange
            const int index = 0;
            var fileToDeleteId = _context.Files.Local[index].Id;

            var expected = _context.Files.Local.ToList();
            expected.RemoveAt(index);

            //Act
            await _fileRepo.DeleteAsync(fileToDeleteId);

            //Assert
            var actual = _context.Files.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Delete_InvalidFileId_DontDelete()
        {
            //Arrange
            var fileToDeleteId = Guid.NewGuid();
            var expected = _context.Files.Local.ToList();

            //Act
            await _fileRepo.DeleteAsync(fileToDeleteId);

            //Assert
            var actual = _context.Files.Local.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Get_ValidFileId_ReturnFile()
        {
            //Arrange
            const int index = 0;
            var fileToGetId = _context.Files.Local[index].Id;
            var expected = _context.Files.Local[index];

            //Act
            var actual = await _fileRepo.GetAsync(fileToGetId);

            //Assert
            Assert.AreEqual(expected.Id, actual.Id);
        }

        [TestMethod]
        public async Task Get_InvalidFileId_ReturnNull()
        {
            //Arrange
            var fileToGetId = Guid.NewGuid();
            File expected = null;

            //Act
            var actual = await _fileRepo.GetAsync(fileToGetId);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task GetAll_ReturnAllFiles()
        {
            //Arrange
            var expected = _context.Files.Local.ToList();

            //Act
            var actual = await _fileRepo.GetAll();

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void GetAllByUserId_ValidUserId_ReturnAllUserFiles()
        {
            //Arrange
            var userId = _userIds[0];
            var expected = new List<File>()
            {
                _context.Files.Local[0],
                _context.Files.Local[1],
                _context.Files.Local[2]
            };

            //Act
            var actual = _fileRepo.GetAllByUserId(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void GetAllByUserId_InvalidUserId_ReturnEmptyList()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var expected = new List<File>();
            //Act
            var actual = _fileRepo.GetAllByUserId(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

      

        [TestMethod]
        public void GetImportant_ValidUserId_ReturnImportantFilesList()
        {
            //Arrange
            var userId = _userIds[0];
            var expected = new List<File>()
            {
                _context.Files.Local[0]
            };

            //Act
            var actual = _fileRepo.GetImportant(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Share_ValidInputValues_SetRecipientsToFile()
        {
            //Arrange
            int index = 0;
            var file = _context.Files.Local[index];

            var recipients = new Collection<ApplicationUser>()
            {
                _context.Users.Local[1]
            };

            var expected = _context.SharedFiles.Local.ToList();

            //Act
            await _fileRepo.ShareForRecipientsAsync(file.Id, recipients);

            //Assert
            var actual = _context.SharedFiles.Local.ToList();
            Assert.AreEqual(expected.Count + 1, actual.Count);
        }

        [TestMethod]
        public void Search_ValidFileName_ReturnFilesList()
        {
            //Arrange
            var fileName = "Test";
            var expected = _context.Files.Local.ToList();

            //Act
            var actual = _fileRepo.GetByName(fileName);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void Search_InvalidFileName_ReturnEmptyList()
        {
            //Arrange
            var fileName = "TestNotExist";
            var expected = new Collection<File>();

            //Act
            var actual = _fileRepo.GetByName(fileName);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task Update_UpdatesFileName_UpdateFile()
        {
            //Arrange 
            var actual = _context.Files.Local[0];
            var fileToUpdate = new File()
            {
                Id = actual.Id,
                Name = "NewName",
                IsImportant = true,
                Description = null
            };

            //Act
            await _fileRepo.UpdateAsync(fileToUpdate);

            //Assert
            Assert.AreEqual(fileToUpdate.Name, actual?.Name);
        }

        [TestMethod]
        public void GetAvailable_ValidUserId_ReturnFilesList()
        {
            //Arrange
            var userId = _userIds[1];
            var expected = new List<File>() { _context.Files.Local[1] };

            //Act
            var actual = _fileRepo.GetAvailable(userId);

            //Assert
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public async Task CreateFileShortLink_ValidFileShortLink_ReturnSetValue()
        {
            //Arrange
            const int index = 0;
            var shortLink = new FileShortLink()
            {
                FullLink = "fullLink",
                Id = _context.Files.Local[index].Id,
                ShortLink = "shortLink"
            };

            //Act
            var actual = await _fileRepo.CreateFileShortLinkAsync(shortLink);

            //Assert
            Assert.AreEqual(shortLink, actual);
        }

        [TestMethod]
        public async Task ShareByLink_AddToFileRecipients_SetToFileRecipients()
        {
            //Arrange
            const int index = 0;
            var expected = _context.SharedFiles.Local.Count;
            var shortLink = new SharedFile()
            {
                ApplicationUserId = _userIds[1],
                FileId = _context.Files.Local[index].Id
            };

            //Act
            await _fileRepo.ShareByLinkAsync(shortLink);

            //Assert
            var actual = _context.SharedFiles.Local.Count;
            Assert.AreEqual(expected + 1, actual);
        }

        [TestMethod]
        public void GetFileByShortLink_ValidShortLink_ReturnFile()
        {
            //Arrange
            var shortLink = _context.FileShortLinks.Local[0].ShortLink;
            var expected = _context.Files.Local[2].Id;
            //Act
            var actual = _fileRepo.GetFileByShortLink(shortLink)?.Id;

            //Assert
            Assert.AreEqual(expected, actual);
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

        ~FileRepositoryTests()
        {
            Dispose();
        }
    }
}