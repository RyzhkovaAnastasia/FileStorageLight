using System;
using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    public partial class Logger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                        Level = c.String(nullable: false),
                        Logger = c.String(nullable: false),
                        Exception = c.String(nullable: false),
                        Callsite = c.String(nullable: false),
                        Properties = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}
