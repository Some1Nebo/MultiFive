namespace MultiFive.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameStateRefactored : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameSnapshots",
                c => new
                    {
                        GameId = c.Guid(nullable: false),
                        LastMessage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.GameId)
                .ForeignKey("dbo.Games", t => t.GameId)
                .ForeignKey("dbo.Messages", t => t.LastMessage_Id)
                .Index(t => t.GameId)
                .Index(t => t.LastMessage_Id);
            
            DropColumn("dbo.Games", "StateNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "StateNumber", c => c.Int(nullable: false));
            DropForeignKey("dbo.GameSnapshots", "LastMessage_Id", "dbo.Messages");
            DropForeignKey("dbo.GameSnapshots", "GameId", "dbo.Games");
            DropIndex("dbo.GameSnapshots", new[] { "LastMessage_Id" });
            DropIndex("dbo.GameSnapshots", new[] { "GameId" });
            DropTable("dbo.GameSnapshots");
        }
    }
}
