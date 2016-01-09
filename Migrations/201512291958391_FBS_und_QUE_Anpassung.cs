namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FBS_und_QUE_Anpassung : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FBS_FeedbackSessions", "FBS_RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.QUE_FeedbackQuestions", "QUE_creationDate", c => c.Int(nullable: false));
            AddColumn("dbo.QUE_FeedbackQuestions", "QUE_RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AlterColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner", c => c.String(nullable: false));
            AlterColumn("dbo.QUE_FeedbackQuestions", "QUE_type", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QUE_FeedbackQuestions", "QUE_type", c => c.String());
            AlterColumn("dbo.FBS_FeedbackSessions", "FBS_sessionOwner", c => c.String());
            DropColumn("dbo.QUE_FeedbackQuestions", "QUE_RowVersion");
            DropColumn("dbo.QUE_FeedbackQuestions", "QUE_creationDate");
            DropColumn("dbo.FBS_FeedbackSessions", "FBS_RowVersion");
        }
    }
}
