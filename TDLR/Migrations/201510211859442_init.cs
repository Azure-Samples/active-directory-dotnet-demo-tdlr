namespace Tdlr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AadObjects",
                c => new
                    {
                        AadObjectID = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AadObjectID);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        TaskText = c.String(nullable: false),
                        Status = c.String(nullable: false),
                        Creator = c.String(nullable: false),
                        CreatorName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TaskID);
            
            CreateTable(
                "dbo.TokenCacheEntries",
                c => new
                    {
                        TokenCacheEntryID = c.Int(nullable: false, identity: true),
                        userObjId = c.String(),
                        cacheBits = c.Binary(),
                        LastWrite = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TokenCacheEntryID);
            
            CreateTable(
                "dbo.Shares",
                c => new
                    {
                        TaskID = c.Int(nullable: false),
                        AadObjectID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.TaskID, t.AadObjectID })
                .ForeignKey("dbo.Tasks", t => t.TaskID, cascadeDelete: true)
                .ForeignKey("dbo.AadObjects", t => t.AadObjectID, cascadeDelete: true)
                .Index(t => t.TaskID)
                .Index(t => t.AadObjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shares", "AadObjectID", "dbo.AadObjects");
            DropForeignKey("dbo.Shares", "TaskID", "dbo.Tasks");
            DropIndex("dbo.Shares", new[] { "AadObjectID" });
            DropIndex("dbo.Shares", new[] { "TaskID" });
            DropTable("dbo.Shares");
            DropTable("dbo.TokenCacheEntries");
            DropTable("dbo.Tasks");
            DropTable("dbo.AadObjects");
        }
    }
}
