using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class DetailsFaqCategory
    {

       
       
            public FaqCategoryDto SelectedFaqCategory { get; set; }
            public IEnumerable<FaqDto> LinkedFaqs { get; set; }
       
    }
}