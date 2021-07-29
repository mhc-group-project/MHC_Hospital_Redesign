using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class DetailsFeedback
    {

        public FeedbackDto SelectedFeedback { get; set; }
       
        public FeedbackCategoryDto RelatedFeedbackCategory { get; set; }

    }
}
