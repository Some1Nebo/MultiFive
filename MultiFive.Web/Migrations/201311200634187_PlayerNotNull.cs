namespace MultiFive.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerNotNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Player_Id", "dbo.Players");
            DropIndex("dbo.AspNetUsers", new[] { "Player_Id" });
            CreateIndex("dbo.AspNetUsers", "Player_Id");
            AddForeignKey("dbo.AspNetUsers", "Player_Id", "dbo.Players", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Player_Id", "dbo.Players");
            DropIndex("dbo.AspNetUsers", new[] { "Player_Id" });
            CreateIndex("dbo.AspNetUsers", "Player_Id");
            AddForeignKey("dbo.AspNetUsers", "Player_Id", "dbo.Players", "Id");
        }
    }
}
