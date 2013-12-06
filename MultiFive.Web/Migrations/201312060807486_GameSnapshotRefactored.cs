namespace MultiFive.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameSnapshotRefactored : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameSnapshots", "LastMessage_Id", "dbo.Messages");
            DropIndex("dbo.GameSnapshots", new[] { "LastMessage_Id" });
            RenameColumn(table: "dbo.GameSnapshots", name: "LastMessage_Id", newName: "LastMessageId");
            AlterColumn("dbo.GameSnapshots", "LastMessageId", c => c.Int(nullable: false));
            CreateIndex("dbo.GameSnapshots", "LastMessageId");
            AddForeignKey("dbo.GameSnapshots", "LastMessageId", "dbo.Messages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameSnapshots", "LastMessageId", "dbo.Messages");
            DropIndex("dbo.GameSnapshots", new[] { "LastMessageId" });
            AlterColumn("dbo.GameSnapshots", "LastMessageId", c => c.Int());
            RenameColumn(table: "dbo.GameSnapshots", name: "LastMessageId", newName: "LastMessage_Id");
            CreateIndex("dbo.GameSnapshots", "LastMessage_Id");
            AddForeignKey("dbo.GameSnapshots", "LastMessage_Id", "dbo.Messages", "Id");
        }
    }
}
