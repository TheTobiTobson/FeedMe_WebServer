using System;
using System.Collections.Generic;
// needed for [Foreign Key]
using System.ComponentModel.DataAnnotations.Schema;
// needed for[Key]
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class QUE_FeedbackQuestions
    {        
        [Key]
        public int QUE_id { get; set; }
        
        //position of question in feddbacksession
        [Required]
        public int QUE_position { get; set; }

        //question text
        public string QUE_text { get; set; }

        //answer to questions with radio buttons
        public string QUE_answerRadioButton { get; set; }

        //question title
        public string QUE_title { get; set; }

        // question Type
        [Required]
        public string QUE_type { get; set; }

        //show question in Feedback. True == show
        public bool QUE_showQuestionInFeedback { get; set; }

        // Get Creation Date from Server. Do not let Client manipulate this - T
        // when Question was originally created
        [Required]
        public int QUE_creationDate { get; set; }

        [ForeignKey("FBS_FeedbackSessions")]
        public int QUE_FBS_id { get; set; }       

        public FBS_FeedbackSessions FBS_FeedbackSessions { get; set; }
    }

    public class QUE_FeedbackQuestionsDTO
    {        
        public int QUE_id { get; set; }
        public int QUE_position { get; set; }
        public string QUE_text { get; set; }
        public string QUE_answerRadioButton { get; set; }
        public string QUE_title { get; set; }
        public string QUE_type { get; set; }
        public bool QUE_showQuestionInFeedback { get; set; }
        public int QUE_creationDate { get; set; }
    }
}