using System;
using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.Services
{
    /// <summary>
    /// StorageService responsive for storage items
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        /// <summary>
        /// StorageService constructor
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork instance</param>
        /// <param name="mapper">IMapper instance</param>
        public StorageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        /// <inheritdoc />
        public async Task<StorageItemsModel> GetItemsByUserId(Guid itemId, string userId)
        {
            Directory directory = await _unitOfWork.DirectoryRepository.GetAsync(itemId);
            if (directory == null)
            {
                directory = await _unitOfWork.DirectoryRepository.GetDefaultDirAsync(userId);
            }
            var storageItems = new List<StorageItemModel>();

            var files = _mapper.Map<IEnumerable<StorageItemModel>>(directory.Files);
            files.ToList().ForEach(x => x.IsFile = true);
            var directories = _mapper.Map<IEnumerable<StorageItemModel>>(_unitOfWork.DirectoryRepository.GetChildDirectories(directory.Id));

            storageItems.AddRange(directories);
            storageItems.AddRange(files);

            return new StorageItemsModel()
            {
                Id = directory.Id,
                Directory = _mapper.Map<DirectoryModel>(directory),
                StorageItems = storageItems
            };
        }
    }
}
