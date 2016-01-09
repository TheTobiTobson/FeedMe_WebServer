namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aenderung_Tabellen_1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QUE_FeedbackQuestions", "QUE_FBS_id", "dbo.FBS_FeedbackSessions");
            DropIndex("dbo.QUE_FeedbackQuestions", new[] { "QUE_FBS_id" });
            RenameColumn(table: "dbo.QUE_FeedbackQuestions", name: "QUE_FBS_id", newName: "FBS_FeedbackSessions_FBS_id");
            AlterColumn("dbo.QUE_FeedbackQuestions", "FBS_FeedbackSessions_FBS_id", c => c.Int());
            CreateIndex("dbo.QUE_FeedbackQuestions", "FBS_FeedbackSessions_FBS_id");
            AddForeignKey("dbo.QUE_FeedbackQuestions", "FBS_FeedbackSessions_FBS_id", "dbo.FBS_FeedbackSessions", "FBS_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QUE_FeedbackQuestions", "FBS_FeedbackSessions_FBS_id", "dbo.FBS_FeedbackSessions");
            DropIndex("dbo.QUE_FeedbackQuestions", new[] { "FBS_FeedbackSessions_FBS_id" });
            AlterColumn("dbo.QUE_FeedbackQuestions", "FBS_FeedbackSessions_FBS_id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.QUE_FeedbackQuestions", name: "FBS_FeedbackSessions_FBS_id", newName: "QUE_FBS_id");
            CreateIndex("dbo.QUE_FeedbackQuestions", "QUE_FBS_id");
            AddForeignKey("dbo.QUE_FeedbackQuestions", "QUE_FBS_id", "dbo.FBS_FeedbackSessions", "FBS_id", cascadeDelete: true);
        }
    }
}
