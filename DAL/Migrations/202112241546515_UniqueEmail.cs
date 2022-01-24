namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueEmail : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUsers", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Email" });
        }
    }
}
