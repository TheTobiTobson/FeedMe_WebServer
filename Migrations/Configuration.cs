namespace WebServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebServer.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebServer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "WebServer.Models.ApplicationDbContext";
        }

        protected override void Seed(WebServer.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //context.FBS_FeedbackSessions.AddOrUpdate(t => t.FBS_id,
            //    new FBS_FeedbackSessions() { FBS_creationDate = 2022, FBS_emailToReportTo = "Past@fakemail.com", FBS_name = "Past", FBS_numberOfQuestions = 3, FBS_sessionOwner = "Hugo" }
            //    );

            //context.QUE_FeedbackQuestions.AddOrUpdate(t => t.QUE_id,
            //    new QUE_FeedbackQuestions() { QUE_position = 1, QUE_text = "Unter den Straﬂenlaternen", QUE_answerRadioButton = "", QUE_title = "TheHeader", QUE_type = "Freitext", QUE_showQuestionInFeedback = true, QUE_FBS_id = 1 },
            //    new QUE_FeedbackQuestions() { QUE_position = 2, QUE_text = "Liegt ein goldener Stern", QUE_answerRadioButton = "", QUE_title = "TheMan", QUE_type = "Freitext", QUE_showQuestionInFeedback = true, QUE_FBS_id = 1 },
            //    new QUE_FeedbackQuestions() { QUE_position = 3, QUE_text = "Wir fressen eure Autos auf", QUE_answerRadioButton = "", QUE_title = "TheGirl", QUE_type = "Freitext", QUE_showQuestionInFeedback = true, QUE_FBS_id = 1 }
            //    );
        }
    }
}
