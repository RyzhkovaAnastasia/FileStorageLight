using System.Data.Common;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DAL.Context
{
    public class FileStorageDbContext : IdentityDbContext<ApplicationUser>, IFileStorageDbContext
    {
        private DbSet<Log> Logs { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileShortLink> FileShortLinks { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }

        public FileStorageDbContext() : base("FileStorageDb")
        {
        }

        public FileStorageDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Directory>()
                .HasOptional(c => c.ParentDirectory)
                .WithMany()
                .HasForeignKey(c => c.ParentDirectoryId);
        }
    }
}
