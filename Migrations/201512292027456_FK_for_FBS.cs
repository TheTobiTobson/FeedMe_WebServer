namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK_for_FBS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.FBS_FeedbackSessions", "ApplicationUser_Id");
            AddForeignKey("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.FBS_FeedbackSessions", "FBS_RowVersion");
            DropColumn("dbo.QUE_FeedbackQuestions", "QUE_RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QUE_FeedbackQuestions", "QUE_RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.FBS_FeedbackSessions", "FBS_RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            DropForeignKey("dbo.FBS_FeedbackSessions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.FBS_FeedbackSessions", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.FBS_FeedbackSessions", "ApplicationUser_Id");
        }
    }
}
