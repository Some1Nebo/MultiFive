namespace MultiFive.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messagesgeneralization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "ChannelId", c => c.Guid());
            RenameColumn("dbo.Messages", "JsonContent", "Content");
            AlterColumn("dbo.Messages", "ReceiverId", c => c.Int());
            DropColumn("dbo.Messages", "GameId");
            DropColumn("dbo.Messages", "CreationTime");
            DropColumn("dbo.Messages", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Messages", "CreationTime", c => c.DateTime(nullable: false));
            RenameColumn("dbo.Messages", "JsonContent", "Content");
            AddColumn("dbo.Messages", "GameId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Messages", "ReceiverId", c => c.Int(nullable: false));
            DropColumn("dbo.Messages", "ChannelId");
        }
    }
}
