using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MHC_Hospital_Redesign.Models
{
    public class FeedbackCategory
    {
        [Key]
        public int FeedbackCategoryID { get; set; }

        public string FeedbackCategoryName { get; set; }
        public string FeedbackCategoryColor { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
    public class FeedbackCategoryDto
    { 
        public int FeedbackCategoryID { get; set; }

        public string FeedbackCategoryName { get; set; }
        public string FeedbackCategoryColor { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
