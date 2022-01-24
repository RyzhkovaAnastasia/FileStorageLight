namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileStorageItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        UploadDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 1000),
                        ParentDirectoryId = c.Guid(nullable: true),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Directories", t => t.ParentDirectoryId)
                .Index(t => t.ParentDirectoryId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.SharedFiles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FileId = c.Guid(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Type = c.String(),
                        Size = c.Long(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 1000),
                        IsImportant = c.Boolean(nullable: false),
                        DirectoryId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directories", t => t.DirectoryId, cascadeDelete: true)
                .Index(t => t.DirectoryId);
            
            CreateTable(
                "dbo.FileShortLinks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FullLink = c.String(),
                        ShortLink = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Directories", "ParentDirectoryId", "dbo.Directories");
            DropForeignKey("dbo.SharedFiles", "FileId", "dbo.Files");
            DropForeignKey("dbo.FileShortLinks", "Id", "dbo.Files");
            DropForeignKey("dbo.Files", "DirectoryId", "dbo.Directories");
            DropForeignKey("dbo.SharedFiles", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Directories", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.FileShortLinks", new[] { "Id" });
            DropIndex("dbo.Files", new[] { "DirectoryId" });
            DropIndex("dbo.SharedFiles", new[] { "ApplicationUserId" });
            DropIndex("dbo.SharedFiles", new[] { "FileId" });
            DropIndex("dbo.Directories", new[] { "ApplicationUserId" });
            DropIndex("dbo.Directories", new[] { "ParentDirectoryId" });
            DropTable("dbo.FileShortLinks");
            DropTable("dbo.Files");
            DropTable("dbo.SharedFiles");
            DropTable("dbo.Directories");
        }
    }
}
