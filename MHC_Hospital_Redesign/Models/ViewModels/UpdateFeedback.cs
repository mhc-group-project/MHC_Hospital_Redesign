using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class UpdateFeedback
    {
      

        public FeedbackDto SelectedFeedback { get; set; }

        public IEnumerable<FeedbackCategoryDto> FeedbackCategoryOptions { get; set; }
        
    }
}
