using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BLL.Exceptions;

namespace BLL.Services
{
    /// <summary>
    /// FileService responsive for file managing
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileSystem _fileSystem;
        private readonly IMapper _mapper;
        private const long MaxSize = 104857600; // 100MB

        /// <summary>
        /// FileService constructor
        /// </summary>
        /// <param name="unitOfWork">file repository instance</param>
        /// <param name="mapper">mapper instance</param>
        /// <param name="fileSystem">file system instance</param>
        public FileService(IUnitOfWork unitOfWork, IMapper mapper, IFileSystem fileSystem)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FileModel>> CreateAsync(HttpPostedFileBase[] files, string physicalPath, Guid parentDirId)
        {
            var fileToView = _mapper.Map<ICollection<FileModel>>(files);
            var filesToDb = _mapper.Map<ICollection<File>>(files).ToList();
            filesToDb.ForEach(f => f.DirectoryId = parentDirId);

            await CreatePhysicalFile(files, physicalPath, parentDirId);
            await _unitOfWork.FileRepository.CreateAsync(filesToDb);
            return fileToView;
        }

        private async Task CreatePhysicalFile(IEnumerable<HttpPostedFileBase> files, string physicalPath, Guid parentDirId)
        {
            var virtualPath = await _unitOfWork.DirectoryRepository.GetFullVirtualLocationPath(parentDirId);
            foreach (var file in files)
            {
                var fileName = _fileSystem.Path.GetFileName(file.FileName);
                var fullPath = _fileSystem.Path.Combine(physicalPath, virtualPath, fileName);


                if (file.ContentLength > MaxSize)
                {
                    throw new FileException("One of the files was too large");
                }

                if (_fileSystem.File.Exists(fullPath))
                {
                    throw new FileException(file.FileName + " already exist in storage");
                }

                try
                {
                    file.SaveAs(fullPath);
                }
                catch
                {
                    throw new FileException("Cannot upload files");
                }
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, string physicalPath)
        {
            var file = await GetAsync(id);
            if (file == null)
            {
                throw new NotFoundException();
            }

            await DeletePhysicalFile(physicalPath, file);
            await _unitOfWork.FileRepository.DeleteAsync(id);
        }

        private async Task DeletePhysicalFile(string physicalPath, FileModel file)
        {
            var virtualPath = await _unitOfWork.FileRepository.GetFullVirtualLocationPath(file.Id);
            var fullPath = _fileSystem.Path.Combine(physicalPath, virtualPath);

            if (_fileSystem.File.Exists(fullPath))
            {
                try
                {
                    _fileSystem.File.Delete(fullPath);
                }
                catch
                {
                    throw new FileException("Cannot delete file");
                }
            }

        }

        /// <inheritdoc />
        public async Task<FileModel> GetAsync(Guid id)
        {
            var file = await _unitOfWork.FileRepository.GetAsync(id);
            if (file == null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<FileModel>(file);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FileModel>> GetAll()
        {
            var files = await _unitOfWork.FileRepository.GetAll();
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        /// <inheritdoc />
        public IEnumerable<FileModel> GetImportant(string userId)
        {
            var files = _unitOfWork.FileRepository.GetImportant(userId);
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        /// <inheritdoc />
        public async Task<Recipients> ShareForRecipientsAsync(Guid id, IEnumerable<UserModel> recipients, IEnumerable<UserModel> allUsers)
        {
            var receivers = _mapper.Map<List<ApplicationUser>>(recipients);
            await _unitOfWork.FileRepository.ShareForRecipientsAsync(id, receivers);

            return new Recipients()
            {
                AllUsers = allUsers,
                SelectedUsersId = receivers.Select(u => u.Id),
            };
        }

        /// <inheritdoc />
        public IEnumerable<FileModel> GetByName(string name)
        {
            IEnumerable<File> files = _unitOfWork.FileRepository.GetByName(name);
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        public IEnumerable<FileModel> GetByNameDateFilter(string name)
        {
            IEnumerable<File> files = _unitOfWork.FileRepository.GetByNameDateFilter(name);
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        public IEnumerable<FileModel> GetByNameTypeFilter(string name)
        {
            IEnumerable<File> files = _unitOfWork.FileRepository.GetByNameTypeFilter(name);
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(FileModel file, string physicalPath)
        {
            var oldFile = await _unitOfWork.FileRepository.GetAsync(file.Id);

            if (oldFile == null)
            {
                throw new NotFoundException();
            }

            var fileToDb = _mapper.Map<File>(file);

            if (file.Name != oldFile.Name)
            {
                await UpdatePhysicalFile(fileToDb, oldFile, physicalPath);
            }

            await _unitOfWork.FileRepository.UpdateAsync(fileToDb);
        }

        private async Task UpdatePhysicalFile(File file, File oldFile, string physicalPath)
        {
            var virtualPath = await _unitOfWork.FileRepository.GetVirtualLocationPath(oldFile.Id);
            var fullOldPath = _fileSystem.Path.Combine(physicalPath, virtualPath, oldFile.Name + oldFile.Type);
            var fullNewPath = _fileSystem.Path.Combine(physicalPath, virtualPath, file.Name + file.Type);
            if (_fileSystem.File.Exists(fullOldPath) && fullNewPath != fullOldPath)
            {
                try
                {
                    _fileSystem.File.Move(fullOldPath, fullNewPath);
                }
                catch
                {
                    throw new FileException("Cannot edit file");
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<FileModel> GetAvailable(string userId)
        {
            var files = _unitOfWork.FileRepository.GetAvailable(userId);
            return _mapper.Map<IEnumerable<FileModel>>(files);
        }

        /// <inheritdoc />
        public async Task ShareByLinkAsync(Guid id, string userId)
        {
            var file = await GetAsync(id);
            if (file == null)
            {
                throw new NotFoundException();
            }

            var fileUserId =  await _unitOfWork.FileRepository.GetFileOwner(file.Id);
            if (fileUserId == userId)
            {
                throw new ForbiddenException("Owner cannot be guest");
            }

            var guest = new SharedFile()
            {
                FileId = id, 
                ApplicationUserId = userId
            };

            await _unitOfWork.FileRepository.ShareByLinkAsync(guest);
        }

        /// <inheritdoc />
        public async Task<byte[]> DownloadFile(Guid id, string physicalPath)
        {
            var file = await _unitOfWork.FileRepository.GetAsync(id);
            if (file == null)
            {
                throw new NotFoundException();
            }
            var virtualPath = await _unitOfWork.FileRepository.GetFullVirtualLocationPath(file.Id);
            var path = _fileSystem.Path.Combine(physicalPath, file.Directory.ApplicationUserId, virtualPath);
            return _fileSystem.File.ReadAllBytes(path);
        }

        private string GetFileShortLink(string fullQuery)
        {
            var urlBytes = Encoding.Default.GetBytes(fullQuery);
            var shortLink = Convert.ToBase64String(urlBytes);
            return shortLink;
        }

        /// <inheritdoc />
        public FileModel GetFileByShortLink(string shortQuery)
        {
            var file = _unitOfWork.FileRepository.GetFileByShortLink(shortQuery);
            if (file == null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<FileModel>(file);
        }

        /// <inheritdoc />
        public async Task<string> CreateLinkAsync(Guid id)
        {
            var absoluteUrl = id.ToString();
            var fileLink = new FileShortLink()
            {
                Id = id, 
                FullLink = absoluteUrl, 
                ShortLink = GetFileShortLink(absoluteUrl)
            };

            var fileShortLink = await _unitOfWork.FileRepository.CreateFileShortLinkAsync(fileLink);
            return fileShortLink?.ShortLink;
        }
    }
}
