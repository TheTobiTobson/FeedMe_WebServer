namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_for_FBS_VersuchSechs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner", c => c.String(nullable: false));
            CreateIndex("dbo.FBS_FeedbackSessions", "ApplicationUser_Id");
            AddForeignKey("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.FBS_FeedbackSessions", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner", c => c.String());
            DropColumn("dbo.FBS_FeedbackSessions", "ApplicationUser_Id");
        }
    }
}
