using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using BLL.Exceptions;

namespace BLL.Services
{
    /// <summary>
    /// DirectoryService service for directory managing
    /// </summary>

    public class DirectoryService : IDirectoryService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// DirectoryService constructor
        /// </summary>
        /// <param name="directory">directory repository instance</param>
        /// <param name="mapper">mapper instance</param>
        /// <param name="fileSystem">file system instance</param>
        public DirectoryService(IDirectoryRepository directory, IMapper mapper, IFileSystem fileSystem)
        {
            _mapper = mapper;
            _directoryRepository = directory;
            _fileSystem = fileSystem;
        }


        /// <inheritdoc />
        public async Task<DirectoryModel> GetAsync(Guid id)
        {
            var directory = await _directoryRepository.GetAsync(id);
            if (directory == null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<DirectoryModel>(directory);
        }


        /// <inheritdoc />
        public async Task CreateAsync(DirectoryModel directory, string physicalPath, string userId)
        {
            directory.ApplicationUserId = userId;
            directory.UploadDate = DateTime.Now;

            var directoryToDb = _mapper.Map<Directory>(directory);
            
            await _directoryRepository.CreateAsync(directoryToDb);
            await CreatePhysicalDirectory(physicalPath, directoryToDb);
            
        }

        private async Task CreatePhysicalDirectory(string physicalPath, Directory directory)
        {
            var virtualPath = await _directoryRepository.GetFullVirtualLocationPath(directory.Id);

            var fullPhysicalPath = _fileSystem.Path.Combine(physicalPath, virtualPath);

            if (!_fileSystem.Directory.Exists(fullPhysicalPath))
            {
                try
                {
                    _fileSystem.Directory.CreateDirectory(fullPhysicalPath);
                }
                catch
                {
                    throw new DirectoryException("Cannot create current directory.");
                }
            }
            else
            {
                throw new DirectoryException("Directory is already exist. Choose another name.");
            }
        }


        /// <inheritdoc />
        public async Task DeleteAsync(Guid id, string physicalPath)
        {
            var directory = await _directoryRepository.GetAsync(id);
            if (directory == null)
            {
                throw new NotFoundException();
            }

            DeletePhysicalDirectory(physicalPath, directory);
            await _directoryRepository.DeleteAsync(id);
        }

        private void DeletePhysicalDirectory(string physicalPath, Directory directory)
        {
            var dirPath = _fileSystem.Path.Combine(physicalPath, directory.Location);
            if (_fileSystem.Directory.Exists(dirPath))
            {
                try
                {
                    _fileSystem.Directory.Delete(dirPath, true);
                }
                catch
                {
                    throw new DirectoryException("Cannot delete current directory");
                }
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(DirectoryModel directory, string physicalPath)
        {
            var oldDir = await _directoryRepository.GetAsync(directory.Id);

            if (oldDir == null)
            {
                throw new NotFoundException();
            }

            var directoryToDb = _mapper.Map<Directory>(directory);
            if (oldDir.Name != directory.Name)
            {
                await UpdatePhysicalDirectory(physicalPath, directoryToDb, oldDir);
            }

            await _directoryRepository.UpdateAsync(directoryToDb);
        }

        private async Task UpdatePhysicalDirectory(string physicalUserPath, Directory newDir, Directory oldDir)
        {
            var fullOldPath = _fileSystem.Path.Combine(physicalUserPath, oldDir.Location ?? string.Empty, oldDir.Name);
            var fullNewPath = _fileSystem.Path.Combine(physicalUserPath, newDir.Location ?? string.Empty, newDir.Name);

            if (_fileSystem.Directory.Exists(fullNewPath))
            {
                throw new DirectoryException("Directory with the same name already exist");
            }
            if (_fileSystem.Directory.Exists(fullOldPath) && fullOldPath != fullNewPath)
            {
                try
                {
                    _fileSystem.Directory.Move(fullOldPath, fullNewPath);
                }
                catch
                {
                    throw new DirectoryException("Cannot edit current directory");
                }
            }
        }
    }
}
