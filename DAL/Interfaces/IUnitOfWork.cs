using System;

namespace DAL.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IFileRepository FileRepository { get; }
        IDirectoryRepository DirectoryRepository { get; }
    }
}
