using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHC_Hospital_Redesign.Models
{
    public class Feedback

    {
        [Key]
        public int FeedbackId { get; set; }
        [Required]
        public string UserName { get; set; }
        
        [AllowHtml]
        [Required]
        public string FeedbackContent { get; set; }
        public DateTime TimeStamp { get; set; }

        [ForeignKey("FeedbackCategory")]
        public int FeedbackCategoryID { get; set; }
        [Required]
        public virtual FeedbackCategory FeedbackCategory { get; set; }
    }
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        [Required(ErrorMessage = "Please Enter User Name.")]
        public string UserName { get; set; }
        
        public string FeedbackContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public int FeedbackCategoryID { get; set; }
        
        public string FeedbackCategoryName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }



    }
}
