namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_in_FBS_eingefuegt : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.FBS_FeedbackSessions", name: "ApplicationUser_Id", newName: "FBS_ApplicationUser_Id");
            RenameIndex(table: "dbo.FBS_FeedbackSessions", name: "IX_ApplicationUser_Id", newName: "IX_FBS_ApplicationUser_Id");
            DropColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner", c => c.String(nullable: false));
            RenameIndex(table: "dbo.FBS_FeedbackSessions", name: "IX_FBS_ApplicationUser_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.FBS_FeedbackSessions", name: "FBS_ApplicationUser_Id", newName: "ApplicationUser_Id");
        }
    }
}
