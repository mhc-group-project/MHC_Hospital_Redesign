using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class DetailsFaq
    {
        public FaqDto SelectedFaq { get; set; }
        public IEnumerable<FaqCategoryDto> LinkedFaqCategories{ get; set; }

        public IEnumerable<FaqCategoryDto> AvailableFaqCategories { get; set; }
    }
}