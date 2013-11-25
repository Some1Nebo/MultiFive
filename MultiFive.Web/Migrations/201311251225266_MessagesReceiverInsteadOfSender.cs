namespace MultiFive.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MessagesReceiverInsteadOfSender : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "ReceiverId", c => c.Int(nullable: false));
            DropColumn("dbo.Messages", "SenderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "SenderId", c => c.Int(nullable: false));
            DropColumn("dbo.Messages", "ReceiverId");
        }
    }
}
