namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FBS_FeedbackSessions",
                c => new
                    {
                        FBS_id = c.Int(nullable: false, identity: true),
                        FBS_name = c.String(),
                        FBS_numberOfQuestions = c.Int(nullable: false),
                        FBS_sessionOwner = c.String(),
                        FBS_creationDate = c.Int(nullable: false),
                        FBS_emailToReportTo = c.String(),
                    })
                .PrimaryKey(t => t.FBS_id);
            
            CreateTable(
                "dbo.QUE_FeedbackQuestions",
                c => new
                    {
                        QUE_id = c.Int(nullable: false, identity: true),
                        QUE_position = c.Int(nullable: false),
                        QUE_text = c.String(),
                        QUE_answerRadioButton = c.String(),
                        QUE_title = c.String(),
                        QUE_type = c.String(),
                        QUE_showQuestionInFeedback = c.Boolean(nullable: false),
                        QUE_FBS_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QUE_id)
                .ForeignKey("dbo.FBS_FeedbackSessions", t => t.QUE_FBS_id, cascadeDelete: true)
                .Index(t => t.QUE_FBS_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QUE_FeedbackQuestions", "QUE_FBS_id", "dbo.FBS_FeedbackSessions");
            DropIndex("dbo.QUE_FeedbackQuestions", new[] { "QUE_FBS_id" });
            DropTable("dbo.QUE_FeedbackQuestions");
            DropTable("dbo.FBS_FeedbackSessions");
        }
    }
}
