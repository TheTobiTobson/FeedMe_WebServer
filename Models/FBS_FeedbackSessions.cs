using System;
using System.Collections.Generic;
// needed for [Foreign Key]
using System.ComponentModel.DataAnnotations.Schema;
// needed for[Key]
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class FBS_FeedbackSessions
    {

        [Key]        
        public int FBS_id { get; set; }

        public string FBS_name { get; set; }

        // Get numberOfQuestions with QUE query. Do not let Client manipulate this - T
        [Required]
        public int FBS_numberOfQuestions { get; set; }

        // Get SessionOwner from LoggedIn user. Do not let Client manipulate this - T

        [MaxLength(128), Column(TypeName = "nvarchar")]
        [ForeignKey("ApplicationUser")]
        public string FBS_ApplicationUser_Id { get; set; }

        // Get Creation Date from Server. Do not let Client manipulate this - T
        [Required]
        public int FBS_creationDate { get; set; }
        
        public string FBS_emailToReportTo { get; set; }
                
        public ApplicationUser ApplicationUser { get; set; }
      
    }
}