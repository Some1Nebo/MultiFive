namespace MultiFive.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "CurrentState", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Width", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Height", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "FieldData", c => c.Binary());
            AddColumn("dbo.Games", "IncrementNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Games", "CurrentPlayer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "CurrentPlayer", c => c.Int(nullable: false));
            DropColumn("dbo.Games", "IncrementNumber");
            DropColumn("dbo.Games", "FieldData");
            DropColumn("dbo.Games", "Height");
            DropColumn("dbo.Games", "Width");
            DropColumn("dbo.Games", "CurrentState");
        }
    }
}
